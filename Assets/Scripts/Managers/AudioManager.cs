using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    bool isMusicActive;
    bool isSoundActive;

    public Sound[] sounds;
    private void Awake()
    {
        isMusicActive = true;
        isSoundActive = true;

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
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
        if (sound.type == Type.sound && isSoundActive)
        {
            sound.source.Play();
        }
        else if (sound.type == Type.music && isMusicActive)
        {
            sound.source.Play();
        }
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
            }
        }
    }
}
