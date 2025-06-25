using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetector : MonoBehaviour
{
    public ParticleEffectsManager particleEffectsManager;

    private void OnTriggerEnter(Collider other)
    {
        //Off foam bubble particles using raycast
        if (other.CompareTag(_Strings.FoamBubble))
        {
            particleEffectsManager.CheckAndStopFoamBubbleEffect(other.gameObject.GetComponent<ParticleSystem>());
        }
    }
}
