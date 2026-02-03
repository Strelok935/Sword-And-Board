using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Interactor interactor;

    public static DialogueManager Instance { get; private set; }
    public static bool IsDialogueActive { get; private set; }

    private DialogueData currentDialogue;
    private int currentLineIndex;

    private bool canAdvance = false;
    private bool isClosing = false; 

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (IsDialogueActive || dialogue == null || dialogue.lines.Length == 0) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        IsDialogueActive = true;
        isClosing = false;

        playerMovement.SetControlsLocked(true);
        interactor.enabled = false;

        dialogueUI.Show();
        DisplayLine();

        StartCoroutine(EnableAdvanceNextFrame());
    }

    IEnumerator EnableAdvanceNextFrame()
    {
        yield return null;
        canAdvance = true;
    }

    void DisplayLine()
    {
        dialogueUI.SetText(currentDialogue.lines[currentLineIndex]);
    }

    public void NextLine()
    {
        if (!IsDialogueActive || !canAdvance || isClosing) return;

        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        DisplayLine();
    }

    public void EndDialogue()
    {
        if (isClosing) return;
        isClosing = true;

        dialogueUI.Hide();
        IsDialogueActive = false;

        playerMovement.SetControlsLocked(false);
        interactor.enabled = true;
        ApplyFlags(currentDialogue);
        StartCoroutine(EndCooldown());
    }

    IEnumerator EndCooldown()
    {
        yield return new WaitForSeconds(0.2f); // prevents same-key re-trigger
        isClosing = false;
    }
    private void ApplyFlags(DialogueData dialogue)
    {
    foreach (var change in dialogue.flagsToSet)
    {
        GameFlags.Instance.SetFlag(change.flagName, change.value);
    }
    }
}