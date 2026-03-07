namespace NovelCore.Runtime.Data
{

/// <summary>
/// Base class for all NovelCore ScriptableObjects with versioning support.
/// Provides common functionality for data validation and serialization.
/// </summary>
public abstract class BaseScriptableObject : ScriptableObject
{
    [SerializeField]
    [Tooltip("Version of this data format for migration purposes")]
    private string _version = "1.0.0";

    /// <summary>
    /// Gets the current version of this ScriptableObject data format.
    /// </summary>
    public string Version => _version;

    /// <summary>
    /// Unique identifier for this ScriptableObject instance.
    /// </summary>
    public string Id
    {
        get
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = System.Guid.NewGuid().ToString();
            }
            return _id;
        }
    }

    [SerializeField]
    [HideInInspector]
    private string _id;

    /// <summary>
    /// Validates this ScriptableObject's data.
    /// Override in derived classes to add specific validation logic.
    /// </summary>
    /// <returns>True if validation passes, false otherwise.</returns>
    public virtual bool Validate()
    {
        return true;
    }

    /// <summary>
    /// Called when the ScriptableObject is first created.
    /// </summary>
    protected virtual void OnEnable()
    {
        // Ensure ID is generated
        if (string.IsNullOrEmpty(_id))
        {
            _id = System.Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// Performs data migration from an older version.
    /// Override in derived classes to handle version-specific migrations.
    /// </summary>
    /// <param name="fromVersion">The version to migrate from.</param>
    public virtual void MigrateFrom(string fromVersion)
    {
        Debug.Log($"Migrating {name} from version {fromVersion} to {_version}");
    }
}

}