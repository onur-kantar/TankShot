using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    enum MatchResult { DRAW, WIN, LOSE }
    [Header("Game Scene Manager")]
    [SerializeField] GameObject[] spawnPoints;
    public GameObject joysticks;
    PlayerInfo playerInfo;
    [HideInInspector] public PlayerInfo[] players;

    [Header("Endgame Manager")]
    [SerializeField] GameObject result;
    [SerializeField] TMP_Text resultText, newScoreText, scoreText;
    [HideInInspector] public bool isMatchOver;

    CountdownController countdownController;
    FirebaseManager firebaseManager;
    VersusScreen versusScreen;

    void Start()
    {
        versusScreen = GetComponent<VersusScreen>();
        firebaseManager = GetComponent<FirebaseManager>();
        countdownController = GetComponent<CountdownController>();
        if (PhotonNetwork.IsConnected)
        {
            isMatchOver = false;
            players = new PlayerInfo [2];
            SpawnPlayer();
        }
    }
    void SpawnPlayer()
    {
        int point = 0;
        PhotonNetwork.NickName = "1";
        if (!PhotonNetwork.IsMasterClient)
        {
            point = 1;
            PhotonNetwork.NickName = "2";
        }
        GameObject go = PhotonNetwork.Instantiate("Tank", spawnPoints[point].transform.position, Quaternion.identity);
        StartCoroutine(firebaseManager.GetUserName(go, point));
    }
    [PunRPC]
    void AddUserID(int viewId, string userId, int point, string name = "", string score = "")
    {
        PhotonView pv = PhotonView.Find(viewId);
        GameObject player = pv.gameObject;
        playerInfo = new PlayerInfo(player, userId, name, score);
        players[point] = playerInfo;
        foreach (PlayerInfo p in players)
        {
            if (p.UserId == null)
            {
                return;
            }
        }
        StartCoroutine(versusScreen.VersusScreenCoroutine());
    }
    public void MatchIsOver(int seconds = 0)
    {
        if (!isMatchOver)
        {
            StartCoroutine(MatchIsOverCoroutine(seconds));
        }
    }
    IEnumerator MatchIsOverCoroutine(int seconds)
    {
        photonView.RPC("MakeTrueRPC", RpcTarget.All);

        yield return new WaitForSeconds(seconds);

        bool p1IsAlive = false, p2IsAlive = false;

        if (players[0].GameObject != null)
        {
            p1IsAlive = (bool)(players[0].GameObject?.GetComponent<TankBody>().isAlive);
        }
        if (players[1].GameObject != null)
        {
            p2IsAlive = (bool)(players[1].GameObject?.GetComponent<TankBody>().isAlive);
        }

        int score = CalculateScore(10);

        if (p1IsAlive == p2IsAlive)
        {
            photonView.RPC("ResultScreenRPC", RpcTarget.All, ((int)MatchResult.DRAW));
        }
        else { 
            if (p1IsAlive)
            {
                if (players[0].GameObject != null)
                    photonView.RPC("ResultScreenRPC", players[0].GameObject.GetComponent<PhotonView>().Owner, ((int)MatchResult.WIN), score);
                if (players[1].GameObject != null)
                    photonView.RPC("ResultScreenRPC", players[1].GameObject.GetComponent<PhotonView>().Owner, ((int)MatchResult.LOSE), -score);

                firebaseManager.UpdateScore(players[0].UserId, score);
                firebaseManager.UpdateScore(players[1].UserId, -score);
            }
            else
            {
                if (players[0].GameObject != null)
                    photonView.RPC("ResultScreenRPC", players[0].GameObject.GetComponent<PhotonView>().Owner, ((int)MatchResult.LOSE), -score);
                if (players[1].GameObject != null)
                    photonView.RPC("ResultScreenRPC", players[1].GameObject.GetComponent<PhotonView>().Owner, ((int)MatchResult.WIN), score);

                firebaseManager.UpdateScore(players[0].UserId, -score);
                firebaseManager.UpdateScore(players[1].UserId, score);
            }
        }
        photonView.RPC("LeaveRoomRPC", RpcTarget.All);
    }
    int CalculateScore(int baseScore)
    {
        countdownController.matchTime++;
        if (countdownController.matchTime < 1)
            countdownController.matchTime = 1;
        int totalScore = baseScore * countdownController.matchTime;
        return totalScore;
    }
    [PunRPC]
    void ResultScreenRPC(int matchResultId, int score = 0)
    {
        Color color = new Color();
        joysticks.SetActive(false);
        result.SetActive(true);
        MatchResult matchResult = ((MatchResult)matchResultId);
        resultText.text = matchResult.ToString();
        if (matchResult == MatchResult.WIN)
        {
            color = new Color(0.1f, 1, 0.1f, 1);
            scoreText.text = "+";
        }
        else if (matchResult == MatchResult.LOSE)
        {
            color = new Color(1, 0.1f, 0.1f, 1);
        }
        else if (matchResult == MatchResult.DRAW)
        {
            color = new Color(1, 1, 0.1f, 1);
        }
        resultText.color = color;
        scoreText.color = color;
        newScoreText.text = (Convert.ToInt32(playerInfo.Score) + score).ToString();
        scoreText.text += score.ToString();
    }
    [PunRPC]
    void MakeTrueRPC()
    {
        isMatchOver = true;
    }
    [PunRPC] //  TODO : -- RPC olmasına gerek var mı
    void LeaveRoomRPC()
    {
        PhotonNetwork.LeaveRoom();
    }
}
