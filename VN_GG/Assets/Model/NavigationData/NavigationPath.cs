using System.Collections.Generic;

[System.Serializable]
public class NavigationPath
{
    public string pathDescription; // "Sağdaki kapıya git" (Editor için not)
    public string destinationNodeID; // Gidilecek hedef görselin ID'si
    
    // Bu yolun aktif olması için gereken şartlar
    public List<GameCondition> conditions;
    
    // Runtime'da kullanılacak referans
    [System.NonSerialized]
    public LocationNode destinationNode;
}