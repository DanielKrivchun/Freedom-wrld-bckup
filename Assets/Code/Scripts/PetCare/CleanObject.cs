using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanObject : MonoBehaviour
{
    public Transform player;
    public PetCareStateManager petCareStateManager;

    private void OnMouseDown()
    {
        petCareStateManager.isReadyForToilet = true;
        player.transform.position = new Vector3(4.5f, 1f, 4.5f);
    }
}
