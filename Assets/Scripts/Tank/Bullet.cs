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
    private void Update()
    {
        Vector2 moveDirection = rb2d.velocity;
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }
    void Move()
    {
        rb2d.AddForce(transform.up * speed);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (ownerTankTurret.ownerTankBody.tankPlayer.IsLocal)
        {
            ICanCollide canCollide = collision.gameObject.GetComponent<ICanCollide>();
            if (canCollide != null)
            {
                canCollide.OnCollide(collision);
            }
        }
    }
    [PunRPC]
    void DestroyBulletRPC()
    {
        Destroy(gameObject);
    }
}
