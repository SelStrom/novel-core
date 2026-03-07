using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using NovelCore.Runtime.Core.AssetManagement;
using NovelCore.Runtime.Data.Characters;

namespace NovelCore.Runtime.Animation
{

/// <summary>
/// Unity-based character animator using sprite swapping.
/// </summary>
public class UnityCharacterAnimator : ICharacterAnimator
{
    private readonly IAssetManager _assetManager;
    
    private CharacterData _characterData;
    private GameObject _targetObject;
    private SpriteRenderer _spriteRenderer;
    private string _currentEmotion;
    
    // Animation state
    private bool _isTalking;
    private float _talkingAnimTimer;
    private const float TalkingAnimSpeed = 0.2f; // Oscillation speed

    public AnimationType AnimationType => AnimationType.Unity;

    public UnityCharacterAnimator(IAssetManager assetManager)
    {
        _assetManager = assetManager ?? throw new System.ArgumentNullException(nameof(assetManager));
    }

    public void Initialize(CharacterData characterData, GameObject targetGameObject)
    {
        _characterData = characterData ?? throw new System.ArgumentNullException(nameof(characterData));
        _targetObject = targetGameObject ?? throw new System.ArgumentNullException(nameof(targetGameObject));

        // Get or add SpriteRenderer
        _spriteRenderer = _targetObject.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            _spriteRenderer = _targetObject.AddComponent<SpriteRenderer>();
        }

        // Set initial emotion
        _currentEmotion = characterData.DefaultEmotion;
        SetEmotion(_currentEmotion);
    }

    public void SetEmotion(string emotionKey)
    {
        if (_characterData == null || _spriteRenderer == null)
        {
            Debug.LogError("UnityCharacterAnimator: Not initialized");
            return;
        }

        var emotionData = _characterData.GetEmotion(emotionKey);
        if (emotionData == null)
        {
            Debug.LogWarning($"UnityCharacterAnimator: Emotion '{emotionKey}' not found for character {_characterData.CharacterName}");
            return;
        }

        _currentEmotion = emotionKey;

        // Load sprite async
        LoadEmotionSpriteAsync(emotionData.Value);
    }

    private async void LoadEmotionSpriteAsync(CharacterEmotion emotion)
    {
        if (emotion.sprite == null || !emotion.sprite.RuntimeKeyIsValid())
        {
            Debug.LogWarning($"UnityCharacterAnimator: No sprite for emotion {emotion.emotionName}");
            return;
        }

        try
        {
            var sprite = await _assetManager.LoadAssetAsync<Sprite>(emotion.sprite);
            if (sprite != null && _spriteRenderer != null)
            {
                _spriteRenderer.sprite = sprite;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"UnityCharacterAnimator: Failed to load sprite: {ex.Message}");
        }
    }

    public async Task PlayEntranceAsync(float duration, CharacterSide fromSide)
    {
        if (_targetObject == null)
        {
            return;
        }

        Vector3 startPos = _targetObject.transform.position;
        Vector3 offscreenPos = startPos;

        // Calculate offscreen position based on side
        float screenWidth = Camera.main ? Camera.main.orthographicSize * Camera.main.aspect * 2f : 10f;
        offscreenPos.x += fromSide == CharacterSide.Left ? -screenWidth : screenWidth;

        // Animate from offscreen to target position
        _targetObject.transform.position = offscreenPos;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Ease-out interpolation
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            _targetObject.transform.position = Vector3.Lerp(offscreenPos, startPos, t);
            await Task.Yield();
        }

        _targetObject.transform.position = startPos;
    }

    public async Task PlayExitAsync(float duration, CharacterSide toSide)
    {
        if (_targetObject == null)
        {
            return;
        }

        Vector3 startPos = _targetObject.transform.position;
        Vector3 offscreenPos = startPos;

        // Calculate offscreen position based on side
        float screenWidth = Camera.main ? Camera.main.orthographicSize * Camera.main.aspect * 2f : 10f;
        offscreenPos.x += toSide == CharacterSide.Left ? -screenWidth : screenWidth;

        // Animate from current position to offscreen
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Ease-in interpolation
            t = Mathf.Pow(t, 3f);
            
            _targetObject.transform.position = Vector3.Lerp(startPos, offscreenPos, t);
            await Task.Yield();
        }

        _targetObject.transform.position = offscreenPos;
    }

    public void PlayIdle()
    {
        _isTalking = false;
        
        // Reset any transformations
        if (_targetObject != null)
        {
            _targetObject.transform.localScale = Vector3.one * (_characterData?.DefaultScale.x ?? 1f);
        }
    }

    public void PlayTalking()
    {
        _isTalking = true;
        _talkingAnimTimer = 0f;
    }

    public void StopTalking()
    {
        _isTalking = false;
        PlayIdle();
    }

    public void Update(float deltaTime)
    {
        if (_isTalking && _targetObject != null)
        {
            // Simple talking animation - slight bobbing/scaling
            _talkingAnimTimer += deltaTime * TalkingAnimSpeed;
            float scale = 1f + Mathf.Sin(_talkingAnimTimer * Mathf.PI * 2f) * 0.02f;
            
            Vector2 baseScale = _characterData?.DefaultScale ?? Vector2.one;
            _targetObject.transform.localScale = new Vector3(baseScale.x * scale, baseScale.y * scale, 1f);
        }
    }

    public void Dispose()
    {
        // TODO: Unload sprites via AssetManager
        _spriteRenderer = null;
        _targetObject = null;
        _characterData = null;
    }
}

}