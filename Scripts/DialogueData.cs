using UnityEngine;

[System.Serializable]
public class FlagChange
{
    public string flagName;
    public bool value;
}
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [TextArea(3,6)]
    public string[] lines;

    [Header("Conditions to SHOW this dialogue")]
    public DialogueCondition[] conditions;

    [Header("Flags to SET after dialogue ends")]
    public FlagChange[] flagsToSet;
}
