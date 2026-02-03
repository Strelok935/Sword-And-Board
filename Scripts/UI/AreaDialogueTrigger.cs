using UnityEngine;

public class AreaDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueData dialogue;

    [TextArea(3,6)]
    [SerializeField] private string defaultDialogue =
        "What was that..., that thought.., i felt it but for a minute, was it real?, it touched my mind like small tulips falling upon the water's surface, light enough to be felt, not heavy enough to sink";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        TryTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Safety net if player starts inside
        TryTrigger(other);
    }

    private void TryTrigger(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (dialogue != null && dialogue.lines != null && dialogue.lines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
        else
        {
            // Create a temporary dialogue on the fly
            DialogueData temp = ScriptableObject.CreateInstance<DialogueData>();
            temp.lines = new string[] { defaultDialogue };
            DialogueManager.Instance.StartDialogue(temp);
        }
    }
}
