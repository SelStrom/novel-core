using System.Threading.Tasks;
using UnityEngine;

namespace NovelCore.Runtime.Core.SceneManagement.Transitions
{
    /// <summary>
    /// Instant cut transition (no animation).
    /// </summary>
    public class CutTransition : ISceneTransition
    {
    private bool _isInitialized;
    
    public void Initialize(Camera camera)
    {
        _isInitialized = true;
    }
    
    public async Task PlayAsync(float duration)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("CutTransition: Not initialized, skipping transition");
            return;
        }
        
        // Instant cut - just yield one frame
        await Task.Yield();
    }
    
    public void Cleanup()
    {
        _isInitialized = false;
    }
}
}
