using UnityEngine;
using UnityEngine.AddressableAssets;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Characters;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Data.Choices;
using NovelCore.Runtime.Core.SceneManagement;
using System.Collections.Generic;

namespace NovelCore.Tests.Runtime.Builders
{

/// <summary>
/// Builder for creating test SceneData instances.
/// </summary>
public class SceneDataBuilder
{
    private string _sceneId = "test_scene";
    private string _sceneName = "Test Scene";
    private AssetReference _backgroundImage;
    private AssetReference _backgroundMusic;
    private List<CharacterPlacement> _characters = new();
    private List<DialogueLineData> _dialogueLines = new();
    private List<ChoiceData> _choices = new();
    private TransitionType _transitionType = TransitionType.Fade;
    private float _transitionDuration = 0.5f;
    private AssetReference _nextScene;
    private bool _autoAdvance = false;
    private float _autoAdvanceDelay = 2.0f;

    public SceneDataBuilder WithSceneId(string sceneId)
    {
        _sceneId = sceneId;
        return this;
    }

    public SceneDataBuilder WithSceneName(string sceneName)
    {
        _sceneName = sceneName;
        return this;
    }

    public SceneDataBuilder WithBackgroundImage(AssetReference backgroundImage)
    {
        _backgroundImage = backgroundImage;
        return this;
    }

    public SceneDataBuilder WithBackgroundMusic(AssetReference backgroundMusic)
    {
        _backgroundMusic = backgroundMusic;
        return this;
    }

    public SceneDataBuilder WithCharacter(CharacterPlacement character)
    {
        _characters.Add(character);
        return this;
    }

    public SceneDataBuilder WithDialogueLine(DialogueLineData dialogueLine)
    {
        _dialogueLines.Add(dialogueLine);
        return this;
    }

    public SceneDataBuilder WithChoice(ChoiceData choice)
    {
        _choices.Add(choice);
        return this;
    }

    public SceneDataBuilder WithTransitionType(TransitionType transitionType)
    {
        _transitionType = transitionType;
        return this;
    }

    public SceneDataBuilder WithAutoAdvance(bool autoAdvance, float delay = 2.0f)
    {
        _autoAdvance = autoAdvance;
        _autoAdvanceDelay = delay;
        return this;
    }

    public SceneDataBuilder WithNextScene(AssetReference nextScene)
    {
        _nextScene = nextScene;
        return this;
    }

    public SceneData Build()
    {
        var sceneData = ScriptableObject.CreateInstance<SceneData>();
        
        typeof(SceneData).GetField("_sceneId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _sceneId);
        typeof(SceneData).GetField("_sceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _sceneName);
        typeof(SceneData).GetField("_backgroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _backgroundImage);
        typeof(SceneData).GetField("_backgroundMusic", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _backgroundMusic);
        typeof(SceneData).GetField("_characters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _characters);
        typeof(SceneData).GetField("_dialogueLines", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _dialogueLines);
        typeof(SceneData).GetField("_choices", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _choices);
        typeof(SceneData).GetField("_transitionType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _transitionType);
        typeof(SceneData).GetField("_transitionDuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _transitionDuration);
        typeof(SceneData).GetField("_nextScene", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _nextScene);
        typeof(SceneData).GetField("_autoAdvance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _autoAdvance);
        typeof(SceneData).GetField("_autoAdvanceDelay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(sceneData, _autoAdvanceDelay);

        return sceneData;
    }
}

/// <summary>
/// Builder for creating test CharacterData instances.
/// </summary>
public class CharacterDataBuilder
{
    private string _characterId = "test_char";
    private string _characterName = "Test Character";
    private string _description = "A test character";
    private string _defaultEmotion = "neutral";
    private List<CharacterEmotion> _emotions = new();
    private AnimationType _animationType = AnimationType.Unity;
    private AssetReference _spineDataAsset;
    private Vector2 _defaultScale = Vector2.one;

    public CharacterDataBuilder WithCharacterId(string characterId)
    {
        _characterId = characterId;
        return this;
    }

    public CharacterDataBuilder WithCharacterName(string characterName)
    {
        _characterName = characterName;
        return this;
    }

    public CharacterDataBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CharacterDataBuilder WithDefaultEmotion(string defaultEmotion)
    {
        _defaultEmotion = defaultEmotion;
        return this;
    }

    public CharacterDataBuilder WithEmotion(CharacterEmotion emotion)
    {
        _emotions.Add(emotion);
        return this;
    }

    public CharacterDataBuilder WithAnimationType(AnimationType animationType)
    {
        _animationType = animationType;
        return this;
    }

    public CharacterDataBuilder WithSpineDataAsset(AssetReference spineDataAsset)
    {
        _spineDataAsset = spineDataAsset;
        return this;
    }

    public CharacterData Build()
    {
        var characterData = ScriptableObject.CreateInstance<CharacterData>();
        
        typeof(CharacterData).GetField("_characterId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(characterData, _characterId);
        typeof(CharacterData).GetField("_characterName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(characterData, _characterName);
        typeof(CharacterData).GetField("_description", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(characterData, _description);
        typeof(CharacterData).GetField("_defaultEmotion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(characterData, _defaultEmotion);
        typeof(CharacterData).GetField("_emotions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(characterData, _emotions);
        typeof(CharacterData).GetField("_animationType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(characterData, _animationType);
        typeof(CharacterData).GetField("_spineDataAsset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(characterData, _spineDataAsset);
        typeof(CharacterData).GetField("_defaultScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(characterData, _defaultScale);

        return characterData;
    }
}

/// <summary>
/// Builder for creating test DialogueLineData instances.
/// </summary>
public class DialogueLineDataBuilder
{
    private string _lineId = "test_line";
    private AssetReference _speaker;
    private string _emotion = "neutral";
    private string _textKey = "test_text_key";
    private string _fallbackText = "Test dialogue text";
    private AssetReference _voiceClip;
    private AssetReference _soundEffect;
    private float _displayDuration = -1f;
    private CharacterAction _characterAction = CharacterAction.None;
    private string _actionParameters = "";

    public DialogueLineDataBuilder WithLineId(string lineId)
    {
        _lineId = lineId;
        return this;
    }

    public DialogueLineDataBuilder WithSpeaker(AssetReference speaker)
    {
        _speaker = speaker;
        return this;
    }

    public DialogueLineDataBuilder WithEmotion(string emotion)
    {
        _emotion = emotion;
        return this;
    }

    public DialogueLineDataBuilder WithTextKey(string textKey)
    {
        _textKey = textKey;
        return this;
    }

    public DialogueLineDataBuilder WithFallbackText(string fallbackText)
    {
        _fallbackText = fallbackText;
        return this;
    }

    public DialogueLineDataBuilder WithVoiceClip(AssetReference voiceClip)
    {
        _voiceClip = voiceClip;
        return this;
    }

    public DialogueLineDataBuilder WithSoundEffect(AssetReference soundEffect)
    {
        _soundEffect = soundEffect;
        return this;
    }

    public DialogueLineDataBuilder WithDisplayDuration(float displayDuration)
    {
        _displayDuration = displayDuration;
        return this;
    }

    public DialogueLineDataBuilder WithCharacterAction(CharacterAction characterAction, string parameters = "")
    {
        _characterAction = characterAction;
        _actionParameters = parameters;
        return this;
    }

    public DialogueLineData Build()
    {
        var dialogueLineData = ScriptableObject.CreateInstance<DialogueLineData>();
        
        typeof(DialogueLineData).GetField("_lineId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _lineId);
        typeof(DialogueLineData).GetField("_speaker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _speaker);
        typeof(DialogueLineData).GetField("_emotion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _emotion);
        typeof(DialogueLineData).GetField("_textKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _textKey);
        typeof(DialogueLineData).GetField("_fallbackText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _fallbackText);
        typeof(DialogueLineData).GetField("_voiceClip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _voiceClip);
        typeof(DialogueLineData).GetField("_soundEffect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _soundEffect);
        typeof(DialogueLineData).GetField("_displayDuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _displayDuration);
        typeof(DialogueLineData).GetField("_characterAction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _characterAction);
        typeof(DialogueLineData).GetField("_actionParameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(dialogueLineData, _actionParameters);

        return dialogueLineData;
    }
}

/// <summary>
/// Builder for creating test ChoiceData instances.
/// </summary>
public class ChoiceDataBuilder
{
    private string _choiceId = "test_choice";
    private string _promptTextKey = "";
    private string _fallbackPromptText = "Choose an option:";
    private List<ChoiceOption> _options = new();
    private float _timerSeconds = 0f;
    private int _defaultOptionIndex = 0;

    public ChoiceDataBuilder WithChoiceId(string choiceId)
    {
        _choiceId = choiceId;
        return this;
    }

    public ChoiceDataBuilder WithPromptTextKey(string promptTextKey)
    {
        _promptTextKey = promptTextKey;
        return this;
    }

    public ChoiceDataBuilder WithFallbackPromptText(string fallbackPromptText)
    {
        _fallbackPromptText = fallbackPromptText;
        return this;
    }

    public ChoiceDataBuilder WithOption(ChoiceOption option)
    {
        _options.Add(option);
        return this;
    }

    public ChoiceDataBuilder WithOption(string optionId, string fallbackText, AssetReference targetScene = null)
    {
        var option = new ChoiceOption
        {
            optionId = optionId,
            textKey = "",
            fallbackText = fallbackText,
            targetScene = targetScene,
            requiredChoices = new List<string>(),
            isAvailable = true,
            icon = null
        };
        _options.Add(option);
        return this;
    }

    public ChoiceDataBuilder WithTimer(float timerSeconds, int defaultOptionIndex)
    {
        _timerSeconds = timerSeconds;
        _defaultOptionIndex = defaultOptionIndex;
        return this;
    }

    public ChoiceData Build()
    {
        var choiceData = ScriptableObject.CreateInstance<ChoiceData>();
        
        typeof(ChoiceData).GetField("_choiceId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _choiceId);
        typeof(ChoiceData).GetField("_promptTextKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _promptTextKey);
        typeof(ChoiceData).GetField("_fallbackPromptText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _fallbackPromptText);
        typeof(ChoiceData).GetField("_options", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _options);
        typeof(ChoiceData).GetField("_timerSeconds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _timerSeconds);
        typeof(ChoiceData).GetField("_defaultOptionIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _defaultOptionIndex);

        return choiceData;
    }
}

}
