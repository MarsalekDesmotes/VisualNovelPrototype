using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // --- Singleton Pattern Başlangıcı ---
    public static GameStateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler arası geçişte bu objenin silinmemesini sağlar
        }
    }
    // --- Singleton Pattern Sonu ---


    // Oyundaki "evet/hayır" veya "yapıldı/yapılmadı" durumlarını tutan bayraklar
    public Dictionary<string, bool> storyFlags = new Dictionary<string, bool>();

    // Karakterlerle olan ilişki seviyelerini tutar
    public Dictionary<string, int> characterRelationships = new Dictionary<string, int>();


    // --- Flag Metotları ---
    public void SetFlag(string key, bool value)
    {
        if (storyFlags.ContainsKey(key))
        {
            storyFlags[key] = value;
        }
        else
        {
            storyFlags.Add(key, value);
        }
    }

    public bool GetFlag(string key)
    {
        return storyFlags.ContainsKey(key) && storyFlags[key];
    }

    // --- İlişki Metotları ---
    public void ChangeRelationship(string characterID, int amount)
    {
        if (characterRelationships.ContainsKey(characterID))
        {
            characterRelationships[characterID] += amount;
        }
        else
        {
            characterRelationships.Add(characterID, amount);
        }
    }

    public int GetRelationship(string characterID)
    {
        if (characterRelationships.ContainsKey(characterID))
        {
            return characterRelationships[characterID];
        }
        return 0; // Eğer daha önce bir ilişki kurulmadıysa varsayılan değer 0
    }

    // --- Oyun Başlangıç Ayarları ---
    public void InitializeGame()
    {
        // Başlangıç flag'lerini ayarla
        SetFlag("conductor_talked", false);
        SetFlag("conductor_key", false);
        SetFlag("puzzle_solved", false);
        SetFlag("game_completed", false);
        
        // Başlangıç ilişkilerini ayarla
        characterRelationships.Clear();
    }

    
    // --- SaveManager için Metotlar ---
    public Dictionary<string, bool> GetAllFlags()
    {
        return new Dictionary<string, bool>(storyFlags);
    }
    
    public Dictionary<string, int> GetAllRelationships()
    {
        return new Dictionary<string, int>(characterRelationships);
    }
    
    // --- Debug Metotları ---
    public void PrintAllFlags()
    {
        Debug.Log("=== STORY FLAGS ===");
        foreach (var flag in storyFlags)
        {
            Debug.Log($"{flag.Key}: {flag.Value}");
        }
        
        Debug.Log("=== CHARACTER RELATIONSHIPS ===");
        foreach (var relationship in characterRelationships)
        {
            Debug.Log($"{relationship.Key}: {relationship.Value}");
        }
    }
}