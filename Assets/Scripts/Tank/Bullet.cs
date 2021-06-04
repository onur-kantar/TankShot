using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] float speed, time;
    [HideInInspector] public TankTurret ownerTankTurret;
    Rigidbody2D rb2d;

    [Header("Particle System")]
    [SerializeField] GameObject hitWallPS;//ateş etme

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Move();
        Destroy(gameObject, time);
    }

    void Move()
    {
        rb2d.AddForce(transform.up * speed);
        //rb2d.velocity = transform.up * speed;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (ownerTankTurret.ownerTankBody.tankPlayer.IsLocal)
            {
                ownerTankTurret.ownerTankBody.OnHit(collision.gameObject.GetComponent<PhotonView>().ViewID);
                gameObject.GetComponent<PhotonView>().RPC("DestroyBulletRPC", RpcTarget.All);

                //ownerTankTurret.ownerTankBody.WhenIHit(collision.gameObject.GetComponent<TankBody>().tankPlayer);
                //collision.gameObject.GetComponent<PhotonView>().RPC("PlayerLose", collision.gameObject.GetComponent<TankBody>().tankPlayer);
                //collision.gameObject.GetComponent<PhotonView>().RPC("PlayerWin", RpcTarget.Others);
            }
            //Destroy(collision.gameObject);
            //Destroy(gameObject); // TODO: -- destroy in everyone
        }
        else
        {
            transform.up = Vector2.Reflect(transform.up, collision.contacts[0].normal); ;
            Instantiate(hitWallPS, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            //Move();
        }
    }
    [PunRPC]
    void DestroyBulletRPC()
    {
        Destroy(gameObject);
    }
}
