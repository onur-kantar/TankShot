using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    LoadingManager loadingManager;
    private void Awake()
    {
        loadingManager = GetComponent<LoadingManager>();
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        loadingManager.photonIsLoaded = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        loadingManager.hasError = true;
    }
}
