using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [Header("UI Components")]
    public Image characterImage;
    public Button characterButton; // İsteğe bağlı
    
    [Header("Character Data")]
    public CharacterData characterData;
    
    private void Start()
    {
        // Karakter butonuna tıklama olayını ekle
        if (characterButton != null)
        {
            characterButton.onClick.AddListener(OnCharacterClicked);
        }
        else
        {
            // Button yoksa Image'a tıklama ekle
            if (characterImage != null)
            {
                // Image'a Button component ekle
                Button button = characterImage.gameObject.GetComponent<Button>();
                if (button == null)
                {
                    button = characterImage.gameObject.AddComponent<Button>();
                }
                button.onClick.AddListener(OnCharacterClicked);
            }
        }
    }
    
    public void Initialize(CharacterData data)
    {
        characterData = data;
        
        // Karakter görselini ayarla
        if (characterData != null && characterData.expressions.Count > 0)
        {
            // İlk ifadeyi kullan (normal)
            var normalExpression = characterData.expressions.Find(e => e.emotion == "normal");
            if (normalExpression != null && normalExpression.sprite != null)
            {
                characterImage.sprite = normalExpression.sprite;
            }
        }
    }
    
    private void OnCharacterClicked()
    {
        if (characterData != null && characterData.startingDialogue != null)
        {
            // Diyalog sistemini başlat
            Debug.Log($"Karakter tıklandı: {characterData.characterName}");
            
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(characterData);
            }
            else
            {
                Debug.LogError("DialogueManager bulunamadı!");
            }
        }
    }
    
    public void SetExpression(string emotion)
    {
        if (characterData != null)
        {
            var expression = characterData.expressions.Find(e => e.emotion == emotion);
            if (expression != null && expression.sprite != null)
            {
                characterImage.sprite = expression.sprite;
            }
        }
    }
}
