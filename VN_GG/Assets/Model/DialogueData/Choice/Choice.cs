using System.Collections.Generic;

[System.Serializable]
public class Choice
{
    public string choiceText; // Butonda yazacak metin
    public string destinationNodeID; // Bu seçenek seçilince gidilecek sonraki konuşmanın ID'si
    public List<DialogueAction> actions; // Bu seçenek seçildiğinde tetiklenecek eylemler
    
    // Runtime'da kullanılacak referans
    [System.NonSerialized]
    public DialogueNode destinationNode;
}