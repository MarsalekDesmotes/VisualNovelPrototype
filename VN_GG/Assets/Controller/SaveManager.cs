using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Singleton Pattern
    public static SaveManager Instance { get; private set; }
    
    [Header("Save Settings")]
    public string saveFileName = "game_save";
    public bool autoSave = true;
    public float autoSaveInterval = 60f; // 60 saniyede bir otomatik kaydet
    
    private float lastSaveTime;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void Start()
    {
        // Oyun başlarken kayıtları yükle
        LoadGame();
    }
    
    private void Update()
    {
        // Otomatik kaydetme
        if (autoSave && Time.time - lastSaveTime > autoSaveInterval)
        {
            SaveGame();
        }
    }
    
    // --- Diyalog Kaydetme Sistemi ---
    public void SaveDialogueProgress(string characterID, string dialogueNodeID)
    {
        if (GameStateManager.Instance == null) return;
        
        string key = $"dialogue_{characterID}_{dialogueNodeID}";
        GameStateManager.Instance.SetFlag(key, true);
        Debug.Log($"Diyalog kaydedildi: {key}");
        
        if (autoSave)
        {
            SaveGame();
        }
    }
    
    public bool HasSeenDialogue(string characterID, string dialogueNodeID)
    {
        if (GameStateManager.Instance == null) return false;
        
        string key = $"dialogue_{characterID}_{dialogueNodeID}";
        return GameStateManager.Instance.GetFlag(key);
    }
    
    // --- Oyun Kaydetme/Yükleme ---
    public void SaveGame()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager bulunamadı!");
            return;
        }
        
        // Tüm flag'leri kaydet
        var allFlags = GameStateManager.Instance.GetAllFlags();
        var allRelationships = GameStateManager.Instance.GetAllRelationships();
        
        // Flag'leri kaydet
        string flagKeys = "";
        foreach (var flag in allFlags)
        {
            PlayerPrefs.SetInt($"flag_{flag.Key}", flag.Value ? 1 : 0);
            flagKeys += flag.Key + ",";
        }
        PlayerPrefs.SetString("all_flag_keys", flagKeys);
        
        // İlişkileri kaydet
        string relationshipKeys = "";
        foreach (var relationship in allRelationships)
        {
            PlayerPrefs.SetInt($"relationship_{relationship.Key}", relationship.Value);
            relationshipKeys += relationship.Key + ",";
        }
        PlayerPrefs.SetString("all_relationship_keys", relationshipKeys);
        
        // Kayıt zamanını kaydet
        PlayerPrefs.SetString("last_save_time", System.DateTime.Now.ToString());
        
        PlayerPrefs.Save();
        lastSaveTime = Time.time;
        Debug.Log("Oyun kaydedildi!");
    }
    
    public void LoadGame()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager bulunamadı!");
            return;
        }
        
        // Flag'leri yükle
        string flagKeys = PlayerPrefs.GetString("all_flag_keys", "");
        if (!string.IsNullOrEmpty(flagKeys))
        {
            string[] keys = flagKeys.Split(',');
            foreach (string key in keys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    bool value = PlayerPrefs.GetInt($"flag_{key}", 0) == 1;
                    GameStateManager.Instance.SetFlag(key, value);
                }
            }
        }
        
        // İlişkileri yükle
        string relationshipKeys = PlayerPrefs.GetString("all_relationship_keys", "");
        if (!string.IsNullOrEmpty(relationshipKeys))
        {
            string[] keys = relationshipKeys.Split(',');
            foreach (string key in keys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    int value = PlayerPrefs.GetInt($"relationship_{key}", 0);
                    GameStateManager.Instance.ChangeRelationship(key, value);
                }
            }
        }
        
        Debug.Log("Oyun yüklendi!");
    }
    
    // --- Kayıt Yönetimi ---
    public void DeleteSave()
    {
        // Tüm flag'leri sil
        string flagKeys = PlayerPrefs.GetString("all_flag_keys", "");
        if (!string.IsNullOrEmpty(flagKeys))
        {
            string[] keys = flagKeys.Split(',');
            foreach (string key in keys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    PlayerPrefs.DeleteKey($"flag_{key}");
                }
            }
        }
        
        // Tüm ilişkileri sil
        string relationshipKeys = PlayerPrefs.GetString("all_relationship_keys", "");
        if (!string.IsNullOrEmpty(relationshipKeys))
        {
            string[] keys = relationshipKeys.Split(',');
            foreach (string key in keys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    PlayerPrefs.DeleteKey($"relationship_{key}");
                }
            }
        }
        
        // Diğer kayıt verilerini sil
        PlayerPrefs.DeleteKey("all_flag_keys");
        PlayerPrefs.DeleteKey("all_relationship_keys");
        PlayerPrefs.DeleteKey("last_save_time");
        
        PlayerPrefs.Save();
        Debug.Log("Kayıt silindi!");
    }
    
    public string GetLastSaveTime()
    {
        return PlayerPrefs.GetString("last_save_time", "Hiç kayıt yok");
    }
    
    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey("all_flag_keys") || PlayerPrefs.HasKey("all_relationship_keys");
    }
    
    // --- Debug Metotları ---
    [ContextMenu("Save Game")]
    public void SaveGameDebug()
    {
        SaveGame();
    }
    
    [ContextMenu("Load Game")]
    public void LoadGameDebug()
    {
        LoadGame();
    }
    
    [ContextMenu("Delete Save")]
    public void DeleteSaveDebug()
    {
        DeleteSave();
    }
    
    [ContextMenu("Print Save Info")]
    public void PrintSaveInfo()
    {
        Debug.Log($"Son kayıt zamanı: {GetLastSaveTime()}");
        Debug.Log($"Kayıt var mı: {HasSaveData()}");
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.PrintAllFlags();
        }
    }
}
