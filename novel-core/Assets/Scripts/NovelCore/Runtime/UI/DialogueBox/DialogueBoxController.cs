using TMPro;

namespace NovelCore.Runtime.UI.DialogueBox
{

/// <summary>
/// Controller for dialogue box UI, handling text display and rendering.
/// </summary>
public class DialogueBoxController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    [Tooltip("TextMeshPro component for dialogue text")]
    private TextMeshProUGUI _dialogueText;

    [SerializeField]
    [Tooltip("TextMeshPro component for speaker name")]
    private TextMeshProUGUI _speakerNameText;

    [SerializeField]
    [Tooltip("Panel background for dialogue box")]
    private GameObject _dialoguePanel;

    [SerializeField]
    [Tooltip("Continue indicator (arrow, icon, etc.)")]
    private GameObject _continueIndicator;

    [Header("Text Animation")]
    [SerializeField]
    [Tooltip("Enable typewriter effect for dialogue text")]
    private bool _enableTypewriter = true;

    [SerializeField]
    [Tooltip("Characters per second for typewriter effect")]
    private float _typewriterSpeed = 30f;

    [Header("Speaker Highlight")]
    [SerializeField]
    [Tooltip("Highlight color for active speaker")]
    private Color _activeSpeakerColor = Color.white;

    [SerializeField]
    [Tooltip("Color for inactive/dimmed text")]
    private Color _inactiveSpeakerColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    private IDialogueSystem _dialogueSystem;
    private ILocalizationService _localizationService;
    
    private bool _isTyping;
    private string _currentFullText;
    private float _typewriterTimer;
    private int _visibleCharCount;

    private void Awake()
    {
        if (_dialogueText == null)
        {
            Debug.LogError("DialogueBoxController: Dialogue text reference not set!");
        }

        if (_dialoguePanel != null)
        {
            _dialoguePanel.SetActive(false);
        }

        if (_continueIndicator != null)
        {
            _continueIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Initialize the dialogue box with required services.
    /// </summary>
    public void Initialize(IDialogueSystem dialogueSystem, ILocalizationService localizationService)
    {
        _dialogueSystem = dialogueSystem ?? throw new System.ArgumentNullException(nameof(dialogueSystem));
        _localizationService = localizationService;

        _dialogueSystem.OnDialogueLineChanged += OnDialogueLineChanged;
        _dialogueSystem.OnDialogueComplete += OnDialogueComplete;
        _dialogueSystem.OnChoicePointReached += OnChoicePointReached;
    }

    private void OnDestroy()
    {
        if (_dialogueSystem != null)
        {
            _dialogueSystem.OnDialogueLineChanged -= OnDialogueLineChanged;
            _dialogueSystem.OnDialogueComplete -= OnDialogueComplete;
            _dialogueSystem.OnChoicePointReached -= OnChoicePointReached;
        }
    }

    private void Update()
    {
        if (_isTyping && _enableTypewriter)
        {
            UpdateTypewriter();
        }

        // Handle input for advancing dialogue
        if (_dialogueSystem != null && _dialogueSystem.IsPlaying && !_dialogueSystem.IsWaitingForChoice)
        {
            // Wait for typewriter to finish before showing continue indicator
            if (!_isTyping && _continueIndicator != null)
            {
                _continueIndicator.SetActive(true);
            }
        }
    }

    private void OnDialogueLineChanged(DialogueLineData lineData)
    {
        if (lineData == null)
        {
            Debug.LogError("DialogueBoxController: Received null dialogue line");
            return;
        }

        // Show dialogue panel
        if (_dialoguePanel != null)
        {
            _dialoguePanel.SetActive(true);
        }

        // Hide continue indicator while typing
        if (_continueIndicator != null)
        {
            _continueIndicator.SetActive(false);
        }

        // Update speaker name
        UpdateSpeakerName(lineData);

        // Get localized text
        string dialogueText = GetLocalizedText(lineData);

        // Display text with or without typewriter effect
        if (_enableTypewriter)
        {
            StartTypewriter(dialogueText);
        }
        else
        {
            DisplayText(dialogueText);
        }
    }

    private void OnDialogueComplete()
    {
        // Hide dialogue panel
        if (_dialoguePanel != null)
        {
            _dialoguePanel.SetActive(false);
        }

        if (_continueIndicator != null)
        {
            _continueIndicator.SetActive(false);
        }

        _isTyping = false;
    }

    private void OnChoicePointReached(ChoiceData choiceData)
    {
        // Hide continue indicator at choice points
        if (_continueIndicator != null)
        {
            _continueIndicator.SetActive(false);
        }

        // Keep dialogue panel visible during choices
        _isTyping = false;
    }

    private void UpdateSpeakerName(DialogueLineData lineData)
    {
        if (_speakerNameText == null)
        {
            return;
        }

        if (lineData.Speaker == null || !lineData.Speaker.RuntimeKeyIsValid())
        {
            // Narrator - hide speaker name
            _speakerNameText.gameObject.SetActive(false);
        }
        else
        {
            // TODO: Load character data to get character name
            // For now, use a placeholder
            _speakerNameText.gameObject.SetActive(true);
            _speakerNameText.text = "Character";
            _speakerNameText.color = _activeSpeakerColor;
        }
    }

    private string GetLocalizedText(DialogueLineData lineData)
    {
        // Try to get localized text
        if (_localizationService != null && !string.IsNullOrEmpty(lineData.TextKey))
        {
            string localizedText = _localizationService.GetLocalizedString(lineData.TextKey);
            if (!string.IsNullOrEmpty(localizedText))
            {
                return localizedText;
            }
        }

        // Fallback to fallback text
        return lineData.FallbackText ?? "[No dialogue text]";
    }

    private void StartTypewriter(string text)
    {
        _currentFullText = text;
        _isTyping = true;
        _typewriterTimer = 0f;
        _visibleCharCount = 0;

        if (_dialogueText != null)
        {
            _dialogueText.text = "";
        }
    }

    private void UpdateTypewriter()
    {
        if (string.IsNullOrEmpty(_currentFullText))
        {
            _isTyping = false;
            return;
        }

        _typewriterTimer += Time.deltaTime * _typewriterSpeed;
        int targetCharCount = Mathf.FloorToInt(_typewriterTimer);

        if (targetCharCount > _visibleCharCount)
        {
            _visibleCharCount = Mathf.Min(targetCharCount, _currentFullText.Length);
            
            if (_dialogueText != null)
            {
                _dialogueText.text = _currentFullText.Substring(0, _visibleCharCount);
            }

            // Check if typing complete
            if (_visibleCharCount >= _currentFullText.Length)
            {
                _isTyping = false;
            }
        }
    }

    private void DisplayText(string text)
    {
        _isTyping = false;
        if (_dialogueText != null)
        {
            _dialogueText.text = text;
        }
    }

    /// <summary>
    /// Skip typewriter effect and show full text immediately.
    /// </summary>
    public void SkipTypewriter()
    {
        if (_isTyping)
        {
            DisplayText(_currentFullText);
        }
    }

    /// <summary>
    /// Show the dialogue box.
    /// </summary>
    public void Show()
    {
        if (_dialoguePanel != null)
        {
            _dialoguePanel.SetActive(true);
        }
    }

    /// <summary>
    /// Hide the dialogue box.
    /// </summary>
    public void Hide()
    {
        if (_dialoguePanel != null)
        {
            _dialoguePanel.SetActive(false);
        }

        if (_continueIndicator != null)
        {
            _continueIndicator.SetActive(false);
        }

        _isTyping = false;
    }

    /// <summary>
    /// Set the typewriter speed.
    /// </summary>
    public void SetTypewriterSpeed(float speed)
    {
        _typewriterSpeed = Mathf.Max(1f, speed);
    }

    /// <summary>
    /// Enable or disable typewriter effect.
    /// </summary>
    public void SetTypewriterEnabled(bool enabled)
    {
        _enableTypewriter = enabled;
    }

    /// <summary>
    /// Highlight active speaker (for multi-character scenes).
    /// </summary>
    public void HighlightActiveSpeaker(string characterId)
    {
        // TODO: Implement speaker highlighting when character system is ready
        Debug.Log($"DialogueBoxController: Highlighting speaker {characterId}");
    }
}

}