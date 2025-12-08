using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public static PlayerAudio Instance;   // Singleton for easy access

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;  // Plays 1-shot effects

    [Header("SFX Clips")]
    [SerializeField] private AudioClip footstepSFX;
    [SerializeField] private AudioClip dialogueAppearSFX;
    [SerializeField] private AudioClip dialogueDisappearSFX;

    private void Awake()
    {
        // Basic singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // --- Public Methods ---

    public void PlayFootstep()
    {
        if (footstepSFX) 
            sfxSource.PlayOneShot(footstepSFX);
    }

    public void PlayDialogueAppear()
    {
        if (dialogueAppearSFX)
            sfxSource.PlayOneShot(dialogueAppearSFX);
    }

    public void PlayDialogueDisappear()
    {
        if (dialogueDisappearSFX)
            sfxSource.PlayOneShot(dialogueDisappearSFX);
    }
}
