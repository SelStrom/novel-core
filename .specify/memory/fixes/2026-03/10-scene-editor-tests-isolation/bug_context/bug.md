# Bug Description

EditMode тесты SceneEditorWindow падают из-за неправильного создания sub-assets для DialogueLineData и ChoiceData.

## Expected Behavior

При создании новых DialogueLineData или ChoiceData через SceneEditorWindow:
1. Объекты должны создаваться как **sub-assets** родительского SceneData
2. **Не должно** создаваться отдельных standalone asset файлов
3. При удалении DialogueLineData/ChoiceData должны удаляться именно sub-assets, а не только ссылки из массива

## Actual Behavior

1. DialogueLineData и ChoiceData создаются как **standalone assets** (отдельные файлы)
2. При подсчёте standalone DialogueLineData assets находится 1 вместо ожидаемых 0
3. Удаление не удаляет sub-assets корректно

## Steps to Reproduce

1. Открыть SceneEditorWindow
2. Создать новую DialogueLineData через метод `CreateDialogueLine()`
3. Проверить количество standalone DialogueLineData assets в проекте
4. Ожидаемо: 0, Фактически: 1

## Failed Tests

### Primary Failures:
1. `CreateDialogueLine_ShouldCreateSubAsset_NotStandaloneAsset` ❌
2. `CreateChoice_ShouldAlsoBeSubAsset` ❌
3. `DeleteDialogueLine_ShouldDeleteSubAsset_NotJustArrayReference` ❌
4. `DeleteChoice_ShouldDeleteSubAsset_NotJustArrayReference` ❌
5. `DeleteMultipleDialogueLines_ShouldDeleteAllSubAssets` ❌
6. `SampleProjectGenerator_CreatesSubAssets_AsExpected` ❌
7. `CreateMultipleDialogueLines_AllShouldBeSubAssets` ❌
8. `DeleteDialogueLine_DuplicateReferences_ShouldDeleteSubAssetOnlyOnce` ❌
9. `DrawDialogue_WithLongFallbackText_ShouldTruncateText` ❌
10. `DrawDialogue_WithNewlyCreatedDialogueLine_ShouldNotThrowNullReferenceException` ❌

**Total Failed**: 11 тестов (из 122 EditMode tests)

## Environment

- **Unity Version**: 6000.0.69f1 (Unity 6 LTS)
- **Platform**: macOS
- **Test Platform**: EditMode
- **NovelCore Version**: 1.0.0 (в разработке)
- **Component**: SceneEditorWindow (Editor tool)

## Root Cause Category

- [ ] Logical error
- [x] API misuse (Unity AssetDatabase.CreateAsset vs AddObjectToAsset)
- [ ] Edge case not handled
- [ ] State management error
- [ ] Memory/resource leak
- [ ] Other
