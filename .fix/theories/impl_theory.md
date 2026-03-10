# Implementation Theory

## Hypothesis

Баг вызван **отсутствием проверки `RuntimeKeyIsValid()`** перед вызовом `LoadAssetAsync()` в методе `CompleteDialogue()`.

## Location

### Primary Location

- **File**: `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`
- **Namespace**: `NovelCore.Runtime.Core.DialogueSystem`
- **Class**: `DialogueSystem`
- **Method**: `CompleteDialogue()`
- **Line Range**: 351-378 (блок загрузки targetScene)

### Secondary Locations

- **File**: `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`
- **Method**: `SelectChoice()`
- **Line Range**: 155-156
- **Reason**: Содержит **правильную реализацию** проверки AssetReference перед загрузкой (эталон для CompleteDialogue)

## Root Cause Hypothesis

### Category

- [x] Logical error (missing validation check)
- [ ] Edge case not handled
- [ ] Race condition
- [ ] State management error
- [ ] Memory/resource leak
- [ ] API misuse
- [ ] Other

### Detailed Explanation

В методе `CompleteDialogue()` (строки 296-387), когда определён `targetSceneRef`, код **напрямую пытается загрузить asset** без предварительной проверки валидности:

```csharp
// Строки 351-378 (ПРОБЛЕМНЫЙ КОД)
if (targetSceneRef != null)
{
    Debug.Log($"DialogueSystem: Loading target scene...");
    
    try
    {
        // ❌ ПРОБЛЕМА: нет проверки RuntimeKeyIsValid()
        var nextScene = await _assetManager.LoadAssetAsync<SceneData>(targetSceneRef);
        
        if (nextScene != null)
        {
            Debug.Log($"DialogueSystem: ✓ Successfully loaded next scene: {nextScene.SceneName}");
            _isPlaying = false;
            OnSceneNavigationRequested?.Invoke(nextScene);
        }
        else
        {
            // ❌ ERROR вместо WARNING
            Debug.LogError($"DialogueSystem: ✗ Failed to load target scene (returned null)");
            _isPlaying = false;
            OnDialogueComplete?.Invoke();
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"DialogueSystem: ✗ Exception loading target scene: {ex.Message}");
        _isPlaying = false;
        OnDialogueComplete?.Invoke();
    }
}
```

**Проблемы**:
1. Отсутствует проверка `targetSceneRef.RuntimeKeyIsValid()` перед `LoadAssetAsync()`
2. Когда `LoadAssetAsync()` возвращает `null` для невалидного reference, логируется **ERROR** вместо **WARNING**
3. Нет раннего выхода для invalid references

### Correct Implementation (из SelectChoice)

В методе `SelectChoice()` (строки 154-183) **правильная реализация** уже существует:

```csharp
// Строки 155-156 (ПРАВИЛЬНЫЙ КОД)
if (selectedOption.targetScene != null && selectedOption.targetScene.RuntimeKeyIsValid())
{
    try
    {
        var targetScene = await _assetManager.LoadAssetAsync<SceneData>(selectedOption.targetScene);
        // ...
    }
    // ...
}
```

**Этот паттерн уже используется в коде**, но не применён в `CompleteDialogue()`.

### Call Stack Analysis

```
DialogueSystem.AdvanceDialogue()
  ↓
DialogueSystem.CompleteDialogue() (async)
  ↓
_assetManager.LoadAssetAsync<SceneData>(targetSceneRef) ← БАГ ЗДЕСЬ
  ↓
MockAssetManager.LoadAssetAsync() (в тестах)
  ↓
return Task.FromResult<T>(null) (для invalid AssetReference)
```

**Analysis**: 
- `targetSceneRef` создаётся с пустым GUID или invalid GUID в тестах
- `MockAssetManager` корректно возвращает `null` для невалидных references
- `CompleteDialogue()` получает `null`, логирует ERROR
- Тесты ожидают WARNING **до** вызова `LoadAssetAsync()`

## Git History Insights

### Recent Changes

Баг был введён при реализации conditional transitions (TransitionRules):

**Анализ метода CompleteDialogue()**:
- Строки 316-342: обработка TransitionRules (conditional logic)
- Строки 344-349: fallback на linear nextScene
- Строки 351-378: загрузка targetSceneRef

**Suspicion Level**: HIGH

**Reason**: При добавлении conditional transitions был введён общий `targetSceneRef` для унификации логики. Проверка `RuntimeKeyIsValid()` была пропущена при рефакторинге.

## Edge Cases Analysis

### Edge Case 1: Empty AssetReference

- **Description**: `new AssetReference("")` (пустой GUID)
- **Current handling**: `RuntimeKeyIsValid()` возвращает `false`, но проверка отсутствует
- **Problem**: Система пытается загрузить, получает null, логирует ERROR

### Edge Case 2: Invalid GUID AssetReference

- **Description**: `new AssetReference("some_invalid_guid")`
- **Current handling**: `RuntimeKeyIsValid()` возвращает `false`, но проверка отсутствует
- **Problem**: Система пытается загрузить, получает null, логирует ERROR

### Edge Case 3: Null AssetReference

- **Description**: `targetSceneRef == null`
- **Current handling**: ✅ Корректно обрабатывается через `if (targetSceneRef != null)` check (строка 352)
- **Problem**: Нет проблемы

## Confidence Score

**95%**

### Reasoning

1. **Явная проблема в коде**: отсутствие `RuntimeKeyIsValid()` check
2. **Эталон существует**: `SelectChoice()` показывает правильную реализацию
3. **Детерминированное воспроизведение**: тесты стабильно падают
4. **Прямое соответствие симптомам**: ERROR лог совпадает с кодом (строка 368)
5. **Тесты явно документируют ожидаемое поведение**: "The fix checks RuntimeKeyIsValid() BEFORE calling LoadAssetAsync"

Остаётся 5% сомнений на случай, если есть архитектурное решение, требующее попытки загрузки для других целей (например, метрики), но тесты этого не предполагают.

## Recommended Fix Approach

### Strategy

Добавить валидацию `RuntimeKeyIsValid()` **ПЕРЕД** попыткой загрузки, по аналогии с `SelectChoice()`.

### Proposed Fix (Conceptual)

```csharp
// Строки 351-387 (ИСПРАВЛЕННЫЙ КОД)
if (targetSceneRef != null)
{
    // ✅ НОВАЯ ПРОВЕРКА: валидация перед загрузкой
    if (!targetSceneRef.RuntimeKeyIsValid())
    {
        // ✅ WARNING вместо ERROR
        Debug.LogWarning($"DialogueSystem: NextScene AssetReference is invalid (RuntimeKey not valid). Completing dialogue.");
        _isPlaying = false;
        OnDialogueComplete?.Invoke();
        return; // ✅ Ранний выход
    }
    
    Debug.Log($"DialogueSystem: Loading target scene...");
    
    try
    {
        var nextScene = await _assetManager.LoadAssetAsync<SceneData>(targetSceneRef);
        
        if (nextScene != null)
        {
            Debug.Log($"DialogueSystem: ✓ Successfully loaded next scene: {nextScene.SceneName}");
            _isPlaying = false;
            OnSceneNavigationRequested?.Invoke(nextScene);
        }
        else
        {
            // ERROR остаётся для случая, когда RuntimeKey валидный, но загрузка всё равно не удалась
            Debug.LogError($"DialogueSystem: ✗ Failed to load target scene (returned null)");
            _isPlaying = false;
            OnDialogueComplete?.Invoke();
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"DialogueSystem: ✗ Exception loading target scene: {ex.Message}");
        _isPlaying = false;
        OnDialogueComplete?.Invoke();
    }
}
else
{
    // Null targetSceneRef - normal end of story
    Debug.Log("DialogueSystem: No target scene determined, dialogue ending normally");
    _isPlaying = false;
    OnDialogueComplete?.Invoke();
}
```

### Changes Summary

1. **Line ~353**: Добавить проверку `!targetSceneRef.RuntimeKeyIsValid()`
2. **Line ~355**: Логировать `Debug.LogWarning()` с сообщением "NextScene AssetReference is invalid"
3. **Line ~356-357**: Установить `_isPlaying = false`, вызвать `OnDialogueComplete`, ранний `return`
4. **Existing error handling**: Оставить для случаев, когда RuntimeKey валидный, но загрузка не удалась

### Impact

- **Tests affected**: 2 падающих теста начнут проходить
- **Lines changed**: +5 lines
- **Behavioral change**: Система не будет пытаться загрузить invalid AssetReference
- **Log changes**: WARNING вместо ERROR для invalid references
