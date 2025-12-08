// 12/5/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponSwitchingSystem : MonoBehaviour
{
    [SerializeField] private List<WeaponBaseSO> availableWeapons; // List of available weapons
    [SerializeField] private ActiveWeapon activeWeapon; // Reference to the ActiveWeapon script
    [SerializeField] private GameObject modelArquebus; // Reference to the Arquebus model GameObject
    [SerializeField] private GameObject modelSword; // Reference to the Sword model GameObject

    private int currentWeaponIndex = 0; // Index of the currently selected weapon
    private PlayerControls playerControls; // Reference to the PlayerControls input action asset
    private InputAction switchWeaponAction; // Input action for the Z key

    private void Awake()
    {
        // Initialize the PlayerControls input action asset
        playerControls = new PlayerControls();
        switchWeaponAction = playerControls.Player.SwitchWeapon; // Assuming "SwitchWeapon" is bound to the Z key
    }

    private void OnEnable()
    {
        // Enable the input system
        playerControls.Enable();

        // Subscribe to the SwitchWeapon action
        switchWeaponAction.performed += OnSwitchWeapon;
    }

    private void OnDisable()
    {
        // Unsubscribe from the SwitchWeapon action
        switchWeaponAction.performed -= OnSwitchWeapon;

        // Disable the input system
        playerControls.Disable();
    }

    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        // Switch to the next weapon
        SwitchToNextWeapon();
    }

    private void SwitchToNextWeapon()
    {
        // Increment the weapon index and wrap around if necessary
        currentWeaponIndex = (currentWeaponIndex + 1) % availableWeapons.Count;

        // Update the active weapon
        UpdateActiveWeapon();
    }

    private void UpdateActiveWeapon()
    {
        // Update the weaponBaseSo in the ActiveWeapon script
        if (activeWeapon != null && availableWeapons.Count > 0)
        {
            WeaponBaseSO selectedWeapon = availableWeapons[currentWeaponIndex];
            activeWeapon.SetWeaponBaseSO(selectedWeapon);

            // Enable/Disable weapon models based on the selected weapon
            if (selectedWeapon.name == "Arquebus")
            {
                modelArquebus.SetActive(true);
                modelSword.SetActive(false);
            }
            else if (selectedWeapon.name == "Sword")
            {
                modelArquebus.SetActive(false);
                modelSword.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"No matching model found for weapon: {selectedWeapon.name}");
            }
        }
        else
        {
            Debug.LogWarning("ActiveWeapon script or availableWeapons list is not properly set.");
        }
    }
}
