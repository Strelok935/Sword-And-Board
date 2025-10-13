using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuParallax : MonoBehaviour
{
    public float offsetMultiplier = 1f;
    public float smoothTime = 0.3f;
    public Vector3 anchorPosition = Vector3.zero; // Anchor position field

    private Vector3 currentVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the camera's position to the anchor position at the start
        transform.position = anchorPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 offset = Camera.main.ScreenToViewportPoint(mousePosition);

            // Calculate the target position relative to the anchor position
            Vector3 targetPosition = anchorPosition + (Vector3)(offset * offsetMultiplier);

            // Smoothly move the camera towards the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }
    }
}