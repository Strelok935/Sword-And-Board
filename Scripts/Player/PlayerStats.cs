using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 5f; // Stamina regeneration per second
    public float staminaDrainRate = 10f; // Stamina drain per second while sprinting
    public float staminaRegenDelay = 2f; // Delay before stamina regeneration starts
    private float staminaRegenTimer = 0f; // Timer to track the delay

    [Header("References")]
    public Transform cameraTransform; // Assign the player's camera here
    public CharacterController characterController; // Assign the CharacterController here
    public PlayerMovement playerMovement; // Reference to the movement script

    private bool isDead = false;

    void Start()
    {
        // Initialize health and stamina
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (isDead) return;

            // Regenerate stamina over time after a delay
        if (currentStamina < maxStamina)
        {
            if (staminaRegenTimer <= 0f)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
            else
            {
                staminaRegenTimer -= Time.deltaTime; // Count down the delay timer
            }
        }

        // Check if the player is dead
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

   
    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // Reset the regeneration delay timer
        staminaRegenTimer = staminaRegenDelay;
    }

    public void RestoreHealth(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void RestoreStamina(float amount)
    {
        if (amount <= 0f) return;
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
    }
    private void Die()
    {
        isDead = true;
        Debug.Log("Player has died.");

        // Lock controls
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Drop the camera to the player's feet
        if (cameraTransform != null)
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, new Vector3(0, 0.5f, 0), 1f);
            cameraTransform.localRotation = Quaternion.Euler(90, 0, 0); // Look down
        }
    }
}
