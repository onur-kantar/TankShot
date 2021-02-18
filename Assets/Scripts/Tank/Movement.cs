using Photon.Pun;
using UnityEngine;

class Movement : MonoBehaviourPun
{
    [SerializeField]
    float speed, yon;

    Rigidbody2D rb2d;
    Vector2 movement;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<Movement>());
        }
    }
    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }
    void FixedUpdate()
    {
        rb2d.MovePosition(rb2d.position + movement.normalized * speed * Time.fixedDeltaTime);
    }
}

