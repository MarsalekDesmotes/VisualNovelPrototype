using System.Collections.Generic;

[System.Serializable]
public class NavigationPath
{
    public string pathDescription; // "Sağdaki kapıya git" (Editor için not)
    public LocationNode destinationNode; // Gidilecek hedef görsel (Node)
    
    // Bu yolun aktif olması için gereken şartlar
    public List<GameCondition> conditions; 
}