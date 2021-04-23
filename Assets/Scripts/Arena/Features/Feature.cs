using Photon.Pun;
using UnityEngine;

public abstract class Feature : MonoBehaviourPun
{
    [HideInInspector] public FeatureCreator featureCreator;

    private void Start()
    {
        CreateFeature();
    }
    [PunRPC]
    public void CreateFeature()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            featureCreator.CreateFeature();
        }
    }
    [PunRPC]
    public void CloseView()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }
    [PunRPC]
    public void DestroyFeatureRPC()
    {
        Destroy(gameObject);
    }
    [PunRPC]
    public void DecreaseFeatureSize()
    {
        featureCreator.currentFeatureCount--;
    }
    public abstract void AddFeature(GameObject player);

}