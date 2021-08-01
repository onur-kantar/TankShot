using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class TurretFeature : Feature
{
    [SerializeField] TurretAttribute turret;
    public override void AddFeature(GameObject player)
    {
        _player = player;
        photonView.RPC("ChangeTurret", RpcTarget.All, player.GetPhotonView().ViewID, photonView.ViewID);
        RemoveFeature();
    }
    public override void RemoveFeature()
    {
        _player.GetComponent<TankBody>().features.Remove(this);
        photonView.RPC("FeatureCollected", RpcTarget.All);
        photonView.RPC("DestroyFeatureRPC", RpcTarget.All);
        photonView.RPC("DecreaseFeatureSize", RpcTarget.MasterClient);
    }
    [PunRPC]
    void ChangeTurret(int viewId, int turretViewId)
    {
        TankTurret player = PhotonView.Find(viewId).gameObject.GetComponent<TankBody>().ownTurret;
        TurretAttribute turretFeature = PhotonView.Find(turretViewId).gameObject.GetComponent<TurretFeature>().turret;

        player.turret = turretFeature;
        player.spriteRenderer.sprite = turretFeature.artwork;
        player.currentAmmo = turretFeature.ammo;
    }
}
