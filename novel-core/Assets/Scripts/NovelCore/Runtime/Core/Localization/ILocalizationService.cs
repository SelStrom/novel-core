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
    /// Attempts to get a localized string by key.
    /// </summary>
    /// <param name="key">Localization key.</param>
    /// <param name="result">When this method returns true, contains the localized string; otherwise, null.</param>
    /// <returns>true if the key exists in the current language dictionary; otherwise, false.</returns>
    bool TryGetLocalizedString(string key, out string result);

    /// <summary>
    /// Attempts to get a localized string by key with formatting.
    /// </summary>
    /// <param name="key">Localization key.</param>
    /// <param name="result">When this method returns true, contains the formatted localized string; otherwise, null.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>true if the key exists and formatting succeeded; otherwise, false.</returns>
    bool TryGetLocalizedString(string key, out string result, params object[] args);

    /// <summary>
    /// Sets the current language.
    /// </summary>
    /// <param name="languageCode">Language code to set.</param>
    void SetLanguage(string languageCode);
}

}
