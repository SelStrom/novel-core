using System;
using UnityEngine;
using NovelCore.Runtime.Core.AssetManagement;
using NovelCore.Runtime.Data.Characters;

namespace NovelCore.Runtime.Animation
{

/// <summary>
/// Factory for creating character animators with dependency injection.
/// </summary>
public class CharacterAnimatorFactory : ICharacterAnimatorFactory
{
    private readonly IAssetManager _assetManager;

    public CharacterAnimatorFactory(IAssetManager assetManager)
    {
        _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
    }

    public ICharacterAnimator Create(AnimationType animationType)
    {
        return animationType switch
        {
            AnimationType.Spine => new SpineCharacterAnimator(_assetManager),
            AnimationType.Unity => new UnityCharacterAnimator(_assetManager),
            AnimationType.Static => new UnityCharacterAnimator(_assetManager),
            _ => new UnityCharacterAnimator(_assetManager)
        };
    }
}

}
