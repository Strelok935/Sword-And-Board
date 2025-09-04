using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform InteractionPoint;
    public LayerMask interactableLayerMask;
    public float InteractionPointRadius = 1f;

    public IInteractable CurrentInteractable { get; private set; } // The interactable object in range

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        // Detect interactable objects within the interaction point radius
        var colliders = Physics.OverlapSphere(InteractionPoint.position, InteractionPointRadius, interactableLayerMask);

        CurrentInteractable = null; // Reset the current interactable

        for (int i = 0; i < colliders.Length; i++)
        {
            var interactable = colliders[i].GetComponent<IInteractable>();
            if (interactable != null)
            {
                CurrentInteractable = interactable;
                break; // Stop after finding the first interactable object
            }
        }
    }
}