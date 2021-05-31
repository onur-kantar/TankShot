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
    [HideInInspector] public List<GameObject> players;

    [Header("Endgame Manager")]
    [SerializeField] GameObject Result;
    [SerializeField] TextMeshProUGUI resultText, scoreText;
    [HideInInspector] public bool isMatchOver;
    DatabaseReference DBreference;
    FirebaseAuth auth;
    FirebaseUser user;

    CountdownController countdownController;
    void Start()
    {
        countdownController = GetComponent<CountdownController>();
        if (PhotonNetwork.IsConnected)
        {
            isMatchOver = false;
            players = new List<GameObject>();
            SpawnPlayer();
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
            auth = FirebaseAuth.DefaultInstance;
            user = auth.CurrentUser;
        }
    }
    //public void BeginGame()
    //{
    //    //Oyun başladığın yapılacaklar
    //    countdownController.Countdown();
    //}
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
            photonView.RPC("ResultScreenRPC", RpcTarget.All, "DRAW", 0);
        }
        else { 
            if (p1IsAlive)
            {
                photonView.RPC("ResultScreenRPC", players[0].GetComponent<PhotonView>().Owner, "WIN", 10);
                photonView.RPC("ResultScreenRPC", players[1].GetComponent<PhotonView>().Owner, "LOSE", -10);
            }
            else
            {
                photonView.RPC("ResultScreenRPC", players[0].GetComponent<PhotonView>().Owner, "LOSE", -10);
                photonView.RPC("ResultScreenRPC", players[1].GetComponent<PhotonView>().Owner, "WIN", 10);
            }
        }
        photonView.RPC("LeaveRoomRPC", RpcTarget.All);
    }
    [PunRPC]
    void ResultScreenRPC(string result, int baseScore)
    {
        int score;
        joysticks.SetActive(false);
        Result.SetActive(true);
        resultText.text = result;
        score = CalculateScore(baseScore);
        scoreText.text = score.ToString() + "PT";
        StartCoroutine(UpdateScoreCoroutine(score));
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
        PhotonNetwork.LoadLevel("Lobby");
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
