# Bug Description

При клике на последнем диалоге сцены, когда нет следующей сцены (`NextScene` == null), система пытается загрузить `SceneData` с пустым или невалидным ключом через Addressables, что приводит к `InvalidKeyException`.

Вместо исключения должно выводиться немодальное окно-заглушку о том, что сюжет закончился.

## Expected Behavior

1. При достижении последнего диалога, если:
   - Нет `NextScene` (null)
   - Нет активных `TransitionRules`
   - Нет `Choices`
2. Должно выводиться UI-уведомление "Сюжет завершён" (немодальное окно)
3. Не должно быть исключений Addressables
4. Диалоговая система должна корректно завершить работу

## Actual Behavior

1. DialogueSystem.CompleteDialogue() вызывается при завершении последнего диалога
2. Проверяются transition rules (нет совпадений)
3. Проверяется `_currentScene.NextScene` (null или невалидная ссылка)
4. Код передает null/пустую AssetReference в `_assetManager.LoadAssetAsync<SceneData>(targetSceneRef)`
5. AddressablesAssetManager получает null или невалидный ключ
6. Addressables.LoadAssetAsync() выбрасывает `InvalidKeyException`:
   ```
   UnityEngine.AddressableAssets.InvalidKeyException: Exception of type 'UnityEngine.AddressableAssets.InvalidKeyException' was thrown. 
   No MergeMode is set to merge the multiple keys requested. Keys=, Type=NovelCore.Runtime.Data.Scenes.SceneData
   ```
7. Exception логируется, но не обрабатывается gracefully
8. Пользователь видит только скрытие диалога без уведомления о завершении

## Steps to Reproduce

1. Создать SceneData с диалогами
2. Установить `NextScene` = null (или оставить пустым)
3. Не устанавливать transition rules или choices
4. Запустить сцену и кликнуть до последнего диалога
5. Кликнуть на последнем диалоге
6. Наблюдать InvalidKeyException в консоли

## Environment

- Unity Version: 6000.0.69f1 (Unity 6 LTS)
- Platform: macOS (darwin 25.2.0)
- NovelCore Version: development build

## Root Cause Analysis

### Location
- File: `DialogueSystem.cs`
- Method: `CompleteDialogue()` (async void)
- Lines: 338-372

### Issue
Код в строках 338-342:
```csharp
if (targetSceneRef == null && _currentScene.NextScene != null)
{
    Debug.Log($"DialogueSystem: Using nextScene for linear progression");
    targetSceneRef = _currentScene.NextScene;
}
```

Проблема: `_currentScene.NextScene` может быть non-null, но **невалидной** AssetReference (пустая).

Далее (строки 345-372):
```csharp
if (targetSceneRef != null)
{
    var nextScene = await _assetManager.LoadAssetAsync<SceneData>(targetSceneRef);
    // ...
}
```

Проблема: Проверка `!= null` не достаточна для AssetReference - нужна проверка `RuntimeKeyIsValid()`.

## Secondary Issues

1. **Отсутствует UI для уведомления о завершении сюжета**
   - Нет существующего UI компонента для отображения "End of Story"
   - `OnDialogueComplete` event вызывается, но нет слушателей для этого кейса

2. **Отсутствует валидация AssetReference перед загрузкой**
   - В AddressablesAssetManager нет проверки на валидность ключа
   - В DialogueSystem нет проверки `RuntimeKeyIsValid()` перед вызовом LoadAssetAsync
