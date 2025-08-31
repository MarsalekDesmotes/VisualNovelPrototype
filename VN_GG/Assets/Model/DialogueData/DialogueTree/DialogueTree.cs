using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Tree", menuName = "Game/Dialogue Tree")]
public class DialogueTree : ScriptableObject
{
    public DialogueNode startingNode;
    public List<DialogueNode> allNodes; // Ağaçtaki tüm düğümleri burada tutmak yönetimi kolaylaştırır
}