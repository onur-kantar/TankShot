using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPopup;

    public void ShowSettingsPanel()
    {
        settingsPopup.SetActive(true);
    }
    public void ShowLobbyPanel()
    {
        settingsPopup.SetActive(false);
    }
}
