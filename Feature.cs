using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public abstract class Feature : MonoBehaviourPun
{
    public GameObject _player;
    [HideInInspector] public FeatureCreator featureCreator;
    [PunRPC]
    public void FeatureCollected()
    {
        GameObject featurePS = Resources.Load<GameObject>("FeaturePS");
        Instantiate(featurePS, transform.position, Quaternion.identity);
        AudioManager.instance.Play("Feature");
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
    public abstract void RemoveFeature();

}