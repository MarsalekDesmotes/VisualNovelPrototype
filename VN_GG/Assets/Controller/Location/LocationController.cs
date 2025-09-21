using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationController : MonoBehaviour
{
    // DIŞARIDAN BAĞLANACAKLAR
    public Image backgroundImageHolder; // Canvas'taki ana arkaplan imajı
    public GameObject navigationButtonPrefab; // Geçişler için oluşturulacak buton prefabı
    public Transform navigationButtonParent; // Butonların ekleneceği panel
    public Transform characterParent; // Karakterlerin ekleneceği panel
    
    // KARAKTER PREFABLARI - Wagon'a göre otomatik seçilecek
    public GameObject conductorPrefab; // Kondüktör prefabı
    public GameObject waiterPrefab; // Garson prefabı
    public GameObject engineerPrefab; // Makinist prefabı

    // SİSTEM REFERANSLARI
    private GameStateManager gameStateManager; // Singleton veya service locator ile erişilecek

    // MEVCUT DURUM
    private WagonData currentWagon;
    private LocationNode currentNode;

    void Start()
    {
        gameStateManager = GameStateManager.Instance;
        if (gameStateManager == null)
        {
            Debug.LogError("GameStateManager bulunamadı!");
        }
    }

    public void EnterWagon(WagonData wagon)
    {
        currentWagon = wagon;
        ChangeNode(wagon.startingNode);
    }

    public void ChangeNode(LocationNode newNode)
    {
        if (newNode == null)
        {
            Debug.LogError("ChangeNode'a null LocationNode geçildi!");
            return;
        }
        
        Debug.Log($"LocationNode değiştiriliyor: {newNode.nodeID}");
        currentNode = newNode;
        RenderNode();
    }
    
    // Node ID'sine göre node değiştir
    public void ChangeNodeByID(string nodeID)
    {
        if (string.IsNullOrEmpty(nodeID))
        {
            Debug.LogError("ChangeNodeByID'e boş nodeID geçildi!");
            return;
        }
        
        if (currentWagon == null)
        {
            Debug.LogError("currentWagon null!");
            return;
        }
        
        // Wagon'daki node'ları ara
        LocationNode targetNode = null;
        foreach (var node in currentWagon.nodes)
        {
            if (node.nodeID == nodeID)
            {
                targetNode = node;
                break;
            }
        }
        
        if (targetNode == null)
        {
            Debug.LogError($"Node bulunamadı: {nodeID}");
            return;
        }
        
        Debug.Log($"Node ID ile değiştiriliyor: {nodeID}");
        ChangeNode(targetNode);
    }
    
    // Mevcut node'u yenile (navigation butonları güncellensin)
    public void RefreshCurrentNode()
    {
        if (currentNode != null)
        {
            RenderNode();
        }
    }

    // Bu fonksiyon mevcut 'Node'a göre ekranı yeniden çizer.
    private void RenderNode()
    {
        if (currentNode == null)
        {
            Debug.LogError("currentNode null! RenderNode çağrılamaz.");
            return;
        }
        
        // 1. Arkaplanı güncelle
        if (backgroundImageHolder != null)
        {
            backgroundImageHolder.sprite = currentNode.backgroundImage;
        }
        else
        {
            Debug.LogWarning("backgroundImageHolder null!");
        }

        // 2. Eski butonları ve karakterleri temizle
        if (navigationButtonParent != null)
        {
            foreach (Transform child in navigationButtonParent) Destroy(child.gameObject);
        }
        else
        {
            Debug.LogWarning("navigationButtonParent null!");
        }
        
        if (characterParent != null)
        {
            foreach (Transform child in characterParent) Destroy(child.gameObject);
        }
        else
        {
            Debug.LogWarning("characterParent null!");
        }

        // 3. Geçerli Navigasyon Yollarını (NavigationPath) oluştur
        Debug.Log($"Navigation paths sayısı: {currentNode.navigationPaths?.Count ?? 0}");
        
        if (currentNode.navigationPaths != null)
        {
            foreach (var path in currentNode.navigationPaths)
            {
                Debug.Log($"Navigation path kontrol ediliyor: {path.pathDescription}");
                bool conditionsMet = AreConditionsMet(path.conditions);
                Debug.Log($"Koşullar sağlandı mı: {conditionsMet}");
                
                if (conditionsMet)
                {
                    // Butonu oluştur
                    GameObject buttonGO = Instantiate(navigationButtonPrefab, navigationButtonParent);
                    Debug.Log($"Navigation butonu oluşturuldu: {path.pathDescription}");
                    
                    // NavigationButtonController'ı ayarla
                    var buttonController = buttonGO.GetComponent<NavigationButtonController>();
                    if (buttonController != null)
                    {
                        buttonController.Initialize(path.pathDescription, path.destinationNodeID);
                    }
                    else
                    {
                        Debug.LogWarning("NavigationButtonController component bulunamadı!");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("currentNode.navigationPaths null!");
        }

        // 4. Karakterleri yerleştir - Wagon'a göre otomatik seç
        SpawnCharactersForCurrentWagon();
    }

    // Verilen şartların GameState'e uyup uymadığını kontrol eder.
    private bool AreConditionsMet(List<GameCondition> conditions)
    {
        if (gameStateManager == null) 
        {
            Debug.LogWarning("GameStateManager bulunamadı! Tüm koşullar geçiliyor.");
            return true; // GameStateManager yoksa tüm koşulları geç
        }
        
        if (conditions == null || conditions.Count == 0)
        {
            Debug.Log("Koşul yok, geçiliyor.");
            return true;
        }
        
        foreach (var condition in conditions)
        {
            // GameStateManager'dan flag'in değerini al ve beklenen değerle karşılaştır.
            // Eğer bir tane bile şart tutmazsa, direkt false dön.
            bool flagValue = gameStateManager.GetFlag(condition.requiredFlag);
            Debug.Log($"Koşul kontrol ediliyor: {condition.requiredFlag} = {flagValue} (beklenen: {condition.expectedValue})");
            
            if (flagValue != condition.expectedValue)
            {
                Debug.Log($"Koşul başarısız: {condition.requiredFlag}");
                return false;
            }
        }
        
        Debug.Log("Tüm koşullar sağlandı!");
        // Tüm şartlar sağlandıysa true dön.
        return true;
    }
    
    // Wagon'a göre karakterleri otomatik spawn et
    private void SpawnCharactersForCurrentWagon()
    {
        if (currentWagon == null) return;
        
        GameObject characterPrefabToUse = null;
        Vector2 characterPosition = Vector2.zero;
        
        // Wagon ID'sine göre karakter seç
        switch (currentWagon.wagonID)
        {
            case "wagon1_passenger":
                characterPrefabToUse = conductorPrefab;
                characterPosition = new Vector2(0.7f, 0.5f); // Sağ tarafta
                break;
                
            case "wagon2_dining":
                characterPrefabToUse = waiterPrefab;
                characterPosition = new Vector2(0.3f, 0.5f); // Sol tarafta
                break;
                
            case "wagon3_engine":
                characterPrefabToUse = engineerPrefab;
                characterPosition = new Vector2(0.5f, 0.5f); // Ortada
                break;
        }
        
        // Karakteri spawn et
        if (characterPrefabToUse != null)
        {
            GameObject charGO = Instantiate(characterPrefabToUse, characterParent);
            
            // Pozisyonu ayarla
            RectTransform rectTransform = charGO.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = characterPosition;
            }
            
            // CharacterController'ı initialize et
            CharacterController characterController = charGO.GetComponent<CharacterController>();
            if (characterController != null)
            {
                // Wagon'a göre karakter datasını al
                CharacterData characterData = GetCharacterDataForWagon(currentWagon.wagonID);
                if (characterData != null)
                {
                    characterController.Initialize(characterData);
                }
            }
            
            Debug.Log($"Karakter spawn edildi: {currentWagon.wagonID} - {characterPrefabToUse.name}");
        }
    }
    
    // Wagon ID'sine göre karakter datasını al
    private CharacterData GetCharacterDataForWagon(string wagonID)
    {
        switch (wagonID)
        {
            case "wagon1_passenger":
                // Kondüktör datasını yükle
                return Resources.Load<CharacterData>("Characters/Conductor/Conductor");
                
            case "wagon2_dining":
                // Garson datasını yükle
                return Resources.Load<CharacterData>("Characters/Waiter/Waiter");
                
            case "wagon3_engine":
                // Makinist datasını yükle
                return Resources.Load<CharacterData>("Characters/Engineer/Engineer");
                
            default:
                return null;
        }
    }
}