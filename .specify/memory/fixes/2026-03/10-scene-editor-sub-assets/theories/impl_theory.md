# Implementation Theory

## Hypothesis

Баг вызван **неправильным использованием Unity AssetDatabase API** в SceneEditorWindow. Методы `CreateNewDialogueLine()` и `CreateNewChoice()` используют `AssetDatabase.CreateAsset()` вместо `AssetDatabase.AddObjectToAsset()`.

## Location

### Primary Location

- **File**: `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- **Namespace**: `NovelCore.Editor.Windows`
- **Class**: `SceneEditorWindow`
- **Methods**:
  - `CreateNewDialogueLine(SerializedObject serializedObject, SerializedProperty dialogueProperty)` - Lines 570-588
  - `CreateNewChoice(SerializedObject serializedObject, SerializedProperty choicesProperty)` - Lines 590-608
- **Line Range**: 570-608

### Secondary Locations (for reference)

- **File**: `novel-core/Assets/Scripts/NovelCore/Editor/Tools/Generators/SampleProjectGenerator.cs`
- **Reason**: Demonstrates correct sub-asset creation pattern (reference implementation)
- **Method**: `CreateScene()` - Lines 420-426

## Root Cause Hypothesis

### Category

- [x] **API misuse** (incorrect usage of Unity AssetDatabase APIs)
- [ ] Logical error
- [ ] Edge case not handled
- [ ] Race condition
- [ ] State management error
- [ ] Memory/resource leak

### Detailed Explanation

#### Bug in CreateNewDialogueLine()

```csharp
// Lines 570-588 in SceneEditorWindow.cs
private void CreateNewDialogueLine(SerializedObject serializedObject, SerializedProperty dialogueProperty)
{
    string path = AssetDatabase.GetAssetPath(_currentScene);
    string directory = System.IO.Path.GetDirectoryName(path);

    var newLine = CreateInstance<DialogueLineData>();
    string linePath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/DialogueLine.asset");

    // ❌ BUG: This creates a standalone asset file
    AssetDatabase.CreateAsset(newLine, linePath);
    AssetDatabase.SaveAssets();

    dialogueProperty.arraySize++;
    var newElement = dialogueProperty.GetArrayElementAtIndex(dialogueProperty.arraySize - 1);
    newElement.objectReferenceValue = newLine;  // References external asset via GUID

    serializedObject.ApplyModifiedProperties();

    _selectedDialogueIndex = dialogueProperty.arraySize - 1;
}
```

**Issue**: 
1. `AssetDatabase.CreateAsset(newLine, linePath)` создает **новый asset-файл** на диске (`DialogueLine 1.asset`)
2. `newElement.objectReferenceValue = newLine` сохраняет ссылку через **GUID** на внешний asset
3. Результат: DialogueLineData существует как **отдельный файл**, а не как **sub-asset** внутри SceneData

#### Bug in CreateNewChoice()

```csharp
// Lines 590-608 in SceneEditorWindow.cs
private void CreateNewChoice(SerializedObject serializedObject, SerializedProperty choicesProperty)
{
    string path = AssetDatabase.GetAssetPath(_currentScene);
    string directory = System.IO.Path.GetDirectoryName(path);

    var newChoice = CreateInstance<ChoiceData>();
    string choicePath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/Choice.asset");

    // ❌ BUG: Same issue as CreateNewDialogueLine
    AssetDatabase.CreateAsset(newChoice, choicePath);
    AssetDatabase.SaveAssets();

    choicesProperty.arraySize++;
    var newElement = choicesProperty.GetArrayElementAtIndex(choicesProperty.arraySize - 1);
    newElement.objectReferenceValue = newChoice;

    serializedObject.ApplyModifiedProperties();

    _selectedChoiceIndex = choicesProperty.arraySize - 1;
}
```

**Issue**: Identical API misuse as in `CreateNewDialogueLine()`.

### Call Stack Analysis

```
User clicks "+ Add Dialogue Line" button
  ↓
SceneEditorWindow.DrawDialogue() [Line 323]
  ↓
SceneEditorWindow.CreateNewDialogueLine() [Line 570]
  ↓
AssetDatabase.CreateAsset(newLine, linePath) ❌ BUG HERE
  ↓
Unity creates standalone asset file
  ↓
SceneData stores GUID reference to external asset
```

**Analysis**: Проблема возникает на уровне asset creation. Вместо добавления объекта как sub-asset в SceneData, создается отдельный asset-файл.

## Git History Insights

### Recent Changes

Проверю git history для SceneEditorWindow.cs:

**Note**: Этот файл был создан недавно (git status показывает изменения). Вероятно, этот паттерн был использован с самого начала и не был замечен в review.

**Suspicion Level**: N/A (не было изменений, которые ввели баг — он существовал с момента создания)

**Reason**: Автор, возможно, не был знаком с паттерном sub-assets из SampleProjectGenerator.

## Edge Cases Analysis

**None applicable**. Это не edge case — баг проявляется в **100% случаев** при создании DialogueLine или Choice через SceneEditorWindow.

## Confidence Score

**95%**

### Reasoning

1. **Root cause очевиден**: Неправильный Unity API (`CreateAsset` вместо `AddObjectToAsset`)
2. **Reference implementation exists**: SampleProjectGenerator демонстрирует правильный подход
3. **Воспроизводимо 100%**: Каждый вызов `CreateNewDialogueLine()` создает standalone asset
4. **Direct evidence**: Файл `DialogueLine 1.asset` существует в git status
5. **Code comparison clear**:
   - ❌ SceneEditorWindow: `AssetDatabase.CreateAsset(newLine, linePath)`
   - ✅ SampleProjectGenerator: `AssetDatabase.AddObjectToAsset(line, scene)`

**Very high confidence** потому что:
- Баг trivial и очевидный (API misuse)
- Фикс trivial (2-3 строки кода)
- Эталонная реализация существует в том же codebase

## Recommended Fix Approach

### Approach 1: Minimal Fix (CreateNewDialogueLine)

```csharp
private void CreateNewDialogueLine(SerializedObject serializedObject, SerializedProperty dialogueProperty)
{
    // Remove directory path logic (not needed for sub-assets)
    var newLine = CreateInstance<DialogueLineData>();
    newLine.name = $"DialogueLine_{dialogueProperty.arraySize + 1}";  // Name for sub-asset

    // ✅ FIX: Add as sub-asset instead of creating standalone file
    AssetDatabase.AddObjectToAsset(newLine, _currentScene);
    EditorUtility.SetDirty(_currentScene);
    AssetDatabase.SaveAssets();

    dialogueProperty.arraySize++;
    var newElement = dialogueProperty.GetArrayElementAtIndex(dialogueProperty.arraySize - 1);
    newElement.objectReferenceValue = newLine;

    serializedObject.ApplyModifiedProperties();

    _selectedDialogueIndex = dialogueProperty.arraySize - 1;
}
```

**Changes**:
1. Remove `string path = AssetDatabase.GetAssetPath(_currentScene);`
2. Remove `string directory = System.IO.Path.GetDirectoryName(path);`
3. Remove `string linePath = AssetDatabase.GenerateUniqueAssetPath(...);`
4. Replace `AssetDatabase.CreateAsset(newLine, linePath);` with `AssetDatabase.AddObjectToAsset(newLine, _currentScene);`
5. Add `EditorUtility.SetDirty(_currentScene);` to mark scene as modified
6. Add `newLine.name = ...` for better Inspector visibility

### Approach 2: Minimal Fix (CreateNewChoice)

```csharp
private void CreateNewChoice(SerializedObject serializedObject, SerializedProperty choicesProperty)
{
    var newChoice = CreateInstance<ChoiceData>();
    newChoice.name = $"Choice_{choicesProperty.arraySize + 1}";

    // ✅ FIX: Add as sub-asset
    AssetDatabase.AddObjectToAsset(newChoice, _currentScene);
    EditorUtility.SetDirty(_currentScene);
    AssetDatabase.SaveAssets();

    choicesProperty.arraySize++;
    var newElement = choicesProperty.GetArrayElementAtIndex(choicesProperty.arraySize - 1);
    newElement.objectReferenceValue = newChoice;

    serializedObject.ApplyModifiedProperties();

    _selectedChoiceIndex = choicesProperty.arraySize - 1;
}
```

**Changes**: Identical to CreateNewDialogueLine fix.

### Expected Result After Fix

**Before fix** (current state):
```yaml
# Scene01_Introduction.asset
_dialogueLines:
- {fileID: 8279926708765509791}  # Sub-asset (from SampleProjectGenerator)
- {fileID: 11400000, guid: 6333a519c5faa4be6b108c80c60a2103, type: 2}  # External asset (bug!)

# Separate file: DialogueLine 1.asset
```

**After fix**:
```yaml
# Scene01_Introduction.asset
_dialogueLines:
- {fileID: 8279926708765509791}  # Sub-asset
- {fileID: 1234567890123456789}  # Sub-asset (new, created through editor)

# No separate DialogueLine files created ✅
```

## Test Strategy

### Unit Test

```csharp
[Test]
public void CreateNewDialogueLine_CreatesSubAsset_NotStandaloneAsset()
{
    // Arrange
    var scene = ScriptableObject.CreateInstance<SceneData>();
    AssetDatabase.CreateAsset(scene, "Assets/TestScene.asset");

    // Act
    // ... call CreateNewDialogueLine (or simulate it)

    // Assert
    Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets/TestScene.asset");
    Assert.That(subAssets.Length, Is.GreaterThan(0), "DialogueLine should be a sub-asset");

    // Verify no standalone asset created
    string[] standaloneAssets = AssetDatabase.FindAssets("DialogueLine t:DialogueLineData");
    Assert.That(standaloneAssets.Length, Is.EqualTo(0), "No standalone asset should exist");
}
```

### Manual Test

1. Open Scene Editor Window
2. Select SceneData
3. Click "+ Add Dialogue Line"
4. Verify in Project Browser: **no** `DialogueLine N.asset` file created
5. Verify in Scene asset: DialogueLine appears as sub-asset (expand SceneData in Project Browser)

## Files Modified

- `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`:
  - `CreateNewDialogueLine()` - Lines 570-588 (~10 lines changed)
  - `CreateNewChoice()` - Lines 590-608 (~10 lines changed)

**Total**: 1 file, ~20 lines changed (mostly deletions + 2-3 additions per method)

## Constitution Compliance

**After fix**:
- ✅ Principle I (Creator-First Design): Clean Project Browser, no manual cleanup
- ✅ Principle III (Asset Pipeline Integrity): Referential integrity maintained
- ✅ Principle VI (Modular Architecture): Scene as atomic unit

**Code Style**:
- ✅ Allman braces (already used in SceneEditorWindow.cs)
- ✅ Underscore prefix for fields (already follows convention)
- ✅ XML documentation present (method already documented)

## Risk Assessment

**Risk Level**: **Very Low**

**Reasoning**:
1. **Minimal code change** (2-3 lines per method)
2. **Well-established Unity pattern** (AddObjectToAsset is standard practice)
3. **Reference implementation exists** (SampleProjectGenerator proves it works)
4. **No breaking changes** (existing SceneData with external references still work)
5. **Easy to test** (visual verification in Unity Editor)

**Potential Issues**:
- ⚠️ Existing standalone DialogueLineData assets will remain (need cleanup guidance for users)
- ✅ New DialogueLines will be sub-assets (correct going forward)
