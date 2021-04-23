using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPopup;
    bool music;
    bool voice;
    bool connection;
    [SerializeField] TMP_Text musicButtonText;
    [SerializeField] TMP_Text voiceButtonText;
    [SerializeField] TMP_Text connectButtonText;

    void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        connection = true;
        music = true;
        voice = true;
    }
    public void ChangeMusic()
    {
        music = !music;
        if (music)
        {
            musicButtonText.text = "MUSIC ON";

        }
        else
        {
            musicButtonText.text = "MUSIC OFF";
        }
    }
    public void ChangeVoice()
    {
        voice = !voice;
        if (voice)
        {
            voiceButtonText.text = "VOICE ON"; ;
        }
        else
        {
            voiceButtonText.text = "VOICE OFF"; ;
        }
    }
    public void ChangeConnection()
    {
        connection = !connection;
        if (connection)
        {
            connectButtonText.text = "CONNECT";
        }
        else
        {
            connectButtonText.text = "DISCONNECT";
        }
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
