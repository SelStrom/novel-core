using System;
using UnityEngine;

namespace NovelCore.Runtime.Core.Localization
{

/// <summary>
/// Interface for localization service.
/// Handles text localization and language switching.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Event raised when the current language changes.
    /// </summary>
    event Action<string> OnLanguageChanged;

    /// <summary>
    /// Gets the current language code (e.g., "en", "ru", "ja").
    /// </summary>
    string CurrentLanguage { get; }

    /// <summary>
    /// Gets a localized string by key.
    /// </summary>
    /// <param name="key">Localization key.</param>
    /// <returns>Localized string or key if not found.</returns>
    string GetLocalizedString(string key);

    /// <summary>
    /// Gets a localized string by key with formatting.
    /// </summary>
    /// <param name="key">Localization key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>Formatted localized string.</returns>
    string GetLocalizedString(string key, params object[] args);

    /// <summary>
    /// Sets the current language.
    /// </summary>
    /// <param name="languageCode">Language code to set.</param>
    void SetLanguage(string languageCode);

    /// <summary>
    /// Checks if a localization key exists.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <returns>True if key exists, false otherwise.</returns>
    bool HasKey(string key);
}

}
