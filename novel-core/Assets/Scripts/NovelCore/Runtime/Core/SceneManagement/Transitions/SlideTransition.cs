using System.Threading.Tasks;
using UnityEngine;

namespace NovelCore.Runtime.Core.SceneManagement.Transitions
{
    /// <summary>
    /// Slide transition effect (slides old scene out, new scene in).
    /// </summary>
    public class SlideTransition : ISceneTransition
    {
    private Camera _camera;
    private GameObject _transitionContainer;
    private bool _isInitialized;
    
    private Vector3 _originalCameraPosition;
    
    public void Initialize(Camera camera)
    {
        _camera = camera ?? Camera.main;
        
        if (_camera == null)
        {
            Debug.LogError("SlideTransition: No camera available for transition");
            return;
        }
        
        _transitionContainer = new GameObject("SlideTransitionContainer");
        _originalCameraPosition = _camera.transform.position;
        
        _isInitialized = true;
    }
    
    public async Task PlayAsync(float duration)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("SlideTransition: Not initialized, skipping transition");
            return;
        }
        
        // Slide camera to the right
        Vector3 targetPosition = _originalCameraPosition + Vector3.right * (_camera.orthographicSize * _camera.aspect * 2f);
        
        await SlideCamera(targetPosition, duration / 2f);
        
        // Reset camera position instantly
        _camera.transform.position = _originalCameraPosition + Vector3.left * (_camera.orthographicSize * _camera.aspect * 2f);
        
        // Slide back to original position
        await SlideCamera(_originalCameraPosition, duration / 2f);
    }
    
    private async Task SlideCamera(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = _camera.transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            // Smooth ease in-out
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            _camera.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);
            
            await Task.Yield();
        }
        
        // Ensure final position is set
        _camera.transform.position = targetPosition;
    }
    
    public void Cleanup()
    {
        if (_transitionContainer != null)
        {
            GameObject.Destroy(_transitionContainer);
            _transitionContainer = null;
        }
        
        // Reset camera position
        if (_camera != null)
        {
            _camera.transform.position = _originalCameraPosition;
        }
        
        _isInitialized = false;
    }
}
}
