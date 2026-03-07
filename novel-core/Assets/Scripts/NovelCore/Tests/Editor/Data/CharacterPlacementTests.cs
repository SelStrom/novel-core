using NUnit.Framework;
using UnityEngine;
using NovelCore.Runtime.Data.Scenes;

namespace NovelCore.Tests.Editor.Data
{

/// <summary>
/// Unit tests for CharacterPlacement struct.
/// </summary>
public class CharacterPlacementTests : BaseTestFixture
{
    [Test]
    public void CharacterPlacement_CreatedWithValues_StoresCorrectly()
    {
        var placement = new CharacterPlacement
        {
            position = new Vector2(0.5f, 0.3f),
            initialEmotion = "happy",
            sortingOrder = 10
        };

        Assert.AreEqual(new Vector2(0.5f, 0.3f), placement.position);
        Assert.AreEqual("happy", placement.initialEmotion);
        Assert.AreEqual(10, placement.sortingOrder);
    }

    [Test]
    public void CharacterPlacement_DefaultValues_AreZero()
    {
        var placement = new CharacterPlacement();

        Assert.AreEqual(Vector2.zero, placement.position);
        Assert.IsNull(placement.initialEmotion);
        Assert.AreEqual(0, placement.sortingOrder);
    }

    [Test]
    public void CharacterPlacement_WithNormalizedPosition_IsValid()
    {
        var placement = new CharacterPlacement
        {
            position = new Vector2(1.0f, 1.0f),
            initialEmotion = "neutral",
            sortingOrder = 1
        };

        Assert.IsTrue(placement.position.x >= 0 && placement.position.x <= 1);
        Assert.IsTrue(placement.position.y >= 0 && placement.position.y <= 1);
    }

    [Test]
    public void CharacterPlacement_CanHaveNegativeSortingOrder()
    {
        var placement = new CharacterPlacement
        {
            position = Vector2.zero,
            initialEmotion = "neutral",
            sortingOrder = -5
        };

        Assert.AreEqual(-5, placement.sortingOrder);
    }
}

}
