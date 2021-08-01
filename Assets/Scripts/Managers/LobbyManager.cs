using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Button searcMatchButton;
    [SerializeField] Button cancelButton;
    [SerializeField] GameObject searchingPopup;
    [SerializeField] TMP_Text searchingText;
    [SerializeField] GameObject loadingScreen;
    SceneLoader sceneLoader;
    UIManager uiManager;
    float timer;
    bool matchIsSearching;

    void Start()
    {
        uiManager = GetComponent<UIManager>();
        sceneLoader = GetComponent<SceneLoader>();

        timer = 0;
        matchIsSearching = false;
        AudioManager.instance.Play("LobbyMusic");
        if (!PhotonNetwork.IsConnected)
        { 
            searcMatchButton.interactable = false;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    void Update()
    {
        if (matchIsSearching)
        {
            timer += Time.deltaTime;
            DisplayTime(timer);
        }
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        searcMatchButton.interactable = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        uiManager.ShowErrorScreen();
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene("Loading");
    }
    public void FindMatch()
    {
        PhotonNetwork.JoinRandomRoom();
        searchingPopup.SetActive(true);
        cancelButton.interactable = false;
        searcMatchButton.interactable = false; // TODO: -- kullanıcı odaya girerken değil ararken kapat
        matchIsSearching = true;
    }
    public void CancelMatch()
    {
        PhotonNetwork.LeaveRoom();
        searcMatchButton.interactable = false;
        searchingPopup.SetActive(false);
        timer = 0;
        matchIsSearching = false;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        int randomRoomName = Random.Range(0, 5000);
        RoomOptions roomOptions =
        new RoomOptions
        {
            //CleanupCacheOnLeave = false,
            EmptyRoomTtl = 0,
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
        };
        PhotonNetwork.CreateRoom(Constant.ROOM_NAME + randomRoomName, roomOptions);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("CancelButtonSetActive", RpcTarget.All);
            AudioManager.instance.Stop("LobbyMusic");
            sceneLoader.LoadScene("Arena");
        }
    }
    [PunRPC]
    void CancelButtonSetActive()
    {
        loadingScreen.SetActive(true);
        cancelButton.interactable = false;
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        searchingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public override void OnJoinedRoom()
    {
        cancelButton.interactable = true;
    }
}
