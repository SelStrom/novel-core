namespace NovelCore.Runtime.Core.DialogueSystem;

/// <summary>
/// Implementation of dialogue system managing dialogue playback, choices, and flow.
/// </summary>
public class DialogueSystem : IDialogueSystem
{
    private readonly IAudioService _audioService;
    private readonly IInputService _inputService;

    private SceneData _currentScene;
    private int _currentLineIndex;
    private bool _isPlaying;
    private bool _isWaitingForChoice;
    private ChoiceData _currentChoice;
    private List<string> _choiceHistory = new();
    private float _autoAdvanceTimer;

    public SceneData CurrentScene => _currentScene;
    public int CurrentLineIndex => _currentLineIndex;
    public bool IsPlaying => _isPlaying;
    public bool IsWaitingForChoice => _isWaitingForChoice;

    public event System.Action<DialogueLineData> OnDialogueLineChanged;
    public event System.Action<ChoiceData> OnChoicePointReached;
    public event System.Action OnDialogueComplete;

    public DialogueSystem(IAudioService audioService, IInputService inputService)
    {
        _audioService = audioService ?? throw new System.ArgumentNullException(nameof(audioService));
        _inputService = inputService ?? throw new System.ArgumentNullException(nameof(inputService));
    }

    public void StartScene(SceneData sceneData)
    {
        if (sceneData == null)
        {
            Debug.LogError("DialogueSystem: Cannot start null scene");
            return;
        }

        if (!sceneData.Validate())
        {
            Debug.LogError($"DialogueSystem: Scene validation failed for {sceneData.SceneName}");
            return;
        }

        _currentScene = sceneData;
        _currentLineIndex = 0;
        _isPlaying = true;
        _isWaitingForChoice = false;
        _autoAdvanceTimer = 0f;

        Debug.Log($"DialogueSystem: Starting scene {sceneData.SceneName}");

        if (_currentScene.DialogueLines.Count > 0)
        {
            DisplayCurrentLine();
        }
        else if (_currentScene.Choices.Count > 0)
        {
            ShowChoices(_currentScene.Choices[0]);
        }
        else
        {
            Debug.LogWarning("DialogueSystem: Scene has no dialogue or choices");
            OnDialogueComplete?.Invoke();
        }
    }

    public void AdvanceDialogue()
    {
        if (!_isPlaying || _isWaitingForChoice)
        {
            return;
        }

        _currentLineIndex++;

        if (_currentLineIndex >= _currentScene.DialogueLines.Count)
        {
            // Check for choices
            if (_currentScene.Choices.Count > 0)
            {
                ShowChoices(_currentScene.Choices[0]);
            }
            else
            {
                CompleteDialogue();
            }
        }
        else
        {
            DisplayCurrentLine();
        }
    }

    public void SelectChoice(int choiceIndex)
    {
        if (!_isWaitingForChoice || _currentChoice == null)
        {
            Debug.LogWarning("DialogueSystem: Not at a choice point");
            return;
        }

        if (choiceIndex < 0 || choiceIndex >= _currentChoice.Options.Count)
        {
            Debug.LogError($"DialogueSystem: Invalid choice index {choiceIndex}");
            return;
        }

        var selectedOption = _currentChoice.Options[choiceIndex];
        
        // Track choice history
        string choiceKey = $"{_currentChoice.ChoiceId}_{selectedOption.optionId}";
        _choiceHistory.Add(choiceKey);
        
        Debug.Log($"DialogueSystem: Choice selected - {choiceKey}");

        _isWaitingForChoice = false;

        // TODO: Load target scene when SceneManager is implemented
        // For now, just complete dialogue
        CompleteDialogue();
    }

    public void SkipCurrentLine()
    {
        if (!_isPlaying || _isWaitingForChoice)
        {
            return;
        }

        AdvanceDialogue();
    }

    public void StopDialogue()
    {
        _isPlaying = false;
        _isWaitingForChoice = false;
        _currentScene = null;
        _currentLineIndex = 0;
        
        Debug.Log("DialogueSystem: Dialogue stopped");
    }

    public DialogueLineData GetCurrentLine()
    {
        if (_currentScene == null || _currentLineIndex >= _currentScene.DialogueLines.Count)
        {
            return null;
        }

        return _currentScene.DialogueLines[_currentLineIndex];
    }

    public bool HasNextLine()
    {
        if (_currentScene == null)
        {
            return false;
        }

        return _currentLineIndex < _currentScene.DialogueLines.Count - 1;
    }

    private void DisplayCurrentLine()
    {
        var currentLine = GetCurrentLine();
        if (currentLine == null)
        {
            Debug.LogError("DialogueSystem: Current line is null");
            return;
        }

        if (!currentLine.Validate())
        {
            Debug.LogError($"DialogueSystem: Line validation failed for {currentLine.LineId}");
            return;
        }

        Debug.Log($"DialogueSystem: Displaying line {currentLine.LineId}");

        // Play voice clip if available
        if (currentLine.VoiceClip != null && currentLine.VoiceClip.RuntimeKeyIsValid())
        {
            // TODO: Load and play voice clip via AssetManager
            Debug.Log($"DialogueSystem: Playing voice clip for line {currentLine.LineId}");
        }

        // Play sound effect if available
        if (currentLine.SoundEffect != null && currentLine.SoundEffect.RuntimeKeyIsValid())
        {
            // TODO: Load and play sound effect via AssetManager
            Debug.Log($"DialogueSystem: Playing sound effect for line {currentLine.LineId}");
        }

        // Notify listeners
        OnDialogueLineChanged?.Invoke(currentLine);

        // Reset auto-advance timer
        if (_currentScene.AutoAdvance && currentLine.DisplayDuration > 0)
        {
            _autoAdvanceTimer = currentLine.DisplayDuration;
        }
        else if (_currentScene.AutoAdvance)
        {
            _autoAdvanceTimer = _currentScene.AutoAdvanceDelay;
        }
    }

    private void ShowChoices(ChoiceData choiceData)
    {
        if (choiceData == null || !choiceData.Validate())
        {
            Debug.LogError("DialogueSystem: Invalid choice data");
            CompleteDialogue();
            return;
        }

        _currentChoice = choiceData;
        _isWaitingForChoice = true;

        Debug.Log($"DialogueSystem: Showing choices for {choiceData.ChoiceId}");
        OnChoicePointReached?.Invoke(choiceData);
    }

    private void CompleteDialogue()
    {
        Debug.Log("DialogueSystem: Dialogue complete");
        _isPlaying = false;
        OnDialogueComplete?.Invoke();
    }

    /// <summary>
    /// Update method to be called each frame for auto-advance functionality.
    /// Should be called by a MonoBehaviour Update loop.
    /// </summary>
    public void Update(float deltaTime)
    {
        if (!_isPlaying || _isWaitingForChoice)
        {
            return;
        }

        // Handle auto-advance
        if (_currentScene != null && _currentScene.AutoAdvance && _autoAdvanceTimer > 0)
        {
            _autoAdvanceTimer -= deltaTime;
            if (_autoAdvanceTimer <= 0)
            {
                AdvanceDialogue();
            }
        }
    }

    /// <summary>
    /// Get the choice history for conditional branching.
    /// </summary>
    public IReadOnlyList<string> GetChoiceHistory() => _choiceHistory.AsReadOnly();

    /// <summary>
    /// Check if a specific choice was made previously.
    /// </summary>
    public bool HasMadeChoice(string choiceKey)
    {
        return _choiceHistory.Contains(choiceKey);
    }

    /// <summary>
    /// Clear choice history (for new game).
    /// </summary>
    public void ClearChoiceHistory()
    {
        _choiceHistory.Clear();
    }
}
