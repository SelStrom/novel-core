namespace NovelCore.Runtime.Core.DialogueSystem;

/// <summary>
/// Interface for the dialogue system managing dialogue playback and state.
/// </summary>
public interface IDialogueSystem
{
    /// <summary>
    /// Currently active scene data.
    /// </summary>
    SceneData CurrentScene { get; }

    /// <summary>
    /// Current dialogue line index.
    /// </summary>
    int CurrentLineIndex { get; }

    /// <summary>
    /// Whether dialogue playback is active.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Whether currently at a choice point.
    /// </summary>
    bool IsWaitingForChoice { get; }

    /// <summary>
    /// Event triggered when a new dialogue line is displayed.
    /// </summary>
    event System.Action<DialogueLineData> OnDialogueLineChanged;

    /// <summary>
    /// Event triggered when a choice point is reached.
    /// </summary>
    event System.Action<ChoiceData> OnChoicePointReached;

    /// <summary>
    /// Event triggered when dialogue playback completes.
    /// </summary>
    event System.Action OnDialogueComplete;

    /// <summary>
    /// Event triggered when a choice leads to a new scene.
    /// </summary>
    event System.Action<SceneData> OnSceneNavigationRequested;

    /// <summary>
    /// Start playing dialogue from a scene.
    /// </summary>
    /// <param name="sceneData">Scene to play.</param>
    void StartScene(SceneData sceneData);

    /// <summary>
    /// Advance to the next dialogue line.
    /// </summary>
    void AdvanceDialogue();

    /// <summary>
    /// Select a choice option and continue.
    /// </summary>
    /// <param name="choiceIndex">Index of the selected choice.</param>
    void SelectChoice(int choiceIndex);

    /// <summary>
    /// Skip current dialogue line (if skippable).
    /// </summary>
    void SkipCurrentLine();

    /// <summary>
    /// Stop dialogue playback.
    /// </summary>
    void StopDialogue();

    /// <summary>
    /// Get the current dialogue line data.
    /// </summary>
    DialogueLineData GetCurrentLine();

    /// <summary>
    /// Check if there are more dialogue lines.
    /// </summary>
    bool HasNextLine();
}
