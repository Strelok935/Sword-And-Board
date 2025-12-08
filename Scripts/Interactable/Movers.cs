using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Required for Handles (only works in the Editor)

public class Movers : MonoBehaviour
{
    private PlayerMovement playerMovement; // Reference to the PlayerMovement script

    [Header("Movement Settings")]
    public Vector3 targetPosition; // Target position to move the platform
    public float speed = 2f; // Speed of movement

    [Header("Rotation Settings")]
    public bool enableRotation = false; // Toggle to enable or disable rotation
    public Vector3 targetRotationEuler; // Target rotation in Euler angles
    public float rotationSpeed = 2f; // Speed of rotation
    private float currentRotationProgress = 0f; // Tracks the current rotation progress in degrees

    [Header("Activator Settings")]
    public GameObject activator;
    public bool requireInteraction = false;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isTriggered = false;
    private bool movingToTarget; // Tracks the current movement direction
    private bool hasReachedDestination = false;

    [Header("Axis Restrictions")]
    public bool restrictX = true;
    public bool restrictY = false;
    public bool restrictZ = true;

    [Header("Gizmo Settings")]
    public Vector3 gizmoOffset = Vector3.zero; // Offset for the Gizmos

    [Header("Delay")]
    public int delayTime = 0; // Delay before activation
    private bool isWaitingForDelay = false; // Tracks if the delay is active

    private CharacterController playerController;
    private Vector3 previousPosition;

    [Header("Audio Settings")]
    public bool hasAudio = false; // Enable or disable audio
    public AudioSource audioSource; // Reference to the AudioSource component

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Initialize movingToTarget based on the platform's starting position
        movingToTarget = Vector3.Distance(transform.position, targetPosition) > 0.001f;

        if (targetPosition == Vector3.zero)
        {
            Debug.LogWarning("Target position is not set for the platform. Movement will not occur.");
        }

        if (activator != null && requireInteraction)
        {
            if (!activator.GetComponent<Collider>())
            {
                Debug.LogWarning($"Activator '{activator.name}' does not have a Collider. Interaction may not work as expected.");
            }
        }

        previousPosition = transform.position; // Initialize previous position
    }

    public void ToggleMovement()
    {
        if (!isTriggered && targetPosition != Vector3.zero) // Prevent re-triggering and ensure targetPosition is valid
        {
            if (delayTime > 0)
            {
                StartCoroutine(DelayedActivation());
            }
            else
            {
                ActivateMovement();
            }
        }
        else if (targetPosition == Vector3.zero)
        {
            Debug.LogWarning("Target position is not set for the platform. Movement will not occur.");
        }
    }

    private void ActivateMovement()
    {
        isTriggered = true; // Allow movement
        hasReachedDestination = false; // Reset destination flag to allow movement

        // Start playing audio if enabled
        if (hasAudio && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private System.Collections.IEnumerator DelayedActivation()
    {
        isWaitingForDelay = true;
        yield return new WaitForSeconds(delayTime); // Wait for the specified delay time
        isWaitingForDelay = false;
        ActivateMovement();
    }

    private void Update()
    {
        if (isTriggered && !isWaitingForDelay)
        {
            MoveAndRotateObject();
        }
    }

    private void MoveAndRotateObject()
    {
        // Determine the target position
        Vector3 targetPos = movingToTarget ? startPosition + targetPosition : startPosition;

        // Apply axis restrictions to the target position
        if (restrictX) targetPos.x = transform.position.x;
        if (restrictY) targetPos.y = transform.position.y;
        if (restrictZ) targetPos.z = transform.position.z;

        // Move the platform towards the target position
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.MoveTowards(currentPosition, targetPos, speed * Time.deltaTime);

        // Calculate the movement delta
        Vector3 movementDelta = transform.position - previousPosition;

        // Move the player along with the elevator
        if (playerController != null)
        {
            playerController.Move(movementDelta);
        }

        previousPosition = transform.position; // Update previous position

        // Handle rotation only if enabled
        if (enableRotation)
        {
            float rotationStep = rotationSpeed * Time.deltaTime; // Calculate the rotation step for this frame
            if (movingToTarget)
            {
                // Rotate only if we haven't completed the full 360 degrees
                if (currentRotationProgress < 360f)
                {
                    transform.Rotate(Vector3.up, rotationStep); // Rotate around the Y-axis
                    currentRotationProgress += rotationStep; // Update the rotation progress
                }
                else
                {
                    currentRotationProgress = 360f; // Clamp to 360 degrees
                }
            }
            else
            {
                // Reset rotation progress when moving back to the start
                if (currentRotationProgress > 0f)
                {
                    transform.Rotate(Vector3.up, -rotationStep); // Rotate back around the Y-axis
                    currentRotationProgress -= rotationStep; // Update the rotation progress
                }
                else
                {
                    currentRotationProgress = 0f; // Clamp to 0 degrees
                }
            }
        }

        // Check if the position and rotation have reached their targets
        if (Vector3.Distance(transform.position, targetPos) < 0.001f &&
            (!enableRotation || Mathf.Approximately(currentRotationProgress, movingToTarget ? 360f : 0f)))
        {
            transform.position = targetPos; // Snap to the exact position
            hasReachedDestination = true; // Mark as complete
            isTriggered = false; // Stop movement and rotation

            // Stop audio if enabled
            if (hasAudio && audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // Toggle the movement direction for the next activation
            movingToTarget = !movingToTarget;
        }
    }

    public bool HasReachedDestination()
    {
        return hasReachedDestination;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<CharacterController>();
            playerMovement = other.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerMovement.SetJumpEnabled(false); // Disable jumping
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = null;

            if (playerMovement != null)
            {
                playerMovement.SetJumpEnabled(true); // Re-enable jumping
                playerMovement = null; // Clear reference
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Calculate the Gizmo's base position with the offset
        Vector3 gizmoBasePosition = transform.position + gizmoOffset;

        // Draw the start position (current position of the platform)
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(gizmoBasePosition, 0.2f);

        // Draw the target position (relative to the platform's position)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gizmoBasePosition + targetPosition, 0.2f);

        // Draw a line between the platform's current position and its target position
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gizmoBasePosition, gizmoBasePosition + targetPosition);

        // Visualize the rotation start and end
        if (enableRotation)
        {
            Gizmos.color = Color.blue;

            // Draw the starting rotation direction
            Quaternion startRot = startRotation;
            Vector3 startDirection = startRot * Vector3.forward; // Forward vector based on start rotation
            Gizmos.DrawLine(transform.position, transform.position + startDirection * 2f); // Scale for visibility

            // Draw the target rotation direction
            Quaternion targetRot = Quaternion.Euler(targetRotationEuler);
            Vector3 targetDirection = targetRot * Vector3.forward; // Forward vector based on target rotation
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + targetDirection * 2f); // Scale for visibility
        }
    }

    // Allow interactive editing of the target position in the Scene view
    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        // Calculate the Gizmo's base position with the offset
        Vector3 gizmoBasePosition = transform.position + gizmoOffset;

        // Draw a handle to move the target position interactively
        targetPosition = Handles.PositionHandle(gizmoBasePosition + targetPosition, Quaternion.identity) - gizmoBasePosition;
#endif
    }
}