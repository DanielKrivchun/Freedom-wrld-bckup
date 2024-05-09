using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanObject : MonoBehaviour
{
    public Transform player;
    public PetCareStateManager petCareStateManager;

    private void OnMouseDown()
    {
        petCareStateManager.isReadyForBath = true;
        player.transform.position = new Vector3(5.5f, 1f, 5.7f);
    }
}
