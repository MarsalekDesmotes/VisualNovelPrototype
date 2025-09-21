using UnityEngine;
using UnityEngine.UI;

public class NavigationButtonController : MonoBehaviour
{
    [Header("UI Components")]
    public Button button;
    public Text buttonText;
    
    [Header("Navigation Data")]
    public string pathDescription; // Sadece açıklama
    public string destinationNodeID; // Hedef node ID'si
    
    private LocationController locationController;
    
    private void Start()
    {
        locationController = FindObjectOfType<LocationController>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }
    
    public void Initialize(string description, string destinationID)
    {
        pathDescription = description;
        destinationNodeID = destinationID;
        
        // Buton metnini ayarla
        if (buttonText != null)
        {
            buttonText.text = pathDescription;
        }
    }
    
    private void OnButtonClicked()
    {
        if (string.IsNullOrEmpty(destinationNodeID))
        {
            Debug.LogError("destinationNodeID boş!");
            return;
        }
        
        if (locationController == null)
        {
            Debug.LogError("locationController null!");
            return;
        }
        
        Debug.Log($"Navigation butonu tıklandı: {pathDescription} -> {destinationNodeID}");
        
        // LocationController'a hedef node ID'sini gönder
        locationController.ChangeNodeByID(destinationNodeID);
    }
    
    // Bu metot artık kullanılmıyor - LocationController'da kontrol ediliyor
    public bool AreConditionsMet()
    {
        return true; // Her zaman true döndür, koşul kontrolü LocationController'da yapılıyor
    }
}
