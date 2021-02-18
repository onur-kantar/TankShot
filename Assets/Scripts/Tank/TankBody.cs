using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TankBody : MonoBehaviour
{
    [Header ("Movement")]
    VariableJoystick movementJoystick;
    [SerializeField]
    float rotateSpeed, speed;

    Vector3 direction;

    public Player tankPlayer;

    void Start()
    {
        tankPlayer = GetComponent<PhotonView>().Owner;
        movementJoystick = GameObject.Find("Movement").GetComponent<VariableJoystick>();
    }
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        if (movementJoystick.Direction.magnitude != 0)
        {
            direction.x = movementJoystick.Horizontal;
            direction.y = movementJoystick.Vertical;
            if (Vector2.Dot(transform.up, direction) < 0)
            {
                transform.up = Vector2.Lerp(transform.up, -direction, rotateSpeed * Time.deltaTime);
                transform.Translate(0f, -speed * Time.deltaTime, 0f);
            }
            else
            {
                transform.up = Vector2.Lerp(transform.up, direction, rotateSpeed * Time.deltaTime);
                transform.Translate(0f, speed * Time.deltaTime, 0f);
            }
        }
    }
    [PunRPC]
    public void OnHit(Player player)
    {
        Debug.Log(player.NickName + " Beni vurdu");
    }
}
