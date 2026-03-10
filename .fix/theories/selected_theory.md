# Selected Theory

## Decision

**Selected**: IMPL_THEORY

## Reasoning

### Theory Comparison

| Aspect                | Spec Theory      | Impl Theory      |
|-----------------------|------------------|------------------|
| Confidence Score      | 30%              | 95%              |
| Evidence Quality      | MEDIUM           | HIGH             |
| Failing Test Alignment| WEAK             | STRONG           |
| Constitution Alignment| ALIGNED          | ALIGNED          |
| Code Location         | N/A              | Precise (lines 351-378) |
| Root Cause Clarity    | Ambiguous        | Clear (missing check) |

### Decision Rationale

#### 1. Evidence Strength

**Impl Theory** предоставляет:
- ✅ Точную локацию бага (строки 351-378)
- ✅ Детальный анализ проблемного кода
- ✅ Эталонную реализацию в том же файле (`SelectChoice()`)
- ✅ Прямое соответствие stack trace из тестов
- ✅ Концептуальный патч

**Spec Theory** показывает:
- ⚠️ Спецификация содержит правильные требования
- ⚠️ Но не детализирует runtime поведение
- ⚠️ Нет противоречий в spec → проблема не в спецификации

#### 2. Test Alignment

**Failing Test Explicitly States**:
> "The fix checks `RuntimeKeyIsValid()` **BEFORE** calling `LoadAssetAsync`"  
> (EndOfStoryReproductionTests.cs:147-148)

Тесты ожидают:
- `LogType.Warning` с regex "NextScene AssetReference is invalid"
- Вызов `OnDialogueComplete` (не `OnSceneNavigationRequested`)
- `IsPlaying == false`

**Impl Theory** объясняет **все симптомы**:
- Отсутствие `RuntimeKeyIsValid()` check → попытка загрузки
- Загрузка возвращает `null` → ERROR лог (строка 368)
- ERROR != ожидаемый WARNING → тест падает

**Spec Theory** не объясняет точные симптомы - только общие требования.

#### 3. Root Cause Coverage

**Impl Theory** покрывает:
- ✅ Почему тесты падают (ERROR вместо WARNING)
- ✅ Как исправить (добавить `RuntimeKeyIsValid()` check)
- ✅ Где исправить (строка 353, после `if (targetSceneRef != null)`)
- ✅ Эталон правильной реализации (`SelectChoice()`, строки 155-156)

**Spec Theory**:
- ⚠️ Указывает на общее требование валидации
- ❌ Не объясняет конкретный механизм бага
- ❌ Не предоставляет решение

### Rejected Theory

**Rejected**: SPEC_THEORY

**Reason for Rejection**:

1. **Спецификация корректна**: Требования к обработке broken links и missing assets описаны в spec.md:95-98
2. **Нет противоречий**: Spec, data-model и tests согласованы
3. **Проблема в имплементации**: Код не реализует требования spec и ожидания tests
4. **Low confidence (30%)**: Сама SpecTheory признаёт, что проблема не в спецификации

**Цитата из SpecTheory**:
> "Низкая уверенность в SpecTheory, потому что:  
> - Спецификация описывает правильное поведение на высоком уровне  
> - Отсутствие детализации runtime validation ≠ некорректная спецификация  
> - **Тесты правильно интерпретируют требования spec**"

## Confidence in Decision

**95%**

### Justification

Высокая уверенность в выборе IMPL_THEORY, потому что:

1. **Детерминированное воспроизведение**: 2 из 3 тестов падают стабильно
2. **Прямое соответствие кода и симптомов**: строка 368 логирует ERROR, который вызывает падение теста
3. **Эталон правильной реализации существует**: `SelectChoice()` показывает, как должен выглядеть код
4. **Тесты документируют ожидаемый фикс**: "checks RuntimeKeyIsValid() BEFORE calling LoadAssetAsync"
5. **Confidence score разница**: 95% (Impl) vs 30% (Spec)

Остаётся 5% сомнений на случай, если:
- Есть скрытые архитектурные требования к попытке загрузки invalid assets
- Тесты написаны неправильно (но они соответствуют spec)

## Recommended Fix Strategy

**Strategy**: Добавить валидацию `RuntimeKeyIsValid()` перед `LoadAssetAsync()` в `CompleteDialogue()`

### Implementation Steps

1. **Добавить проверку** после `if (targetSceneRef != null)` (строка ~353):
   ```csharp
   if (!targetSceneRef.RuntimeKeyIsValid())
   {
       Debug.LogWarning($"DialogueSystem: NextScene AssetReference is invalid (RuntimeKey not valid). Completing dialogue.");
       _isPlaying = false;
       OnDialogueComplete?.Invoke();
       return;
   }
   ```

2. **Сохранить ERROR handling** для случая, когда RuntimeKey валидный, но загрузка не удалась:
   ```csharp
   if (nextScene != null) { ... }
   else {
       Debug.LogError($"DialogueSystem: ✗ Failed to load target scene (returned null)");
       // ...
   }
   ```

3. **Запустить тесты** для проверки:
   - `EndOfStory_WithEmptyNextSceneReference_ShouldCompleteGracefullyWithoutException` → должен пройти
   - `EndOfStory_WithInvalidAssetReference_ShouldValidateBeforeLoad` → должен пройти
   - `EndOfStory_WithNullNextScene_ShouldCompleteGracefully` → должен остаться passing

### Files to Modify

- `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs` (5 строк добавлено)

### Expected Test Results

**Before fix**:
- Total: 3 tests
- Passed: 1
- Failed: 2

**After fix**:
- Total: 3 tests
- Passed: 3
- Failed: 0

### Constitution Compliance

- ✅ **Principle VI (Testing)**: Tests уже существуют, фикс сделает их passing
- ✅ **Code Style Standards**: Allman braces, underscore для private fields
- ✅ **AI Development Constraints**: Только .cs файлы изменены

## Next Steps

1. Применить патч к `DialogueSystem.cs`
2. Скомпилировать (Unity batch mode)
3. Запустить `EndOfStoryReproductionTests` (PlayMode)
4. Проверить, что все 3 теста проходят
5. Запустить полный test suite для проверки регрессий
6. Создать break tests (optional)
7. Коммит с сообщением согласно convention
