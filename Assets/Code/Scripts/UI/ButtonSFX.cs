using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public AudioClip sfxClip;

    public void PlaySound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxClip);
        }
    }
}