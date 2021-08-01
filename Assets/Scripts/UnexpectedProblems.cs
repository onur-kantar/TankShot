using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnexpectedProblems : MonoBehaviourPunCallbacks
{
    GameSceneManager gameSceneManager;

    private void Awake()
    {
        gameSceneManager = GetComponent<GameSceneManager>();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        gameSceneManager.MatchIsOver(0);
    }
}
