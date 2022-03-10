using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;
    public AudioSource[] sources;

    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;


            sources = GetComponentsInChildren<AudioSource>();
        }
        else
        {
            DestroyObject(gameObject);
        }
    }



    public void StopSound(string soundName)
    {
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i].gameObject.name.CompareTo(soundName) == 0)
            {
                sources[i].Stop();
            }
        }
    }

    public void PlaySound(string soundName)
    {
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i].gameObject.name.CompareTo(soundName) == 0)
            {
                sources[i].Play();
            }
        }
    }

    public void StopAllSound()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].Stop();
        }
    }

    public void SoundAllMute()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].mute = true;
        }
    }

    public void SoundAllOn()
    {
        for (int i  = 0; i < sources.Length; i++)
        {
            sources[i].mute = false;
        }
    }

    public void BGMSoundSpeedOn()
    {
        sources[0].pitch = 1.1f;
    }

    public void BGMSoundSpeedOff()
    {
        sources[0].pitch = 1;
    }

}