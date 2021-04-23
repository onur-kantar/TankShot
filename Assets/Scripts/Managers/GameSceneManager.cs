using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviourPun
{
    [Header("Game Scene Manager")]
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject joysticks;
    [SerializeField] int matchTime;
    [SerializeField] TextMeshProUGUI matchTimeText;
    [HideInInspector] public List<GameObject> players;

    [Header("Endgame Manager")]
    [SerializeField] GameObject Result;
    [SerializeField] TextMeshProUGUI resultText, scoreText;
    [HideInInspector] public bool isMatchOver;
    DatabaseReference DBreference;
    FirebaseAuth auth;
    FirebaseUser user;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            isMatchOver = false;
            players = new List<GameObject>();
            SpawnPlayer();
            Countdown();
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
            auth = FirebaseAuth.DefaultInstance;
            user = auth.CurrentUser;
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
        PhotonNetwork.Instantiate("Tank", spawnPoints[point].transform.position, Quaternion.identity);
        
    }
    void Countdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CountdownCoroutine());
        } 
    }
    IEnumerator CountdownCoroutine() // TODO: --Lobbymanager'in timer'ını kullan
    {
        while (matchTime >= 0 && !isMatchOver)
        {
            photonView.RPC("CountdownRPC", RpcTarget.All);
            yield return new WaitForSeconds(1);
        }
        MatchIsOver();
    }
    [PunRPC]
    void CountdownRPC()
    {
        float minutes = Mathf.FloorToInt(matchTime / 60);
        float seconds = Mathf.FloorToInt(matchTime % 60);
        matchTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        matchTime--;
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

        bool p1IsAlive = players[0].GetComponent<TankBody>().isAlive;
        bool p2IsAlive = players[1].GetComponent<TankBody>().isAlive;
        if (p1IsAlive == p2IsAlive)
        {
            photonView.RPC("ResultScreenRPC", RpcTarget.All, "Draw", 0);
        }
        else { 
            if (p1IsAlive)
            {
                photonView.RPC("ResultScreenRPC", players[0].GetComponent<PhotonView>().Owner, "Win", 10);
                photonView.RPC("ResultScreenRPC", players[1].GetComponent<PhotonView>().Owner, "Lose", -10);
            }
            else
            {
                photonView.RPC("ResultScreenRPC", players[0].GetComponent<PhotonView>().Owner, "Lose", -10);
                photonView.RPC("ResultScreenRPC", players[1].GetComponent<PhotonView>().Owner, "Win", 10);
            }
        }
        photonView.RPC("LeaveRoomRPC", RpcTarget.All);
    }
    [PunRPC]
    void ResultScreenRPC(string result, int baseScore)
    {
        joysticks.SetActive(false);
        Result.SetActive(true);
        resultText.text = result;
        scoreText.text = CalculateScore(baseScore).ToString();
        StartCoroutine(UpdateScoreCoroutine(int.Parse(scoreText.text)));
    }
    int CalculateScore(int baseScore)
    {
        matchTime++;
        if (matchTime < 1)
            matchTime = 1;
        int totalScore = baseScore * matchTime;
        return totalScore;
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
    public void GoToLobby()
    {
        PhotonNetwork.LoadLevel(0);
    }

    IEnumerator UpdateScoreCoroutine(int score)
    {
        var DBTaskGet = DBreference.Child("users").Child(user.UserId).Child("score").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTaskGet.IsCompleted);

        if (DBTaskGet.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTaskGet.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTaskGet.Result;

            int baseScore = Convert.ToInt32(snapshot.Value);
            baseScore += score;

            var DBTaskSet = DBreference.Child("users").Child(user.UserId).Child("score").SetValueAsync(baseScore);

            yield return new WaitUntil(predicate: () => DBTaskSet.IsCompleted);

            if (DBTaskSet.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTaskSet.Exception}");
            }
            else
            {
            }
        }
        
    }
}
