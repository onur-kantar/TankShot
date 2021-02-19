using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] spawnPoints;
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }
    void SpawnPlayer()
    {
        int point = 0; 
        if(!PhotonNetwork.IsMasterClient)
        {
            point = 1;
        }
        PhotonNetwork.Instantiate("Tank", spawnPoints[point].transform.position, Quaternion.identity);
    }

}
