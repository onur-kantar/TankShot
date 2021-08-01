using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class TankBody : MonoBehaviourPun, ICanCollide
{
    public TankTurret ownTurret;
    [SerializeField] float rotateSpeed;
    [HideInInspector] public float defaultSpeed;
    public float speed;
    VariableJoystick movementJoystick;
    Vector3 direction;
    [HideInInspector] public bool isAlive;
    GameSceneManager gameSceneManager;
    GameObject mainCamera;
    public Player tankPlayer;
    [HideInInspector] public GameObject shield;
    public List<Feature> features = new List<Feature>();

    [Header("Camera Limits")]
    [SerializeField] float leftLimit;
    [SerializeField] float rightLimit;
    [SerializeField] float bottomLimit;
    [SerializeField] float topLimit;

    [Header("Particle System")]
    [SerializeField] GameObject deathPS;

    void Start()
    {
        defaultSpeed = speed;
        isAlive = true;
        tankPlayer = photonView.Owner;
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        movementJoystick = GameObject.Find("Movement").GetComponent<VariableJoystick>();
        if (photonView.IsMine)
        {
            mainCamera = GameObject.Find("Main Camera");
        }

    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Move();
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
            mainCamera.transform.position = new Vector3
                (
                    Mathf.Clamp(mainCamera.transform.position.x, leftLimit, rightLimit),
                    Mathf.Clamp(mainCamera.transform.position.y, bottomLimit, topLimit),
                    mainCamera.transform.position.z
                );
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Feature feature = collision.gameObject.GetComponent<Feature>();
        if (feature != null)
        {
            if (photonView.IsMine)
            {
                foreach (Feature f in features)
                {
                    if (feature.GetType().Name == f.GetType().Name)
                    {
                        f.RemoveFeature();
                        break;
                    }
                }
                features.Add(feature);
                feature.AddFeature(gameObject);
            }
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
    [PunRPC]
    void OnCollideRPC(int viewId)
    {
        GameObject player = PhotonView.Find(viewId).gameObject;
        player.GetComponent<TankBody>().isAlive = false;
        Instantiate(deathPS, player.transform.position, Quaternion.identity);
        AudioManager.instance.Play("Explosion");
        player.SetActive(false);
    }

    public void OnCollide(Collision2D collision)
    {
        photonView.RPC("OnCollideRPC", RpcTarget.All, photonView.ViewID);
        collision.otherCollider.gameObject.GetComponent<PhotonView>().RPC("DestroyBulletRPC", RpcTarget.All);
        gameSceneManager.MatchIsOver(3); //TODO: -- should it work in everyone
    }
}
