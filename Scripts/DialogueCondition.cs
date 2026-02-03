[System.Serializable]
public class DialogueCondition
{
    public string requiredFlag;
    public bool mustBeTrue = true;

    public bool IsMet()
    {
        bool value = GameFlags.Instance.GetFlag(requiredFlag);
        return value == mustBeTrue;
    }
    
}
