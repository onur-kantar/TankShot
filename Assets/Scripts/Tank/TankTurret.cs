using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TankTurret : MonoBehaviourPunCallbacks
{
    public TankBody ownerTankBody;

    [Header ("Rotate")]
    [SerializeField] float rotateSpeed;
    VariableJoystick aimJoystick;
    Vector3 direction;

    [Header("Shoot")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] float coolDown, maxAmmo;
    float timeStamp;
    float currentAmmo;
    bool permissionToShoot;

    [Header("Laser")]
    [SerializeField] float distanceRay;
    LineRenderer lineRenderer;
    RaycastHit2D hit;
    
    void Start()
    {
        aimJoystick = GameObject.Find("Aim").GetComponent<VariableJoystick>();
        currentAmmo = maxAmmo;
        lineRenderer = GetComponent<LineRenderer>();
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Rotate();
            ShootLaser();
            Shoot();
        }
    }

    #region Rotate
    void Rotate()
    {
        if (aimJoystick.Direction.magnitude != 0)
        {
            direction.x = aimJoystick.Horizontal;
            direction.y = aimJoystick.Vertical;
            transform.up = Vector2.Lerp(transform.up, direction, rotateSpeed * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region Shoot
    void Shoot()
    {
        if (aimJoystick.isDrop && permissionToShoot)
        {
            if (CoolDown() && currentAmmo > 0)
            {
                //Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
                //GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
                //bullet.GetComponent<Bullet>().ownerTankTurret = this;
                //ownerTankBody.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.Others);
                photonView.RPC("ShootRPC", RpcTarget.All, shootPoint.position, shootPoint.rotation, PhotonNetwork.AllocateViewID(false));
                ReduceAmmo();
            }
            aimJoystick.isDrop = false;
        }
    }
    [PunRPC]
    public void ShootRPC(Vector3 bulletStartPosition, Quaternion bulletStartRotation, int viewId)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletStartPosition, bulletStartRotation);
        bullet.GetComponent<PhotonView>().ViewID = viewId;
        bullet.GetComponent<Bullet>().ownerTankTurret = this;
    }
    //TODO: --
    void ReduceAmmo()
    {
        currentAmmo--;
    }
    public void IncreaseAmmo()
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
    }//TODO: -- IEnumerator
    #endregion

    #region Laser
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
    #endregion
}
