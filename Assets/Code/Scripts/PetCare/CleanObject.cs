using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanObject : MonoBehaviour
{
    public Transform player;
    public PetCareStateManager petCareStateManager;

    private void OnMouseDown()
    {
        if(petCareStateManager.petDataRef.petData.cleanliness < 100)
        {
            petCareStateManager.isReadyForBath = true;
            player.transform.position = new Vector3(1.85f, 0.4f, 1.85f);
        }
    }
}
