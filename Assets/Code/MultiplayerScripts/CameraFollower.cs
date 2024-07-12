using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform player;
    public float distance = 3;
    public float height = 2;
    public float shoulderOffset = 2;
    public Vector3 HeightOffSet ;
    //public bool switchShoulder;
    public float smoothTime = 0.25f;

    Vector3 lookTarget;
    Vector3 lookTargetVelocity;
    Vector3 currentVelocity;
    private Vector3 target;
    private Vector3 verticalPosition;
    private Vector3 shoulderPosition;

    void LateUpdate()
    {
        target = player.position + (-player.transform.forward * distance);

        verticalPosition = Vector3.up * height;
        //shoulderPosition = switchShoulder ? transform.right * -shoulderOffset : transform.right * shoulderOffset;
        shoulderPosition = transform.right * shoulderOffset;

        transform.position = Vector3.SmoothDamp(transform.position, target + shoulderPosition + verticalPosition, ref currentVelocity, smoothTime);

        lookTarget = Vector3.SmoothDamp(lookTarget, player.position + verticalPosition + shoulderPosition + HeightOffSet, ref lookTargetVelocity, smoothTime);
        transform.LookAt(lookTarget);
    }
}

