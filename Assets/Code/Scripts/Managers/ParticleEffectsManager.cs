using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsManager : MonoBehaviour
{
    [SerializeField]
    ParticleSystem happyEffect;

    [SerializeField]
    ParticleSystem sleepEffect;

    [SerializeField]
    List<ParticleSystem> foamBubbleEffects;

    public int numOfFoamBubbles = 0;

    public void PlayHappyEffect()
    {
        if(!happyEffect.isPlaying)
        {
            happyEffect.Play();
        }
    }

    public void StartSleepEffect() 
    { 
        sleepEffect.Play();
    }

    public void StopSleepEffect()
    {
        sleepEffect.Stop();
    }
    
    public void CheckAndStartFoamBubbleEffect(ParticleSystem foamBubbleEffect)
    {
        foreach(ParticleSystem particle in foamBubbleEffects)
        {
            if(particle == foamBubbleEffect && !particle.isPlaying)
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
                particle.Stop();
                particle.gameObject.SetActive(false);
                numOfFoamBubbles--;
            }
        }
    }

    public bool IsAllFoamCleared()
    {
        foreach (ParticleSystem particle in foamBubbleEffects)
        {
            if (particle.gameObject.activeInHierarchy)
            {
                return false;
            }
        }

        numOfFoamBubbles = 0;
        return true;
    }
}
