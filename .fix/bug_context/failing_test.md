# Failing Test

Тест уже существует в: `Assets/Scripts/NovelCore/Tests/Runtime/Core/DialogueSystem/EndOfStoryReproductionTests.cs`

## Test 1: EndOfStory_WithEmptyNextSceneReference_ShouldCompleteGracefullyWithoutException

```csharp
[UnityTest]
public IEnumerator EndOfStory_WithEmptyNextSceneReference_ShouldCompleteGracefullyWithoutException()
{
    // Arrange: Create final scene with empty (but non-null) AssetReference
    var emptySceneRef = new UnityEngine.AddressableAssets.AssetReference("");
    
    var finalSceneData = new SceneDataBuilder()
        .WithSceneId("scene_final")
        .WithSceneName("Final Scene")
        .WithDialogueLine(new DialogueLineDataBuilder()
            .WithLineId("line_final")
            .WithFallbackText("The end")
            .Build())
        .WithNextScene(emptySceneRef)
        .Build();

    bool dialogueCompleteFired = false;
    bool navigationFired = false;
    
    _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;
    _dialogueSystem.OnSceneNavigationRequested += (scene) => navigationFired = true;

    // Act: Start scene and advance to last dialogue
    _dialogueSystem.StartScene(finalSceneData);
    yield return null;

    // Expect warning log (not error)
    LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("NextScene AssetReference is invalid"));
    
    _dialogueSystem.AdvanceDialogue();
    yield return new WaitForSeconds(0.2f);

    // Assert: Should complete gracefully without InvalidKeyException
    Assert.IsTrue(dialogueCompleteFired, 
        "OnDialogueComplete should fire when reaching end of story with invalid NextScene");
    Assert.IsFalse(navigationFired, 
        "OnSceneNavigationRequested should NOT fire with invalid next scene");
    Assert.IsFalse(_dialogueSystem.IsPlaying, 
        "Dialogue should stop playing at end of story");
}
```

**Current Result**: ❌ FAILED
- Expected: `LogType.Warning` with message "NextScene AssetReference is invalid"
- Actual: `LogType.Error` with message "DialogueSystem: ✗ Failed to load target scene (returned null)"

## Test 2: EndOfStory_WithInvalidAssetReference_ShouldValidateBeforeLoad

```csharp
[UnityTest]
public IEnumerator EndOfStory_WithInvalidAssetReference_ShouldValidateBeforeLoad()
{
    // Arrange: Create scene with AssetReference that has invalid GUID
    var invalidRef = new UnityEngine.AddressableAssets.AssetReference("some_invalid_guid");
    
    var sceneData = new SceneDataBuilder()
        .WithSceneId("scene_invalid_ref")
        .WithSceneName("Scene Invalid Ref")
        .WithDialogueLine(new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test")
            .Build())
        .WithNextScene(invalidRef)
        .Build();

    bool dialogueCompleteFired = false;
    _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;

    // Act
    _dialogueSystem.StartScene(sceneData);
    yield return null;

    // The fix checks RuntimeKeyIsValid() BEFORE calling LoadAssetAsync
    // This prevents InvalidKeyException from being thrown
    LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("NextScene AssetReference is invalid"));
    
    _dialogueSystem.AdvanceDialogue();
    yield return new WaitForSeconds(0.2f);

    // Assert: Should complete gracefully
    Assert.IsTrue(dialogueCompleteFired, 
        "Should complete dialogue when AssetReference is invalid");
    Assert.IsFalse(_dialogueSystem.IsPlaying);
}
```

**Current Result**: ❌ FAILED
- Expected: `LogType.Warning` with message "NextScene AssetReference is invalid"
- Actual: `LogType.Error` with message "DialogueSystem: ✗ Failed to load target scene (returned null)"

## Test 3: EndOfStory_WithNullNextScene_ShouldCompleteGracefully

```csharp
[UnityTest]
public IEnumerator EndOfStory_WithNullNextScene_ShouldCompleteGracefully()
{
    // Arrange: Create final scene with null NextScene
    var finalSceneData = new SceneDataBuilder()
        .WithSceneId("scene_final_null")
        .WithSceneName("Final Scene Null")
        .WithDialogueLine(new DialogueLineDataBuilder()
            .WithLineId("line_final_null")
            .WithFallbackText("The end (null nextScene)")
            .Build())
        .Build();

    bool dialogueCompleteFired = false;
    _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;

    // Act
    _dialogueSystem.StartScene(finalSceneData);
    yield return null;

    _dialogueSystem.AdvanceDialogue();
    yield return new WaitForSeconds(0.1f);

    // Assert
    Assert.IsTrue(dialogueCompleteFired, 
        "OnDialogueComplete should fire when NextScene is null");
    Assert.IsFalse(_dialogueSystem.IsPlaying);
}
```

**Current Result**: ✅ PASSED
- Null `NextScene` обрабатывается корректно (строка 345-349 в DialogueSystem.cs)

## Summary

2 из 3 тестов падают из-за отсутствия валидации `RuntimeKeyIsValid()` перед попыткой загрузки asset.
