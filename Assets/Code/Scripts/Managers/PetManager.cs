using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetManager : MonoBehaviour
{
    public NeedsController needsController;

    public static PetManager instance;
    private void Awake()
    { //Do not set anything in the instance in an awake because it will be called before this is initialized

        if (instance == null)
        { //Video mentions this might not be the best way to do this for a BIG game. (Not exactly sure why)
            instance = this;
        }
        else
            Debug.LogWarning("More than one PetManage in the Scene");
    }

    public void Die()
    {
        Debug.Log("Dead");
    }
}
