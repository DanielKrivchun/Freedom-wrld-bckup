using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathObject : MonoBehaviour
{
    public Transform player;
    public GameObject foamBubblesHolder;

    [Space]
    public int cleanlinessMultiplier;

    [HideInInspector]
    public bool isReadyForBath, isSoapUsed, isShowerUsed;

    Vector3 startPos;

    private void OnEnable()
    {
        if (foamBubblesHolder != null)
        {
            foamBubblesHolder.SetActive(true);

            for (int i = 0; i < foamBubblesHolder.transform.childCount; i++)
            {
                foamBubblesHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        startPos = player.transform.position;
    }

    private void OnDisable()
    {
        if (foamBubblesHolder != null)
        {
            foamBubblesHolder.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (PetCareStateManager.instance.petDataRef.petData.cleanliness < 100)
        {
            isReadyForBath = true;
            isSoapUsed = false;
            isShowerUsed = false;
            player.transform.position = new Vector3(1.85f, 0.4f, 2f);
            player.GetComponent<CharacterController>().enabled = false;
        }
    }

    public IEnumerator ResetPlayerToMainPosition()
    {
        yield return new WaitForSeconds(2f);
        player.transform.position = startPos;
        player.GetComponent<CharacterController>().enabled = true;
    }
}
