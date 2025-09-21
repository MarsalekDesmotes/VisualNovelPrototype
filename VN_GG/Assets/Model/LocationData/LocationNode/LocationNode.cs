using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationNode
{
    public string nodeID; // "Mutfak_TezgahUstu" (Anlaşılır bir isim)
    public Sprite backgroundImage; // Ekranda gösterilecek arkaplan görseli
    public List<NavigationPath> navigationPaths; // Bu görselden gidilebilecek yollar
    public List<CharacterPlacement> charactersInNode; // Bu görselde bulunan karakterler
    
    // Döngüsel referansları önlemek için destinationNode'u string ID olarak tut
    [System.NonSerialized]
    public LocationNode destinationNode;
}