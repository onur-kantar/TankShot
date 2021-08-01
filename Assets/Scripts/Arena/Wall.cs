using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviourPun, ICanCollide
{
    [SerializeField] GameObject hitWallPS;

    public void OnCollide(Collision2D collision)
    {
        photonView.RPC("OnCollideRPC", RpcTarget.All, collision.contacts[0].point, collision.contacts[0].normal);
        //Instantiate(hitWallPS, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        //AudioManager.instance.Play("HitWall");
    }
    [PunRPC]
    void OnCollideRPC(Vector2 point, Vector2 normal)
    {
        Instantiate(hitWallPS, point, Quaternion.LookRotation(normal));
        AudioManager.instance.Play("HitWall");
    }
}
