using System;
using System.Collections.Generic;
using UnityEngine;

namespace NovelCore.Runtime.Core.Localization
{

/// <summary>
/// Basic implementation of ILocalizationService.
/// Uses a simple dictionary for localization strings.
/// </summary>
public class BasicLocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _localizationData;
    private string _currentLanguage = "en";

    public event Action<string> OnLanguageChanged;
    public string CurrentLanguage => _currentLanguage;

    public BasicLocalizationService()
    {
        _localizationData = new Dictionary<string, Dictionary<string, string>>();
        InitializeDefaultLanguages();
    }

    private void InitializeDefaultLanguages()
    {
        _localizationData["en"] = new Dictionary<string, string>
        {
            ["dialogue.continue"] = "Continue",
            ["dialogue.skip"] = "Skip",
            ["dialogue.auto"] = "Auto",
            ["dialogue.save"] = "Save",
            ["dialogue.load"] = "Load",
            ["dialogue.settings"] = "Settings",
            ["choice.timeout"] = "Time remaining: {0}s",
            ["choice.default"] = "Continue...",
        };

        _localizationData["ru"] = new Dictionary<string, string>
        {
            ["dialogue.continue"] = "Продолжить",
            ["dialogue.skip"] = "Пропустить",
            ["dialogue.auto"] = "Авто",
            ["dialogue.save"] = "Сохранить",
            ["dialogue.load"] = "Загрузить",
            ["dialogue.settings"] = "Настройки",
            ["choice.timeout"] = "Осталось времени: {0}с",
            ["choice.default"] = "Продолжить...",
        };
    }

    public bool TryGetLocalizedString(string key, out string result)
    {
        result = null;

        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        if (_localizationData.TryGetValue(_currentLanguage, out var languageData))
        {
            if (languageData.TryGetValue(key, out result))
            {
                return true;
            }
        }

        Debug.LogWarning($"Localization key not found: {key} for language: {_currentLanguage}");
        return false;
    }

    public bool TryGetLocalizedString(string key, out string result, params object[] args)
    {
        if (!TryGetLocalizedString(key, out string baseString))
        {
            result = null;
            return false;
        }

        try
        {
            result = string.Format(baseString, args);
            return true;
        }
        catch (FormatException ex)
        {
            Debug.LogError($"Localization formatting error for key '{key}': {ex.Message}");
            result = baseString;
            return false;
        }
    }

    public void SetLanguage(string languageCode)
    {
        if (!_localizationData.ContainsKey(languageCode))
        {
            Debug.LogWarning($"Language not supported: {languageCode}. Available: {string.Join(", ", _localizationData.Keys)}");
            return;
        }

        _currentLanguage = languageCode;
        OnLanguageChanged?.Invoke(_currentLanguage);
        Debug.Log($"Language changed to: {_currentLanguage}");
    }
}

}
