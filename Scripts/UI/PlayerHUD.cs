using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats; // Reference to the PlayerStats script
    public Image healthBarFill; // Reference to the HealthBar fill image
    public Image staminaBarFill; // Reference to the StaminaBar fill image
    public TMP_Text healthText; // Reference to the Health Text (TextMeshPro)
    public TMP_Text staminaText; // Reference to the Stamina Text (TextMeshPro)
    public TMP_Text goldText; // Reference to the Gold Text (TextMeshPro)
    public PlayerInventory playerInventory; // Reference to the PlayerInventory script

    void Start()
    {
        // Initialize the health and stamina bars
        if (playerStats != null)
        {
            UpdateHealthBar();
            UpdateStaminaBar();
            UpdateText();
           
        }
    }

    void Update()
    {
        if (playerStats != null)
        {
            // Update the health and stamina bars
            UpdateHealthBar();
            UpdateStaminaBar();

            // Update the text
            UpdateText();
        }
    }

    void UpdateHealthBar()
    {
        // Update the fill amount of the health bar
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = playerStats.currentHealth / playerStats.maxHealth;
        }
    }

    void UpdateStaminaBar()
    {
        // Update the fill amount of the stamina bar
        if (staminaBarFill != null)
        {
            staminaBarFill.fillAmount = playerStats.currentStamina / playerStats.maxStamina;
        }
    }

    void UpdateText()
    {
        // Update the health and stamina text
        if (healthText != null)
        {
            healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
        }

        if (staminaText != null)
        {
            staminaText.text = $"{playerStats.currentStamina}/{playerStats.maxStamina}";
        }
        
        if (goldText != null)
        {
            goldText.text = playerInventory.GetGold().ToString();
        }
    }
}