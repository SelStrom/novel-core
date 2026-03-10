# Bug Description

Тесты `EndOfStoryReproductionTests` падают из-за неожиданных сообщений об ошибках при обработке невалидных `AssetReference`.

## Expected Behavior

При встрече с невалидным (empty или invalid GUID) `AssetReference` в `NextScene`:
1. Система должна логировать **WARNING** (не ERROR)
2. Тесты ожидают `LogAssert.Expect(LogType.Warning, ...)` для сообщения "NextScene AssetReference is invalid"
3. Диалоговая система должна gracefully завершиться через `OnDialogueComplete`, а не пытаться загрузить сцену

## Actual Behavior

1. Система пытается загрузить невалидный `AssetReference` через `LoadAssetAsync`
2. Загрузка возвращает `null`
3. Логируется **ERROR**: "DialogueSystem: ✗ Failed to load target scene (returned null)"
4. Тесты падают из-за неожиданного ERROR лога

## Steps to Reproduce

1. Создать `SceneData` с невалидным `AssetReference` в `NextScene` (пустой GUID или несуществующий)
2. Запустить сцену через `DialogueSystem.StartScene()`
3. Продвинуть диалог до конца через `AdvanceDialogue()`
4. Система пытается загрузить невалидный `AssetReference`
5. Получаем ERROR вместо WARNING

## Failing Tests

- `EndOfStory_WithEmptyNextSceneReference_ShouldCompleteGracefullyWithoutException` - FAILED
- `EndOfStory_WithInvalidAssetReference_ShouldValidateBeforeLoad` - FAILED
- `EndOfStory_WithNullNextScene_ShouldCompleteGracefully` - PASSED

## Environment

- Unity Version: 6000.0.69f1
- Platform: macOS
- NovelCore Version: 001-visual-novel-constructor
- Test Platform: PlayMode

## Root Cause Hypothesis

В методе `CompleteDialogue()` (DialogueSystem.cs:296-387):
- Не проверяется валидность `AssetReference` перед вызовом `LoadAssetAsync`
- Отсутствует проверка `RuntimeKeyIsValid()` для `targetSceneRef`
- Код пытается загрузить невалидный asset, что возвращает null и логирует ERROR
