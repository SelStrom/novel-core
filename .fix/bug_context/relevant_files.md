# Relevant Files

## Primary Files (directly affected)

- `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`
  - Location: `NovelCore.Runtime.Core.DialogueSystem.DialogueSystem.CompleteDialogue()`, lines 296-387
  - Reason: Метод `CompleteDialogue()` не валидирует `AssetReference` перед загрузкой через `LoadAssetAsync`

## Secondary Files (dependencies)

- `Assets/Scripts/NovelCore/Tests/Runtime/Core/DialogueSystem/EndOfStoryReproductionTests.cs`
  - Location: Test class с падающими тестами
  - Reason: Содержит регрессионные тесты для валидации обработки невалидных `AssetReference`

- `Assets/Scripts/NovelCore/Tests/Runtime/Builders/TestDataBuilders.cs`
  - Location: `SceneDataBuilder`
  - Reason: Используется для создания тестовых `SceneData` с невалидными `AssetReference`

- `Assets/Scripts/NovelCore/Tests/Runtime/Core/MockImplementations.cs`
  - Location: `MockAssetManager`
  - Reason: Mock implementation возвращает `null` для невалидных `AssetReference`

## Related Code Pattern

В `SelectChoice()` (DialogueSystem.cs:155) уже есть правильная проверка:
```csharp
if (selectedOption.targetScene != null && selectedOption.targetScene.RuntimeKeyIsValid())
{
    // Load the scene
}
```

Эта же проверка отсутствует в `CompleteDialogue()` для `targetSceneRef`.
