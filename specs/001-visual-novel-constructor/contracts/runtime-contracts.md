# Runtime System Contracts

**Feature**: Visual Novel Constructor  
**Date**: 2026-03-06  
**Purpose**: Define interfaces exposed by runtime systems for loose coupling

## Overview

This document defines the contracts (interfaces) between major runtime systems. These enable dependency injection via VContainer and support independent testing per constitution Principle VI (Modular Architecture).

## Core System Interfaces

### IDialogueSystem

Manages dialogue playback, text display, and line progression.

```csharp
namespace NovelCore.Runtime.Core.DialogueSystem
{
    public interface IDialogueSystem
    {
        /// <summary>
        /// Start playing dialogue from a scene's dialogue list.
        /// </summary>
        /// <param name="dialogueLines">Ordered list of dialogue to display</param>
        /// <param name="onComplete">Callback when all dialogue finished</param>
        void PlayDialogue(IEnumerable<DialogueLineData> dialogueLines, Action onComplete);
        
        /// <summary>
        /// Advance to next dialogue line (player clicked or auto-advance triggered).
        /// </summary>
        void AdvanceLine();
        
        /// <summary>
        /// Skip remaining dialogue in current sequence.
        /// </summary>
        void SkipAll();
        
        /// <summary>
        /// Pause/resume dialogue playback (for menus, choices).
        /// </summary>
        void SetPaused(bool paused);
        
        /// <summary>
        /// Get current dialogue line index.
        /// </summary>
        int CurrentLineIndex { get; }
        
        /// <summary>
        /// Check if dialogue is currently active.
        /// </summary>
        bool IsPlaying { get; }
        
        /// <summary>
        /// Event fired when a new line starts displaying.
        /// </summary>
        event Action<DialogueLineData> OnLineStarted;
        
        /// <summary>
        /// Event fired when current line finishes displaying.
        /// </summary>
        event Action<DialogueLineData> OnLineCompleted;
    }
}
```

**Dependencies**: `ILocalizationService`, `IAudioService`, `ICharacterAnimator`

**Usage Example**:
```csharp
public class SceneController {
    private readonly IDialogueSystem _dialogueSystem;
    
    public SceneController(IDialogueSystem dialogueSystem) {
        _dialogueSystem = dialogueSystem;
    }
    
    public void StartScene(SceneData scene) {
        _dialogueSystem.PlayDialogue(scene.dialogueLines, OnDialogueComplete);
    }
}
```

---

### ISceneManager

Handles scene loading, transitions, and asset management.

```csharp
namespace NovelCore.Runtime.Core.SceneManagement
{
    public interface ISceneManager
    {
        /// <summary>
        /// Load and activate a scene with transition effect.
        /// </summary>
        /// <param name="sceneData">Scene to load</param>
        /// <param name="transition">Transition type (fade, slide, etc.)</param>
        /// <param name="onComplete">Callback when scene ready</param>
        void LoadScene(SceneData sceneData, TransitionType transition, Action onComplete);
        
        /// <summary>
        /// Preload next scene assets in background (hide latency).
        /// </summary>
        /// <param name="sceneData">Scene to preload</param>
        void PreloadScene(SceneData sceneData);
        
        /// <summary>
        /// Unload assets from previous scene to free memory.
        /// </summary>
        void UnloadPreviousScene();
        
        /// <summary>
        /// Get currently active scene data.
        /// </summary>
        SceneData CurrentScene { get; }
        
        /// <summary>
        /// Check if a scene is currently loading.
        /// </summary>
        bool IsLoading { get; }
        
        /// <summary>
        /// Event fired when scene load starts.
        /// </summary>
        event Action<SceneData> OnSceneLoadStarted;
        
        /// <summary>
        /// Event fired when scene load completes.
        /// </summary>
        event Action<SceneData> OnSceneLoadCompleted;
    }
}
```

**Dependencies**: `IAssetManager`, `IAudioService`

---

### ISaveSystem

Manages save/load operations, auto-save, and cloud sync.

```csharp
namespace NovelCore.Runtime.Core.SaveSystem
{
    public interface ISaveSystem
    {
        /// <summary>
        /// Save current game state to specified slot.
        /// </summary>
        /// <param name="slotIndex">1-10 for manual saves, 0 for auto-save</param>
        /// <param name="onComplete">Callback with success/failure</param>
        void SaveGame(int slotIndex, Action<bool> onComplete);
        
        /// <summary>
        /// Load game state from specified slot.
        /// </summary>
        /// <param name="slotIndex">Save slot to load</param>
        /// <param name="onComplete">Callback with loaded data or null if failed</param>
        void LoadGame(int slotIndex, Action<SaveData> onComplete);
        
        /// <summary>
        /// Delete save data from specified slot.
        /// </summary>
        void DeleteSave(int slotIndex);
        
        /// <summary>
        /// Get list of all available saves with metadata.
        /// </summary>
        /// <returns>List of save metadata (timestamp, thumbnail, etc.)</returns>
        List<SaveMetadata> GetAllSaves();
        
        /// <summary>
        /// Check if auto-save is enabled.
        /// </summary>
        bool AutoSaveEnabled { get; set; }
        
        /// <summary>
        /// Trigger cloud sync for all saves (Steam/iCloud/Google Play).
        /// </summary>
        /// <param name="onComplete">Callback with success/failure</param>
        void SyncWithCloud(Action<bool> onComplete);
        
        /// <summary>
        /// Event fired when save operation completes.
        /// </summary>
        event Action<int, bool> OnSaveCompleted;
        
        /// <summary>
        /// Event fired when load operation completes.
        /// </summary>
        event Action<SaveData> OnLoadCompleted;
    }
}
```

**Dependencies**: `IPlatformService` (for cloud sync), `ISceneManager` (for scene restoration)

---

### IAssetManager

Wraps Unity Addressables for asset loading/unloading.

```csharp
namespace NovelCore.Runtime.Core.AssetManagement
{
    public interface IAssetManager
    {
        /// <summary>
        /// Load asset asynchronously by Addressables reference.
        /// </summary>
        /// <typeparam name="T">Asset type (Sprite, AudioClip, etc.)</typeparam>
        /// <param name="assetReference">Addressables asset reference</param>
        /// <param name="onComplete">Callback with loaded asset</param>
        void LoadAssetAsync<T>(AssetReference<T> assetReference, Action<T> onComplete) where T : UnityEngine.Object;
        
        /// <summary>
        /// Unload asset and free memory.
        /// </summary>
        /// <param name="assetReference">Asset to unload</param>
        void UnloadAsset(AssetReference assetReference);
        
        /// <summary>
        /// Preload multiple assets (backgrounds, characters for next scene).
        /// </summary>
        /// <param name="assetReferences">List of assets to preload</param>
        /// <param name="onComplete">Callback when all loaded</param>
        void PreloadAssets(IEnumerable<AssetReference> assetReferences, Action onComplete);
        
        /// <summary>
        /// Validate asset reference points to existing asset.
        /// </summary>
        /// <returns>True if asset exists and loadable</returns>
        bool ValidateAssetReference(AssetReference assetReference);
        
        /// <summary>
        /// Get loading progress for current operation (0.0 to 1.0).
        /// </summary>
        float LoadingProgress { get; }
    }
}
```

**Dependencies**: Unity Addressables (com.unity.addressables)

---

### IAudioService

Audio playback wrapper (music, SFX, voice).

```csharp
namespace NovelCore.Runtime.Core.AudioSystem
{
    public interface IAudioService
    {
        /// <summary>
        /// Play background music with looping.
        /// </summary>
        /// <param name="musicClip">Audio clip to play</param>
        /// <param name="fadeInDuration">Fade in time in seconds (0 = instant)</param>
        /// <param name="volume">Volume (0.0 to 1.0)</param>
        void PlayMusic(AudioClip musicClip, float fadeInDuration = 1.0f, float volume = 1.0f);
        
        /// <summary>
        /// Stop currently playing music.
        /// </summary>
        /// <param name="fadeOutDuration">Fade out time in seconds</param>
        void StopMusic(float fadeOutDuration = 1.0f);
        
        /// <summary>
        /// Play sound effect (one-shot, no looping).
        /// </summary>
        /// <param name="sfxClip">Sound effect clip</param>
        /// <param name="volume">Volume (0.0 to 1.0)</param>
        void PlaySFX(AudioClip sfxClip, float volume = 1.0f);
        
        /// <summary>
        /// Play voice line (character dialogue).
        /// </summary>
        /// <param name="voiceClip">Voice audio clip</param>
        /// <param name="onComplete">Callback when voice line finishes</param>
        void PlayVoice(AudioClip voiceClip, Action onComplete = null);
        
        /// <summary>
        /// Stop currently playing voice line.
        /// </summary>
        void StopVoice();
        
        /// <summary>
        /// Set master volume for all audio.
        /// </summary>
        float MasterVolume { get; set; }
        
        /// <summary>
        /// Set music volume (affects only music channel).
        /// </summary>
        float MusicVolume { get; set; }
        
        /// <summary>
        /// Set SFX volume (affects only SFX channel).
        /// </summary>
        float SFXVolume { get; set; }
        
        /// <summary>
        /// Set voice volume (affects only voice channel).
        /// </summary>
        float VoiceVolume { get; set; }
        
        /// <summary>
        /// Mute/unmute all audio.
        /// </summary>
        bool IsMuted { get; set; }
    }
}
```

**Dependencies**: UnityEngine.AudioSource

---

### IInputService

Input abstraction (mouse, touch, keyboard, gamepad).

```csharp
namespace NovelCore.Runtime.Core.InputHandling
{
    public interface IInputService
    {
        /// <summary>
        /// Check if primary action button pressed this frame (left click, tap, A button).
        /// </summary>
        bool GetPrimaryActionDown();
        
        /// <summary>
        /// Check if cancel button pressed (right click, back, B button).
        /// </summary>
        bool GetCancelDown();
        
        /// <summary>
        /// Check if skip button held (space, hold tap, X button).
        /// </summary>
        bool GetSkipHeld();
        
        /// <summary>
        /// Get pointer position in screen coordinates.
        /// </summary>
        Vector2 GetPointerPosition();
        
        /// <summary>
        /// Check if pointer is over UI element.
        /// </summary>
        bool IsPointerOverUI();
        
        /// <summary>
        /// Event fired on primary action.
        /// </summary>
        event Action OnPrimaryAction;
        
        /// <summary>
        /// Event fired on cancel action.
        /// </summary>
        event Action OnCancelAction;
    }
}
```

**Dependencies**: Unity Input System (com.unity.inputsystem)

---

### IPlatformService

Platform-specific functionality abstraction (Steam, iOS, Android).

```csharp
namespace NovelCore.Runtime.Platform.Interfaces
{
    public interface IPlatformService
    {
        /// <summary>
        /// Initialize platform SDK (Steamworks, Game Center, Google Play).
        /// </summary>
        /// <param name="onComplete">Callback with success/failure</param>
        void Initialize(Action<bool> onComplete);
        
        /// <summary>
        /// Upload save data to cloud storage.
        /// </summary>
        /// <param name="fileName">Save file name</param>
        /// <param name="data">Serialized save data</param>
        /// <param name="onComplete">Callback with success/failure</param>
        void UploadSaveToCloud(string fileName, byte[] data, Action<bool> onComplete);
        
        /// <summary>
        /// Download save data from cloud storage.
        /// </summary>
        /// <param name="fileName">Save file name</param>
        /// <param name="onComplete">Callback with data or null if failed</param>
        void DownloadSaveFromCloud(string fileName, Action<byte[]> onComplete);
        
        /// <summary>
        /// Unlock achievement (Steam, Game Center, Google Play).
        /// </summary>
        /// <param name="achievementId">Platform-specific achievement ID</param>
        void UnlockAchievement(string achievementId);
        
        /// <summary>
        /// Show platform-specific UI (achievements, leaderboards).
        /// </summary>
        void ShowAchievementsUI();
        
        /// <summary>
        /// Check if running on Steam platform.
        /// </summary>
        bool IsSteam { get; }
        
        /// <summary>
        /// Check if running on mobile platform (iOS or Android).
        /// </summary>
        bool IsMobile { get; }
        
        /// <summary>
        /// Get platform-specific user ID.
        /// </summary>
        string GetUserId();
    }
}
```

**Implementations**:
- `SteamPlatformService`: Uses Steamworks.NET
- `iOSPlatformService`: Uses iCloud, Game Center
- `AndroidPlatformService`: Uses Google Play Games Services
- `DefaultPlatformService`: No-op for standalone builds without platform SDK

---

## Animation System Contracts

### ICharacterAnimator

Abstraction over Unity Animator and Spine animations.

```csharp
namespace NovelCore.Runtime.Animation
{
    public interface ICharacterAnimator
    {
        /// <summary>
        /// Set character emotion (changes sprite or Spine skin/animation).
        /// </summary>
        /// <param name="emotionName">Emotion key (e.g., "happy", "sad")</param>
        void SetEmotion(string emotionName);
        
        /// <summary>
        /// Play entrance animation (character appears on screen).
        /// </summary>
        /// <param name="side">Enter from left or right</param>
        /// <param name="duration">Animation duration in seconds</param>
        /// <param name="onComplete">Callback when animation finishes</param>
        void PlayEnterAnimation(EnterSide side, float duration, Action onComplete);
        
        /// <summary>
        /// Play exit animation (character leaves screen).
        /// </summary>
        /// <param name="side">Exit to left or right</param>
        /// <param name="duration">Animation duration in seconds</param>
        /// <param name="onComplete">Callback when animation finishes</param>
        void PlayExitAnimation(ExitSide side, float duration, Action onComplete);
        
        /// <summary>
        /// Move character to new position.
        /// </summary>
        /// <param name="targetPosition">Normalized screen coordinates</param>
        /// <param name="duration">Movement duration in seconds</param>
        void MoveTo(Vector2 targetPosition, float duration);
        
        /// <summary>
        /// Set character scale (size multiplier).
        /// </summary>
        Vector2 Scale { get; set; }
        
        /// <summary>
        /// Set character sorting order (Z-layer).
        /// </summary>
        int SortingOrder { get; set; }
    }
    
    public enum EnterSide { Left, Right, Top, Bottom, Center }
    public enum ExitSide { Left, Right, Top, Bottom }
}
```

**Implementations**:
- `UnityCharacterAnimator`: Uses Unity Animator + Sprite Renderer
- `SpineCharacterAnimator`: Uses Spine-Unity SkeletonAnimation

---

## Localization Contract

### ILocalizationService

Wrapper around Unity Localization package.

```csharp
namespace NovelCore.Runtime.Core.Localization
{
    public interface ILocalizationService
    {
        /// <summary>
        /// Get localized string by key for current locale.
        /// </summary>
        /// <param name="key">Localization table key</param>
        /// <returns>Translated text or key if missing</returns>
        string GetLocalizedString(string key);
        
        /// <summary>
        /// Get localized string with variable substitution.
        /// </summary>
        /// <param name="key">Localization table key</param>
        /// <param name="variables">Variables to substitute (e.g., {name} → "Alice")</param>
        string GetLocalizedString(string key, Dictionary<string, string> variables);
        
        /// <summary>
        /// Change active locale (e.g., "en-US" → "ja-JP").
        /// </summary>
        /// <param name="localeCode">ISO locale code</param>
        /// <param name="onComplete">Callback when locale switched</param>
        void SetLocale(string localeCode, Action onComplete);
        
        /// <summary>
        /// Get currently active locale.
        /// </summary>
        string CurrentLocale { get; }
        
        /// <summary>
        /// Get list of available locales in project.
        /// </summary>
        List<string> AvailableLocales { get; }
        
        /// <summary>
        /// Event fired when locale changes.
        /// </summary>
        event Action<string> OnLocaleChanged;
    }
}
```

**Dependencies**: Unity Localization (com.unity.localization)

---

## VContainer Registration Example

```csharp
using VContainer;
using VContainer.Unity;

namespace NovelCore.Runtime.Core
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Core Systems
            builder.Register<IDialogueSystem, DialogueSystem>(Lifetime.Singleton);
            builder.Register<ISceneManager, SceneManager>(Lifetime.Singleton);
            builder.Register<ISaveSystem, SaveSystem>(Lifetime.Singleton);
            builder.Register<IAssetManager, AddressablesAssetManager>(Lifetime.Singleton);
            builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);
            builder.Register<IInputService, UnityInputService>(Lifetime.Singleton);
            builder.Register<ILocalizationService, UnityLocalizationService>(Lifetime.Singleton);
            
            // Platform Service (runtime selection)
            #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            builder.Register<IPlatformService, SteamPlatformService>(Lifetime.Singleton);
            #elif UNITY_IOS
            builder.Register<IPlatformService, iOSPlatformService>(Lifetime.Singleton);
            #elif UNITY_ANDROID
            builder.Register<IPlatformService, AndroidPlatformService>(Lifetime.Singleton);
            #else
            builder.Register<IPlatformService, DefaultPlatformService>(Lifetime.Singleton);
            #endif
            
            // Animation (both implementations registered, selected at runtime per character)
            builder.Register<UnityCharacterAnimator>(Lifetime.Transient);
            builder.Register<SpineCharacterAnimator>(Lifetime.Transient);
        }
    }
}
```

---

## Testing Contracts

All interfaces are mockable for unit testing:

```csharp
[Test]
public void DialogueSystem_AdvancesLine_WhenPlayerClicks()
{
    // Arrange
    var mockAudio = new Mock<IAudioService>();
    var mockLocalization = new Mock<ILocalizationService>();
    var dialogueSystem = new DialogueSystem(mockAudio.Object, mockLocalization.Object);
    
    // Act
    dialogueSystem.PlayDialogue(testDialogueLines, () => {});
    dialogueSystem.AdvanceLine();
    
    // Assert
    Assert.AreEqual(1, dialogueSystem.CurrentLineIndex);
}
```

---

## Contract Versioning

Interfaces follow semantic versioning. Breaking changes require major version bump and upgrade path documentation.

**Version 1.0.0**: All contracts defined above  
**Future Version 1.1.0**: Add new methods (non-breaking)  
**Future Version 2.0.0**: Change method signatures (breaking, requires migration guide)

---

## Conclusion

All runtime system contracts defined. Interfaces enable:
- Dependency injection via VContainer (Principle VI)
- Independent unit testing with mocks
- Platform-specific implementations (Principle II)
- Loose coupling between subsystems (Principle VI)

Ready to proceed to quickstart.md creation.
