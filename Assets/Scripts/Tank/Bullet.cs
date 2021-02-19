using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float speed, time;
    Rigidbody2D rb2d;
    Vector2 newDirection;
    [HideInInspector] public TankTurret ownerTankTurret;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Move();
        Destroy(gameObject, time);
    }
    void Move()
    {
        rb2d.velocity = transform.up * speed;
    }
    void OnDestroy()
    {
        ownerTankTurret.IncreaseAmmo();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (ownerTankTurret.ownerTankBody.tankPlayer.IsLocal)
            {
                ownerTankTurret.ownerTankBody.GetComponent<PhotonView>().RPC("OnHit", collision.gameObject.GetComponent<TankBody>().tankPlayer, ownerTankTurret.ownerTankBody.tankPlayer);
                Debug.Log(ownerTankTurret.ownerTankBody.tankPlayer.NickName);
            }
            //Destroy(collision.gameObject);
            //Destroy(gameObject);
        }
        else
        {
            newDirection = Vector2.Reflect(transform.up, collision.contacts[0].normal);
            transform.up = newDirection;
            Move();
        }
    }
}
