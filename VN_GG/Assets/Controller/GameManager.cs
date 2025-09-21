using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Setup")]
    public WagonData startingWagon; // Oyunun başlayacağı vagon
    public LocationController locationController; // LocationController referansı
    
    void Start()
    {
        // GameStateManager'ı başlat
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.InitializeGame();
        }
        
        // İlk vagona gir
        if (locationController != null && startingWagon != null)
        {
            locationController.EnterWagon(startingWagon);
        }
        else
        {
            Debug.LogError("LocationController veya StartingWagon atanmamış!");
        }
    }
    
    // Debug için - oyun durumunu yazdır
    [ContextMenu("Print Game State")]
    public void PrintGameState()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.PrintAllFlags();
        }
    }
    
    // Test için - anahtarı al
    [ContextMenu("Get Conductor Key")]
    public void GetConductorKey()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetFlag("conductor_key", true);
            Debug.Log("Kondüktör anahtarı alındı!");
        }
    }
    
    // Test için - bulmacayı çöz
    [ContextMenu("Solve Puzzle")]
    public void SolvePuzzle()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetFlag("puzzle_solved", true);
            Debug.Log("Bulmaca çözüldü!");
        }
    }
}
