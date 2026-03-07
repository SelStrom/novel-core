namespace NovelCore.Runtime.Core.SceneManagement
{

/// <summary>
/// Types of scene transitions.
/// </summary>
public enum TransitionType
{
    /// <summary>
    /// Instant transition with no visual effect.
    /// </summary>
    Cut,
    
    /// <summary>
    /// Fade to black then fade in to new scene.
    /// </summary>
    Fade,
    
    /// <summary>
    /// Slide transition (left, right, up, down).
    /// </summary>
    Slide,
    
    /// <summary>
    /// Custom transition using shader effects.
    /// </summary>
    Custom
}

}
