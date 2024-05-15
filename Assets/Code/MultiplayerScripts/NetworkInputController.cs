using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInputController : MonoBehaviour
{
}

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
}
