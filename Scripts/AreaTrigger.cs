// 7/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public AudioClip areaMusic; // Assign the area-specific music in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            SoundManager soundManager = FindObjectOfType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.PlayAreaMusic(areaMusic);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // When the player leaves the area
        {
            SoundManager soundManager = FindObjectOfType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.PlayLevelMusic(); // Revert to level music
            }
        }
    }
}
