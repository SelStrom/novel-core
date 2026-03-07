namespace NovelCore.Runtime.Data.Scenes
{

/// <summary>
/// ScriptableObject representing a single scene in a visual novel.
/// Contains background, characters, dialogue, and choices.
/// </summary>
[CreateAssetMenu(fileName = "NewScene", menuName = "NovelCore/Scene Data", order = 1)]
public class SceneData : BaseScriptableObject
{
    [Header("Scene Information")]
    [SerializeField]
    [Tooltip("Unique identifier for this scene")]
    private string _sceneId;

    [SerializeField]
    [Tooltip("Display name for this scene (for editor use)")]
    private string _sceneName;

    [Header("Visual Content")]
    [SerializeField]
    [Tooltip("Background image for this scene")]
    private AssetReference _backgroundImage;

    [SerializeField]
    [Tooltip("Background music for this scene")]
    private AssetReference _backgroundMusic;

    [Header("Characters")]
    [SerializeField]
    [Tooltip("Characters present in this scene")]
    private List<CharacterPlacement> _characters = new();

    [Header("Narrative Content")]
    [SerializeField]
    [Tooltip("Dialogue lines in this scene")]
    private List<DialogueLineData> _dialogueLines = new();

    [SerializeField]
    [Tooltip("Player choices in this scene")]
    private List<ChoiceData> _choices = new();

    [Header("Scene Transition")]
    [SerializeField]
    [Tooltip("Type of transition when entering this scene")]
    private TransitionType _transitionType = TransitionType.Fade;

    [SerializeField]
    [Tooltip("Duration of scene transition in seconds")]
    private float _transitionDuration = 0.5f;

    [SerializeField]
    [Tooltip("Enable auto-advance for dialogue in this scene")]
    private bool _autoAdvance = false;

    [SerializeField]
    [Tooltip("Delay between auto-advancing dialogue lines")]
    private float _autoAdvanceDelay = 2.0f;

    // Properties
    public string SceneId => _sceneId;
    public string SceneName => _sceneName;
    public AssetReference BackgroundImage => _backgroundImage;
    public AssetReference BackgroundMusic => _backgroundMusic;
    public IReadOnlyList<CharacterPlacement> Characters => _characters;
    public IReadOnlyList<DialogueLineData> DialogueLines => _dialogueLines;
    public IReadOnlyList<ChoiceData> Choices => _choices;
    public TransitionType TransitionType => _transitionType;
    public float TransitionDuration => _transitionDuration;
    public bool AutoAdvance => _autoAdvance;
    public float AutoAdvanceDelay => _autoAdvanceDelay;

    public override bool Validate()
    {
        if (string.IsNullOrEmpty(_sceneId))
        {
            Debug.LogError($"SceneData {name}: sceneId is required");
            return false;
        }

        if (_dialogueLines.Count == 0 && _choices.Count == 0)
        {
            Debug.LogWarning($"SceneData {name}: No dialogue or choices defined");
        }

        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        // Generate scene ID if not set
        if (string.IsNullOrEmpty(_sceneId))
        {
            _sceneId = $"scene_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}

/// <summary>
/// Defines placement of a character within a scene.
/// </summary>
[System.Serializable]
public struct CharacterPlacement
{
    [Tooltip("Reference to the character")]
    public AssetReference character;

    [Tooltip("Normalized position on screen (0-1)")]
    public Vector2 position;

    [Tooltip("Initial emotion for this character")]
    public string initialEmotion;

    [Tooltip("Sorting order (higher values render on top)")]
    public int sortingOrder;
}

/// <summary>
/// Types of scene transitions.
/// </summary>
public enum TransitionType
{
    Cut,      // Instant transition
    Fade,     // Fade to black then fade in
    Slide,    // Slide transition
    Custom    // Custom transition shader
}

}