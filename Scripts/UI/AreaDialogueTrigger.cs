using UnityEngine;
using TMPro;
using System.Collections;
public class AreaDialogueTrigger : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string dialogueText;

    [SerializeField] private TextMeshProUGUI dialogueUI;
    // Drag your UI object here in Inspector
    [SerializeField] private float showDelay = 2f;
    [SerializeField] private float textLifetime = 5f;
    bool hasTriggered = false;
    [SerializeField] bool selfDestruct = false;

    [SerializeField] private AudioClip appearSFX;
    [SerializeField] private AudioClip disappearSFX;
    [SerializeField] private AudioSource audioSource;

    private string defaultString =
     "What was that..., that thought.., i felt it but for a minute, was it real?, it touched my mind like small tulips falling upon the water's surface, light enough to be felt, not heavy enough to sink";


    private void OnTriggerEnter(Collider other)
    {
        hasTriggered = true;
        
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeSequence());
            string textToUse = string.IsNullOrEmpty(dialogueText) ? defaultString : dialogueText;
            dialogueUI.text = textToUse;
            if (selfDestruct)
            {
                Destroy(gameObject);
            }

        }
    }
    private IEnumerator FadeSequence()
{
    // --- PREPARE THE TEXT BEFORE SHOWING ANYTHING ---

    // Save original color
    Color baseColor = dialogueUI.color;

    // Set alpha to 0 *before* showing the UI
    dialogueUI.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        // Now activate UI (it will be invisible)
        dialogueUI.gameObject.SetActive(true);
    
     if (audioSource && appearSFX)
        audioSource.PlayOneShot(appearSFX);

    // --- FADE IN ---
    float fadeInDuration = 1f;
    float t = 0f;

    while (t < fadeInDuration)
    {
        t += Time.deltaTime;
        float alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
        dialogueUI.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        yield return null;
    }

    // Make sure it's fully visible at the end
    dialogueUI.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);


    // --- WAIT FOR LIFETIME ---
    yield return new WaitForSeconds(textLifetime);
    
    if (audioSource && disappearSFX)
        audioSource.PlayOneShot(disappearSFX);

    // --- FADE OUT ---
    float fadeOutDuration = 1f;
    t = 0f;

    while (t < fadeOutDuration)
    {
        t += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
        dialogueUI.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        yield return null;
    }

    // Hide object after fade-out
    dialogueUI.gameObject.SetActive(false);

    // Restore original color (invisible now)
    dialogueUI.color = baseColor;
}
}

