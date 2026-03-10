using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using NovelCore.Runtime.Core.AssetManagement;
using NovelCore.Runtime.Core.AudioSystem;
using NovelCore.Runtime.Core.InputHandling;
using NovelCore.Runtime.Core.SceneManagement;
using NovelCore.Runtime.Data.Scenes;

namespace NovelCore.Tests.Runtime.Core
{

/// <summary>
/// Mock implementation of IAssetManager for testing.
/// </summary>
public class MockAssetManager : IAssetManager
{
    private Dictionary<string, Object> _loadedAssets = new();

    public void AddAsset<T>(string key, T asset) where T : Object
    {
        _loadedAssets[key] = asset;
    }

    public Task<T> LoadAssetAsync<T>(object key) where T : Object
    {
        if (key == null)
        {
            return Task.FromResult<T>(null);
        }

        // Handle AssetReference specially
        if (key is AssetReference assetRef)
        {
            // Try to get the asset key from AssetReference
            // AssetReference.RuntimeKey might be a GUID, but we can use AssetGUID as fallback
            var runtimeKey = assetRef.RuntimeKey;
            if (runtimeKey != null)
            {
                var runtimeKeyString = runtimeKey.ToString();
                if (_loadedAssets.TryGetValue(runtimeKeyString, out var asset))
                {
                    return Task.FromResult(asset as T);
                }
            }
            
            // Fallback: try AssetGUID
            if (!string.IsNullOrEmpty(assetRef.AssetGUID))
            {
                if (_loadedAssets.TryGetValue(assetRef.AssetGUID, out var asset))
                {
                    return Task.FromResult(asset as T);
                }
            }
            
            return Task.FromResult<T>(null);
        }

        var keyString = key.ToString();
        if (_loadedAssets.TryGetValue(keyString, out var foundAsset))
        {
            return Task.FromResult(foundAsset as T);
        }

        return Task.FromResult<T>(null);
    }

    public Task<IList<T>> LoadAssetsAsync<T>(IEnumerable<object> keys) where T : Object
    {
        var results = new List<T>();
        foreach (var key in keys)
        {
            if (key != null && _loadedAssets.TryGetValue(key.ToString(), out var asset))
            {
                if (asset is T typedAsset)
                {
                    results.Add(typedAsset);
                }
            }
        }
        return Task.FromResult<IList<T>>(results);
    }

    public void ReleaseAsset<T>(T asset) where T : Object
    {
    }

    public Task PreloadSceneAssetsAsync(SceneData sceneData)
    {
        return Task.CompletedTask;
    }

    public void UnloadUnusedAssets()
    {
    }

    public bool AssetExists(object key)
    {
        if (key == null)
        {
            return false;
        }

        // Handle AssetReference specially (same logic as LoadAssetAsync)
        if (key is AssetReference assetRef)
        {
            // Try RuntimeKey first
            var runtimeKey = assetRef.RuntimeKey;
            if (runtimeKey != null && _loadedAssets.ContainsKey(runtimeKey.ToString()))
            {
                return true;
            }
            
            // Fallback: try AssetGUID
            if (!string.IsNullOrEmpty(assetRef.AssetGUID) && _loadedAssets.ContainsKey(assetRef.AssetGUID))
            {
                return true;
            }
            
            return false;
        }

        return _loadedAssets.ContainsKey(key.ToString());
    }
}

/// <summary>
/// Mock implementation of IAudioService for testing.
/// </summary>
public class MockAudioService : IAudioService
{
    public List<string> PlayedMusic = new();
    public List<string> PlayedSFX = new();
    
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;
    private bool _isMusicPlaying = false;

    public float MusicVolume => _musicVolume;
    public float SFXVolume => _sfxVolume;
    public bool IsMusicPlaying => _isMusicPlaying;

    public void PlayMusic(AudioClip musicClip, bool loop = true, float fadeInDuration = 0f)
    {
        if (musicClip != null)
        {
            PlayedMusic.Add(musicClip.name);
            _isMusicPlaying = true;
        }
    }

    public void StopMusic(float fadeOutDuration = 0f)
    {
        _isMusicPlaying = false;
    }

    public void PlaySFX(AudioClip sfxClip, float volume = 1f)
    {
        if (sfxClip != null)
        {
            PlayedSFX.Add(sfxClip.name);
        }
    }

    public void SetMasterVolume(float volume)
    {
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
    }

    public void PauseAll()
    {
    }

    public void ResumeAll()
    {
    }
}

/// <summary>
/// Mock implementation of IInputService for testing.
/// </summary>
public class MockInputService : IInputService
{
    public event System.Action OnPrimaryAction;
    public event System.Action OnSecondaryAction;
    public event System.Action OnCancelAction;

    public Vector2 PointerPosition { get; set; }
    public bool WasPrimaryActionPerformed { get; set; }
    public bool IsPointerPressed { get; set; }
    public bool InputEnabled { get; set; } = true;

    public void TriggerPrimaryAction()
    {
        OnPrimaryAction?.Invoke();
    }

    public void TriggerSecondaryAction()
    {
        OnSecondaryAction?.Invoke();
    }

    public void TriggerCancelAction()
    {
        OnCancelAction?.Invoke();
    }
}

/// <summary>
/// Mock implementation of ISceneManager for testing.
/// </summary>
public class MockSceneManager : ISceneManager
{
    public SceneData CurrentScene { get; private set; }
    public bool IsLoading { get; private set; }
    
    public event System.Action<SceneData> OnSceneLoadStart;
    public event System.Action<SceneData> OnSceneLoadComplete;
    public event System.Action<SceneData> OnSceneTransitionStart;
    public event System.Action<SceneData> OnSceneTransitionComplete;

    public Dictionary<string, string> CharacterEmotions = new();
    public Dictionary<string, bool> CharacterVisibility = new();
    public Dictionary<string, Vector2> CharacterPositions = new();
    
    public int LoadSceneCallCount { get; private set; }
    public string LastLoadedSceneGuid { get; private set; }
    
    public void Reset()
    {
        LoadSceneCallCount = 0;
        LastLoadedSceneGuid = null;
        CurrentScene = null;
        CharacterEmotions.Clear();
        CharacterVisibility.Clear();
        CharacterPositions.Clear();
    }

    public void LoadScene(SceneData sceneData)
    {
        LoadSceneCallCount++;
        LastLoadedSceneGuid = sceneData?.name ?? "null";
        CurrentScene = sceneData;
        OnSceneLoadStart?.Invoke(sceneData);
        OnSceneLoadComplete?.Invoke(sceneData);
    }

    public async Task LoadSceneAsync(SceneData sceneData)
    {
        LoadSceneCallCount++;
        LastLoadedSceneGuid = sceneData?.name ?? "null";
        
        IsLoading = true;
        OnSceneLoadStart?.Invoke(sceneData);
        
        CurrentScene = sceneData;
        
        await Task.Delay(10);
        
        IsLoading = false;
        OnSceneLoadComplete?.Invoke(sceneData);
    }

    public void UnloadCurrentScene()
    {
        CurrentScene = null;
    }

    public Task PreloadSceneAsync(SceneData sceneData)
    {
        return Task.CompletedTask;
    }

    public void UpdateCharacterEmotion(string characterId, string emotion)
    {
        CharacterEmotions[characterId] = emotion;
    }

    public void SetCharacterVisible(string characterId, bool visible)
    {
        CharacterVisibility[characterId] = visible;
    }

    public void MoveCharacter(string characterId, Vector2 targetPosition, float duration = 0.5f)
    {
        CharacterPositions[characterId] = targetPosition;
    }

    public void TransitionToScene(SceneData targetScene, TransitionType transitionType, float duration)
    {
        OnSceneTransitionStart?.Invoke(targetScene);
        CurrentScene = targetScene;
        OnSceneTransitionComplete?.Invoke(targetScene);
    }

    public bool NavigateBack()
    {
        // Mock implementation - always returns false for tests
        return false;
    }

    public bool NavigateForward()
    {
        // Mock implementation - always returns false for tests
        return false;
    }

    public bool CanNavigateBack()
    {
        // Mock implementation - always returns false for tests
        return false;
    }

    public bool CanNavigateForward()
    {
        // Mock implementation - always returns false for tests
        return false;
    }
}

}
