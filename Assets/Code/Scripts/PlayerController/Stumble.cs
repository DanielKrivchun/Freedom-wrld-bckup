using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;

public class Stumble : NetworkBehaviour
{

    public NavmeshMultiplayer player;

    private void OnTriggerEnter(Collider other)
    {
        if (Runner.IsServer)
        {
            Debug.Log("Stumbe  " + other.gameObject.name + "  My name " + gameObject.name);
            switch (other.tag)
            {

            }
        }
    }
}
