using System;
using UnityEngine;
using System.Threading.Tasks;

namespace NovelCore.Runtime.Platform.Interfaces
{

/// <summary>
/// Platform abstraction interface for platform-specific functionality.
/// Implementations exist for Steam, iOS, and Android platforms.
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Gets the name of the current platform.
    /// </summary>
    string PlatformName { get; }

    /// <summary>
    /// Initializes the platform-specific services.
    /// </summary>
    /// <returns>True if initialization succeeded, false otherwise.</returns>
    bool Initialize();

    /// <summary>
    /// Shuts down platform-specific services.
    /// </summary>
    void Shutdown();

    /// <summary>
    /// Checks if cloud save is available on this platform.
    /// </summary>
    bool IsCloudSaveAvailable { get; }

    /// <summary>
    /// Saves data to platform cloud storage.
    /// </summary>
    /// <param name="key">Storage key.</param>
    /// <param name="data">Data to save.</param>
    /// <returns>True if save succeeded.</returns>
    Task<bool> SaveToCloudAsync(string key, byte[] data);

    /// <summary>
    /// Loads data from platform cloud storage.
    /// </summary>
    /// <param name="key">Storage key.</param>
    /// <returns>Loaded data, or null if not found.</returns>
    Task<byte[]> LoadFromCloudAsync(string key);
}

}