// 12/16/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private WeaponBaseSO weaponBaseSo;
    [SerializeField] private List<string> ignoredTags; // List of tags to ignore during raycast

    private Weapon currentRangedWeapon; // Reference to the current ranged weapon (e.g., Arquebus)
    private SwordWeapon currentSwordWeapon; // Reference to the current sword weapon
    private float timeSinceLastShot = 0f;

    private PlayerControls playerControls; // Reference to the PlayerControls input action asset
    private InputAction attackAction; // Input action for the Attack button

    private void Awake()
    {
        // Initialize the PlayerControls input action asset
        playerControls = new PlayerControls();
        attackAction = playerControls.Player.Attack; // Get the Attack action from the Player action map

        // Initialize weapon references
        currentRangedWeapon = GetComponentInChildren<Weapon>();
        currentSwordWeapon = GetComponentInChildren<SwordWeapon>();
    }

    private void OnEnable()
    {
        // Enable the input action when the script is enabled
        attackAction.Enable();
    }

    private void OnDisable()
    {
        // Disable the input action when the script is disabled
        attackAction.Disable();
    }

    void Update()
    {
        // Increment the time since the last shot
        timeSinceLastShot += Time.deltaTime;

        // Check for player input to shoot
        if (attackAction.ReadValue<float>() > 0) // Check if the Attack button is pressed
        {
            HandleAttack();
        }
    }

    public void HandleAttack()
    {
        if (currentRangedWeapon != null && currentRangedWeapon.gameObject.activeSelf)
        {
            HandleRangedAttack();
        }
        else if (currentSwordWeapon != null && currentSwordWeapon.gameObject.activeSelf)
        {
            HandleSwordAttack();
        }
    }

    

    private void HandleRangedAttack()
    {
        // Ensure the fire rate matches the animation duration
        float adjustedFireRate = currentRangedWeapon != null ? currentRangedWeapon.GetAttackAnimationDuration() : weaponBaseSo.fireRate;

        // Check if enough time has passed since the last shot
        if (timeSinceLastShot >= adjustedFireRate) // Higher fireRate means slower shooting
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, weaponBaseSo.range);

            foreach (RaycastHit hit in hits)
            {
                // Skip objects with ignored tags
                if (ignoredTags.Contains(hit.collider.gameObject.tag))
                {
                    Debug.Log($"Ignored object with tag: {hit.collider.gameObject.tag}");
                    continue; // Skip this object and continue to the next hit
                }

                // Pass the hit information to the Weapon script for further processing
                currentRangedWeapon.Shooting(weaponBaseSo, hit);

                // Break after processing the first valid hit
                break;
            }

            // If no valid hit is found, play sound and VFX without hit
            if (hits.Length == 0 || hits.All(h => ignoredTags.Contains(h.collider.gameObject.tag)))
            {
                currentRangedWeapon.Shooting(weaponBaseSo);
            }

            // Reset the time since the last shot
            timeSinceLastShot = 0f;
        }
    }

    private void HandleSwordAttack()
    {
        if (currentSwordWeapon != null)
        {
            currentSwordWeapon.Attack();
        }
    }

    public void SetWeaponBaseSO(WeaponBaseSO newWeaponBaseSO)
    {
        weaponBaseSo = newWeaponBaseSO;

        // Update the currentWeapon reference to the active weapon's Weapon component
        currentRangedWeapon = GetComponentInChildren<Weapon>();
        currentSwordWeapon = GetComponentInChildren<SwordWeapon>();

        Debug.Log($"Switched to weapon: {weaponBaseSo.name}");
    }

    public void SetCurrentWeapon(Weapon newWeapon)
    {
        currentRangedWeapon = newWeapon;
        Debug.Log($"Current ranged weapon updated to: {newWeapon.name}");
    }

    public void SetCurrentSwordWeapon(SwordWeapon newSwordWeapon)
    {
        currentSwordWeapon = newSwordWeapon;
        Debug.Log($"Current sword weapon updated to: {newSwordWeapon.name}");
    }
}
