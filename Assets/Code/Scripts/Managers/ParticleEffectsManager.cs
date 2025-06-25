using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsManager : MonoBehaviour
{
    [Header("Pet Care Effects")]
    [SerializeField] ParticleSystem happyEffect;

    [SerializeField] ParticleSystem sleepEffect;

    [SerializeField] ParticleSystem fairyEffect;

    [SerializeField] ParticleSystem goldenFairyEffect;

    [Space]
    [SerializeField] List<ParticleSystem> foamBubbleEffects;

    [Header("Pet Training Effects")]
    [SerializeField] ParticleSystem runningDirtEffect;

    [SerializeField] ParticleSystem swimmingWaterSplashEffect;

    [SerializeField] ParticleSystem flyingWindEffect;

    [SerializeField] ParticleSystem puzzleEffect;

    [HideInInspector]
    public int numOfFoamBubbles = 0;

    #region PET CARE PARTICLE EFFECTS
    //Happy particle effect
    public void PlayHappyEffect()
    {
        if(!happyEffect.isPlaying)
        {
            happyEffect.Play();
        }
    }

    public void PlayFairyEffect() 
    {
        fairyEffect.Play();
    }

    public void PlayGoldenFairyEffect()
    {
        goldenFairyEffect.Play();
    }

    //Sleep particle effect
    public void StartSleepEffect() 
    { 
        sleepEffect.Play();
    }

    public void StopSleepEffect()
    {
        sleepEffect.Stop();
    }

    //Foam Bubble particle effect
    public void CheckAndStartFoamBubbleEffect(ParticleSystem foamBubbleEffect)
    {
        foreach (ParticleSystem particle in foamBubbleEffects)
        {
            if (particle == foamBubbleEffect && !particle.isPlaying)
            {
                particle.Play();
                numOfFoamBubbles++;
            }
        }
    }

    public void CheckAndStopFoamBubbleEffect(ParticleSystem foamBubbleEffect)
    {
        foreach (ParticleSystem particle in foamBubbleEffects)
        {
            if (particle == foamBubbleEffect && particle.isPlaying)
            {
                StartCoroutine(StopParticleEffect(particle));
            }
        }
    }

    IEnumerator StopParticleEffect(ParticleSystem particle)
    {
        yield return new WaitForSeconds(1.5f);
        particle.Stop();
        particle.gameObject.SetActive(false);
    }

    //Checking if all foam bubbles generated
    public bool IsAllFoamBubbleaGenerated()
    {
        foreach (ParticleSystem particle in foamBubbleEffects)
        {
            if (particle.isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    //Checking if all foam bubbles cleared
    public bool IsAllFoamBubblesCleared()
    {
        foreach (ParticleSystem particle in foamBubbleEffects)
        {
            if (particle.isPlaying)
            {
                return false;
            }
        }

        return true;
    }
    #endregion


    #region PET TRAINING PARTICLE EFFECTS
    //Running dirt particle effect
    public void StartRunningDirtEffect()
    {
        runningDirtEffect.Play();
    }

    public void StopRunningDirtEffect()
    {
        runningDirtEffect.Stop();
    }

    //Swimming water splash particle effect
    public void StartSwimmingWaterSplashEffect()
    {
        swimmingWaterSplashEffect.Play();
    }

    public void StopSwimmingWaterSplashEffect()
    {
        swimmingWaterSplashEffect.Stop();
    }

    //Flying wind particle effect
    public void StartFlyingWindEffect()
    {
        flyingWindEffect.Play();
    }

    public void StopFlyingWindEffect()
    {
        flyingWindEffect.Stop();
    }

    //Puzzle particle effect
    public void StartPuzzleEffect()
    {
        puzzleEffect.Play();
    }

    public void StopPuzzleEffect()
    {
        puzzleEffect.Stop();
    }
    #endregion
}
