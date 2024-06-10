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
    ParticleSystem flyingWindEffect;

    [Space]
    [SerializeField]
    List<ParticleSystem> foamBubbleEffects;

    public int numOfFoamBubbles = 0;

    //Happy particle effect
    public void PlayHappyEffect()
    {
        if(!happyEffect.isPlaying)
        {
            happyEffect.Play();
        }
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

    //Flying wind particle effect
    public void StartFlyingWindEffect()
    {
        flyingWindEffect.Play();
    }

    public void StopFlyingWindEffect()
    {
        flyingWindEffect.Stop();
    }

    //Foam Bubble particle effect
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
                StartCoroutine(StopParticleEffect(particle));
            }
        }
    }

    IEnumerator StopParticleEffect(ParticleSystem particle)
    {
        yield return new WaitForSeconds(1f);
        particle.Stop();
        particle.gameObject.SetActive(false);
    }

    public bool IsAllFoamBubbleaGenerated()
    {
        foreach (ParticleSystem particle in foamBubbleEffects)
        {
            if (particle.gameObject.activeInHierarchy)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsAllFoamBubblesCleared()
    {
        foreach (ParticleSystem particle in foamBubbleEffects)
        {
            if (particle.gameObject.activeInHierarchy)
            {
                return false;
            }
        }

        return true;
    }
}
