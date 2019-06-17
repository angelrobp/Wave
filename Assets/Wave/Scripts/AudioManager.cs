﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Static Instance
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned AudioManager", typeof(AudioManager)).GetComponent<AudioManager>();
                }
            }
            return instance;
        }        
        private set
        {
            instance = value;
        }

    }
    #endregion

    #region Fields
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource musicSource2;
    [SerializeField]
    private AudioSource sfxSource;

    [Header("List of characters")]
    [SerializeField]
    private List<AudioClip> clipList = new List<AudioClip>();

    private bool firstMusicSourcePlaying;
    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;

            //Evitar destrucción del objeto entre escenas
            DontDestroyOnLoad(this.gameObject);

            //Crea fuentes de audio y guarda las referencias
            musicSource = this.gameObject.AddComponent<AudioSource>();
            musicSource2 = this.gameObject.AddComponent<AudioSource>();
            sfxSource = this.gameObject.AddComponent<AudioSource>();

            //Bucle de la musica principal
            musicSource.loop = true;
            musicSource2.loop = true;

            PlayMusic(clipList[0]);
        }
    }

    public AudioClip getClip (int id)
    {
        return clipList[id];
    }
    public void PlayMusic(AudioClip musicClip)
    {
        //Determina que ruta esta activa
        AudioSource activeSource = (firstMusicSourcePlaying) ? musicSource : musicSource2;

        activeSource.clip = musicClip;
        activeSource.volume = 1;
        activeSource.Play();
    }

    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        //Determina que ruta esta activa
        AudioSource activeSource = (firstMusicSourcePlaying) ? musicSource : musicSource2;

        StartCoroutine(UpdateMusicWithFade(activeSource, newClip, transitionTime));

    }

    public void PlayMusicWithCrossFade(AudioClip musicClip, float transitionTime = 1.0f)
    {
        //Determina que ruta esta activa
        AudioSource activeSource = (firstMusicSourcePlaying) ? musicSource : musicSource2;
        AudioSource newSource = (firstMusicSourcePlaying) ? musicSource2 : musicSource;

        //Intercambia las pistas
        firstMusicSourcePlaying = !firstMusicSourcePlaying;

        //Cambia las pistas y comienza el crossfade
        newSource.clip = musicClip;
        newSource.Play();
        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime));
    }


    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        //Comprobacion de la ruta que esta activa y reproduciendose
        if (!activeSource.isPlaying)
            activeSource.Play();

        float t = 0.0f;

        //Fade out
        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }

        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();

        //Fade in
        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }
    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime)
    {
        float t = 0.0f;

        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            original.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            yield return null;
        }

        original.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        musicSource2.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
