// 12/9/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndCubeTrigger : MonoBehaviour
{
    [SerializeField] private GameObject endText; // Field to hold the EndText GameObject
    [SerializeField] private Image fadeImage; // Field to hold the FadeImage UI element
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade effect

    private void OnTriggerEnter(Collider other)
    {
        // Pause the game
        Time.timeScale = 0;

        // Activate the EndText GameObject
        if (endText != null)
        {
            endText.SetActive(true);
        }
        else
        {
            Debug.LogWarning("EndText GameObject is not assigned in the EndCubeTrigger script.");
        }

        // Start the fade to black effect
        if (fadeImage != null)
        {
            StartCoroutine(FadeToBlack());
        }
        else
        {
            Debug.LogWarning("FadeImage is not assigned in the EndCubeTrigger script.");
        }
    }

    private IEnumerator FadeToBlack()
    {
        // Ensure the FadeImage is active
        fadeImage.gameObject.SetActive(true);

        // Gradually increase the alpha value of the image to make the screen black
        Color color = fadeImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time since the game is paused
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Ensure the alpha is fully opaque
        color.a = 1f;
        fadeImage.color = color;
    }
}