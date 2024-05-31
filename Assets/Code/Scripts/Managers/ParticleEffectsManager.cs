using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsManager : MonoBehaviour
{
    [SerializeField]
    ParticleSystem happyEffect;

    [SerializeField]
    ParticleSystem sleepEffect;

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

    public bool IsAllFoamCleared()
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
