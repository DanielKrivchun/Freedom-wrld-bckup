using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float swipeSpeed = 5f; // Adjust the speed of the camera movement
    public float xOffset = -5f; // Minimum x position
    public float smoothTime = 0.3f; // Adjust the smooth time for damping

    private Vector2 startPos;
    private bool isDragging = false;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Check for mouse/touch input
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // If dragging, calculate swipe distance and move camera
        if (isDragging)
        {
            Vector2 currentPos = Input.mousePosition;
            float swipeDistance = currentPos.x - startPos.x;

            // Move the camera based on swipe distance
            Vector3 newPosition = transform.position + Vector3.right * swipeDistance * swipeSpeed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, -xOffset, xOffset);
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

            // Update the start position for the next frame
            startPos = currentPos;
        }
    }
}
