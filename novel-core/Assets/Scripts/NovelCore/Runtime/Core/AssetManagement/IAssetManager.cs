namespace NovelCore.Runtime.Core.AssetManagement;

/// <summary>
/// Interface for asset loading and management via Addressables.
/// Provides a wrapper around Unity Addressables for loading game assets.
/// </summary>
public interface IAssetManager
{
    /// <summary>
    /// Loads an asset asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of asset to load.</typeparam>
    /// <param name="key">Addressable key or AssetReference.</param>
    /// <returns>Task containing the loaded asset.</returns>
    Task<T> LoadAssetAsync<T>(object key) where T : UnityEngine.Object;

    /// <summary>
    /// Loads multiple assets asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of assets to load.</typeparam>
    /// <param name="keys">Collection of addressable keys.</param>
    /// <returns>Task containing list of loaded assets.</returns>
    Task<IList<T>> LoadAssetsAsync<T>(IEnumerable<object> keys) where T : UnityEngine.Object;

    /// <summary>
    /// Releases a loaded asset.
    /// </summary>
    /// <typeparam name="T">Type of asset to release.</typeparam>
    /// <param name="asset">Asset instance to release.</param>
    void ReleaseAsset<T>(T asset) where T : UnityEngine.Object;

    /// <summary>
    /// Preloads assets for a scene to reduce loading hitches.
    /// </summary>
    /// <param name="sceneData">Scene to preload assets for.</param>
    /// <returns>Task that completes when preloading is done.</returns>
    Task PreloadSceneAssetsAsync(Data.Scenes.SceneData sceneData);

    /// <summary>
    /// Unloads all unused assets to free memory.
    /// </summary>
    void UnloadUnusedAssets();

    /// <summary>
    /// Checks if an asset exists at the given key.
    /// </summary>
    /// <param name="key">Addressable key to check.</param>
    /// <returns>True if asset exists.</returns>
    bool AssetExists(object key);
}
