using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            // Sync slider values from AudioManager
            masterVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.GetMasterVolume());
            musicVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
            sfxVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());

            // Add listeners to update AudioManager when sliders are changed
            masterVolumeSlider.onValueChanged.AddListener(value => AudioManager.Instance.ChangeMasterVolume(value));
            musicVolumeSlider.onValueChanged.AddListener(value => AudioManager.Instance.ChangeMusicVolume(value));
            sfxVolumeSlider.onValueChanged.AddListener(value => AudioManager.Instance.ChangeSfxVolume(value));
        }
    }
}