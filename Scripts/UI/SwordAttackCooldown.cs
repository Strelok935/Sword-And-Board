// 12/16/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SwordAttackCooldown : MonoBehaviour
{
    [SerializeField] private Slider swordAttackSlider; // Reference to the SwordAttack slider
    [SerializeField] private Weapon swordWeapon; // Reference to the Sword Weapon script
    private float cooldownTimer;
    private bool isCooldownActive;

    private void Start()
    {
        if (swordAttackSlider == null)
        {
            Debug.LogError("SwordAttack Slider is not assigned!");
        }

        if (swordWeapon == null)
        {
            Debug.LogError("Sword Weapon script is not assigned!");
        }

        // Initialize the slider value to max (100%)
        swordAttackSlider.value = swordAttackSlider.maxValue;
    }

    private void Update()
    {
        if (isCooldownActive)
        {
            // Increment the cooldown timer
            cooldownTimer += Time.deltaTime;

            // Calculate the slider value based on the cooldown timer and weapon's fire rate
            swordAttackSlider.value = Mathf.Clamp((cooldownTimer / swordWeapon.GetAttackAnimationDuration()) * swordAttackSlider.maxValue, 0, swordAttackSlider.maxValue);

            // Check if cooldown is complete
            if (cooldownTimer >= swordWeapon.GetAttackAnimationDuration())
            {
                isCooldownActive = false;
                swordAttackSlider.value = swordAttackSlider.maxValue; // Reset slider to max value
            }
        }
    }

    public void StartCooldown()
    {
        // Start the cooldown process
        isCooldownActive = true;
        cooldownTimer = 0f;
        swordAttackSlider.value = 0; // Reset slider to 0
    }
}
