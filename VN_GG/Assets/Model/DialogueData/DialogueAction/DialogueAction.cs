[System.Serializable]
public class DialogueAction
{
    public enum ActionType
    {
        SetStoryFlag,
        ChangeRelationship
    }

    public ActionType actionType;
    public string parameter1; // Flag'in adı veya Karakter ID'si
    public bool boolValue;   // Flag'in değeri (true/false)
    public int intValue;     // İlişki değeri (+5, -10 vs.)
}