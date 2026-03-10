using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using NovelCore.Runtime.Core.AssetManagement;
using NovelCore.Runtime.Core.AudioSystem;
using NovelCore.Runtime.Core.SceneManagement.Transitions;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Characters;
using NovelCore.Runtime.Animation;

namespace NovelCore.Runtime.Core.SceneManagement
{

/// <summary>
/// Manages scene loading, rendering backgrounds, and character positioning.
/// </summary>
public class SceneManager : ISceneManager
{
    private readonly IAssetManager _assetManager;
    private readonly IAudioService _audioService;
    private readonly ICharacterAnimatorFactory _animatorFactory;
    private readonly ISceneNavigationHistory _navigationHistory;
    private readonly SceneTransitionFactory _transitionFactory;

    private SceneData _currentScene;
    private bool _isLoading;
    private ISceneTransition _currentTransition;
    private bool _isNavigating; // Flag to prevent history recording during navigation

    // Rendering components
    private GameObject _backgroundContainer;
    private SpriteRenderer _backgroundRenderer;
    private Dictionary<string, GameObject> _characterObjects = new();
    private Dictionary<string, ICharacterAnimator> _characterAnimators = new();

    // Scene hierarchy
    private const string BackgroundLayerName = "Background";
    private const string CharacterLayerName = "Characters";
    private const int BackgroundSortingOrder = -100;
    private const int CharacterBaseSortingOrder = 0;

    public SceneData CurrentScene => _currentScene;
    public bool IsLoading => _isLoading;

    public event System.Action<SceneData> OnSceneLoadStart;
    public event System.Action<SceneData> OnSceneLoadComplete;
    public event System.Action<SceneData> OnSceneTransitionStart;
    public event System.Action<SceneData> OnSceneTransitionComplete;

    public SceneManager(
        IAssetManager assetManager, 
        IAudioService audioService,
        ICharacterAnimatorFactory animatorFactory,
        ISceneNavigationHistory navigationHistory)
    {
        _assetManager = assetManager ?? throw new System.ArgumentNullException(nameof(assetManager));
        _audioService = audioService ?? throw new System.ArgumentNullException(nameof(audioService));
        _animatorFactory = animatorFactory ?? throw new System.ArgumentNullException(nameof(animatorFactory));
        _navigationHistory = navigationHistory; // Can be injected as singleton from VContainer
        _transitionFactory = new SceneTransitionFactory();

        InitializeSceneHierarchy();
    }

    public void LoadScene(SceneData sceneData)
    {
        LoadSceneAsync(sceneData).ConfigureAwait(false);
    }

    public async Task LoadSceneAsync(SceneData sceneData)
    {
        if (sceneData == null)
        {
            Debug.LogError("SceneManager: Cannot load null scene");
            return;
        }

        if (_isLoading)
        {
            Debug.LogWarning("SceneManager: Scene already loading, ignoring request");
            return;
        }

        _isLoading = true;
        OnSceneLoadStart?.Invoke(sceneData);

        Debug.Log($"SceneManager: Loading scene {sceneData.SceneName}");

        // Unload previous scene
        UnloadCurrentScene();

        // Start transition
        OnSceneTransitionStart?.Invoke(sceneData);
        
        // Create and play transition effect
        _currentTransition = _transitionFactory.CreateTransition(sceneData.TransitionType);
        var transitionTask = _currentTransition.PlayAsync(sceneData.TransitionDuration);

        try
        {
            // Load background
            await LoadBackgroundAsync(sceneData);

            // Load and position characters
            await LoadCharactersAsync(sceneData);

            // Play background music with fade in
            if (sceneData.BackgroundMusic != null && sceneData.BackgroundMusic.RuntimeKeyIsValid())
            {
                await LoadAndPlayMusicAsync(sceneData.BackgroundMusic, fadeInDuration: 1.0f);
            }

            _currentScene = sceneData;

            // Wait for transition to complete
            await transitionTask;
            
            // Clean up transition
            _currentTransition?.Cleanup();
            _currentTransition = null;

            OnSceneTransitionComplete?.Invoke(sceneData);
            OnSceneLoadComplete?.Invoke(sceneData);
            
            Debug.Log("SceneManager: Scene load complete event fired");

            // Add to navigation history (if not navigating and history is available)
            if (!_isNavigating && _navigationHistory != null)
            {
                var historyEntry = new SceneHistoryEntry(
                    sceneId: sceneData.SceneId,
                    dialogueLineIndex: 0, // Start at beginning of scene
                    gameStateSnapshot: new Dictionary<string, object>(), // TODO: Get from GameStateManager
                    sceneData: sceneData // Store reference for easy loading
                );
                _navigationHistory.Push(historyEntry);
                Debug.Log($"SceneManager: Added scene '{sceneData.SceneId}' to navigation history");
            }

            Debug.Log($"SceneManager: Scene {sceneData.SceneName} loaded successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SceneManager: Failed to load scene {sceneData.SceneName}: {ex.Message}");
            
            // Clean up transition on error
            _currentTransition?.Cleanup();
            _currentTransition = null;
        }
        finally
        {
            _isLoading = false;
        }
    }

    public void UnloadCurrentScene()
    {
        if (_currentScene == null)
        {
            return;
        }

        Debug.Log($"SceneManager: Unloading scene {_currentScene.SceneName}");

        // Clear background
        if (_backgroundRenderer != null)
        {
            _backgroundRenderer.sprite = null;
        }

        // Destroy all character objects
        foreach (var characterObj in _characterObjects.Values)
        {
            if (characterObj != null)
            {
                GameObject.Destroy(characterObj);
            }
        }
        _characterObjects.Clear();
        _characterAnimators.Clear();

        // Stop music
        _audioService.StopMusic();

        // TODO: Release loaded assets via AssetManager

        _currentScene = null;
    }

    public async Task PreloadSceneAsync(SceneData sceneData)
    {
        if (sceneData == null)
        {
            Debug.LogWarning("SceneManager: Cannot preload null scene");
            return;
        }

        Debug.Log($"SceneManager: Preloading scene {sceneData.SceneName}");

        // TODO: Preload assets via AssetManager
        await Task.CompletedTask;
    }

    public void UpdateCharacterEmotion(string characterId, string emotion)
    {
        if (!_characterAnimators.ContainsKey(characterId))
        {
            Debug.LogWarning($"SceneManager: Character {characterId} not found in scene");
            return;
        }

        Debug.Log($"SceneManager: Updating character {characterId} emotion to {emotion}");

        var animator = _characterAnimators[characterId];
        animator?.SetEmotion(emotion);
    }

    public void SetCharacterVisible(string characterId, bool visible)
    {
        if (!_characterObjects.ContainsKey(characterId))
        {
            Debug.LogWarning($"SceneManager: Character {characterId} not found in scene");
            return;
        }

        var characterObj = _characterObjects[characterId];
        if (characterObj != null)
        {
            characterObj.SetActive(visible);
        }
    }

    public void MoveCharacter(string characterId, Vector2 targetPosition, float duration = 0.5f)
    {
        if (!_characterObjects.ContainsKey(characterId))
        {
            Debug.LogWarning($"SceneManager: Character {characterId} not found in scene");
            return;
        }

        var characterObj = _characterObjects[characterId];
        if (characterObj == null)
        {
            return;
        }

        // Convert normalized position (0-1) to world position
        Vector3 worldPosition = NormalizedToWorldPosition(targetPosition);

        // TODO: Animate movement with tweening
        // For now, instant move
        characterObj.transform.position = worldPosition;

        Debug.Log($"SceneManager: Moved character {characterId} to {worldPosition}");
    }

    private void InitializeSceneHierarchy()
    {
        // Create background container
        _backgroundContainer = new GameObject("Background");
        _backgroundRenderer = _backgroundContainer.AddComponent<SpriteRenderer>();
        _backgroundRenderer.sortingOrder = BackgroundSortingOrder;
        
        // Position background at origin
        _backgroundContainer.transform.position = Vector3.zero;
        
        // Prevent Unity from destroying background container on scene load
        // (SceneManager is a Singleton, so background should persist)
        UnityEngine.Object.DontDestroyOnLoad(_backgroundContainer);

        Debug.Log("SceneManager: Scene hierarchy initialized");
    }

    private async Task LoadBackgroundAsync(SceneData sceneData)
    {
        if (sceneData.BackgroundImage == null || !sceneData.BackgroundImage.RuntimeKeyIsValid())
        {
            Debug.LogWarning($"SceneManager: No background image for scene {sceneData.SceneName}");
            return;
        }

        Debug.Log($"SceneManager: Loading background for {sceneData.SceneName}");

        try
        {
            // Load sprite via AssetManager (Constitution Principle III: Addressables)
            Sprite backgroundSprite = await _assetManager.LoadAssetAsync<Sprite>(sceneData.BackgroundImage);
            
            if (backgroundSprite == null)
            {
                Debug.LogWarning($"SceneManager: Failed to load background sprite for {sceneData.SceneName}");
                return;
            }

            // Defensive check: Ensure background renderer still exists
            // (This should not happen after DontDestroyOnLoad fix, but added for safety)
            if (_backgroundRenderer == null)
            {
                Debug.LogError("SceneManager: Background renderer was destroyed! Re-initializing scene hierarchy...");
                InitializeSceneHierarchy();
            }

            // Assign to renderer
            _backgroundRenderer.sprite = backgroundSprite;
            
            Debug.Log($"SceneManager: Background sprite loaded successfully: {backgroundSprite.name}");
            
            // Scale background to fit screen (maintain aspect ratio)
            ScaleBackgroundToFitScreen();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SceneManager: Error loading background for {sceneData.SceneName}: {ex.Message}");
            Debug.LogException(ex);
        }
    }

    private async Task LoadCharactersAsync(SceneData sceneData)
    {
        if (sceneData.Characters == null || sceneData.Characters.Count == 0)
        {
            Debug.Log($"SceneManager: No characters in scene {sceneData.SceneName}");
            return;
        }

        Debug.Log($"SceneManager: Loading {sceneData.Characters.Count} characters");

        foreach (var characterPlacement in sceneData.Characters)
        {
            await LoadCharacterAsync(characterPlacement);
        }
    }

    private async Task LoadCharacterAsync(CharacterPlacement placement)
    {
        if (placement.character == null || !placement.character.RuntimeKeyIsValid())
        {
            Debug.LogWarning("SceneManager: Invalid character reference in placement");
            return;
        }

        try
        {
            // Load character data
            var characterData = await _assetManager.LoadAssetAsync<CharacterData>(placement.character);
            if (characterData == null)
            {
                Debug.LogError("SceneManager: Failed to load character data");
                return;
            }

            string characterId = characterData.CharacterId;

            // Create character GameObject
            var characterObj = new GameObject($"Character_{characterData.CharacterName}");
            
            // Prevent Unity from destroying character objects on scene load
            // (SceneManager is a Singleton, characters should persist across scene transitions)
            UnityEngine.Object.DontDestroyOnLoad(characterObj);
            
            // Position character
            Vector3 worldPosition = NormalizedToWorldPosition(placement.position);
            characterObj.transform.position = worldPosition;
            characterObj.transform.localScale = new Vector3(
                characterData.DefaultScale.x, 
                characterData.DefaultScale.y, 
                1f);

            // Create appropriate animator based on animation type
            ICharacterAnimator animator = _animatorFactory.Create(characterData.AnimationType);
            animator.Initialize(characterData, characterObj);
            animator.SetEmotion(placement.initialEmotion);

            // Store references
            _characterObjects[characterId] = characterObj;
            _characterAnimators[characterId] = animator;

            Debug.Log($"SceneManager: Character {characterData.CharacterName} loaded at {worldPosition} with {characterData.AnimationType} animator");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SceneManager: Error loading character: {ex.Message}");
        }
    }

    private void ScaleBackgroundToFitScreen()
    {
        if (_backgroundRenderer == null || _backgroundRenderer.sprite == null)
        {
            return;
        }

        // Get camera bounds
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("SceneManager: No main camera found");
            return;
        }

        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Get sprite bounds
        Sprite sprite = _backgroundRenderer.sprite;
        float spriteWidth = sprite.bounds.size.x;
        float spriteHeight = sprite.bounds.size.y;

        // Calculate scale to fit screen
        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;
        float scale = Mathf.Max(scaleX, scaleY); // Use max to cover screen

        _backgroundContainer.transform.localScale = new Vector3(scale, scale, 1f);
    }

    private Vector3 NormalizedToWorldPosition(Vector2 normalizedPos)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("SceneManager: No main camera found, using default position");
            return new Vector3(normalizedPos.x, normalizedPos.y, 0f);
        }

        // Convert normalized position (0-1) to world position
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float worldX = (normalizedPos.x - 0.5f) * cameraWidth;
        float worldY = (normalizedPos.y - 0.5f) * cameraHeight;

        return new Vector3(worldX, worldY, 0f);
    }

    /// <summary>
    /// Navigate back to the previous scene in history.
    /// </summary>
    public bool NavigateBack()
    {
        if (_navigationHistory == null)
        {
            Debug.LogWarning("SceneManager: Navigation history not available");
            return false;
        }

        if (!_navigationHistory.CanNavigateBack())
        {
            Debug.LogWarning("SceneManager: Cannot navigate back (no previous scene in history)");
            return false;
        }

        var previousEntry = _navigationHistory.NavigateBack();
        if (previousEntry == null || previousEntry.sceneData == null)
        {
            Debug.LogError("SceneManager: Failed to get previous scene from history");
            return false;
        }

        Debug.Log($"SceneManager: Navigating back to scene '{previousEntry.sceneId}' at line {previousEntry.dialogueLineIndex}");

        // Load the scene with navigation flag set to prevent adding to history
        _isNavigating = true;
        NavigateToSceneAsync(previousEntry.sceneData);

        // TODO: Restore dialogue position to previousEntry.dialogueLineIndex
        // This requires DialogueSystem integration

        return true;
    }

    /// <summary>
    /// Navigate forward to the next scene in history.
    /// </summary>
    public bool NavigateForward()
    {
        if (_navigationHistory == null)
        {
            Debug.LogWarning("SceneManager: Navigation history not available");
            return false;
        }

        if (!_navigationHistory.CanNavigateForward())
        {
            Debug.LogWarning("SceneManager: Cannot navigate forward (no next scene in history)");
            return false;
        }

        var nextEntry = _navigationHistory.NavigateForward();
        if (nextEntry == null || nextEntry.sceneData == null)
        {
            Debug.LogError("SceneManager: Failed to get next scene from history");
            return false;
        }

        Debug.Log($"SceneManager: Navigating forward to scene '{nextEntry.sceneId}' at line {nextEntry.dialogueLineIndex}");

        // Load the scene with navigation flag set to prevent adding to history
        _isNavigating = true;
        NavigateToSceneAsync(nextEntry.sceneData);

        // TODO: Restore dialogue position to nextEntry.dialogueLineIndex
        // This requires DialogueSystem integration

        return true;
    }

    /// <summary>
    /// Helper method for navigation that properly handles async loading.
    /// </summary>
    private async void NavigateToSceneAsync(SceneData sceneData)
    {
        try
        {
            await LoadSceneAsync(sceneData);
        }
        finally
        {
            _isNavigating = false;
        }
    }

    /// <summary>
    /// Check if back navigation is possible.
    /// </summary>
    public bool CanNavigateBack()
    {
        return _navigationHistory != null && _navigationHistory.CanNavigateBack();
    }

    /// <summary>
    /// Check if forward navigation is possible.
    /// </summary>
    public bool CanNavigateForward()
    {
        return _navigationHistory != null && _navigationHistory.CanNavigateForward();
    }

    /// <summary>
    /// Clean up resources on disposal.
    /// </summary>
    public void Dispose()
    {
        UnloadCurrentScene();
        
        // Clean up active transition
        _currentTransition?.Cleanup();
        _currentTransition = null;

        if (_backgroundContainer != null)
        {
            GameObject.Destroy(_backgroundContainer);
        }
    }

    /// <summary>
    /// Loads and plays background music asynchronously with fade in.
    /// </summary>
    private async Task LoadAndPlayMusicAsync(AssetReference musicReference, float fadeInDuration = 1.0f)
    {
        try
        {
            // Stop current music with fade out if playing
            if (_audioService.IsMusicPlaying)
            {
                _audioService.StopMusic(fadeOutDuration: 0.5f);
                await Task.Delay((int)(500)); // Wait for fade out
            }

            // Load new music clip
            AudioClip musicClip = await _assetManager.LoadAssetAsync<AudioClip>(musicReference);
            if (musicClip != null)
            {
                _audioService.PlayMusic(musicClip, loop: true, fadeInDuration: fadeInDuration);
                Debug.Log($"SceneManager: Playing background music {musicClip.name}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SceneManager: Failed to load background music: {ex.Message}");
        }
    }
}

}