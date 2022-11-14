using UnityEngine.UI;
using UnityEngine;
using System;

public static class SoundMessages
{
    public static Action<AudioClip> PlayMusic;
    public static Action<AudioClip> PlaySFX;
    public static Action PlayClickSFX;
}

public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Audio UI")]
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    [Header("Sound Base Settings")]
    [SerializeField] private float baseMusicSound;
    [SerializeField] private float baseSFXSound;

    [Header("Common Sounds")]
    [SerializeField] private AudioClip clickSound; 


    private void Awake()
    {
        VolumeInit();

        SoundMessages.PlayMusic += PlayMusic;
        SoundMessages.PlaySFX += PlaySFX;
        SoundMessages.PlayClickSFX += PlayClickSound;

        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void OnDestroy()
    {
        VolumeDeInit();

        SoundMessages.PlayMusic -= PlayMusic;
        SoundMessages.PlaySFX -= PlaySFX;
        SoundMessages.PlayClickSFX -= PlayClickSound;

        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }

    private void VolumeInit()
    {
        //Music Volume Init
        musicSource.volume = PlayerPrefs.GetFloat("Music Volume", baseMusicSound);
        musicVolumeSlider.value = musicSource.volume;

        //SFX Volume Init
        sfxSource.volume = PlayerPrefs.GetFloat("SFX Volume", baseSFXSound);
        sfxVolumeSlider.value = sfxSource.volume;

    }

    private void VolumeDeInit()
    {
        PlayerPrefs.SetFloat("Music Volume", musicSource.volume);
        PlayerPrefs.SetFloat("SFX Volume", sfxSource.volume);
    }    

    private void SetMusicVolume(float changedVolume)
    {
        musicSource.volume = changedVolume;

    }

    private void SetSFXVolume(float changedVolume)
    {
        sfxSource.volume = changedVolume;
    }

    private void PlayMusic(AudioClip musicClip)
    {
        if(musicSource.isPlaying) musicSource.Stop();
        musicSource.clip = musicClip;
        musicSource.Play();

    }

    private void PlaySFX(AudioClip audioClip)
    {
        sfxSource.PlayOneShot(audioClip);
    }

    private void PlayClickSound()
	{
        PlaySFX(clickSound);
	}
}
