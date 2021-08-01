using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldFeature : Feature
{
    [SerializeField] float time;
    GameObject shield;
    public override void AddFeature(GameObject player)
    {
        _player = player;
        StartCoroutine(SpeedFeatureCoroutine(player));
    }
    public override void RemoveFeature()
    {
        _player.GetComponent<TankBody>().features.Remove(this);
        shield.GetComponent<PhotonView>().RPC("DestroyShieldRPC", RpcTarget.All);
        photonView.RPC("DestroyFeatureRPC", RpcTarget.All);
        photonView.RPC("DecreaseFeatureSize", RpcTarget.MasterClient);
    }
    IEnumerator SpeedFeatureCoroutine(GameObject player)
    {
        shield = PhotonNetwork.Instantiate("Shield", player.transform.position, Quaternion.identity);
        photonView.RPC("ShieldRPC", RpcTarget.All, player.GetComponent<PhotonView>().ViewID, shield.GetComponent<PhotonView>().ViewID);
        photonView.RPC("FeatureCollected", RpcTarget.All);

        yield return new WaitForSeconds(time);

        RemoveFeature();
    }
    [PunRPC]
    void ShieldRPC(int playerID, int shieldID)
    {
        GameObject player = PhotonView.Find(playerID).gameObject;
        GameObject shield = PhotonView.Find(shieldID).gameObject;

        shield.GetComponent<Shield>().ownerTank = player.transform;

        Collider2D[] colliders = player.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            Physics2D.IgnoreCollision(collider, shield.GetComponent<Collider2D>());
        }
    }    
}
