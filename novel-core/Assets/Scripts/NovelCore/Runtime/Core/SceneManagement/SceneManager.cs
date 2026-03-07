using System.Threading.Tasks;

namespace NovelCore.Runtime.Core.SceneManagement;

/// <summary>
/// Manages scene loading, rendering backgrounds, and character positioning.
/// </summary>
public class SceneManager : ISceneManager
{
    private readonly IAssetManager _assetManager;
    private readonly IAudioService _audioService;

    private SceneData _currentScene;
    private bool _isLoading;

    // Rendering components
    private GameObject _backgroundContainer;
    private SpriteRenderer _backgroundRenderer;
    private Dictionary<string, GameObject> _characterObjects = new();
    private Dictionary<string, SpriteRenderer> _characterRenderers = new();

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

    public SceneManager(IAssetManager assetManager, IAudioService audioService)
    {
        _assetManager = assetManager ?? throw new System.ArgumentNullException(nameof(assetManager));
        _audioService = audioService ?? throw new System.ArgumentNullException(nameof(audioService));

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

        try
        {
            // Load background
            await LoadBackgroundAsync(sceneData);

            // Load and position characters
            await LoadCharactersAsync(sceneData);

            // Play background music
            if (sceneData.BackgroundMusic != null && sceneData.BackgroundMusic.RuntimeKeyIsValid())
            {
                // TODO: Load music via AssetManager
                Debug.Log($"SceneManager: Playing background music for {sceneData.SceneName}");
            }

            _currentScene = sceneData;

            OnSceneTransitionComplete?.Invoke(sceneData);
            OnSceneLoadComplete?.Invoke(sceneData);

            Debug.Log($"SceneManager: Scene {sceneData.SceneName} loaded successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SceneManager: Failed to load scene {sceneData.SceneName}: {ex.Message}");
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
        _characterRenderers.Clear();

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
        if (!_characterRenderers.ContainsKey(characterId))
        {
            Debug.LogWarning($"SceneManager: Character {characterId} not found in scene");
            return;
        }

        Debug.Log($"SceneManager: Updating character {characterId} emotion to {emotion}");

        // TODO: Load and swap character sprite based on emotion
        // This will be implemented when character system is complete
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

        // TODO: Load sprite via AssetManager
        // For now, just log
        await Task.CompletedTask;

        // _backgroundRenderer.sprite = loadedSprite;
        
        // Scale background to fit screen
        ScaleBackgroundToFitScreen();
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

        // TODO: Get character ID from loaded CharacterData
        string characterId = placement.character.AssetGUID;

        // Create character GameObject
        var characterObj = new GameObject($"Character_{characterId}");
        var spriteRenderer = characterObj.AddComponent<SpriteRenderer>();

        // Set sorting order
        spriteRenderer.sortingOrder = CharacterBaseSortingOrder + placement.sortingOrder;

        // Position character
        Vector3 worldPosition = NormalizedToWorldPosition(placement.position);
        characterObj.transform.position = worldPosition;

        // Store references
        _characterObjects[characterId] = characterObj;
        _characterRenderers[characterId] = spriteRenderer;

        // TODO: Load character sprite via AssetManager based on initialEmotion
        Debug.Log($"SceneManager: Character {characterId} positioned at {worldPosition}");

        await Task.CompletedTask;
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
    /// Clean up resources on disposal.
    /// </summary>
    public void Dispose()
    {
        UnloadCurrentScene();

        if (_backgroundContainer != null)
        {
            GameObject.Destroy(_backgroundContainer);
        }
    }
}
