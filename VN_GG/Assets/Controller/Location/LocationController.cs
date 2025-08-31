using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationController : MonoBehaviour
{
    // DIŞARIDAN BAĞLANACAKLAR
    public Image backgroundImageHolder; // Canvas'taki ana arkaplan imajı
    public GameObject navigationButtonPrefab; // Geçişler için oluşturulacak buton prefabı
    public Transform navigationButtonParent; // Butonların ekleneceği panel
    public GameObject characterPrefab; // Karakterleri göstermek için prefab
    public Transform characterParent; // Karakterlerin ekleneceği panel

    // SİSTEM REFERANSLARI
    private GameStateManager gameStateManager; // Singleton veya service locator ile erişilecek

    // MEVCUT DURUM
    private WagonData currentWagon;
    private LocationNode currentNode;

    void Start()
    {
        // Örnek: gameStateManager = GameManager.Instance.GameStateManager;
    }

    public void EnterWagon(WagonData wagon)
    {
        currentWagon = wagon;
        ChangeNode(wagon.startingNode);
    }

    private void ChangeNode(LocationNode newNode)
    {
        currentNode = newNode;
        RenderNode();
    }

    // Bu fonksiyon mevcut 'Node'a göre ekranı yeniden çizer.
    private void RenderNode()
    {
        // 1. Arkaplanı güncelle
        backgroundImageHolder.sprite = currentNode.backgroundImage;

        // 2. Eski butonları ve karakterleri temizle
        foreach (Transform child in navigationButtonParent) Destroy(child.gameObject);
        foreach (Transform child in characterParent) Destroy(child.gameObject);

        // 3. Geçerli Navigasyon Yollarını (NavigationPath) oluştur
        foreach (var path in currentNode.navigationPaths)
        {
            if (AreConditionsMet(path.conditions))
            {
                // Butonu oluştur (pozisyonunu, görselini vs. path'e göre ayarlayabilirsin)
                GameObject buttonGO = Instantiate(navigationButtonPrefab, navigationButtonParent);
                // Butona tıklandığında ne olacağını belirle
                buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                    ChangeNode(path.destinationNode);
                });
            }
        }

        // 4. Karakterleri yerleştir
        foreach (var placement in currentNode.charactersInNode)
        {
            GameObject charGO = Instantiate(characterPrefab, characterParent);
            // Karakterin pozisyonunu, görselini vs. ayarla
            // charGO.GetComponent<CharacterController>().Initialize(placement.character);
            // charGO.GetComponent<RectTransform>().anchoredPosition = placement.positionOnScreen;
        }
    }

    // Verilen şartların GameState'e uyup uymadığını kontrol eder.
    private bool AreConditionsMet(List<GameCondition> conditions)
    {
        foreach (var condition in conditions)
        {
            // GameStateManager'dan flag'in değerini al ve beklenen değerle karşılaştır.
            // Eğer bir tane bile şart tutmazsa, direkt false dön.
            // bool flagValue = gameStateManager.GetFlag(condition.requiredFlag);
            // if (flagValue != condition.expectedValue)
            // {
            //     return false;
            // }
        }
        // Tüm şartlar sağlandıysa true dön.
        return true;
    }
}