using Photon.Pun;
using UnityEngine;

public class TankTurret : MonoBehaviourPunCallbacks
{
    [Header ("Rotate")]
    VariableJoystick aimJoystick;
    [SerializeField]
    float rotateSpeed;
    Vector3 direction;

    [Header("Shoot")]
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    Transform shootPoint;
    [SerializeField]
    float coolDown, maxAmmo;
    float timeStamp;
    static float currentAmmo;
    bool permissionToShoot;

    [Header("Laser")]
    [SerializeField]
    float distanceRay;
    LineRenderer lineRenderer;
    RaycastHit2D hit;

    public TankBody ownerTankBody;
    //private PhotonView photonView;
    void Start()
    {
        //photonView = GetComponent<PhotonView>();
        aimJoystick = GameObject.Find("Aim").GetComponent<VariableJoystick>();
        currentAmmo = maxAmmo;
        lineRenderer = GetComponent<LineRenderer>();
    }
    void FixedUpdate()
    {
        Rotate();
        ShootLaser();
        Shoot();
    }
    void Rotate()
    {
        if (aimJoystick.Direction.magnitude != 0)
        {
            direction.x = aimJoystick.Horizontal;
            direction.y = aimJoystick.Vertical;
            transform.up = Vector2.Lerp(transform.up, direction, rotateSpeed * Time.fixedDeltaTime);
        }
    }

    void Shoot()
    {
        if (aimJoystick.isDrop && permissionToShoot)
        {
            if (CoolDown() && currentAmmo > 0)
            {
                //Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
                GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
                bullet.GetComponent<Bullet>().ownerTankBody = ownerTankBody;
                ownerTankBody.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.Others);
                //photonView.RPC("ShootRPC", RpcTarget.Others, shootPoint.position);
                ReduceAmmo();
            }
            aimJoystick.isDrop = false;
        }
    }
    [PunRPC]
    public void ShootRPC(Vector3 bulletStartPosition)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletStartPosition, shootPoint.rotation);
        bullet.GetComponent<Bullet>().ownerTankBody = ownerTankBody;
    }
    //TODO: --
    public static void ReduceAmmo()
    {
        currentAmmo--;
    }
    public static void IncreaseAmmo()
    {
        currentAmmo++;
    }
    bool CoolDown()
    {
        if (timeStamp <= Time.time)
        {
            timeStamp = Time.time + coolDown;
            return true;
        }
        return false;
    }
    void ShootLaser()
    {
        if (aimJoystick.Direction.magnitude > 0.3f)
        {
            int layerMask = 1 << 8;
            layerMask = ~layerMask;
            hit = Physics2D.Raycast(shootPoint.position, shootPoint.up, distanceRay, layerMask);
            if (hit)
            {
                Draw2DRay(shootPoint.position, hit.point);
            }
            else
            {
                Draw2DRay(shootPoint.position, shootPoint.position + shootPoint.up * distanceRay);
            }
            permissionToShoot = true;
        }
        else if (!aimJoystick.isDrop)
        {
            Draw2DRay(Vector3.zero, Vector3.zero);
            permissionToShoot = false;
        }
        
    }
    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}
