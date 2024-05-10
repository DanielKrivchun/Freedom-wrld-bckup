using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatObject : MonoBehaviour
{
    public PetCareInputManager petInputManager;

    private void OnMouseDown()
    {
        if (petInputManager.petCareStateManager.hunger < 100)
        {
            petInputManager.ShowPetEatingObject();
            gameObject.SetActive(false);
        }
    }
}
