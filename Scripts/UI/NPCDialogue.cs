using UnityEngine;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData[] dialogueOptions;

    public UnityAction<IInteractable> OnInteract { get; set; }

    public void Interact(Interactor interactor, out bool interactionSuccess)
    {
        if (DialogueManager.IsDialogueActive)
        {
            interactionSuccess = false;
            return;
        }

        DialogueData chosenDialogue = GetValidDialogue();

        if (chosenDialogue == null)
        {
            interactionSuccess = false;
            return;
        }

        DialogueManager.Instance.StartDialogue(chosenDialogue);
        interactionSuccess = true;
    }

    private DialogueData GetValidDialogue()
    {
        foreach (var dialogue in dialogueOptions)
        {
            if (dialogue.conditions == null || dialogue.conditions.Length == 0)
                return dialogue;

            bool valid = true;

            foreach (var condition in dialogue.conditions)
            {
                if (!condition.IsMet())
                {
                    valid = false;
                    break;
                }
            }

            if (valid) return dialogue;
        }

        return null;
    }

    public void StopInteract() { }
}
