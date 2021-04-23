using Photon.Pun;
using System.Collections;
using UnityEngine;

public class SpeedFeature : Feature
{
    [SerializeField] float newSpeed;
    [SerializeField] float time;

    public override void AddFeature(GameObject player)
    {
        StartCoroutine(SpeedFeatureCoroutine(player));
    }

    IEnumerator SpeedFeatureCoroutine(GameObject player)
    {
        player.gameObject.GetComponent<TankBody>().speed = newSpeed;
        photonView.RPC("CloseView", RpcTarget.All);

        yield return new WaitForSeconds(time);

        player.gameObject.GetComponent<TankBody>().speed = player.gameObject.GetComponent<TankBody>().defaultSpeed;
        photonView.RPC("CreateFeature", RpcTarget.MasterClient);
        photonView.RPC("DestroyFeatureRPC", RpcTarget.All);
        photonView.RPC("DecreaseFeatureSize", RpcTarget.MasterClient);
    }
}
