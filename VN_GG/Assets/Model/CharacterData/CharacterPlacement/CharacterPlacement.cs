using UnityEngine;

[System.Serializable]
public class CharacterPlacement
{
    public CharacterData character; // Hangi karakterin yerleştirileceği (Daha önceki mimariden)
    public Vector2 positionOnScreen; // Ekrandaki pozisyonu (normalized, 0-1 arası)
}