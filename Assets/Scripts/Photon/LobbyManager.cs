using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Button searcMatchButton;
    [SerializeField]
    TextMeshProUGUI loadText; 

    void Start()
    {
        searcMatchButton.interactable = false;
        loadText.text = "Bağlanılıyor...";
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        loadText.text = "Sunucuya Bağlanıldı!\nSunucu: " + PhotonNetwork.CloudRegion;
        searcMatchButton.interactable = true;
    }
    public void FindMatch()
    {
        loadText.text = "Maç Aranıyor...";
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        loadText.text = "Maç Bulunamadı Oda Oluşturuluyor...";
        MakeRoom();
    }
    private void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);
        RoomOptions roomOptions =
        new RoomOptions
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
        };
        PhotonNetwork.CreateRoom("RoomName_" + randomRoomName, roomOptions);
        loadText.text = "Maç Bulunamadığı İçin Oda Oluşturuldu\nOyuncu Bekleniyor...";
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
