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
    [SerializeField] GameObject errorScreen;

    public void ShowSettingsPanel()
    {
        settingsPopup.SetActive(true);
    }
    public void ShowLobbyPanel()
    {
        settingsPopup.SetActive(false);
    }

    public void ShowErrorScreen()
    {
        errorScreen.SetActive(true);
    }
}
