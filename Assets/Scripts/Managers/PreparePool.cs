using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparePool : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs;
    void Start()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && prefabs != null && pool.ResourceCache.Count == 0)
        {
            foreach (GameObject prefab in prefabs)
            {
                pool.ResourceCache.Add(prefab.name, prefab);
            }
        }
    }

}
