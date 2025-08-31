using System.Collections.Generic;

[System.Serializable]
public class Choice
{
    public string choiceText; // Butonda yazacak metin
    public DialogueNode destinationNode; // Bu seçenek seçilince gidilecek sonraki konuşma
    public List<DialogueAction> actions; // Bu seçenek seçildiğinde tetiklenecek eylemler
}