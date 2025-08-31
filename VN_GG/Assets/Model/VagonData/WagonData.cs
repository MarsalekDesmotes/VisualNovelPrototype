// Assets/ScriptableObjects/Wagons/Wagon1.asset olarak kaydedilecek
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Wagon", menuName = "Game/Wagon Data")]
public class WagonData : ScriptableObject
{
    public string wagonID; // "Vagon1_Mutfak"
    public List<LocationNode> nodes; // Bu vagondaki tüm görseller/düğümler
    public LocationNode startingNode; // Bu vagona girildiğinde gösterilecek ilk görsel
}