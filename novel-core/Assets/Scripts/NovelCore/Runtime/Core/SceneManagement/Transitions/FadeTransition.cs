using System.Threading.Tasks;
using UnityEngine;

namespace NovelCore.Runtime.Core.SceneManagement.Transitions
{
    /// <summary>
    /// Fade to black transition effect.
    /// </summary>
    public class FadeTransition : ISceneTransition
    {
    private Camera _camera;
    private GameObject _fadeOverlay;
    private SpriteRenderer _fadeRenderer;
    private bool _isInitialized;
    
    public void Initialize(Camera camera)
    {
        _camera = camera ?? Camera.main;
        
        if (_camera == null)
        {
            Debug.LogError("FadeTransition: No camera available for transition");
            return;
        }
        
        // Create fade overlay
        _fadeOverlay = new GameObject("FadeOverlay");
        _fadeRenderer = _fadeOverlay.AddComponent<SpriteRenderer>();
        
        // Create a white texture for fading
        Texture2D fadeTexture = new Texture2D(1, 1);
        fadeTexture.SetPixel(0, 0, Color.black);
        fadeTexture.Apply();
        
        Sprite fadeSprite = Sprite.Create(
            fadeTexture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
        
        _fadeRenderer.sprite = fadeSprite;
        _fadeRenderer.sortingOrder = 10000; // Very high to render on top
        
        // Scale to cover entire screen
        float cameraHeight = _camera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * _camera.aspect;
        _fadeOverlay.transform.localScale = new Vector3(cameraWidth * 2f, cameraHeight * 2f, 1f);
        _fadeOverlay.transform.position = _camera.transform.position + Vector3.forward * (_camera.nearClipPlane + 0.1f);
        
        // Start invisible
        Color color = _fadeRenderer.color;
        color.a = 0f;
        _fadeRenderer.color = color;
        
        _isInitialized = true;
    }
    
    public async Task PlayAsync(float duration)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("FadeTransition: Not initialized, skipping transition");
            return;
        }
        
        // Fade out (to black)
        await FadeToAlpha(1f, duration / 2f);
        
        // Hold for a frame
        await Task.Yield();
        
        // Fade in (from black)
        await FadeToAlpha(0f, duration / 2f);
    }
    
    private async Task FadeToAlpha(float targetAlpha, float duration)
    {
        if (_fadeRenderer == null)
            return;
        
        Color startColor = _fadeRenderer.color;
        float startAlpha = startColor.a;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            _fadeRenderer.color = newColor;
            
            await Task.Yield();
        }
        
        // Ensure final alpha is set
        Color finalColor = startColor;
        finalColor.a = targetAlpha;
        _fadeRenderer.color = finalColor;
    }
    
    public void Cleanup()
    {
        if (_fadeOverlay != null)
        {
            GameObject.Destroy(_fadeOverlay);
            _fadeOverlay = null;
            _fadeRenderer = null;
        }
        
        _isInitialized = false;
    }
}
}
