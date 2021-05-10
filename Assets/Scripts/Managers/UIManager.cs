using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPopup;

    void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
    }
    public void ShowSettingsPanel()
    {
        settingsPopup.SetActive(true);
    }
    public void ShowLobbyPanel()
    {
        settingsPopup.SetActive(false);
    }
}
