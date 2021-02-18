using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
{
    [SerializeField] MonoBehaviour[] scripts;
    [SerializeField] GameObject turret;
    void Start()
    {
        //PhotonNetwork.SendRate = 200;
        //PhotonNetwork.SerializationRate = 100;
        if (!photonView.IsMine)
        {
            foreach (MonoBehaviour script in scripts)
            {
                Destroy(script);
            }
        }
    }
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(turret.transform.up);
    //    }
    //    else
    //    {
    //        turret.transform.up = (Vector3)stream.ReceiveNext();
    //    }
    //}
}
