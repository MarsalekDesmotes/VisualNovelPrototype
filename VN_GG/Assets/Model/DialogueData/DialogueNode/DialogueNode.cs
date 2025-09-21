using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string nodeID; // Editor'de takip için (isteğe bağlı)
    public CharacterData speaker; // Konuşan karakter
    
    [TextArea(3, 10)] // Inspector'da daha geniş bir metin alanı sağlar
    public string dialogueText;
    
    public string emotion; // Bu konuşmada karakterin hangi ifadesini kullanacağı ("mutlu", "kızgın")
    
    public List<Choice> choices;
    
    // Döngüsel referansları önlemek için destinationNode'u string ID olarak tut
    [System.NonSerialized]
    public DialogueNode destinationNode;
}