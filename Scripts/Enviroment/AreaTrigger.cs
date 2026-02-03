// 7/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public AudioClip areaMusic; // Assign the area-specific music in the Inspector
    private SoundManager soundManager;

    private void Awake()
    {
        UnityEngine.Object soundManagerObject = UnityEngine.Object.FindFirstObjectByType<SoundManager>();
        soundManager = soundManagerObject as SoundManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
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
            if (soundManager != null)
            {
                soundManager.PlayLevelMusic(); // Revert to level music
            }
        }
    }
}
