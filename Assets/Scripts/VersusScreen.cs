using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersusScreen : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject versusScreen;
    [SerializeField] TMP_Text player1Name;
    [SerializeField] TMP_Text player2Name;
    [SerializeField] TMP_Text player1Score;
    [SerializeField] TMP_Text player2Score;
    GameSceneManager gameSceneManager;
    CountdownController countdownController;
    PlayerInfo[] players;
    void Start()
    {
        countdownController = GetComponent<CountdownController>();
        gameSceneManager = GetComponent<GameSceneManager>();
        players = gameSceneManager.players;
    }
    public IEnumerator VersusScreenCoroutine()
    {
        player1Name.text = players[0].Name;
        player2Name.text = players[1].Name;
        player1Score.text = players[0].Score;
        player2Score.text = players[1].Score;
        yield return new WaitForSeconds(1);
        versusScreen.SetActive(true);
        loadingScreen.SetActive(false);
        yield return new WaitForSeconds(5);
        versusScreen.SetActive(false);
        countdownController.StartCountdown();
    }
}
