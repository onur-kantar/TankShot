using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureCreator : MonoBehaviourPun
{
    [SerializeField] ArenaCreator arenaCreator;
    [SerializeField] GameSceneManager gameSceneManager;
    [SerializeField] PreparePool preparePool;
    [SerializeField] int time;
    [SerializeField] int totalFeatureCount;
    [HideInInspector] public int currentFeatureCount;
    List<string> featurePrefabs;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            featurePrefabs = new List<string>();
            foreach (GameObject prefab in preparePool.prefabs)
            {
                if (prefab.tag == "Feature")
                {
                    featurePrefabs.Add(prefab.name);
                }
            }
            //featurePrefabs = preparePool.features;
            currentFeatureCount = 0;
            CreateFeature();
        }
    }
    public void CreateFeature()
    {
        StartCoroutine(FeatureCreatorCoroutine());

    }
    IEnumerator FeatureCreatorCoroutine()
    {            
        yield return new WaitForSeconds(time);
        if (currentFeatureCount < totalFeatureCount && currentFeatureCount >= 0)
        {
            int r, c;
            int feature = Random.Range(0, featurePrefabs.Count);
            do
            {
                r = Random.Range(0, arenaCreator.arena.GetLength(0));
                c = Random.Range(0, arenaCreator.arena.GetLength(1));
            }
            while (arenaCreator.arena[r, c].isThereFeature == true);

            GameObject go = PhotonNetwork.Instantiate(featurePrefabs[feature], arenaCreator.arena[r, c].position, Quaternion.identity);
            arenaCreator.arena[r, c].isThereFeature = true;
            photonView.RPC("FeatureCreatorRPC", RpcTarget.All, go.GetComponent<PhotonView>().ViewID);
            currentFeatureCount++;
        }
    }
    [PunRPC]
    void FeatureCreatorRPC(int viewId)
    {
        GameObject go = PhotonView.Find(viewId).gameObject;
        go.GetComponent<Feature>().featureCreator = this;
    }
}
