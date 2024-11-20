using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer mainAudioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadMasterVolume();
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadMusicVolume();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadSFXVolume();
        }

    }

    public void ChangeMasterVolume(float sliderValue)
    {
        mainAudioMixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);

        PlayerPrefs.SetFloat("masterVolume", sliderValue); // Save the slider value (not dB)
    }

    public void ChangeMusicVolume(float sliderValue)
    {
        mainAudioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);

        PlayerPrefs.SetFloat("musicVolume", sliderValue); // Save the slider value (not dB)
    }

    public void ChangeSfxVolume(float sliderValue)
    {
        mainAudioMixer.SetFloat("SfxVol", Mathf.Log10(sliderValue) * 20);

        PlayerPrefs.SetFloat("sfxVolume", sliderValue); // Save the slider value (not dB)
    }

    public void LoadMasterVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        ChangeMasterVolume(masterSlider.value);
    }

    public void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        ChangeMusicVolume(musicSlider.value);
    }

    public void LoadSFXVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        ChangeSfxVolume(sfxSlider.value);
    }

    public float GetMasterVolume()
    {
        if (mainAudioMixer.GetFloat("MasterVol", out float dbValue))
        {
            float minDb = -80f;
            float maxDb = 0f;
            // Map dB back to slider's 0-1 range
            return (dbValue - minDb) / (maxDb - minDb);
        }
        return 0.5f; // Default to 50% if no value exists
    }

    public float GetMusicVolume()
    {
        if (mainAudioMixer.GetFloat("MusicVol", out float dbValue))
        {
            float minDb = -80f;
            float maxDb = 0f;
            // Map dB back to slider's 0-1 range
            return (dbValue - minDb) / (maxDb - minDb);
        }
        return 0.5f; // Default to 50% if no value exists
    }

    public float GetSFXVolume()
    {
        if (mainAudioMixer.GetFloat("SfxVol", out float dbValue))
        {
            float minDb = -80f;
            float maxDb = 0f;
            // Map dB back to slider's 0-1 range
            return (dbValue - minDb) / (maxDb - minDb);
        }
        return 0.5f; // Default to 50% if no value exists
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When a new scene is loaded, sync settings if the sliders exist
        SettingsUIManager settingsUI = FindObjectOfType<SettingsUIManager>();
        if (settingsUI != null)
        {
            // Update sliders from current AudioManager state
            settingsUI.masterVolumeSlider.SetValueWithoutNotify(GetMasterVolume());
            settingsUI.musicVolumeSlider.SetValueWithoutNotify(GetMusicVolume());
            settingsUI.sfxVolumeSlider.SetValueWithoutNotify(GetSFXVolume());
        }
    }
}