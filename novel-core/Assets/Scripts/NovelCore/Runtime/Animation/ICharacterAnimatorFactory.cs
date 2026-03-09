using System;
using UnityEngine;
using NovelCore.Runtime.Data.Characters;

namespace NovelCore.Runtime.Animation
{

/// <summary>
/// Factory interface for creating character animators based on animation type.
/// </summary>
public interface ICharacterAnimatorFactory
{
    /// <summary>
    /// Creates a character animator based on the animation type.
    /// </summary>
    /// <param name="animationType">Type of animation system to use.</param>
    /// <returns>Character animator instance.</returns>
    ICharacterAnimator Create(AnimationType animationType);
}

}
