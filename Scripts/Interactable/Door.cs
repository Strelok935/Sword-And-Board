// 7/8/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public Transform doorTransform; // The transform of the door to rotate/move
    public float openAngle = 90f;   // Angle to open the door
    public float openSpeed = 10f;  // Speed of the door opening/closing
    public bool isLocked = false;  // Whether the door is locked
    public string lockedMessage = "The door is locked."; // Message when locked

    private bool isOpen = false;    // Is the door currently open?
    private bool isAnimating = false; // Is the door currently animating?

    private Quaternion closedRotation; // Original rotation of the door
    private Quaternion openRotation;   // Target rotation when open

    public UnityAction<IInteractable> OnInteract { get; set; } // Required by IInteractable

    private void Start()
    {
        // Store the initial rotation of the door
        closedRotation = doorTransform.localRotation;
    }

    public void Interact(Interactor interactor, out bool interactionSuccess)
    {
        Debug.Log("Door Interact called.");

        if (isLocked)
        {
            Debug.Log(lockedMessage);
            interactionSuccess = false;
            return;
        }

        if (isAnimating)
        {
            Debug.Log("Door is already animating.");
            interactionSuccess = false;
            return;
        }

        // Determine the open direction based on the player's position
        Vector3 toPlayer = interactor.transform.position - doorTransform.position;
        Vector3 doorForward = doorTransform.forward;

        // Check if the player is in front or behind the door
        float dotProduct = Vector3.Dot(doorForward, toPlayer);
        float adjustedOpenAngle = dotProduct > 0 ? openAngle : -openAngle;

        // Calculate the open rotation dynamically
        openRotation = closedRotation * Quaternion.Euler(0, adjustedOpenAngle, 0);

        // Start the door animation
        StartCoroutine(ToggleDoor());

        interactionSuccess = true;
    }

    public void StopInteract()
    {
        // Optional: Add logic if needed when interaction stops
    }

    private IEnumerator ToggleDoor()
    {
        isAnimating = true;
        Debug.Log("Door animation started.");

        // Determine the target rotation
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;

        // Animate the door rotation
        while (Quaternion.Angle(doorTransform.localRotation, targetRotation) > 0.01f)
        {
            doorTransform.localRotation = Quaternion.Lerp(doorTransform.localRotation, targetRotation, Time.deltaTime * openSpeed);

            // Break early if the rotation is very close to the target
            if (Quaternion.Angle(doorTransform.localRotation, targetRotation) < 1f)
            {
                break;
            }

            yield return null;
        }

        // Snap to the final rotation
        doorTransform.localRotation = targetRotation;

        // Toggle the door state
        isOpen = !isOpen;
        isAnimating = false;
        Debug.Log("Door animation completed.");
    }
}