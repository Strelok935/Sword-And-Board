using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable
{
    public List<Movers> connectedMovers;
    public Animator leverAnimator;
    public Renderer leverRenderer;
    public Color highlightColor = Color.yellow;
    private Color originalColor;

    private bool isActivated = false;
    public bool isInteractable = true;
    private float cooldownTime = 1.5f;

    public UnityAction<IInteractable> OnInteract { get; set; }

    [Header("Flag Requirement")]
    public string requiredFlag;
    public bool flagMustBeTrue = true; // if false → flag must NOT exist

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip failureSound;

    private void Start()
    {
        if (leverRenderer != null)
            originalColor = leverRenderer.material.color;
    }

    private void Update()
    {
        if (isActivated && connectedMovers != null && AllMoversReachedDestination())
        {
            Invoke(nameof(ResetInteractable), cooldownTime);
            isActivated = false;
            leverAnimator.SetBool("Return", true);
        }
    }

    public void Interact(Interactor interactor, out bool interactionSuccess)
    {
        // -------- FLAG CHECK --------
        if (!IsFlagConditionMet())
        {
            PlaySound(failureSound);
            Debug.Log("Lever interaction failed — flag condition not met.");
            interactionSuccess = false;
            return;
        }

        // -------- NORMAL LEVER LOGIC --------
        if (isInteractable && connectedMovers != null && connectedMovers.Count > 0)
        {
            PlaySound(successSound);

            leverAnimator.SetBool("Activate", true);

            foreach (var mover in connectedMovers)
                if (mover != null)
                    mover.ToggleMovement();

            isActivated = true;
            isInteractable = false;
            ResetHighlight();

            interactionSuccess = true;
        }
        else
        {
            PlaySound(failureSound);
            interactionSuccess = false;
        }
    }

    public void StopInteract() { }

    // ---------------- FLAG SYSTEM ----------------
    private bool IsFlagConditionMet()
    {
        if (string.IsNullOrEmpty(requiredFlag)) return true;

        bool hasFlag = GameFlags.Instance.HasFlag(requiredFlag);
        return flagMustBeTrue ? hasFlag : !hasFlag;
    }

    // ---------------- AUDIO ----------------
    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }

    // ---------------- VISUALS ----------------
    public void Highlight()
    {
        if (leverRenderer != null)
            leverRenderer.material.color = highlightColor;
    }

    public void ResetHighlight()
    {
        if (leverRenderer != null)
            leverRenderer.material.color = originalColor;
    }

    private void ResetInteractable()
    {
        isInteractable = true;
        Debug.Log("Lever is now interactable again.");
    }

    public void OnActivationAnimationEnd() => leverAnimator.SetBool("Activate", false);
    public void OnReturnAnimationEnd() => leverAnimator.SetBool("Return", false);

    private bool AllMoversReachedDestination()
    {
        foreach (var mover in connectedMovers)
            if (mover != null && !mover.HasReachedDestination())
                return false;

        return true;
    }
}
