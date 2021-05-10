using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    bool isMusicActive;
    bool isSoundActive;
    bool isVibrationActive;

    [SerializeField] TMP_Text musicButtonText;
    [SerializeField] TMP_Text soundButtonText;
    [SerializeField] TMP_Text vibrationButtonText;

    public Sound[] sounds;
    private void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
        GetMusic();
        GetSound();
        GetVibration();
    }

    private void Start()
    {
        Play("LobbyMusic");
    }
    public void Play (string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound.name == null)
            return;
        sound.source.Play();
    }
    public void ToggleSounds()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.type == Type.sound)
            {
                if (isSoundActive)
                {
                    sound.source.volume = 0;
                }
                else
                {
                    sound.source.volume = sound.volume;
                }
                isSoundActive = !isSoundActive;
                PlayerPrefs.SetInt("sound", Convert.ToInt32(isSoundActive));
                GetSound();
            }
        }
    }
    public void ToggleMusics()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.type == Type.music)
            {
                if (isMusicActive)
                {
                    sound.source.volume = 0;
                }
                else
                {
                    sound.source.volume = sound.volume;
                }
                isMusicActive = !isMusicActive;
                PlayerPrefs.SetInt("music", Convert.ToInt32(isMusicActive));
                GetMusic();
            }
        }
    }

    void GetMusic()
    {
        isMusicActive = Convert.ToBoolean(PlayerPrefs.GetInt("music", 1));
        foreach (Sound sound in sounds)
        {
            if (sound.type == Type.music)
            {
                if (isMusicActive)
                {
                    sound.source.volume = sound.volume;
                    musicButtonText.text = "MUSIC\nON";
                }
                else
                {
                    sound.source.volume = 0;
                    musicButtonText.text = "MUSIC\nOFF";
                }
            }
        }
    }
    void GetSound()
    {
        isSoundActive = Convert.ToBoolean(PlayerPrefs.GetInt("sound", 1));
        foreach (Sound sound in sounds)
        {
            if (sound.type == Type.sound)
            {
                if (isSoundActive)
                {
                    sound.source.volume = sound.volume;
                    soundButtonText.text = "SOUND\nON";
                }
                else
                {
                    sound.source.volume = 0;
                    soundButtonText.text = "SOUND\nOFF";
                }
            }
        }
    }
    void GetVibration()
    {
        isVibrationActive = Convert.ToBoolean(PlayerPrefs.GetInt("vibration", 1));

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
