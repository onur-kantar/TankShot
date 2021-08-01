using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviourPun, ICanCollide
{
    [HideInInspector] public Transform ownerTank;
    [SerializeField] GameObject hitShieldPS;
    private void Update()
    {
        transform.position = ownerTank.position;
    }
    void FixedUpdate()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Bullet"))
            {
                if (hitCollider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    if (Vector3.Dot((transform.position - hitCollider.gameObject.transform.position),
                                     hitCollider.gameObject.transform.position - (hitCollider.gameObject.transform.position + hitCollider.gameObject.transform.up)) < 0)
                    {
                        Physics2D.IgnoreCollision(hitCollider.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(hitCollider.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                    }
                }
            }
        }
    }
    public void OnCollide(Collision2D collision)
    {
        if (Vector3.Dot((transform.position - collision.gameObject.transform.position),
                            collision.gameObject.transform.position - (collision.gameObject.transform.position + collision.gameObject.transform.up)) >= 0)
        {
            photonView.RPC("OnCollideRPC", RpcTarget.All, collision.contacts[0].point, collision.contacts[0].normal);
            //AudioManager.instance.Play("HitShield");
            //Instantiate(hitShieldPS, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }
    }
    [PunRPC]
    void DestroyShieldRPC()
    {
        Destroy(gameObject);
    }
    [PunRPC]
    void OnCollideRPC(Vector2 point, Vector2 normal)
    {
        Instantiate(hitShieldPS, point, Quaternion.LookRotation(normal));
        AudioManager.instance.Play("HitShield");
    }
}

