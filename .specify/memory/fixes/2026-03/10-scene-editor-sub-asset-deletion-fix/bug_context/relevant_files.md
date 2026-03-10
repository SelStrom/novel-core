# Relevant Files

## Primary Files (directly affected)

### 1. `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- **Location**: Class `SceneEditorWindow`, method `DrawDialogue()` (line 305-310)
- **Reason**: Содержит логику удаления DialogueLine — использует только `DeleteArrayElementAtIndex()`, не удаляет sub-asset
- **Buggy Code**:
  ```csharp
  if (GUILayout.Button("×", GUILayout.Width(25)))
  {
      dialogueProperty.DeleteArrayElementAtIndex(i);
      serializedObject.ApplyModifiedProperties();
      return;
  }
  ```

- **Location**: Class `SceneEditorWindow`, method `DrawChoices()` (line 400-404)
- **Reason**: Аналогичная проблема с удалением ChoiceData
- **Buggy Code**:
  ```csharp
  if (GUILayout.Button("×", GUILayout.Width(25)))
  {
      choicesProperty.DeleteArrayElementAtIndex(i);
      serializedObject.ApplyModifiedProperties();
      return;
  }
  ```

## Secondary Files (dependencies)

### 2. `novel-core/Assets/Scripts/NovelCore/Runtime/Data/Scenes/SceneData.cs`
- **Reason**: Определяет структуру SceneData с массивами `_dialogueLines` и `_choices`, которые содержат ссылки на sub-assets

### 3. `novel-core/Assets/Scripts/NovelCore/Runtime/Data/Dialogue/DialogueLineData.cs`
- **Reason**: ScriptableObject, который создается как sub-asset при вызове `CreateNewDialogueLine()`

### 4. `novel-core/Assets/Scripts/NovelCore/Runtime/Data/Choices/ChoiceData.cs`
- **Reason**: ScriptableObject, который создается как sub-asset при вызове `CreateNewChoice()`

## Related Files (context)

### 5. `.specify/memory/fixes/2026-03/10-scene-editor-sub-assets/`
- **Reason**: Предыдущий fix для **создания** sub-assets (был баг, что создавались standalone файлы). Текущий баг — обратная проблема при **удалении** sub-assets.
