using UnityEngine;
using Photon.Pun;

public class TurretFeature : Feature
{
    [SerializeField] Turret turret;

    public override void AddFeature(GameObject player)
    {
        photonView.RPC("ChangeTurret", RpcTarget.All, player.GetPhotonView().ViewID, photonView.ViewID);
        photonView.RPC("CreateFeature", RpcTarget.MasterClient);//RPC yaz sonuna
        photonView.RPC("DestroyFeatureRPC", RpcTarget.All);
        photonView.RPC("DecreaseFeatureSize", RpcTarget.MasterClient);
    }
    [PunRPC]
    void ChangeTurret(int viewId, int turretViewId)
    {
        TankTurret player = PhotonView.Find(viewId).gameObject.GetComponent<TankBody>().ownTurret;
        Turret turretFeature = PhotonView.Find(turretViewId).gameObject.GetComponent<TurretFeature>().turret;

        player.turret = turretFeature;
        player.spriteRenderer.sprite = turretFeature.artwork;
        player.currentAmmo = turretFeature.ammo;
    }
}
