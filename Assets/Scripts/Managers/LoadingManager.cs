using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [HideInInspector] public bool photonIsLoaded = false;
    [HideInInspector] public bool firebaseIsLoaded = false;
    [HideInInspector] public bool hasError = false;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject errorScreen;
    bool sceneIsLoading = false;
    private void Update()
    {
        if (hasError && !errorScreen.activeInHierarchy)
        {
           loadingScreen.SetActive(false);
           errorScreen.SetActive(true);
        }
        else if(photonIsLoaded && firebaseIsLoaded && !sceneIsLoading)
        {
            StartCoroutine(LoadAsynchronously());
        }
    }
    IEnumerator LoadAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Lobby");
        sceneIsLoading = true;
        while (!operation.isDone)
        {
            yield return null;
        }
    }
    public void Reload()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
