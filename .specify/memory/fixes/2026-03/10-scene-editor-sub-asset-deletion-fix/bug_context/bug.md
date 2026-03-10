# Bug Description

При удалении DialogueLineData или ChoiceData через кнопку "×" в SceneEditorWindow, sub-asset остается в asset database и не удаляется. Это приводит к "мусорным" sub-assets, которые видны при инспектировании SceneData в Project Browser.

## Expected Behavior

При нажатии кнопки "×" для удаления DialogueLine или Choice:
1. Ссылка на DialogueLineData/ChoiceData должна быть удалена из массива SceneData._dialogueLines или _choices
2. **Sub-asset DialogueLineData/ChoiceData должен быть физически удален из asset database**
3. В Project Browser при раскрытии SceneData не должны отображаться удаленные sub-assets

## Actual Behavior

При нажатии кнопки "×":
1. ✅ Ссылка удаляется из массива SceneData (dialogueProperty.DeleteArrayElementAtIndex(i))
2. ❌ Sub-asset **НЕ удаляется** физически из asset database
3. ❌ В Project Browser при раскрытии SceneData всё ещё видны "orphaned" sub-assets

## Steps to Reproduce

1. Открыть SceneEditorWindow (Window > NovelCore > Scene Editor)
2. Загрузить любую SceneData (например, `Sample/Scenes/Scene01_Introduction.asset`)
3. Создать новую DialogueLine через кнопку "+ Add Dialogue Line"
4. Sub-asset с именем `dialog_line_XXX` появляется внутри SceneData
5. Нажать кнопку "×" рядом с созданной DialogueLine для удаления
6. **Ожидаемо**: DialogueLine исчезает и из списка, и из asset database
7. **Фактически**: DialogueLine исчезает из списка, но sub-asset остается в Project Browser

## Environment

- Unity Version: 6000.0.69f1 (Unity 6 Long Term Support)
- Platform: macOS (darwin 25.2.0)
- NovelCore Version: dev (main branch)
- Affected Components: SceneEditorWindow, DialogueLineData, ChoiceData
