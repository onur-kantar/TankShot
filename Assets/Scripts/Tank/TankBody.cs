using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TankBody : MonoBehaviourPun
{
    [SerializeField] float rotateSpeed;
    [SerializeField] float speed;
    VariableJoystick movementJoystick;
    Vector3 direction;
    [HideInInspector] public bool isAlive;
    GameSceneManager gameSceneManager;
    public Player tankPlayer;

    void Start()
    {
        isAlive = true;
        tankPlayer = photonView.Owner;
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        gameSceneManager.players.Add(gameObject); //TODO: -- Just master
        movementJoystick = GameObject.Find("Movement").GetComponent<VariableJoystick>();
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Move();
        }
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

    public void OnHit(int viewId)
    {
        photonView.RPC("OnHitRPC", RpcTarget.All, viewId);
        gameSceneManager.MatchIsOver(3); //TODO: -- should it work in everyone
    }
    [PunRPC]
    void OnHitRPC(int viewId)
    {
        GameObject player = PhotonView.Find(viewId).gameObject;
        player.GetComponent<TankBody>().isAlive = false;
        player.SetActive(false);
    }
}
