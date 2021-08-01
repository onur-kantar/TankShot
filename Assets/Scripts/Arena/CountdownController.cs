using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownController : MonoBehaviourPun
{
    public int matchTime;
    GameSceneManager gameSceneManager;
    [SerializeField] FeatureCreator featureCreator;
    [SerializeField] TextMeshProUGUI matchTimeText;
    [SerializeField] int starCountdownTime;
    [SerializeField] TextMeshProUGUI starCountdownText;
    [SerializeField] GameObject starCountdownPanel;

    private void Start()
    {
        gameSceneManager = GetComponent<GameSceneManager>();
    }
    public void StartCountdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartCountdownCoroutine());
        }
    }
    IEnumerator StartCountdownCoroutine()
    {
        //starCountdownPanel.gameObject.SetActive(true);
        //matchTimeText.gameObject.SetActive(false);
        photonView.RPC("ChangePanelRPC", RpcTarget.All, true, false);
        while (starCountdownTime > 0)
        {
            photonView.RPC("StartCountdownRPC", RpcTarget.All, starCountdownTime.ToString());
            //starCountdownText.text = starCountdownTime.ToString();
            yield return new WaitForSeconds(1);
            starCountdownTime--;
        }
        photonView.RPC("StartCountdownRPC", RpcTarget.All, "GO!");
        yield return new WaitForSeconds(1);
        photonView.RPC("ChangePanelRPC", RpcTarget.All, false, true);
        //starCountdownPanel.gameObject.SetActive(false);
        //matchTimeText.gameObject.SetActive(true);
        featureCreator.isCreating = true;
        Countdown();
    }
    [PunRPC]
    void ChangePanelRPC(bool countdownPanel, bool timeText)
    {
        starCountdownPanel.gameObject.SetActive(countdownPanel);
        matchTimeText.gameObject.SetActive(timeText);
    }
    [PunRPC]
    void StartCountdownRPC(string text)
    {
        starCountdownText.text = text;
    }
    void Countdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }
    IEnumerator CountdownCoroutine() // TODO: --Lobbymanager'in timer'ýný kullan
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
