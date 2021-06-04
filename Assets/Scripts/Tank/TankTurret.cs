using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class TankTurret : MonoBehaviourPunCallbacks
{
    public TankBody ownerTankBody;

    [Header("Rotate")]
    [SerializeField] float rotateSpeed;
    VariableJoystick aimJoystick;
    Vector3 direction;

    [Header("Shoot")]
    [SerializeField] Transform shootPoint;
    float timeStamp;
    [HideInInspector] public float currentAmmo;
    bool permissionToShoot;

    [Header("Turret")]
    [SerializeField] Turret defaultTurret;
    [HideInInspector] public Turret turret;
    public SpriteRenderer spriteRenderer;

    [Header("Laser")]
    [SerializeField] float distanceRay;
    LineRenderer lineRenderer;
    RaycastHit2D hit;

    [Header("Particle System")]
    [SerializeField] ParticleSystem smokePS;

    void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RevertDefaultTurret", RpcTarget.All, photonView.ViewID);
            aimJoystick = GameObject.Find("Aim").GetComponent<VariableJoystick>();
            lineRenderer = GetComponent<LineRenderer>();
        }
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Rotate();
            Laser();
            Shoot();
        }
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
                //GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
                //bullet.GetComponent<Bullet>().ownerTankTurret = this;
                //ownerTankBody.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.Others);
                VibrationManager.Vibrate(100);
                StartCoroutine(CameraShaker.instance.Shake(.1f, .2f));
                StartCoroutine(ShootCoroutine());
            }
            aimJoystick.isDrop = false; //TODO: -- Event
        }
    }
    [PunRPC]
    void ShootRPC(Vector3 bulletStartPosition, Quaternion bulletStartRotation, int viewId)
    {
        GameObject bullet = Instantiate(turret.bullet, bulletStartPosition, bulletStartRotation);
        bullet.GetComponent<PhotonView>().ViewID = viewId;
        bullet.GetComponent<Bullet>().ownerTankTurret = this;
        smokePS.Play();
    }
    IEnumerator ShootCoroutine()
    {
        if (turret.bulletCount > 1)
        {
            for (int i = 0; i < turret.bulletCount; i++)
            {
                Quaternion randomRotate = Random.rotation;
                randomRotate = Quaternion.RotateTowards(shootPoint.rotation, randomRotate, 10);
                photonView.RPC("ShootRPC", RpcTarget.All, shootPoint.position, randomRotate, PhotonNetwork.AllocateViewID(false));
            }
        }
        else
        {
            photonView.RPC("ShootRPC", RpcTarget.All, shootPoint.position, shootPoint.rotation, PhotonNetwork.AllocateViewID(false));
        }
        ReduceAmmo();
        if (turret.renewalTime > 0)
        { 
            yield return new WaitForSeconds(turret.renewalTime);
            IncreaseAmmo();
        }
        else if (currentAmmo == 0)
        {
            photonView.RPC("RevertDefaultTurret", RpcTarget.All, photonView.ViewID);
        }
    }
    //TODO: -- photon instantiate dene
    void ReduceAmmo()
    {
        currentAmmo--;
    }
    void IncreaseAmmo()
    {
        currentAmmo++;
    }
    bool CoolDown()
    {
        if (timeStamp <= Time.time)
        {
            timeStamp = Time.time + turret.coolDown;
            return true;
        }
        return false;
    }//TODO: -- IEnumerator

    void Laser()
    {
        if (aimJoystick.Direction.magnitude > 0.3f)
        {
            LayerMask mask = LayerMask.GetMask("Raycast");
            hit = Physics2D.Raycast(shootPoint.position, shootPoint.up, distanceRay, mask);
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
    [PunRPC] //TODO: -- başka bir yöntem?
    void RevertDefaultTurret(int viewId)
    {
        TankTurret tankTurret = PhotonView.Find(viewId).gameObject.GetComponent<TankTurret>();
        tankTurret.turret = tankTurret.defaultTurret;
        tankTurret.spriteRenderer.sprite = tankTurret.defaultTurret.artwork;
        tankTurret.currentAmmo = tankTurret.defaultTurret.ammo;
    }
}
