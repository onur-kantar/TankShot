using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [HideInInspector] public TankBody owner;

    void Update()
    {
        if(owner != null)
            transform.position = owner.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            CircleCollider2D m_Collider = GetComponent<CircleCollider2D>();
            Vector3 m_Center = m_Collider.bounds.center;
            if (!(m_Collider.radius / 3 > Vector3.Distance(collision.transform.position, m_Center)))
            {
                collision.gameObject.GetComponent<PhotonView>().RPC("DestroyBulletRPC", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    void DestroyShieldRPC()
    {
        Destroy(gameObject);
    }
}
