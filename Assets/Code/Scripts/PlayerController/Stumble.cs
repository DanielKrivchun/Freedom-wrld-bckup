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
        if (Utils.IsLocalPlayer(Object))
        {
            Debug.Log("Stumbe  " + other.transform.root.name + "  My name " + transform.root.name);
            player.RPC_Stumble(player.MyName);
        }
    }
}
