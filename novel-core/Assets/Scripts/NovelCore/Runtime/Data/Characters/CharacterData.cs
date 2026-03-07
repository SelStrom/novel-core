namespace NovelCore.Runtime.Data.Characters;

/// <summary>
/// ScriptableObject representing a character with multiple emotional states.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacter", menuName = "NovelCore/Character Data", order = 2)]
public class CharacterData : BaseScriptableObject
{
    [Header("Character Information")]
    [SerializeField]
    [Tooltip("Unique identifier for this character")]
    private string _characterId;

    [SerializeField]
    [Tooltip("Display name of the character")]
    private string _characterName;

    [SerializeField]
    [Tooltip("Character description (for editor use)")]
    [TextArea(3, 5)]
    private string _description;

    [Header("Visual Representation")]
    [SerializeField]
    [Tooltip("Default emotion key")]
    private string _defaultEmotion = "neutral";

    [SerializeField]
    [Tooltip("Dictionary of emotions to sprite references")]
    private List<CharacterEmotion> _emotions = new();

    [Header("Animation")]
    [SerializeField]
    [Tooltip("Type of animation system to use")]
    private AnimationType _animationType = AnimationType.Unity;

    [SerializeField]
    [Tooltip("Spine skeleton data asset (if using Spine)")]
    private AssetReference _spineDataAsset;

    [SerializeField]
    [Tooltip("Default scale for this character")]
    private Vector2 _defaultScale = Vector2.one;

    // Properties
    public string CharacterId => _characterId;
    public string CharacterName => _characterName;
    public string Description => _description;
    public string DefaultEmotion => _defaultEmotion;
    public IReadOnlyList<CharacterEmotion> Emotions => _emotions;
    public AnimationType AnimationType => _animationType;
    public AssetReference SpineDataAsset => _spineDataAsset;
    public Vector2 DefaultScale => _defaultScale;

    /// <summary>
    /// Gets the emotion data for a specific emotion key.
    /// </summary>
    public CharacterEmotion? GetEmotion(string emotionKey)
    {
        return _emotions.FirstOrDefault(e => e.emotionName == emotionKey);
    }

    public override bool Validate()
    {
        if (string.IsNullOrEmpty(_characterId))
        {
            Debug.LogError($"CharacterData {name}: characterId is required");
            return false;
        }

        if (string.IsNullOrEmpty(_characterName))
        {
            Debug.LogError($"CharacterData {name}: characterName is required");
            return false;
        }

        if (_emotions.Count == 0)
        {
            Debug.LogError($"CharacterData {name}: At least one emotion is required");
            return false;
        }

        if (!_emotions.Any(e => e.emotionName == _defaultEmotion))
        {
            Debug.LogError($"CharacterData {name}: Default emotion '{_defaultEmotion}' not found in emotions list");
            return false;
        }

        if (_animationType == AnimationType.Spine && _spineDataAsset == null)
        {
            Debug.LogError($"CharacterData {name}: Spine animation type requires spineDataAsset");
            return false;
        }

        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (string.IsNullOrEmpty(_characterId))
        {
            _characterId = $"char_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}

/// <summary>
/// Defines a character's emotional state with associated sprite/animation.
/// </summary>
[System.Serializable]
public struct CharacterEmotion
{
    [Tooltip("Emotion key (e.g., 'happy', 'sad', 'angry')")]
    public string emotionName;

    [Tooltip("Sprite for this emotion (Unity Animator)")]
    public AssetReference sprite;

    [Tooltip("Spine skin name (if using Spine)")]
    public string spineSkin;

    [Tooltip("Spine animation name (if using Spine)")]
    public string spineAnimation;
}

/// <summary>
/// Animation system types for characters.
/// </summary>
public enum AnimationType
{
    Unity,   // Unity Animator
    Spine,   // Spine 2D skeletal animation
    Static   // No animation (sprite swap only)
}
