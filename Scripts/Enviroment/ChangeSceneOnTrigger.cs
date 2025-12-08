
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTrigger : MonoBehaviour
{
    public string sceneName; // Name of the scene to load
    public float delay = 3f; // Delay in seconds before changing the scene

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Start the coroutine to change the scene after a delay
            StartCoroutine(ChangeSceneAfterDelay());
        }
    }

    private System.Collections.IEnumerator ChangeSceneAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }
}
