using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFeature : Feature
{
    [SerializeField] float newSpeed;
    [SerializeField] float time;
    public override void AddFeature(GameObject player)
    {
        _player = player;
        StartCoroutine(SpeedFeatureCoroutine(player));
    }

    public override void RemoveFeature()
    {
        _player.GetComponent<TankBody>().features.Remove(this);
        _player.gameObject.GetComponent<TankBody>().speed = _player.gameObject.GetComponent<TankBody>().defaultSpeed;
        photonView.RPC("DestroyFeatureRPC", RpcTarget.All);
        photonView.RPC("DecreaseFeatureSize", RpcTarget.MasterClient);
    }

    IEnumerator SpeedFeatureCoroutine(GameObject player)
    {
        _player = player;
        _player.gameObject.GetComponent<TankBody>().speed = newSpeed;
        photonView.RPC("FeatureCollected", RpcTarget.All);

        yield return new WaitForSeconds(time);

        RemoveFeature();
    }
}
