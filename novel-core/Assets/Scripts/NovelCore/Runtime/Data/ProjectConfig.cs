using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Characters;

namespace NovelCore.Runtime.Data
{

/// <summary>
/// Represents a complete visual novel project with metadata and global settings.
/// This is the root configuration object for a visual novel.
/// </summary>
[CreateAssetMenu(fileName = "NewProject", menuName = "NovelCore/Project Config", order = 0)]
public class ProjectConfig : BaseScriptableObject
{
    [Header("Project Metadata")]
    [SerializeField]
    [Tooltip("Unique project identifier (GUID)")]
    private string _projectId;

    [SerializeField]
    [Tooltip("Display name of the project")]
    private string _projectName;

    [SerializeField]
    [Tooltip("Author/Creator name")]
    private string _author;

    [SerializeField]
    [Tooltip("Project description")]
    [TextArea(3, 5)]
    private string _description;

    [SerializeField]
    [Tooltip("Semantic version (e.g., 1.0.0)")]
    private string _version = "1.0.0";

    [Header("Project Dates")]
    [SerializeField]
    [Tooltip("Project creation date")]
    private string _createdDate;

    [SerializeField]
    [Tooltip("Last modification date")]
    private string _lastModifiedDate;

    [Header("Scene Configuration")]
    [SerializeField]
    [Tooltip("Starting scene (entry point of the visual novel)")]
    private AssetReference _startingScene;

    [Header("Localization")]
    [SerializeField]
    [Tooltip("Default locale (ISO code, e.g., 'en-US', 'ru-RU')")]
    private string _defaultLocale = "en-US";

    [SerializeField]
    [Tooltip("Supported locales for this project")]
    private List<string> _supportedLocales = new List<string> { "en-US" };

    [Header("Build Settings")]
    [SerializeField]
    [Tooltip("Target platforms for this project")]
    private PlatformFlags _targetPlatforms = PlatformFlags.Windows | PlatformFlags.macOS;

    // Properties
    public string ProjectId => _projectId;
    public string ProjectName => _projectName;
    public string Author => _author;
    public string Description => _description;
    public string Version => _version;
    public string CreatedDate => _createdDate;
    public string LastModifiedDate => _lastModifiedDate;
    public AssetReference StartingScene => _startingScene;
    public string DefaultLocale => _defaultLocale;
    public IReadOnlyList<string> SupportedLocales => _supportedLocales;
    public PlatformFlags TargetPlatforms => _targetPlatforms;

    /// <summary>
    /// Updates the last modified date to current time.
    /// Call this whenever project configuration changes.
    /// </summary>
    public void MarkAsModified()
    {
        _lastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public override bool Validate()
    {
        bool isValid = true;

        if (string.IsNullOrEmpty(_projectId))
        {
            Debug.LogError($"ProjectConfig {name}: projectId is required");
            isValid = false;
        }

        if (string.IsNullOrEmpty(_projectName))
        {
            Debug.LogError($"ProjectConfig {name}: projectName is required");
            isValid = false;
        }

        if (_projectName != null && _projectName.Length > 100)
        {
            Debug.LogError($"ProjectConfig {name}: projectName exceeds maximum length of 100 characters");
            isValid = false;
        }

        if (_startingScene == null || !_startingScene.RuntimeKeyIsValid())
        {
            Debug.LogError($"ProjectConfig {name}: startingScene must reference a valid SceneData asset");
            isValid = false;
        }

        if (_targetPlatforms == 0)
        {
            Debug.LogError($"ProjectConfig {name}: At least one target platform must be selected");
            isValid = false;
        }

        if (string.IsNullOrEmpty(_defaultLocale))
        {
            Debug.LogWarning($"ProjectConfig {name}: defaultLocale is not set, using 'en-US'");
            _defaultLocale = "en-US";
        }

        if (_supportedLocales == null || _supportedLocales.Count == 0)
        {
            Debug.LogWarning($"ProjectConfig {name}: No supported locales defined, adding default locale");
            _supportedLocales = new List<string> { _defaultLocale };
        }

        if (!_supportedLocales.Contains(_defaultLocale))
        {
            Debug.LogWarning($"ProjectConfig {name}: Default locale '{_defaultLocale}' not in supported locales, adding it");
            _supportedLocales.Insert(0, _defaultLocale);
        }

        return isValid;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // Generate project ID if not set
        if (string.IsNullOrEmpty(_projectId))
        {
            _projectId = Guid.NewGuid().ToString();
        }

        // Set creation date if not set
        if (string.IsNullOrEmpty(_createdDate))
        {
            _createdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // Initialize last modified date
        if (string.IsNullOrEmpty(_lastModifiedDate))
        {
            _lastModifiedDate = _createdDate;
        }
    }
}

/// <summary>
/// Platform flags for build targets.
/// Can be combined using bitwise OR.
/// </summary>
[Flags]
public enum PlatformFlags
{
    None = 0,
    Windows = 1 << 0,
    macOS = 1 << 1,
    iOS = 1 << 2,
    Android = 1 << 3,
    Linux = 1 << 4,
    Steam = 1 << 5  // Steam integration enabled
}

}
