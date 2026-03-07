namespace NovelCore.Runtime.UI.ChoiceButtons
{

/// <summary>
/// Controller for choice button UI, managing display and selection of choices.
/// </summary>
public class ChoiceUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    [Tooltip("Parent container for choice buttons")]
    private Transform _choiceContainer;

    [SerializeField]
    [Tooltip("Prefab for individual choice buttons")]
    private GameObject _choiceButtonPrefab;

    [SerializeField]
    [Tooltip("Choice panel background")]
    private GameObject _choicePanel;

    [Header("Layout Settings")]
    [SerializeField]
    [Tooltip("Spacing between choice buttons")]
    private float _buttonSpacing = 10f;

    [SerializeField]
    [Tooltip("Maximum buttons to display at once")]
    private int _maxVisibleButtons = 6;

    [Header("Timer Settings")]
    [SerializeField]
    [Tooltip("Timer display text")]
    private TMPro.TextMeshProUGUI _timerText;

    [SerializeField]
    [Tooltip("Timer fill image")]
    private UnityEngine.UI.Image _timerFillImage;

    private IDialogueSystem _dialogueSystem;
    private ILocalizationService _localizationService;
    
    private ChoiceData _currentChoice;
    private List<GameObject> _activeButtons = new();
    private float _timerRemaining;
    private bool _isTimerActive;

    private void Awake()
    {
        if (_choicePanel != null)
        {
            _choicePanel.SetActive(false);
        }

        if (_timerText != null)
        {
            _timerText.gameObject.SetActive(false);
        }

        if (_timerFillImage != null)
        {
            _timerFillImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Initialize the choice UI with required services.
    /// </summary>
    public void Initialize(IDialogueSystem dialogueSystem, ILocalizationService localizationService)
    {
        _dialogueSystem = dialogueSystem ?? throw new System.ArgumentNullException(nameof(dialogueSystem));
        _localizationService = localizationService;

        _dialogueSystem.OnChoicePointReached += OnChoicePointReached;
        _dialogueSystem.OnDialogueLineChanged += OnDialogueLineChanged;
        _dialogueSystem.OnDialogueComplete += OnDialogueComplete;
    }

    private void OnDestroy()
    {
        if (_dialogueSystem != null)
        {
            _dialogueSystem.OnChoicePointReached -= OnChoicePointReached;
            _dialogueSystem.OnDialogueLineChanged -= OnDialogueLineChanged;
            _dialogueSystem.OnDialogueComplete -= OnDialogueComplete;
        }
    }

    private void Update()
    {
        if (_isTimerActive && _timerRemaining > 0)
        {
            UpdateTimer();
        }
    }

    private void OnChoicePointReached(ChoiceData choiceData)
    {
        if (choiceData == null)
        {
            Debug.LogError("ChoiceUIController: Received null choice data");
            return;
        }

        _currentChoice = choiceData;
        DisplayChoices(choiceData);
    }

    private void OnDialogueLineChanged(DialogueLineData lineData)
    {
        // Hide choices when dialogue continues
        HideChoices();
    }

    private void OnDialogueComplete()
    {
        // Hide choices when dialogue ends
        HideChoices();
    }

    private void DisplayChoices(ChoiceData choiceData)
    {
        // Clear existing buttons
        ClearChoiceButtons();

        // Show choice panel
        if (_choicePanel != null)
        {
            _choicePanel.SetActive(true);
        }

        // Display prompt text if available
        string promptText = GetLocalizedText(choiceData.PromptTextKey, choiceData.FallbackPromptText);
        if (!string.IsNullOrEmpty(promptText))
        {
            Debug.Log($"ChoiceUIController: Prompt - {promptText}");
        }

        // Create buttons for each option
        int buttonCount = Mathf.Min(choiceData.Options.Count, _maxVisibleButtons);
        for (int i = 0; i < buttonCount; i++)
        {
            var option = choiceData.Options[i];
            CreateChoiceButton(option, i);
        }

        // Setup timer if applicable
        if (choiceData.TimerSeconds > 0)
        {
            StartTimer(choiceData.TimerSeconds);
        }

        Debug.Log($"ChoiceUIController: Displaying {buttonCount} choices");
    }

    private void CreateChoiceButton(ChoiceOption option, int index)
    {
        if (_choiceButtonPrefab == null || _choiceContainer == null)
        {
            Debug.LogError("ChoiceUIController: Choice button prefab or container not set");
            return;
        }

        // Instantiate button
        GameObject buttonObj = Instantiate(_choiceButtonPrefab, _choiceContainer);
        _activeButtons.Add(buttonObj);

        // Get button component
        var button = buttonObj.GetComponent<UnityEngine.UI.Button>();
        if (button == null)
        {
            Debug.LogError("ChoiceUIController: Choice button prefab missing Button component");
            return;
        }

        // Get text component (try TextMeshProUGUI first, then Text)
        var tmpText = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            string optionText = GetLocalizedText(option.textKey, option.fallbackText);
            tmpText.text = optionText;
        }
        else
        {
            var legacyText = buttonObj.GetComponentInChildren<UnityEngine.UI.Text>();
            if (legacyText != null)
            {
                string optionText = GetLocalizedText(option.textKey, option.fallbackText);
                legacyText.text = optionText;
            }
        }

        // Set button icon if available
        if (option.icon != null)
        {
            var iconImage = buttonObj.GetComponentInChildren<UnityEngine.UI.Image>();
            if (iconImage != null)
            {
                iconImage.sprite = option.icon;
            }
        }

        // Setup button click handler
        int choiceIndex = index;
        button.onClick.AddListener(() => OnChoiceSelected(choiceIndex));

        // Disable button if not available
        button.interactable = option.isAvailable;
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        if (_dialogueSystem == null || _currentChoice == null)
        {
            Debug.LogError("ChoiceUIController: Cannot select choice - dialogue system not initialized");
            return;
        }

        Debug.Log($"ChoiceUIController: Choice {choiceIndex} selected");

        // Stop timer
        StopTimer();

        // Hide choices
        HideChoices();

        // Notify dialogue system
        _dialogueSystem.SelectChoice(choiceIndex);
    }

    private void HideChoices()
    {
        // Clear buttons
        ClearChoiceButtons();

        // Hide panel
        if (_choicePanel != null)
        {
            _choicePanel.SetActive(false);
        }

        // Stop timer
        StopTimer();

        _currentChoice = null;
    }

    private void ClearChoiceButtons()
    {
        foreach (var button in _activeButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }
        _activeButtons.Clear();
    }

    private void StartTimer(float duration)
    {
        _timerRemaining = duration;
        _isTimerActive = true;

        if (_timerText != null)
        {
            _timerText.gameObject.SetActive(true);
        }

        if (_timerFillImage != null)
        {
            _timerFillImage.gameObject.SetActive(true);
            _timerFillImage.fillAmount = 1f;
        }
    }

    private void UpdateTimer()
    {
        _timerRemaining -= Time.deltaTime;

        if (_timerText != null)
        {
            _timerText.text = Mathf.CeilToInt(_timerRemaining).ToString();
        }

        if (_timerFillImage != null)
        {
            float fillAmount = _timerRemaining / _currentChoice.TimerSeconds;
            _timerFillImage.fillAmount = fillAmount;
        }

        // Timer expired
        if (_timerRemaining <= 0)
        {
            OnTimerExpired();
        }
    }

    private void StopTimer()
    {
        _isTimerActive = false;
        _timerRemaining = 0;

        if (_timerText != null)
        {
            _timerText.gameObject.SetActive(false);
        }

        if (_timerFillImage != null)
        {
            _timerFillImage.gameObject.SetActive(false);
        }
    }

    private void OnTimerExpired()
    {
        if (_currentChoice == null)
        {
            return;
        }

        Debug.Log($"ChoiceUIController: Timer expired, selecting default option {_currentChoice.DefaultOptionIndex}");
        
        OnChoiceSelected(_currentChoice.DefaultOptionIndex);
    }

    private string GetLocalizedText(string textKey, string fallbackText)
    {
        // Try to get localized text
        if (_localizationService != null && !string.IsNullOrEmpty(textKey))
        {
            string localizedText = _localizationService.GetLocalizedString(textKey);
            if (!string.IsNullOrEmpty(localizedText))
            {
                return localizedText;
            }
        }

        // Fallback to fallback text
        return fallbackText ?? "[No text]";
    }

    /// <summary>
    /// Show the choice panel.
    /// </summary>
    public void Show()
    {
        if (_choicePanel != null)
        {
            _choicePanel.SetActive(true);
        }
    }

    /// <summary>
    /// Hide the choice panel.
    /// </summary>
    public void Hide()
    {
        HideChoices();
    }
}

}