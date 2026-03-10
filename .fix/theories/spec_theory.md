# Specification Theory

## Hypothesis

Баг **НЕ** вызван проблемами в спецификации. Спецификация корректно описывает ожидаемое поведение.

## Evidence

### Spec Requirements for Edge Cases

**spec.md:95** явно требует обработку broken scene links:
> "What happens when the user creates a choice that leads to a non-existent scene? **System should warn about broken links** and prevent build until resolved."

**spec.md:98** требует обработку missing assets:
> "How does the system handle missing audio files referenced in scenes? **System should show missing asset warnings** and provide placeholder audio during preview."

### Data Model Validation

**data-model.md:71-77** определяет validation rules для `SceneData`:
```markdown
**Validation Rules**:
- `backgroundImage` must resolve to valid Addressable
- Circular scene references detected and warned
```

Спецификация **явно требует валидацию AssetReferences**, но **не детализирует** поведение runtime системы при невалидных references.

### Expected Behavior Mismatch

Спецификация говорит о **warnings** для broken links:
- "System should **warn** about broken links"
- "System should show missing asset **warnings**"

Текущая реализация логирует **ERROR** вместо **WARNING** при встрече с invalid AssetReference.

### Ambiguity Found

Спецификация **не определяет четко**:
1. На каком этапе должна происходить валидация (build-time vs runtime)?
2. Что должно произойти в runtime, если невалидный AssetReference все же присутствует?
3. Должна ли система пытаться загрузить невалидный AssetReference или проверить его заранее?

Однако, **тесты EndOfStoryReproductionTests явно ожидают**:
- Проверку `RuntimeKeyIsValid()` **ПЕРЕД** попыткой загрузки
- Логирование **WARNING** (не ERROR)
- Graceful completion через `OnDialogueComplete`

## Expected Behavior Mismatch

- **According to spec**: System should **warn** about broken links/missing assets
- **According to tests**: System should log **WARNING** and validate `RuntimeKeyIsValid()` before load
- **Actual behavior**: System tries to load invalid AssetReference, gets null, logs **ERROR**

**Bug behavior aligns with**: Neither spec nor tests - это implementation bug

## Constitution Alignment

Проверка соответствия Principle VI - Testing:

Тесты `EndOfStoryReproductionTests` написаны согласно Constitution:
- EditMode strategy (PlayMode для integration)
- Regression tests для конкретного бага (InvalidKeyException)
- Используют builders для test data

**Principle VI NOT violated** - тесты корректны, реализация не соответствует требованиям.

## Confidence Score

**30%**

### Reasoning

Спецификация **содержит требования** к обработке broken links и missing assets, но:
1. **Не детализирует runtime поведение** - только общие edge case guidelines
2. **Тесты более детальны** - они определяют точное ожидаемое поведение (WARNING + validation)
3. **Нет противоречий в спецификации** - требования согласованы
4. **Проблема в реализации** - код не соответствует ни spec, ни тестам

Низкая уверенность в SpecTheory, потому что:
- Спецификация описывает правильное поведение на высоком уровне
- Отсутствие детализации runtime validation ≠ некорректная спецификация
- Тесты правильно интерпретируют требования spec

## Recommended Action

- [ ] Update specification - **НЕ требуется**
- [ ] Clarify ambiguous requirements - **Опционально**: добавить explicit guidance по runtime AssetReference validation
- [ ] Resolve contradictions - **Не применимо**: противоречий нет
- [x] **Update implementation to match spec** - Реализовать валидацию AssetReference согласно spec requirements

### Clarification Recommendation (Optional)

Для будущей ясности, можно добавить в `data-model.md` раздел **Runtime Validation** для SceneData:

```markdown
**Runtime Validation**:
- Before loading NextScene, check `AssetReference.RuntimeKeyIsValid()`
- If invalid: log WARNING "NextScene AssetReference is invalid", complete dialogue gracefully
- If valid but load fails: log ERROR, complete dialogue gracefully
```

Это уточнит поведение, но **не является обязательным** для исправления текущего бага.
