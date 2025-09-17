using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform InteractionPoint; // The point from which interactions are detected
    public LayerMask interactableLayerMask; // Layer mask for interactable objects
    public float InteractionPointRadius = 1f; // Radius for detecting interactables

    public IInteractable CurrentInteractable { get; private set; } // The interactable object in range

    private PlayerInput _playerInput; // Reference to the PlayerInput component

    private void Awake()
    {
        // Get the PlayerInput component from the player
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on the player.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to the Interact action
        if (_playerInput != null)
        {
            _playerInput.actions["Interact"].performed += OnInteractPerformed;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the Interact action
        if (_playerInput != null)
        {
            _playerInput.actions["Interact"].performed -= OnInteractPerformed;
        }
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        // Detect interactable objects within the interaction radius
        var colliders = Physics.OverlapSphere(InteractionPoint.position, InteractionPointRadius, interactableLayerMask);

        CurrentInteractable = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            var interactable = colliders[i].GetComponent<IInteractable>();
            if (interactable != null)
            {
                CurrentInteractable = interactable;
                break;
            }
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        // Trigger interaction if an interactable object is detected
        if (CurrentInteractable != null)
        {
            CurrentInteractable.Interact(this, out bool success);
            if (success)
            {
                Debug.Log("Interaction successful!");
            }
        }
    }
}