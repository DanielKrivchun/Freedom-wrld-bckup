using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Start()
    {
        // Load the saved volume levels and apply them to the sliders
        masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
        musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();

        // Add listeners to call AudioManager methods when sliders change
        masterVolumeSlider.onValueChanged.AddListener(value => AudioManager.Instance.ChangeMasterVolume(value));
        musicVolumeSlider.onValueChanged.AddListener(value => AudioManager.Instance.ChangeMusicVolume(value));
        sfxVolumeSlider.onValueChanged.AddListener(value => AudioManager.Instance.ChangeSfxVolume(value));
    }
}