using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using NovelCore.Runtime.Data.Characters;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Data.Choices;
using NovelCore.Runtime.Core.SceneManagement;

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
    [Tooltip("Next scene to load after dialogue completes (for linear progression). Ignored if choices are present.")]
    private AssetReference _nextScene;

    [SerializeField]
    [Tooltip("Conditional transition rules evaluated before nextScene. First matching rule determines target scene.")]
    private List<SceneTransitionRule> _transitionRules = new();

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
    public AssetReference NextScene => _nextScene;
    public IReadOnlyList<SceneTransitionRule> TransitionRules => _transitionRules;
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

        // Validate nextScene reference if set
        if (_nextScene != null && _nextScene.RuntimeKeyIsValid())
        {
            // Note: Circular reference detection should be done in editor validation
            // as it requires loading all scenes in the graph
            Debug.Log($"SceneData {name}: nextScene reference is valid");
        }

        // Warn if both choices and nextScene are defined (choices take priority)
        if (_choices.Count > 0 && _nextScene != null && _nextScene.RuntimeKeyIsValid())
        {
            Debug.LogWarning($"SceneData {name}: Both choices and nextScene are defined. Choices will take priority, nextScene will be ignored.");
        }

        // Validate transition rules
        if (_transitionRules != null && _transitionRules.Count > 0)
        {
            for (int i = 0; i < _transitionRules.Count; i++)
            {
                var rule = _transitionRules[i];
                if (rule == null)
                {
                    Debug.LogError($"SceneData {name}: Transition rule at index {i} is null");
                    return false;
                }

                if (!rule.IsValid())
                {
                    Debug.LogError($"SceneData {name}: Transition rule at index {i} is invalid");
                    return false;
                }

                // Check for duplicate priorities
                for (int j = i + 1; j < _transitionRules.Count; j++)
                {
                    if (_transitionRules[j] != null && _transitionRules[j].Priority == rule.Priority)
                    {
                        Debug.LogWarning($"SceneData {name}: Duplicate priority {rule.Priority} found at indices {i} and {j}. Evaluation order may be ambiguous.");
                    }
                }
            }

            Debug.Log($"SceneData {name}: {_transitionRules.Count} transition rules validated");
        }

        // Warn if transition rules, choices, and nextScene are all defined
        if (_transitionRules != null && _transitionRules.Count > 0 && _choices.Count > 0)
        {
            Debug.LogWarning($"SceneData {name}: Both transition rules and choices are defined. Choices take priority over transition rules.");
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

}