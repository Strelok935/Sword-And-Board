using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLanternLight : MonoBehaviour
{
    // Serialized field to assign the lantern light in the Unity Editor
    [SerializeField] private Light lanternLight;

    // Boolean to control the light state
    public bool isLanternOn = true;

    // Input system reference
    private PlayerControls playerControls;

    private void Awake()
    {
        // Initialize the input system
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        // Enable the input system
        playerControls.Enable();

        // Subscribe to the ToggleLantern action
        playerControls.Player.ToggleLantern.performed += ToggleLantern;
    }

    private void OnDisable()
    {
        // Unsubscribe from the ToggleLantern action
        playerControls.Player.ToggleLantern.performed -= ToggleLantern;

        // Disable the input system
        playerControls.Disable();
    }

    private void Start()
    {
        // Ensure the light is set to its initial state
        if (lanternLight != null)
        {
            lanternLight.enabled = isLanternOn;
        }
        else
        {
            Debug.LogWarning("Lantern light is not assigned in the inspector.");
        }
    }

    private void ToggleLantern(InputAction.CallbackContext context)
    {
        // Toggle the light on or off based on the boolean
        if (lanternLight != null)
        {
            isLanternOn = !isLanternOn;
            lanternLight.enabled = isLanternOn;
        }
        else
        {
            Debug.LogWarning("Lantern light is not assigned in the inspector.");
        }
    }
}