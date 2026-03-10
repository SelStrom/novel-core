# Selected Theory

## Decision

**Selected**: **IMPL_THEORY**

## Reasoning

### Theory Comparison

| Aspect                | Spec Theory      | Impl Theory      |
|-----------------------|------------------|------------------|
| Confidence Score      | 15%              | **95%**          |
| Evidence Quality      | LOW              | **HIGH**         |
| Failing Test Alignment| WEAK             | **STRONG**       |
| Constitution Alignment| N/A              | ALIGNED          |

### Decision Rationale

1. **Evidence Strength**: 
   - Impl Theory предоставляет конкретные error messages ("Expected: 0 But was: 2")
   - Spec Theory не находит проблем в спецификации
   - Код SceneEditorWindow УЖЕ правильный (использует AddObjectToAsset)

2. **Test Alignment**: 
   - Failing tests явно показывают проблему cleanup
   - Error messages указывают на orphaned assets от предыдущих тестов
   - REPRODUCTION tests должны падать (они правильно демонстрируют старый баг)
   - BREAK tests должны проходить, но падают из-за test pollution

3. **Root Cause Coverage**: 
   - Impl Theory полностью объясняет все 11 failed tests
   - Spec Theory не применима к тестовой инфраструктуре

### Rejected Theory

**Rejected**: SPEC_THEORY

**Reason for Rejection**:
- Нет противоречий в спецификациях
- Это не баг feature implementation, а баг test infrastructure
- Constitution не содержит детальных требований к test cleanup
- Confidence score только 15%

## Confidence in Decision

**100%**

### Justification

Очевидно, что это проблема теста, а не спецификации:
1. SceneEditorWindow код правильный
2. Error messages указывают на orphaned assets
3. TearDown методы недостаточно строгие
4. Нет альтернативных объяснений

## Recommended Fix Strategy

### Implementation Changes Required

- [x] Fix code at identified location (TearDown methods)
- [x] Add comprehensive cleanup
- [x] Add AssetDatabase.Refresh() for cache invalidation
- [x] Consider test-specific directories for better isolation
- [ ] Refactor if architectural issue (NOT NEEDED)
- [x] Add regression tests (break tests already exist)

### Specific Actions

1. **Улучшить TearDown в SceneEditorWindowSubAssetBreakTests.cs**:
   - Добавить cleanup всех DialogueLineData и ChoiceData в Assets/
   - Добавить AssetDatabase.Refresh()
   - Фильтровать по названию (содержит "Test")

2. **Улучшить TearDown в SceneEditorWindowSubAssetReproductionTests.cs**:
   - Аналогично

3. **Улучшить TearDown в SceneEditorWindowSubAssetDeletionReproductionTests.cs**:
   - Проверить и улучшить cleanup

4. **Добавить helper method для cleanup**:
   - Создать `CleanupAllTestAssets()` helper
   - Использовать во всех TearDown методах

## Next Steps

1. Применить Fix 1 из impl_theory.md
2. Запустить тесты для verification
3. Убедиться, что все 11 tests теперь проходят
4. Добавить break tests если нужно
