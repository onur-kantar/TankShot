using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel(sceneName);
        while (PhotonNetwork.LevelLoadingProgress < 1f)
        {
            yield return null;
        }
    }
}
