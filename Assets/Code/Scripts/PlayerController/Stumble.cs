using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;

public class Stumble : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (Runner.IsServer)
        {
            Debug.Log("Stumbe  " + other.gameObject.name);
            switch (other.tag)
            {

            }
        }
    }
}
