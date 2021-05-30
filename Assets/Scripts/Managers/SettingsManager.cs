using System;
using TMPro;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    bool isMusicActive;
    bool isSoundActive;
    bool isVibrationActive;

    [SerializeField] TMP_Text musicButtonText;
    [SerializeField] TMP_Text soundButtonText;
    [SerializeField] TMP_Text vibrationButtonText;

    private void Start()
    {
        GetMusic();
        GetSound();
        GetVibration();
    }

    public void ToggleMusics()
    {
        isMusicActive = !isMusicActive;
        PlayerPrefs.SetInt("music", Convert.ToInt32(isMusicActive));
        GetMusic();
    }
    public void ToggleSounds()
    {
        isSoundActive = !isSoundActive;
        PlayerPrefs.SetInt("sound", Convert.ToInt32(isSoundActive));
        GetSound();
    }
    public void ToggleVibration()
    {
        isVibrationActive = !isVibrationActive;
        PlayerPrefs.SetInt("vibration", Convert.ToInt32(isVibrationActive));
        GetVibration();
    }
    void GetMusic()
    {
        isMusicActive = Convert.ToBoolean(PlayerPrefs.GetInt("music", 1));
        foreach (Sound sound in AudioManager.instance.sounds)
        {
            if (sound.type == Type.music)
            {
                if (isMusicActive)
                {
                    musicButtonText.text = "MUSIC\nON";
                    sound.source.volume = sound.volume;
                }
                else
                {
                    musicButtonText.text = "MUSIC\nOFF";
                    sound.source.volume = 0;
                }
            }
        }
    }
    void GetSound()
    {
        isSoundActive = Convert.ToBoolean(PlayerPrefs.GetInt("sound", 1));
        foreach (Sound sound in AudioManager.instance.sounds)
        {
            if (sound.type == Type.sound)
            {
                if (isSoundActive)
                {
                    soundButtonText.text = "SOUND\nON";
                    sound.source.volume = sound.volume;
                }
                else
                {
                    soundButtonText.text = "SOUND\nOFF";
                    sound.source.volume = 0;
                }
            }
        }
    }
    void GetVibration()
    {
        isVibrationActive = Convert.ToBoolean(PlayerPrefs.GetInt("vibration", 1));
        VibrationManager.isVibrationActive = isVibrationActive;
        if (isVibrationActive)
        {
            vibrationButtonText.text = "VIBRATION\nON";
        }
        else
        {
            vibrationButtonText.text = "VIBRATION\nOFF";
        }
    }
}
