using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ShieldFeature : Feature
{
    [SerializeField] float time;

    public override void AddFeature(GameObject player)
    {
        StartCoroutine(SpeedFeatureCoroutine(player));
    }

    IEnumerator SpeedFeatureCoroutine(GameObject player)
    {
        GameObject shield = PhotonNetwork.Instantiate("Shield", player.transform.position, Quaternion.identity);
        photonView.RPC("ShieldRPC", RpcTarget.All, player.GetComponent<PhotonView>().ViewID, shield.GetComponent<PhotonView>().ViewID);
        photonView.RPC("CloseView", RpcTarget.All);

        yield return new WaitForSeconds(time);

        shield.GetComponent<PhotonView>().RPC("DestroyShieldRPC", RpcTarget.All);
        photonView.RPC("CreateFeature", RpcTarget.MasterClient);
        photonView.RPC("DestroyFeatureRPC", RpcTarget.All);
        photonView.RPC("DecreaseFeatureSize", RpcTarget.MasterClient);
        //PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    void ShieldRPC(int playerID, int shieldID)
    {
        GameObject player = PhotonView.Find(playerID).gameObject;
        GameObject shield = PhotonView.Find(shieldID).gameObject;

        shield.GetComponent<Shield>().owner = player.GetComponent<TankBody>();
    }

}
