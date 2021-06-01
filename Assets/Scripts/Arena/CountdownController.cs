using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownController : MonoBehaviourPun
{
    public int matchTime;
    GameSceneManager gameSceneManager;
    [SerializeField] TextMeshProUGUI matchTimeText;
    [SerializeField] int starCountdownTime;
    [SerializeField] TextMeshProUGUI starCountdownText;
    [SerializeField] GameObject starCountdownPanel;

    private void Start()
    {
        gameSceneManager = GetComponent<GameSceneManager>();
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartCountdownRPC", RpcTarget.All);
        }
    }
    IEnumerator StartCountdownCoroutine()
    {
        starCountdownPanel.gameObject.SetActive(true);
        matchTimeText.gameObject.SetActive(false);

        while (starCountdownTime > 0)
        {
            starCountdownText.text = starCountdownTime.ToString();
            yield return new WaitForSeconds(1);
            starCountdownTime--;
        }
        starCountdownText.text = "GO!";
        yield return new WaitForSeconds(1);
        starCountdownPanel.gameObject.SetActive(false);
        matchTimeText.gameObject.SetActive(true);
        Countdown();
    }
    [PunRPC]
    void StartCountdownRPC()
    {
        StartCoroutine(StartCountdownCoroutine());
    }
    void Countdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }
    IEnumerator CountdownCoroutine() // TODO: --Lobbymanager'in timer'�n� kullan
    {
        while (matchTime >= 0 && !gameSceneManager.isMatchOver)
        {
            photonView.RPC("CountdownRPC", RpcTarget.All);
            yield return new WaitForSeconds(1);
        }
        gameSceneManager.MatchIsOver();
    }
    [PunRPC]
    void CountdownRPC()
    {
        float minutes = Mathf.FloorToInt(matchTime / 60);
        float seconds = Mathf.FloorToInt(matchTime % 60);
        matchTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        matchTime--;
    }
}