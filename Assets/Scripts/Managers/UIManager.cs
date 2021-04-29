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
    bool sound;
    bool vibration;
    [SerializeField] TMP_Text musicButtonText;
    [SerializeField] TMP_Text soundButtonText;
    [SerializeField] TMP_Text vibrationButtonText;

    void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        vibration = true;
        music = true;
        sound = true;
    }
    public void ToggleMusicUI()
    {
        music = !music;
        if (music)
        {
            musicButtonText.text = "MUSIC\nON";

        }
        else
        {
            musicButtonText.text = "MUSIC\nOFF";
        }
    }
    public void ToggleSoundUI()
    {
        sound = !sound;
        if (sound)
        {
            soundButtonText.text = "SOUND\nON"; ;
        }
        else
        {
            soundButtonText.text = "SOUND\nOFF"; ;
        }
    }
    public void ToggleVibrationUI()
    {
        vibration = !vibration;
        if (vibration)
        {
            vibrationButtonText.text = "VIBRATION\nON";
        }
        else
        {
            vibrationButtonText.text = "VIBRATION\nOFF";
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
