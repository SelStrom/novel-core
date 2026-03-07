namespace NovelCore.Runtime.Data.Choices
{

/// <summary>
/// ScriptableObject representing a player choice point.
/// </summary>
[CreateAssetMenu(fileName = "NewChoice", menuName = "NovelCore/Choice Data", order = 4)]
public class ChoiceData : BaseScriptableObject
{
    [Header("Choice Information")]
    [SerializeField]
    [Tooltip("Unique identifier for this choice")]
    private string _choiceId;

    [SerializeField]
    [Tooltip("Localization key for the choice prompt (optional)")]
    private string _promptTextKey;

    [SerializeField]
    [Tooltip("Fallback prompt text")]
    [TextArea(1, 2)]
    private string _fallbackPromptText;

    [Header("Options")]
    [SerializeField]
    [Tooltip("Choice options (2-6 options)")]
    private List<ChoiceOption> _options = new();

    [Header("Timer")]
    [SerializeField]
    [Tooltip("Time limit for choice in seconds (0 = no timer)")]
    private float _timerSeconds = 0f;

    [SerializeField]
    [Tooltip("Default option index if timer expires")]
    private int _defaultOptionIndex = 0;

    // Properties
    public string ChoiceId => _choiceId;
    public string PromptTextKey => _promptTextKey;
    public string FallbackPromptText => _fallbackPromptText;
    public IReadOnlyList<ChoiceOption> Options => _options;
    public float TimerSeconds => _timerSeconds;
    public int DefaultOptionIndex => _defaultOptionIndex;

    public override bool Validate()
    {
        if (string.IsNullOrEmpty(_choiceId))
        {
            Debug.LogError($"ChoiceData {name}: choiceId is required");
            return false;
        }

        if (_options.Count < 2 || _options.Count > 6)
        {
            Debug.LogError($"ChoiceData {name}: Must have between 2 and 6 options");
            return false;
        }

        if (_timerSeconds > 0 && (_defaultOptionIndex < 0 || _defaultOptionIndex >= _options.Count))
        {
            Debug.LogError($"ChoiceData {name}: Invalid defaultOptionIndex for timed choice");
            return false;
        }

        // Check for duplicate option IDs
        var optionIds = _options.Select(o => o.optionId).ToList();
        if (optionIds.Count != optionIds.Distinct().Count())
        {
            Debug.LogError($"ChoiceData {name}: Duplicate option IDs found");
            return false;
        }

        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (string.IsNullOrEmpty(_choiceId))
        {
            _choiceId = $"choice_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}

/// <summary>
/// Represents a single option within a choice.
/// </summary>
[System.Serializable]
public struct ChoiceOption
{
    [Tooltip("Unique identifier for this option")]
    public string optionId;

    [Tooltip("Localization key for option text")]
    public string textKey;

    [Tooltip("Fallback text if localization not found")]
    public string fallbackText;

    [Tooltip("Target scene to load when selected")]
    public AssetReference targetScene;

    [Tooltip("Previous choices required for this option to be available")]
    public List<string> requiredChoices;

    [Tooltip("Runtime availability flag")]
    public bool isAvailable;

    [Tooltip("Optional icon for this option")]
    public Sprite icon;
}

}