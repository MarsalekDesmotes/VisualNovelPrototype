using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("General Info")]
    public string characterID; // Kod içinde kullanılacak benzersiz isim, örn: "NPC_Elara"
    public string characterName; // Ekranda görünecek isim, örn: "Elara"
    
    [Header("Audio")]
    public AudioClip textBlipSound;     // Diyalog metni yazılırken çıkan ses (tık tık)
    public AudioClip entranceSound;     // Karakter ekrana ilk geldiğinde çalan ses
    public AudioClip themeMusic;        // Karakterin tema müziği
    public AudioClip mutterSound;       // Homurdanma, iç çekme gibi tepki sesi

    [Header("Visuals")]
    public List<CharacterExpression> expressions; // Karakterin tüm ifadelerini (sprite'larını) tutan liste

    [Header("Dialogue")]
    public DialogueTree startingDialogue; // Karakterle ilk konuşmada başlayacak diyalog ağacı
    
    // Not: Karakterle olan ilişki (relationship) sayacı burada DEĞİL,
    // GameStateManager'da tutulur. Çünkü bu, karakterin bir özelliği değil,
    // oyuncunun o karakterle olan *ilerlemesidir*.
}