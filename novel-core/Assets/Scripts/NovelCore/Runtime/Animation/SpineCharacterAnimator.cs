using System.Threading.Tasks;
using Spine.Unity;

namespace NovelCore.Runtime.Animation;

/// <summary>
/// Spine-based character animator using skeletal animation.
/// </summary>
public class SpineCharacterAnimator : ICharacterAnimator
{
    private readonly IAssetManager _assetManager;
    
    private CharacterData _characterData;
    private GameObject _targetObject;
    private SkeletonAnimation _skeletonAnimation;
    private string _currentEmotion;

    public AnimationType AnimationType => AnimationType.Spine;

    public SpineCharacterAnimator(IAssetManager assetManager)
    {
        _assetManager = assetManager ?? throw new System.ArgumentNullException(nameof(assetManager));
    }

    public void Initialize(CharacterData characterData, GameObject targetGameObject)
    {
        _characterData = characterData ?? throw new System.ArgumentNullException(nameof(characterData));
        _targetObject = targetGameObject ?? throw new System.ArgumentNullException(nameof(targetGameObject));

        // Load Spine skeleton data
        LoadSpineDataAsync();
    }

    private async void LoadSpineDataAsync()
    {
        if (_characterData.SpineDataAsset == null || !_characterData.SpineDataAsset.RuntimeKeyIsValid())
        {
            Debug.LogError("SpineCharacterAnimator: No Spine data asset specified");
            return;
        }

        try
        {
            var skeletonData = await _assetManager.LoadAssetAsync<SkeletonDataAsset>(_characterData.SpineDataAsset);
            if (skeletonData != null)
            {
                InitializeSpineComponent(skeletonData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SpineCharacterAnimator: Failed to load Spine data: {ex.Message}");
        }
    }

    private void InitializeSpineComponent(SkeletonDataAsset skeletonData)
    {
        // Get or add SkeletonAnimation component
        _skeletonAnimation = _targetObject.GetComponent<SkeletonAnimation>();
        if (_skeletonAnimation == null)
        {
            _skeletonAnimation = _targetObject.AddComponent<SkeletonAnimation>();
        }

        _skeletonAnimation.skeletonDataAsset = skeletonData;
        _skeletonAnimation.Initialize(true);

        // Set initial emotion
        _currentEmotion = _characterData.DefaultEmotion;
        SetEmotion(_currentEmotion);
    }

    public void SetEmotion(string emotionKey)
    {
        if (_characterData == null || _skeletonAnimation == null)
        {
            Debug.LogWarning("SpineCharacterAnimator: Not fully initialized yet");
            return;
        }

        var emotionData = _characterData.GetEmotion(emotionKey);
        if (emotionData == null)
        {
            Debug.LogWarning($"SpineCharacterAnimator: Emotion '{emotionKey}' not found for character {_characterData.CharacterName}");
            return;
        }

        _currentEmotion = emotionKey;

        // Set Spine skin if specified
        if (!string.IsNullOrEmpty(emotionData.Value.spineSkin))
        {
            _skeletonAnimation.skeleton.SetSkin(emotionData.Value.spineSkin);
            _skeletonAnimation.skeleton.SetSlotsToSetupPose();
        }

        // Play Spine animation if specified
        if (!string.IsNullOrEmpty(emotionData.Value.spineAnimation))
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, emotionData.Value.spineAnimation, true);
        }
    }

    public async Task PlayEntranceAsync(float duration, CharacterSide fromSide)
    {
        if (_targetObject == null)
        {
            return;
        }

        // Similar to Unity animator but with Spine
        Vector3 startPos = _targetObject.transform.position;
        Vector3 offscreenPos = startPos;

        float screenWidth = Camera.main ? Camera.main.orthographicSize * Camera.main.aspect * 2f : 10f;
        offscreenPos.x += fromSide == CharacterSide.Left ? -screenWidth : screenWidth;

        _targetObject.transform.position = offscreenPos;
        
        // Play entrance animation if available
        if (_skeletonAnimation != null)
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, "entrance", false);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f); // Ease-out
            
            _targetObject.transform.position = Vector3.Lerp(offscreenPos, startPos, t);
            await Task.Yield();
        }

        _targetObject.transform.position = startPos;
        
        // Return to idle/current emotion animation
        SetEmotion(_currentEmotion);
    }

    public async Task PlayExitAsync(float duration, CharacterSide toSide)
    {
        if (_targetObject == null)
        {
            return;
        }

        Vector3 startPos = _targetObject.transform.position;
        Vector3 offscreenPos = startPos;

        float screenWidth = Camera.main ? Camera.main.orthographicSize * Camera.main.aspect * 2f : 10f;
        offscreenPos.x += toSide == CharacterSide.Left ? -screenWidth : screenWidth;

        // Play exit animation if available
        if (_skeletonAnimation != null)
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, "exit", false);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.Pow(t, 3f); // Ease-in
            
            _targetObject.transform.position = Vector3.Lerp(startPos, offscreenPos, t);
            await Task.Yield();
        }

        _targetObject.transform.position = offscreenPos;
    }

    public void PlayIdle()
    {
        if (_skeletonAnimation == null)
        {
            return;
        }

        // Play idle animation
        _skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
    }

    public void PlayTalking()
    {
        if (_skeletonAnimation == null)
        {
            return;
        }

        // Play talking animation if available
        var trackEntry = _skeletonAnimation.AnimationState.SetAnimation(0, "talking", true);
        if (trackEntry == null)
        {
            // Fallback to idle with talking skin/emotion
            PlayIdle();
        }
    }

    public void StopTalking()
    {
        PlayIdle();
    }

    public void Update(float deltaTime)
    {
        // Spine handles its own updates via SkeletonAnimation component
    }

    public void Dispose()
    {
        // TODO: Unload Spine data via AssetManager
        _skeletonAnimation = null;
        _targetObject = null;
        _characterData = null;
    }
}
