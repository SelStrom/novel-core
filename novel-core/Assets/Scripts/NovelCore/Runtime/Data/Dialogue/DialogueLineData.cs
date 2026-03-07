namespace NovelCore.Runtime.Data.Dialogue
{

/// <summary>
/// ScriptableObject representing a single line of dialogue.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogueLine", menuName = "NovelCore/Dialogue Line", order = 3)]
public class DialogueLineData : BaseScriptableObject
{
    [Header("Dialogue Information")]
    [SerializeField]
    [Tooltip("Unique identifier for this dialogue line")]
    private string _lineId;

    [Header("Speaker")]
    [SerializeField]
    [Tooltip("Character speaking this line (null for narrator)")]
    private AssetReference _speaker;

    [SerializeField]
    [Tooltip("Emotion of the speaker during this line")]
    private string _emotion = "neutral";

    [Header("Text Content")]
    [SerializeField]
    [Tooltip("Localization table key for the dialogue text")]
    private string _textKey;

    [SerializeField]
    [Tooltip("Fallback text if localization key not found")]
    [TextArea(2, 4)]
    private string _fallbackText;

    [Header("Audio")]
    [SerializeField]
    [Tooltip("Voice clip for this line (optional)")]
    private AssetReference _voiceClip;

    [SerializeField]
    [Tooltip("Sound effect to play with this line (optional)")]
    private AssetReference _soundEffect;

    [Header("Timing")]
    [SerializeField]
    [Tooltip("Display duration (-1 = wait for input)")]
    private float _displayDuration = -1f;

    [Header("Character Action")]
    [SerializeField]
    [Tooltip("Character action during this line")]
    private CharacterAction _characterAction = CharacterAction.None;

    [SerializeField]
    [Tooltip("JSON parameters for character action")]
    [TextArea(1, 3)]
    private string _actionParameters;

    // Properties
    public string LineId => _lineId;
    public AssetReference Speaker => _speaker;
    public string Emotion => _emotion;
    public string TextKey => _textKey;
    public string FallbackText => _fallbackText;
    public AssetReference VoiceClip => _voiceClip;
    public AssetReference SoundEffect => _soundEffect;
    public float DisplayDuration => _displayDuration;
    public CharacterAction CharacterAction => _characterAction;
    public string ActionParameters => _actionParameters;

    public override bool Validate()
    {
        if (string.IsNullOrEmpty(_lineId))
        {
            Debug.LogError($"DialogueLineData {name}: lineId is required");
            return false;
        }

        if (string.IsNullOrEmpty(_textKey))
        {
            Debug.LogWarning($"DialogueLineData {name}: textKey is empty, using fallback text");
        }

        if (_characterAction != CharacterAction.None && string.IsNullOrEmpty(_actionParameters))
        {
            Debug.LogWarning($"DialogueLineData {name}: Character action specified but no parameters provided");
        }

        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (string.IsNullOrEmpty(_lineId))
        {
            _lineId = $"line_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}

/// <summary>
/// Character actions that can occur during dialogue.
/// </summary>
public enum CharacterAction
{
    None,   // No action
    Enter,  // Character enters the scene
    Exit,   // Character exits the scene
    Move    // Character moves to new position
}

}