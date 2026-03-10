# Relevant Files

## Primary Files (directly affected)

### 1. `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- **Location**: `NovelCore.Editor.Windows.SceneEditorWindow`
- **Methods**:
  - `CreateNewDialogueLine(SerializedObject, SerializedProperty)` - Lines 570-588
  - `CreateNewChoice(SerializedObject, SerializedProperty)` - Lines 590-608
- **Reason**: Использует `AssetDatabase.CreateAsset()` вместо `AssetDatabase.AddObjectToAsset()`
- **Issue**: Creates standalone assets instead of sub-assets

### 2. `novel-core/Assets/Scripts/NovelCore/Editor/Tools/Generators/SampleProjectGenerator.cs`
- **Location**: `NovelCore.Editor.Tools.Generators.SampleProjectGenerator`
- **Methods**:
  - `CreateScene()` - Lines 410-470
  - `CreateDialogueLine()` - Lines 472-485
- **Reason**: Эталонная реализация с sub-assets (правильный подход)
- **Reference**: Line 424 - `AssetDatabase.AddObjectToAsset(line, scene)`

## Secondary Files (dependencies)

### 3. `novel-core/Assets/Scripts/NovelCore/Runtime/Data/Scenes/SceneData.cs`
- **Location**: `NovelCore.Runtime.Data.Scenes.SceneData`
- **Fields**:
  - `_dialogueLines` (List<DialogueLineData>)
  - `_choices` (List<ChoiceData>)
- **Reason**: Хранит ссылки на dialogue lines и choices
- **Expected behavior**: Sub-assets хранятся внутри SceneData asset

### 4. `novel-core/Assets/Scripts/NovelCore/Runtime/Data/Dialogue/DialogueLineData.cs`
- **Location**: `NovelCore.Runtime.Data.Dialogue.DialogueLineData`
- **Type**: ScriptableObject
- **Reason**: Тип объекта, который должен быть sub-asset

### 5. `novel-core/Assets/Scripts/NovelCore/Runtime/Data/Choices/ChoiceData.cs`
- **Location**: `NovelCore.Runtime.Data.Choices.ChoiceData`
- **Type**: ScriptableObject
- **Reason**: Тип объекта, который должен быть sub-asset

## Test Files

### 6. `novel-core/Assets/Scripts/NovelCore/Tests/Editor/Windows/SceneEditorWindowTests.cs`
- **Location**: Existing test file for SceneEditorWindow
- **Reason**: May need updates to verify sub-asset creation

## Asset Files (evidence)

### 7. `novel-core/Assets/Content/Projects/Sample/Scenes/Scene01_Introduction.asset`
- **Type**: SceneData asset with sub-assets
- **Contains**: 3 DialogueLineData sub-assets (embedded in same file)
- **Evidence**: Lines 83-86 show fileID references (not GUIDs) for sub-assets
- **Structure**:
  ```yaml
  _dialogueLines:
  - {fileID: 8279926708765509791}  # Sub-asset
  - {fileID: -4011206537604049886} # Sub-asset
  - {fileID: 8964838172112927967}  # Sub-asset
  - {fileID: 11400000, guid: 6333a519c5faa4be6b108c80c60a2103, type: 2}  # External asset (bug!)
  ```

### 8. `novel-core/Assets/Content/Projects/Sample/Scenes/DialogueLine 1.asset`
- **Type**: Standalone DialogueLineData asset (bug evidence)
- **Created by**: SceneEditorWindow (manual user action)
- **GUID**: `6333a519c5faa4be6b108c80c60a2103`
- **Problem**: Этот файл не должен существовать; DialogueLineData должна быть sub-asset

## Git Status Evidence

```
?? "novel-core/Assets/Content/Projects/Sample/Scenes/DialogueLine 1.asset"
?? "novel-core/Assets/Content/Projects/Sample/Scenes/DialogueLine 1.asset.meta"
```

Эти файлы созданы через SceneEditorWindow и демонстрируют баг.
