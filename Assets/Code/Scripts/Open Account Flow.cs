using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAccountFlow : MonoBehaviour
{
    public GameObject AccountFlow;

    public void ToggleAccountFlow()
    {
        if (!AccountFlow.activeInHierarchy)
        {
            AccountFlow.SetActive(true);
        }
        else
        {
            AccountFlow.SetActive(false);
        }
    }
}