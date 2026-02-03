using System;
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

// Validate references
if (activeWeapon == null)
{
Debug.LogError("ActiveWeapon script is not assigned!");
}

if (modelArquebus == null || modelSword == null)
{
Debug.LogError("Weapon models are not assigned!");
}

if (availableWeapons == null || availableWeapons.Count == 0)
{
Debug.LogError("AvailableWeapons list is empty or not assigned!");
}
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

// Enable/Disable weapon models and their Weapon components based on the selected weapon
if (selectedWeapon.name == "Arquebus")
{
if (modelArquebus != null && modelArquebus.GetComponent<Weapon>() != null)
{
modelArquebus.SetActive(true);
modelArquebus.GetComponent<Weapon>().enabled = true; // Enable Weapon component
}
else
{
Debug.LogError("Arquebus model or Weapon component is missing!");
}

if (modelSword != null && modelSword.GetComponent<SwordWeapon>() != null)
{
modelSword.SetActive(false);
modelSword.GetComponent<SwordWeapon>().enabled = false; // Disable SwordWeapon component
modelSword.GetComponent<SwordWeapon>().HideSliderImmediately(); // Hide the sword slider immediately
}
else
{
Debug.LogError("Sword model or SwordWeapon component is missing!");
}

activeWeapon.SetCurrentWeapon(modelArquebus.GetComponent<Weapon>()); // Update currentWeapon reference
}
else if (selectedWeapon.name == "Sword")
{
if (modelSword != null && modelSword.GetComponent<SwordWeapon>() != null)
{
modelSword.SetActive(true);
modelSword.GetComponent<SwordWeapon>().enabled = true; // Enable SwordWeapon component
}
else
{
Debug.LogError("Sword model or SwordWeapon component is missing!");
}

if (modelArquebus != null && modelArquebus.GetComponent<Weapon>() != null)
{
modelArquebus.SetActive(false);
modelArquebus.GetComponent<Weapon>().enabled = false; // Disable Weapon component
}
else
{
Debug.LogError("Arquebus model or Weapon component is missing!");
}

activeWeapon.SetCurrentSwordWeapon(modelSword.GetComponent<SwordWeapon>()); // Update currentSwordWeapon reference
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
