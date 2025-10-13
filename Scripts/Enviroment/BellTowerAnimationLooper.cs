using System;
using UnityEngine;

public class BellTowerAnimationLooper : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private float loopInterval = 30f; // Interval in seconds
    private float timer;
    private bool isWaitingForNextRing = false; // Tracks whether we are waiting for the next ring

    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on this GameObject.");
        }

        // Initialize the animation state
        animator.SetBool("IsRinging", true);
        animator.SetBool("IsIdle", false);
    }

    void Update()
    {
        if (animator == null || audioSource == null) return;

        // Check the current animation state
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("BellRinging"))
        {
            // If the BellRinging animation has finished, transition to Idle
            if (stateInfo.normalizedTime >= 1.0f && !isWaitingForNextRing)
            {
                animator.SetBool("IsRinging", false);
                animator.SetBool("IsIdle", true);
                isWaitingForNextRing = true; // Start waiting for the next ring
                timer = 0f; // Reset the timer
            }
        }

        if (isWaitingForNextRing)
        {
            // Increment the timer while waiting for the next ring
            timer += Time.deltaTime;

            // Check if the timer has reached the loop interval
            if (timer >= loopInterval)
            {
                // Reset the timer and trigger the next ring
                timer = 0f;
                animator.SetBool("IsRinging", true);
                animator.SetBool("IsIdle", false);
                isWaitingForNextRing = false; // Stop waiting
            }
        }

        // Control the audio playback based on IsRinging
        bool isRinging = animator.GetBool("IsRinging");
        if (isRinging && !audioSource.isPlaying)
        {
            // Play the audio if IsRinging is true and the audio is not already playing
            audioSource.Play();
        }
        else if (!isRinging && audioSource.isPlaying)
        {
            // Stop the audio if IsRinging is false and the audio is currently playing
            audioSource.Stop();
        }
    }
}