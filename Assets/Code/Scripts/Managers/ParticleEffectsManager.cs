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
    
    public void CheckAndStartFoamBubbleEffect(Transform foamBubbleEffect)
    {
        foreach(ParticleSystem particle in foamBubbleEffects)
        {
            if(particle == foamBubbleEffect && !particle.isPlaying)
            {
                particle.Play();
            }
        }
    }

    public void CheckAndStopFoamBubbleEffect(Transform foamBubbleEffect)
    {
        foreach (ParticleSystem particle in foamBubbleEffects)
        {
            if (particle == foamBubbleEffect && !particle.isPlaying)
            {
                particle.Stop();
            }
        }
    }
}
