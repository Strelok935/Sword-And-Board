using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable
{
    public List<Movers> connectedMovers; // List of connected Movers objects
    public Animator leverAnimator; // Reference to the Animator component for the lever
    public Renderer leverRenderer; // Reference to the Renderer component for visual changes
    public Color highlightColor = Color.yellow; // Color to use for highlighting
    private Color originalColor; // Original color of the lever

    private bool isActivated = false; // Tracks if the lever is currently activated
    public bool isInteractable = true; // Tracks if the lever can be interacted with

    private float cooldownTime = 1.5f; // Cooldown duration after reverse animation

    public UnityAction<IInteractable> OnInteract { get; set; } // Required by IInteractable

    private void Start()
    {
        // Store the original color of the lever
        if (leverRenderer != null)
        {
            originalColor = leverRenderer.material.color;
        }
    }

    private void Update()
    {
        // Check if all connected Movers objects have reached their destinations
        if (isActivated && connectedMovers != null && AllMoversReachedDestination())
        {
            // Start cooldown timer to make the lever interactable again
            Invoke(nameof(ResetInteractable), cooldownTime);
            isActivated = false;

            // Trigger the Return animation
            leverAnimator.SetBool("Return", true);
        }
    }

    public void Interact(Interactor interactor, out bool interactionSuccess)
    {
        if (isInteractable && connectedMovers != null && connectedMovers.Count > 0)
        {
            Debug.Log("Lever Interacted: Triggering activation animation.");
            leverAnimator.SetBool("Activate", true); // Trigger the Activation animation

            // Activate all connected Movers objects
            foreach (var mover in connectedMovers)
            {
                if (mover != null)
                {
                    mover.ToggleMovement(); // Only call ToggleMovement, do not modify mover properties
                }
            }

            // Set the lever as non-interactable
            isActivated = true;
            isInteractable = false;

            // Reset the lever's color
            ResetHighlight();

            interactionSuccess = true;
        }
        else if (!isInteractable)
        {
            Debug.Log("Lever is not interactable at the moment.");
            interactionSuccess = false;
        }
        else
        {
            interactionSuccess = false;
        }
    }

    public void StopInteract()
    {
        // Optional: Add logic if needed when interaction stops
    }

    public void Highlight()
    {
        // Change the lever's color to the highlight color
        if (leverRenderer != null)
        {
            leverRenderer.material.color = highlightColor;
        }
    }

    public void ResetHighlight()
    {
        // Reset the lever's color to its original color
        if (leverRenderer != null)
        {
            leverRenderer.material.color = originalColor;
        }
    }

    private void ResetInteractable()
    {
        isInteractable = true;
        Debug.Log("Lever is now interactable again.");
    }

    // Animation Event: Called at the end of the Activation animation
    public void OnActivationAnimationEnd()
    {
        Debug.Log("Activation animation ended. Resetting Activate parameter.");
        leverAnimator.SetBool("Activate", false);
    }

    // Animation Event: Called at the end of the Return animation
    public void OnReturnAnimationEnd()
    {
        Debug.Log("Return animation ended. Resetting Return parameter.");
        leverAnimator.SetBool("Return", false);
    }

    private bool AllMoversReachedDestination()
    {
        // Check if all connected Movers objects have reached their destinations
        foreach (var mover in connectedMovers)
        {
            if (mover != null && !mover.HasReachedDestination())
            {
                return false;
            }
        }
        return true;
    }
}