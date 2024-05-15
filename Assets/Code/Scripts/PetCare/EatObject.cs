using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatObject : MonoBehaviour
{
    public PetCareInputManager petInputManager;
    public PetCareStateManager petCareStateManager;

    private void OnMouseDown()
    {
        if (petCareStateManager.petDataRef.petData.hunger < 100)
        {
            petInputManager.ShowPetEatingObject();
            gameObject.SetActive(false);
        }
    }
}
