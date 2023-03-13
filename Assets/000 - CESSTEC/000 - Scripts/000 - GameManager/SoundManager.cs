using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private event EventHandler VolumeChange;
    public event EventHandler OnVolumeChange
    {
        add
        {
            if (VolumeChange == null || !VolumeChange.GetInvocationList().Contains(value))
                VolumeChange += value;
        }
        remove { VolumeChange -= value; }
    }
    public float CurrentVolume
    {
        get => currentVolume;
        set
        {
            currentVolume = value;
            VolumeChange?.Invoke(this, EventArgs.Empty);
        }
    }

    [SerializeField] private AudioSource bgSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;

    [Header("DEBUGGER")]
    [ReadOnly] [SerializeField] private float currentVolume;

    //  ==============================

    int voiceNarration;

    //  ==============================

    private void Awake()
    {
        StartCoroutine(CheckVolumeSaveData());
        OnVolumeChange += VolumeCheck;
    }

    private void OnDisable()
    {
        OnVolumeChange -= VolumeCheck;
    }

    private void VolumeCheck(object sender, EventArgs e)
    {
        ChangeVolume();
    }

    public IEnumerator CheckVolumeSaveData()
    {
        if (!PlayerPrefs.HasKey("volumeData"))
        {
            CurrentVolume = 1;
            PlayerPrefs.SetFloat("volumeData", 1);
        }
        else
            CurrentVolume = PlayerPrefs.GetFloat("volumeData");

        yield return null;
    }

    public void SetBGMusic(AudioClip clip)
    {
        LeanTween.value(gameObject, CurrentVolume, 0f, 0.25f).setEase(LeanTweenType.easeOutCubic).setOnUpdate((float val) =>
        {
            bgSource.volume = val;
        }).setOnComplete(() =>
        {
            bgSource.Stop();
            bgSource.clip = clip;
            bgSource.Play();
            LeanTween.value(gameObject, 0f, CurrentVolume, 0.25f).setEase(LeanTweenType.easeOutCubic).setOnUpdate((float val) =>
            {
                bgSource.volume = val;
            }).setOnComplete(() =>
            {
                bgSource.volume = CurrentVolume;
            });
        });
    }

    public void PlayVoiceNarration(AudioClip clip)
    {
        voiceNarration = LeanTween.value(gameObject, 0f, CurrentVolume, 0.25f).setEase(LeanTweenType.easeOutCubic).setOnUpdate((float val) =>
        {
            voiceSource.clip = clip;
            voiceSource.Play();
            voiceSource.volume = val;
        }).id;
    }

    public void StopVoiceNarration()
    {
        if (voiceNarration != 0) LeanTween.cancel(voiceNarration);

        voiceNarration = LeanTween.value(gameObject, CurrentVolume, 0f, 0.25f).setEase(LeanTweenType.easeOutCubic).setOnUpdate((float val) =>
        {
            voiceSource.Stop();
            voiceSource.clip = null;
        }).id;
    }

    public void PlaySFX(AudioClip clip) => sfxSource.PlayOneShot(clip);

    private void ChangeVolume()
    {
        if (voiceNarration != 0) LeanTween.cancel(voiceNarration);

        //bgSource.volume = CurrentVolume;
        //sfxSource.volume = CurrentVolume;
        voiceSource.volume = CurrentVolume;
    }
}
