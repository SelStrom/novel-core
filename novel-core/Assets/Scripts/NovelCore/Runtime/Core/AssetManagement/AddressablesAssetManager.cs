using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NovelCore.Runtime.Core.AssetManagement
{

/// <summary>
/// Implementation of IAssetManager using Unity Addressables.
/// Manages asset loading, caching, and release for the visual novel system.
/// </summary>
public class AddressablesAssetManager : IAssetManager
{
    private readonly Dictionary<object, AsyncOperationHandle> _loadedAssets = new();
    private readonly Dictionary<object, int> _referenceCount = new();

    public async Task<T> LoadAssetAsync<T>(object key) where T : UnityEngine.Object
    {
        if (key == null)
        {
            Debug.LogError("AddressablesAssetManager: Cannot load asset with null key");
            return null;
        }

        // Check if already loaded
        if (_loadedAssets.TryGetValue(key, out var existingHandle))
        {
            _referenceCount[key]++;
            return existingHandle.Result as T;
        }

        try
        {
            AsyncOperationHandle<T> handle;

            // Handle different key types
            if (key is AssetReference assetRef)
            {
                handle = Addressables.LoadAssetAsync<T>(assetRef);
            }
            else if (key is string stringKey)
            {
                handle = Addressables.LoadAssetAsync<T>(stringKey);
            }
            else
            {
                Debug.LogError($"AddressablesAssetManager: Unsupported key type {key.GetType()}");
                return null;
            }

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedAssets[key] = handle;
                _referenceCount[key] = 1;
                Debug.Log($"AddressablesAssetManager: Loaded asset {key}");
                return handle.Result;
            }
            else
            {
                Debug.LogError($"AddressablesAssetManager: Failed to load asset {key}: {handle.OperationException}");
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AddressablesAssetManager: Exception loading asset {key}: {ex.Message}");
            return null;
        }
    }

    public async Task<IList<T>> LoadAssetsAsync<T>(IEnumerable<object> keys) where T : UnityEngine.Object
    {
        var tasks = keys.Select(key => LoadAssetAsync<T>(key));
        var results = await Task.WhenAll(tasks);
        return results.Where(r => r != null).ToList();
    }

    public void ReleaseAsset<T>(T asset) where T : UnityEngine.Object
    {
        if (asset == null)
        {
            return;
        }

        // Find the key for this asset
        var kvp = _loadedAssets.FirstOrDefault(pair => pair.Value.Result == asset);
        if (kvp.Key == null)
        {
            Debug.LogWarning($"AddressablesAssetManager: Asset {asset.name} not found in loaded assets");
            return;
        }

        var key = kvp.Key;
        _referenceCount[key]--;

        if (_referenceCount[key] <= 0)
        {
            Addressables.Release(_loadedAssets[key]);
            _loadedAssets.Remove(key);
            _referenceCount.Remove(key);
            Debug.Log($"AddressablesAssetManager: Released asset {key}");
        }
    }

    public async Task PreloadSceneAssetsAsync(Data.Scenes.SceneData sceneData)
    {
        if (sceneData == null)
        {
            Debug.LogWarning("AddressablesAssetManager: Cannot preload null scene data");
            return;
        }

        var tasks = new List<Task>();

        // Preload background
        if (sceneData.BackgroundImage != null && sceneData.BackgroundImage.RuntimeKeyIsValid())
        {
            tasks.Add(LoadAssetAsync<Sprite>(sceneData.BackgroundImage));
        }

        // Preload background music
        if (sceneData.BackgroundMusic != null && sceneData.BackgroundMusic.RuntimeKeyIsValid())
        {
            tasks.Add(LoadAssetAsync<AudioClip>(sceneData.BackgroundMusic));
        }

        // Preload character sprites
        foreach (var characterPlacement in sceneData.Characters)
        {
            if (characterPlacement.character != null && characterPlacement.character.RuntimeKeyIsValid())
            {
                tasks.Add(LoadAssetAsync<Data.Characters.CharacterData>(characterPlacement.character));
            }
        }

        await Task.WhenAll(tasks);
        Debug.Log($"AddressablesAssetManager: Preloaded assets for scene {sceneData.SceneId}");
    }

    public void UnloadUnusedAssets()
    {
        var keysToRemove = _referenceCount
            .Where(kvp => kvp.Value <= 0)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            if (_loadedAssets.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _loadedAssets.Remove(key);
            }
            _referenceCount.Remove(key);
        }

        if (keysToRemove.Count > 0)
        {
            Debug.Log($"AddressablesAssetManager: Unloaded {keysToRemove.Count} unused assets");
        }
    }

    public bool AssetExists(object key)
    {
        if (key == null)
        {
            return false;
        }

        if (key is AssetReference assetRef)
        {
            return assetRef.RuntimeKeyIsValid();
        }

        // For string keys, we'd need to check the Addressables catalog
        // This is a simplified implementation
        return true;
    }
}

}