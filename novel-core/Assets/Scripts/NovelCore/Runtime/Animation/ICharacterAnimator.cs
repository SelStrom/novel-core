using System;
using UnityEngine;
using System.Threading.Tasks;
using NovelCore.Runtime.Data.Characters;

namespace NovelCore.Runtime.Animation
{

/// <summary>
/// Interface for character animation systems.
/// Abstracts Unity Animator and Spine animation backends.
/// </summary>
public interface ICharacterAnimator
{
    /// <summary>
    /// Animation system type.
    /// </summary>
    AnimationType AnimationType { get; }

    /// <summary>
    /// Initialize the animator with character data.
    /// </summary>
    /// <param name="characterData">Character to animate.</param>
    /// <param name="targetGameObject">GameObject to attach animation components to.</param>
    void Initialize(CharacterData characterData, GameObject targetGameObject);

    /// <summary>
    /// Switch to a different emotion/sprite.
    /// </summary>
    /// <param name="emotionKey">Emotion to display.</param>
    void SetEmotion(string emotionKey);

    /// <summary>
    /// Play entrance animation.
    /// </summary>
    /// <param name="duration">Animation duration.</param>
    /// <param name="fromSide">Side to enter from (left/right).</param>
    System.Threading.Tasks.Task PlayEntranceAsync(float duration, CharacterSide fromSide);

    /// <summary>
    /// Play exit animation.
    /// </summary>
    /// <param name="duration">Animation duration.</param>
    /// <param name="toSide">Side to exit to (left/right).</param>
    System.Threading.Tasks.Task PlayExitAsync(float duration, CharacterSide toSide);

    /// <summary>
    /// Play idle animation/state.
    /// </summary>
    void PlayIdle();

    /// <summary>
    /// Play talking animation (if supported).
    /// </summary>
    void PlayTalking();

    /// <summary>
    /// Stop talking animation.
    /// </summary>
    void StopTalking();

    /// <summary>
    /// Update animator (called each frame).
    /// </summary>
    void Update(float deltaTime);

    /// <summary>
    /// Clean up animator resources.
    /// </summary>
    void Dispose();
}

/// <summary>
/// Character entrance/exit side.
/// </summary>
public enum CharacterSide
{
    Left,
    Right,
    Center
}

}