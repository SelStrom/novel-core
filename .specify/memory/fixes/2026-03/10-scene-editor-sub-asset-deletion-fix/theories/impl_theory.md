# Implementation Theory

## Hypothesis

Баг вызван **неполной реализацией удаления sub-assets** в SceneEditorWindow. Метод удаления DialogueLine/Choice использует только `DeleteArrayElementAtIndex()`, который удаляет **ссылку из массива**, но НЕ вызывает `AssetDatabase.RemoveObjectFromAsset()` для удаления самого sub-asset из AssetDatabase.

## Location

### Primary Location

- **File**: `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- **Namespace**: `NovelCore.Editor.Windows`
- **Class**: `SceneEditorWindow`
- **Methods**:
  - `DrawDialogue()` - Delete button handler (Lines 305-310)
  - `DrawChoices()` - Delete button handler (Lines 400-404)

### Deletion Code для DialogueLine (Lines 305-310)

```csharp
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    dialogueProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    return;
}
```

### Deletion Code для Choice (Lines 400-404)

```csharp
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    choicesProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    return;
}
```

## Root Cause Hypothesis

### Category

- [x] **Incomplete implementation** (missing cleanup step)
- [ ] Logical error
- [ ] Edge case not handled
- [ ] Race condition
- [ ] State management error
- [ ] Memory/resource leak

### Detailed Explanation

#### Problem: Two-Step Deletion Not Implemented

Unity AssetDatabase sub-asset deletion requires **two distinct steps**:

1. **Step 1: Remove sub-asset from AssetDatabase**
   ```csharp
   var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
   AssetDatabase.RemoveObjectFromAsset(subAsset);
   ```

2. **Step 2: Remove reference from parent array**
   ```csharp
   dialogueProperty.DeleteArrayElementAtIndex(i);
   serializedObject.ApplyModifiedProperties();
   ```

**Current Implementation**: Только Step 2 реализован ❌

**Result**: Sub-asset остается в AssetDatabase как "orphaned asset" (не referenced SceneData, но физически существует)

#### Unity SerializedProperty.DeleteArrayElementAtIndex() Behavior

Unity documentation:
> `DeleteArrayElementAtIndex()` removes the element at the specified index. If the element is an object reference and the reference is NOT null, the first call sets the reference to null. The second call removes the array element.

**Current code behavior**:
1. First click "×": Sets `objectReferenceValue` to `null` (DialogueLine reference = null)
2. Second click "×": Removes array element (DialogueLine entry disappears from list)

**Problem**: Neither click удаляет физический sub-asset из AssetDatabase!

### Call Stack Analysis

```
User clicks "×" button
  ↓
SceneEditorWindow.DrawDialogue() [Line 305]
  ↓
GUILayout.Button("×") returns true
  ↓
dialogueProperty.DeleteArrayElementAtIndex(i) ← ONLY REMOVES REFERENCE
  ↓
serializedObject.ApplyModifiedProperties()
  ↓
AssetDatabase.SaveAssets() (implicit via Unity)
  ↓
Result: Reference removed, sub-asset STILL EXISTS in AssetDatabase ❌
```

**Missing Step** (должно быть **перед** DeleteArrayElementAtIndex):
```
AssetDatabase.RemoveObjectFromAsset(subAsset);
EditorUtility.SetDirty(_currentScene);
```

### Comparison with Creation Code

**Creation** (Lines 570-586) — **CORRECT** (после fix в commit 8f7eb74):
```csharp
var newLine = CreateInstance<DialogueLineData>();
newLine.name = $"dialog_line_{(dialogueProperty.arraySize + 1):D3}";

AssetDatabase.AddObjectToAsset(newLine, _currentScene);  // ✅ Add to AssetDatabase
EditorUtility.SetDirty(_currentScene);
AssetDatabase.SaveAssets();

dialogueProperty.arraySize++;
var newElement = dialogueProperty.GetArrayElementAtIndex(dialogueProperty.arraySize - 1);
newElement.objectReferenceValue = newLine;  // Add reference to array

serializedObject.ApplyModifiedProperties();
```

**Deletion** (Lines 305-310) — **INCORRECT** (текущий код):
```csharp
// ❌ MISSING: AssetDatabase.RemoveObjectFromAsset(subAsset);

dialogueProperty.DeleteArrayElementAtIndex(i);  // Remove reference from array
serializedObject.ApplyModifiedProperties();
return;
```

**Symmetry Analysis**: Creation вызывает `AddObjectToAsset()`, но deletion НЕ вызывает `RemoveObjectFromAsset()` — **нарушение симметрии!**

## Git History Insights

### Recent Changes

**Commit**: `e28ee5e` - "refactor(editor): unify sub-asset naming to snake_case with zero-padding"
- **Date**: Recent (after creation fix)
- **Changes**: Унификация naming convention для sub-assets (`dialog_line_001`, `choice_001`)
- **Suspicion Level**: **LOW** (изменение только naming, не затрагивает deletion logic)

**Commit**: `8f7eb74` - "fix(editor): create DialogueLineData and ChoiceData as sub-assets"
- **Date**: 2026-03-10 (сегодня, ранее)
- **Changes**: Fixed **creation** of sub-assets (`AssetDatabase.AddObjectToAsset()`)
- **Suspicion Level**: **LOW** (fix был для creation, не для deletion)
- **Analysis**: Этот fix **ввел правильное создание** sub-assets, но **НЕ добавил deletion logic**

**Root Cause Timing**:
- Bug existed **с момента создания** SceneEditorWindow
- Deletion logic **никогда не был реализован** (не regression, а missing feature)
- Previous fix (8f7eb74) исправил creation, но **пропустил deletion** (incomplete fix)

## Edge Cases Analysis

### Edge Case 1: Null Reference

**Scenario**: `dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue == null`

**Current Behavior**: `DeleteArrayElementAtIndex()` removes array element (correct)

**Fixed Behavior**: Должен проверить `if (subAsset != null)` перед `RemoveObjectFromAsset()`

```csharp
var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
if (subAsset != null)
{
    AssetDatabase.RemoveObjectFromAsset(subAsset);
}
dialogueProperty.DeleteArrayElementAtIndex(i);
```

### Edge Case 2: External Asset Reference (not sub-asset)

**Scenario**: DialogueLineData существует как **standalone asset** (создан до fix 8f7eb74)

**Current Behavior**: Reference удаляется, standalone asset остается (это корректно — не удаляем чужие assets)

**Fixed Behavior**: Должен проверять, является ли asset sub-asset текущей SceneData:

```csharp
var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
{
    AssetDatabase.RemoveObjectFromAsset(subAsset);
}
dialogueProperty.DeleteArrayElementAtIndex(i);
```

**Important**: `AssetDatabase.IsSubAsset()` предотвращает удаление standalone assets, которые могут быть shared между SceneData.

### Edge Case 3: Multiple References to Same Sub-Asset

**Scenario**: Один и тот же DialogueLineData referenced несколько раз в массиве (unlikely, но возможно)

**Current Behavior**: Удаление одной ссылки не влияет на другие

**Fixed Behavior**: `RemoveObjectFromAsset()` удалит sub-asset, другие ссылки станут null

**Solution**: Это edge case **не нужно обрабатывать** — sub-assets не должны быть shared (это противоречит design pattern "scene as atomic unit")

## Confidence Score

**90%**

### Reasoning

1. **Root cause очевиден**: Отсутствие `AssetDatabase.RemoveObjectFromAsset()` call
2. **Воспроизводимо 100%**: Каждое удаление оставляет orphaned sub-asset
3. **Direct evidence from code**: Deletion code не содержит AssetDatabase API calls
4. **Symmetry violation**: Creation uses `AddObjectToAsset()`, deletion does NOT use `RemoveObjectFromAsset()`
5. **Previous fix precedent**: Commit 8f7eb74 исправил creation, но пропустил deletion (incomplete fix)

**Confidence не 100%** потому что:
- Нужно подтвердить, что `AssetDatabase.IsSubAsset()` check необходим (edge case 2)
- Возможно, есть причина, почему deletion намеренно оставляет sub-assets (unlikely, но нужно убедиться)

## Recommended Fix Approach

### Fix 1: DialogueLine Deletion (Lines 305-310)

**Before**:
```csharp
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    dialogueProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    return;
}
```

**After**:
```csharp
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    // Get reference to sub-asset before removing from array
    var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
    
    // Remove sub-asset from AssetDatabase (only if it's a sub-asset)
    if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
    {
        AssetDatabase.RemoveObjectFromAsset(subAsset);
        EditorUtility.SetDirty(_currentScene);
    }
    
    // Remove reference from array
    dialogueProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    
    // Save changes
    AssetDatabase.SaveAssets();
    
    return;
}
```

### Fix 2: Choice Deletion (Lines 400-404)

**Identical fix** as DialogueLine deletion:

```csharp
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    var subAsset = choicesProperty.GetArrayElementAtIndex(i).objectReferenceValue;
    
    if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
    {
        AssetDatabase.RemoveObjectFromAsset(subAsset);
        EditorUtility.SetDirty(_currentScene);
    }
    
    choicesProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    AssetDatabase.SaveAssets();
    
    return;
}
```

### Changes Summary

**Per method** (DialogueLine + Choice):
1. ✅ Add: `var subAsset = property.GetArrayElementAtIndex(i).objectReferenceValue;`
2. ✅ Add: `if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))`
3. ✅ Add: `AssetDatabase.RemoveObjectFromAsset(subAsset);`
4. ✅ Add: `EditorUtility.SetDirty(_currentScene);`
5. ✅ Add: `AssetDatabase.SaveAssets();`

**Total**: ~6 new lines per deletion handler (2 handlers = ~12 lines total)

## Expected Result After Fix

**Before fix**:
```
SceneData (Scene01_Introduction.asset)
├─ dialog_line_001 (sub-asset, visible in Inspector)
├─ dialog_line_002 (sub-asset, visible in Inspector)
└─ dialog_line_003 (sub-asset, visible in Inspector)

User clicks "×" on dialog_line_002
↓
SceneData (Scene01_Introduction.asset)
├─ dialog_line_001 (sub-asset, visible in Inspector)
├─ dialog_line_002 ❌ ORPHANED (still exists as sub-asset, but not referenced!)
└─ dialog_line_003 (sub-asset, visible in Inspector)
```

**After fix**:
```
SceneData (Scene01_Introduction.asset)
├─ dialog_line_001 (sub-asset)
├─ dialog_line_002 (sub-asset)
└─ dialog_line_003 (sub-asset)

User clicks "×" on dialog_line_002
↓
SceneData (Scene01_Introduction.asset)
├─ dialog_line_001 (sub-asset)
└─ dialog_line_003 (sub-asset)  ✅ dialog_line_002 DELETED from AssetDatabase
```

## Constitution Compliance

**After fix**:
- ✅ **Principle III (Asset Pipeline Integrity)**: Orphaned sub-assets удаляются, asset pipeline clean
- ✅ **Principle I (Creator-First Design)**: Creators не видят мусорные sub-assets в Inspector
- ✅ **Principle VI (Code Style)**: Allman braces, underscore fields (SceneEditorWindow already follows)

## Risk Assessment

**Risk Level**: **Low**

**Reasoning**:
1. **Minimal code change** (~12 lines total)
2. **Well-established Unity pattern** (`RemoveObjectFromAsset()` is standard practice)
3. **Safety check included** (`IsSubAsset()` prevents deleting standalone assets)
4. **No breaking changes** (existing orphaned sub-assets remain, but NEW deletions work correctly)
5. **Easy to test** (visual verification in Unity Inspector)

**Potential Issues**:
- ✅ Existing orphaned sub-assets will remain until manual cleanup (acceptable — fix prevents future accumulation)
- ✅ Standalone assets (created before fix 8f7eb74) won't be deleted (correct behavior — `IsSubAsset()` check)

## Test Strategy

**Failing Test** (already created):
- `DeleteDialogueLine_ShouldDeleteSubAsset_NotJustArrayReference()`
- `DeleteChoice_ShouldDeleteSubAsset_NotJustArrayReference()`
- `DeleteMultipleDialogueLines_ShouldDeleteAllSubAssets()`

**After Fix Validation**:
1. Run failing tests → should PASS ✅
2. Manual test: Create DialogueLine via SceneEditorWindow → Delete → Verify sub-asset removed from Inspector
3. Edge case test: Create standalone DialogueLineData → Reference in SceneData → Delete → Verify standalone asset remains (not deleted)
