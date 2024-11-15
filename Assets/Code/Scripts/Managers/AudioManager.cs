using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer mainAudioMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeVolumes(); // Initialize the volume levels at startup
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize AudioMixer volumes based on saved PlayerPrefs
    private void InitializeVolumes()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            mainAudioMixer.SetFloat("MasterVol", masterVolume);
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            mainAudioMixer.SetFloat("MusicVol", musicVolume);
        }

        if (PlayerPrefs.HasKey("SfxVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("SfxVolume");
            mainAudioMixer.SetFloat("SfxVol", sfxVolume);
        }
    }

    public void ChangeMasterVolume(float value)
    {
        mainAudioMixer.SetFloat("MasterVol", value);
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void ChangeMusicVolume(float value)
    {
        mainAudioMixer.SetFloat("MusicVol", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void ChangeSfxVolume(float value)
    {
        mainAudioMixer.SetFloat("SfxVol", value);
        PlayerPrefs.SetFloat("SfxVolume", value);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume()
    {
        mainAudioMixer.GetFloat("MasterVol", out float value);
        return value;
    }

    public float GetMusicVolume()
    {
        mainAudioMixer.GetFloat("MusicVol", out float value);
        return value;
    }

    public float GetSFXVolume()
    {
        mainAudioMixer.GetFloat("SfxVol", out float value); // Fetch from AudioMixer
        return value;
    }
}