using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowerLineJoint : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public List<Transform> jointTransform;

    void Start()
    {
        // Set the initial positions for the line renderer
        lineRenderer.positionCount = jointTransform.Count;

        for (int i = 0; i < jointTransform.Count; i++)
        {
            lineRenderer.SetPosition(i, jointTransform[i].position);
        }
    }

    void Update()
    {
        // Update the positions of the line renderer to follow the draggable object
        for (int i = 0; i < jointTransform.Count; i++)
        {
            lineRenderer.SetPosition(i, jointTransform[i].position);
        }    
    }
}
