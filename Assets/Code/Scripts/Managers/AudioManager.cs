using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer mainAudioMixer;
    public Slider masterVol, musicVol, sfxVol;

    private AudioSource musicSource;

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

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true; // Make the music loop
    }

    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("MasterVol", masterVol.value);
    }

    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("MusicVol", musicVol.value);
    }

    public void ChangeSfxVolume()
    {
        mainAudioMixer.SetFloat("SfxVol", sfxVol.value);
    }
}