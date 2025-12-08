using UnityEngine;
using UnityEngine.InputSystem; // Import the Input System namespace
using System.Collections.Generic;
using System.Linq;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private WeaponBaseSO weaponBaseSo;
    [SerializeField] private List<string> ignoredTags; // List of tags to ignore during raycast

    private Weapon currentWeapon;
    private float timeSinceLastShot = 0f;

    private PlayerControls playerControls; // Reference to the PlayerControls input action asset
    private InputAction attackAction; // Input action for the Attack button

    private void Awake()
    {
        currentWeapon = GetComponentInChildren<Weapon>();

        // Initialize the PlayerControls input action asset
        playerControls = new PlayerControls();
        attackAction = playerControls.Player.Attack; // Get the Attack action from the Player action map
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
            HandleShoot();
        }
    }

    public void HandleShoot()
    {
        // Check if enough time has passed since the last shot
        if (timeSinceLastShot >= weaponBaseSo.fireRate) // Higher fireRate means slower shooting
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
                currentWeapon.Shooting(weaponBaseSo, hit);

                // Break after processing the first valid hit
                break;
            }

            // If no valid hit is found, play sound and VFX without hit
            if (hits.Length == 0 || hits.All(h => ignoredTags.Contains(h.collider.gameObject.tag)))
            {
                currentWeapon.Shooting(weaponBaseSo);
            }

            // Reset the time since the last shot
            timeSinceLastShot = 0f;
        }
    }
    public void SetWeaponBaseSO(WeaponBaseSO newWeaponBaseSO)
    {
        weaponBaseSo = newWeaponBaseSO;
        Debug.Log($"Switched to weapon: {weaponBaseSo.name}");
    }
    
}