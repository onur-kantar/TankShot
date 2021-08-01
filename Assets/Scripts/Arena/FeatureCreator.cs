using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureCreator : MonoBehaviourPun
{
    [SerializeField] ArenaCreator arenaCreator;
    [SerializeField] GameSceneManager gameSceneManager;
    [SerializeField] PreparePool preparePool;
    [SerializeField] float time;
    [SerializeField] int totalFeatureCount;
    [HideInInspector] public int currentFeatureCount;
    [HideInInspector] public bool isCreating;
    List<string> featurePrefabs;

    [SerializeField] GameObject featurePref;

    void Start()
    {
        isCreating = false;
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
            currentFeatureCount = 0;
        }
    }
    private void Update()
    {
        if (!gameSceneManager.isMatchOver && currentFeatureCount < totalFeatureCount && currentFeatureCount >= 0)
        {
            StartCoroutine(FeatureCreatorCoroutine());
            isCreating = false;
        }
    }
    IEnumerator FeatureCreatorCoroutine()
    {
        if (isCreating)
        {
            yield return new WaitForSeconds(time);
            if (!gameSceneManager.isMatchOver)
            {
                int r, c;
                int feature = Random.Range(0, featurePrefabs.Count);
                do
                {
                    r = Random.Range(0, arenaCreator.arena.GetLength(0));
                    c = Random.Range(0, arenaCreator.arena.GetLength(1));
                }
                while (arenaCreator.arena[r, c].isThereFeature == true);

                GameObject go = PhotonNetwork.Instantiate(featurePrefabs[feature], arenaCreator.arena[r, c].position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                arenaCreator.arena[r, c].isThereFeature = true;
                photonView.RPC("FeatureCreatorRPC", RpcTarget.All, go.GetComponent<PhotonView>().ViewID);
                currentFeatureCount++;
            }
        }
        isCreating = true;
    }
    [PunRPC]
    void FeatureCreatorRPC(int viewId)
    {
        GameObject go = PhotonView.Find(viewId).gameObject;
        go.GetComponent<Feature>().featureCreator = this;
    }
}
