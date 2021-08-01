using Photon.Pun;
using System.Collections;
using UnityEngine;

public struct PlayerInfo
{
    public GameObject GameObject;
    public string UserId;
    public string Name;
    public string Score;
    public PlayerInfo(GameObject GameObject, string UserId, string Name, string Score)
    {
        this.GameObject = GameObject;
        this.UserId = UserId;
        this.Name = Name;
        this.Score = Score;
    }
}
