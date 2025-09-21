using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    // Singleton Pattern
    public static DialogueManager Instance { get; private set; }
    
    [Header("UI Components")]
    public GameObject dialoguePanel; // Diyalog paneli
    public TMP_Text dialogueText; // Diyalog metni
    public Transform choiceButtonParent; // Seçenek butonlarının parent'ı
    public GameObject choiceButtonPrefab; // Seçenek butonu prefabı
    
    [Header("Spacing")]
    public float buttonSpacing = 10f; // Butonlar arası mesafe
    
    [Header("Typewriter Effect")]
    public float typewriterSpeed = 0.05f; // Her karakter arası süre
    public bool useTypewriterEffect = true;
    
    [Header("Fade Effect")]
    public float fadeInDuration = 0.5f; // Fade süresi
    
    [Header("Character Display")]
    public TMP_Text characterNameText; // Karakter ismi
    public Image characterImage; // Karakter görseli
    
    // Mevcut diyalog durumu
    private DialogueTree currentDialogueTree;
    private DialogueNode currentNode;
    private CharacterData currentCharacter;
    
    // Typewriter effect için
    private Coroutine typewriterCoroutine;
    private bool isTypewriterActive = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        // Başlangıçta diyalog panelini gizle
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
    
    // Diyalog başlat
    public void StartDialogue(CharacterData character)
    {
        if (character == null || character.startingDialogue == null)
        {
            Debug.LogError("Karakter veya diyalog ağacı bulunamadı!");
            return;
        }
        
        currentCharacter = character;
        currentDialogueTree = character.startingDialogue;
        currentNode = currentDialogueTree.startingNode;
        
        // Diyalog panelini göster
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        
        // Karakter bilgilerini göster
        if (characterNameText != null)
        {
            characterNameText.text = character.characterName;
        }
        
        // Karakter görselini ayarla
        if (characterImage != null && character.expressions.Count > 0)
        {
            var normalExpression = character.expressions.Find(e => e.emotion == "normal");
            if (normalExpression != null && normalExpression.sprite != null)
            {
                characterImage.sprite = normalExpression.sprite;
            }
        }
        
        // İlk diyalog node'unu göster
        ShowDialogueNode(currentNode);
    }
    
    // Diyalog node'unu göster
    private void ShowDialogueNode(DialogueNode node)
    {
        if (node == null) return;
        
        currentNode = node;
        
        // Karakter ifadesini güncelle
        if (currentCharacter != null && !string.IsNullOrEmpty(node.emotion))
        {
            SetCharacterExpression(node.emotion);
        }
        
        // Bu diyalogu kaydet
        if (currentCharacter != null && !string.IsNullOrEmpty(node.nodeID))
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveDialogueProgress(currentCharacter.characterID, node.nodeID);
            }
        }
        
        // Typewriter effect ile metni göster
        if (useTypewriterEffect && dialogueText != null)
        {
            StartTypewriterEffect(node.dialogueText, node.choices);
        }
        else
        {
            // Typewriter effect yoksa direkt göster
            if (dialogueText != null)
            {
                dialogueText.text = node.dialogueText;
            }
            CreateChoiceButtons(node.choices);
        }
    }
    
    // Seçenek butonlarını oluştur
    private void CreateChoiceButtons(List<Choice> choices)
    {
        // Eski butonları temizle
        if (choiceButtonParent != null)
        {
            foreach (Transform child in choiceButtonParent)
            {
                Destroy(child.gameObject);
            }
        }
        
        // Yeni butonları oluştur
        if (choices != null && choices.Count > 0)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];
                if (choiceButtonPrefab != null && choiceButtonParent != null)
                {
                    GameObject buttonGO = Instantiate(choiceButtonPrefab, choiceButtonParent);
                    Button button = buttonGO.GetComponent<Button>();
                    
                    // TextMeshPro component'ini bul (prefab'da olmalı)
                    TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = choice.choiceText;
                    }
                    else
                    {
                        Debug.LogWarning("ChoiceButtonPrefab'da TMP_Text component bulunamadı!");
                    }
                    
                    if (button != null)
                    {
                        button.onClick.AddListener(() => OnChoiceSelected(choice));
                    }
                    
                    // Buton pozisyonunu ayarla (mesafe için)
                    RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
                    if (buttonRect != null)
                    {
                        buttonRect.anchoredPosition = new Vector2(0, -i * (buttonRect.sizeDelta.y + buttonSpacing));
                    }
                }
            }
        }
        else
        {
            // Seçenek yoksa "Devam" butonu oluştur
            CreateContinueButton();
        }
    }
    
    // "Devam" butonu oluştur
    private void CreateContinueButton()
    {
        if (choiceButtonPrefab != null && choiceButtonParent != null)
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choiceButtonParent);
            Button button = buttonGO.GetComponent<Button>();
            
            // TextMeshPro component'ini bul (prefab'da olmalı)
            TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "Devam";
            }
            else
            {
                Debug.LogWarning("ChoiceButtonPrefab'da TMP_Text component bulunamadı!");
            }
            
            if (button != null)
            {
                button.onClick.AddListener(CloseDialogue);
            }
        }
    }
    
    // Seçenek seçildiğinde
    private void OnChoiceSelected(Choice choice)
    {
        // Seçenek eylemlerini çalıştır
        if (choice.actions != null)
        {
            foreach (var action in choice.actions)
            {
                ExecuteDialogueAction(action);
            }
        }
        
        // Hedef node'a git
        if (choice.destinationNode != null)
        {
            ShowDialogueNode(choice.destinationNode);
        }
        else
        {
            // Hedef node yoksa diyalogu kapat
            CloseDialogue();
        }
    }
    
    // Diyalog eylemini çalıştır
    private void ExecuteDialogueAction(DialogueAction action)
    {
        if (GameStateManager.Instance == null) 
        {
            Debug.LogError("GameStateManager bulunamadı!");
            return;
        }
        
        switch (action.actionType)
        {
            case DialogueAction.ActionType.SetStoryFlag:
                GameStateManager.Instance.SetFlag(action.parameter1, action.boolValue);
                Debug.Log($"Flag set edildi: {action.parameter1} = {action.boolValue}");
                
                // Tüm flag'leri yazdır
                GameStateManager.Instance.PrintAllFlags();
                break;
                
            case DialogueAction.ActionType.ChangeRelationship:
                GameStateManager.Instance.ChangeRelationship(action.parameter1, action.intValue);
                Debug.Log($"İlişki değişti: {action.parameter1} += {action.intValue}");
                break;
        }
    }
    
    // Karakter ifadesini ayarla
    private void SetCharacterExpression(string emotion)
    {
        if (currentCharacter == null) return;
        
        var expression = currentCharacter.expressions.Find(e => e.emotion == emotion);
        if (expression != null && expression.sprite != null && characterImage != null)
        {
            characterImage.sprite = expression.sprite;
        }
    }
    
    // Diyalogu kapat
    public void CloseDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        // Seçenek butonlarını temizle
        if (choiceButtonParent != null)
        {
            foreach (Transform child in choiceButtonParent)
            {
                Destroy(child.gameObject);
            }
        }
        
        // LocationController'ı yenile (navigation butonları güncellensin)
        LocationController locationController = FindObjectOfType<LocationController>();
        if (locationController != null)
        {
            locationController.RefreshCurrentNode();
        }
    }
    
    // Typewriter effect başlat
    private void StartTypewriterEffect(string fullText, List<Choice> choices)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }
        
        typewriterCoroutine = StartCoroutine(TypewriterEffect(fullText, choices));
    }
    
    // Typewriter effect coroutine
    private System.Collections.IEnumerator TypewriterEffect(string fullText, List<Choice> choices)
    {
        isTypewriterActive = true;
        
        if (dialogueText != null)
        {
            dialogueText.text = "";
            
            for (int i = 0; i <= fullText.Length; i++)
            {
                dialogueText.text = fullText.Substring(0, i);
                yield return new WaitForSeconds(typewriterSpeed);
            }
        }
        
        isTypewriterActive = false;
        
        // Typewriter bittikten sonra seçenekleri fade ile göster
        yield return new WaitForSeconds(0.2f); // Kısa bekleme
        CreateChoiceButtonsWithFade(choices);
    }
    
    // Seçenek butonlarını fade effect ile oluştur
    private void CreateChoiceButtonsWithFade(List<Choice> choices)
    {
        // Eski butonları temizle
        if (choiceButtonParent != null)
        {
            foreach (Transform child in choiceButtonParent)
            {
                Destroy(child.gameObject);
            }
        }
        
        // Yeni butonları oluştur
        if (choices != null && choices.Count > 0)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];
                if (choiceButtonPrefab != null && choiceButtonParent != null)
                {
                    GameObject buttonGO = Instantiate(choiceButtonPrefab, choiceButtonParent);
                    Button button = buttonGO.GetComponent<Button>();
                    
                    // TextMeshPro component'ini bul (prefab'da olmalı)
                    TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = choice.choiceText;
                    }
                    else
                    {
                        Debug.LogWarning("ChoiceButtonPrefab'da TMP_Text component bulunamadı!");
                    }
                    
                    if (button != null)
                    {
                        button.onClick.AddListener(() => OnChoiceSelected(choice));
                    }
                    
                    // Buton pozisyonunu ayarla (mesafe için)
                    RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
                    if (buttonRect != null)
                    {
                        buttonRect.anchoredPosition = new Vector2(0, -i * (buttonRect.sizeDelta.y + buttonSpacing));
                    }
                    
                    // Fade effect başlat
                    StartCoroutine(FadeInButton(buttonGO, i * 0.1f));
                }
            }
        }
        else
        {
            // Seçenek yoksa "Devam" butonu oluştur
            CreateContinueButtonWithFade();
        }
    }
    
    // "Devam" butonu fade effect ile oluştur
    private void CreateContinueButtonWithFade()
    {
        if (choiceButtonPrefab != null && choiceButtonParent != null)
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choiceButtonParent);
            Button button = buttonGO.GetComponent<Button>();
            
            // TextMeshPro component'ini bul (prefab'da olmalı)
            TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "Devam";
            }
            else
            {
                Debug.LogWarning("ChoiceButtonPrefab'da TMP_Text component bulunamadı!");
            }
            
            if (button != null)
            {
                button.onClick.AddListener(CloseDialogue);
            }
            
            // Fade effect başlat
            StartCoroutine(FadeInButton(buttonGO, 0f));
        }
    }
    
    // Buton fade effect
    private System.Collections.IEnumerator FadeInButton(GameObject buttonGO, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // CanvasGroup component'i ekle
        CanvasGroup canvasGroup = buttonGO.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = buttonGO.AddComponent<CanvasGroup>();
        }
        
        // Başlangıçta görünmez yap
        canvasGroup.alpha = 0f;
        
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
}
