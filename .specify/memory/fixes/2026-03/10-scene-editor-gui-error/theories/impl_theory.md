# Implementation Theory

## Hypothesis

Баг вызван **early return statement** в deletion handlers, который выходит из метода до вызова `EditorGUILayout.EndHorizontal()`, нарушая баланс Unity GUI layout Begin/End calls.

## Location

### Primary Locations

**1. DrawDialogue() method**:
- **File**: `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- **Line**: 319 (`return` inside delete button handler)
- **Context**: Lines 285-322 (BeginHorizontal → delete button → EndHorizontal)

**2. DrawChoices() method**:
- **File**: Same file
- **Line**: 424 (`return` inside delete button handler)
- **Context**: Lines 390-427 (BeginHorizontal → delete button → EndHorizontal)

**3. DrawCharacters() method** (not mentioned in bug report, but same issue):
- **File**: Same file
- **Line**: 231 (`return` inside delete button handler)
- **Context**: Lines 211-234 (BeginHorizontal → delete button → EndHorizontal)

## Root Cause Hypothesis

### Category

- [x] **Logical error** (control flow exits before cleanup code)
- [ ] Edge case not handled
- [ ] API misuse
- [ ] Race condition

### Detailed Explanation

Unity IMGUI (EditorGUI/GUILayout) требует строгого баланса `Begin`/`End` вызовов:

```csharp
EditorGUILayout.BeginHorizontal();  // ← Push layout state onto stack
// ... GUI elements ...
EditorGUILayout.EndHorizontal();    // ← Pop layout state from stack
```

**Если `EndHorizontal()` не вызван**, Unity GUI stack становится несбалансированным → Unity throws error.

#### Buggy Code (DrawDialogue, line 285-322)

```csharp
for (int i = 0; i < dialogueProperty.arraySize; i++)
{
    EditorGUILayout.BeginHorizontal();  // Line 285 ← PUSH

    // ... GUI elements (Toggle, label, etc.) ...

    if (GUILayout.Button("×", GUILayout.Width(25)))  // Line 305
    {
        // Deletion logic (lines 307-317)
        var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
        if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
        {
            AssetDatabase.RemoveObjectFromAsset(subAsset);
            EditorUtility.SetDirty(_currentScene);
        }
        dialogueProperty.DeleteArrayElementAtIndex(i);
        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        
        return;  // ❌ Line 319 - EXIT METHOD HERE
    }

    EditorGUILayout.EndHorizontal();  // ❌ Line 322 - NEVER REACHED if button clicked ← POP SKIPPED

    // ... rest of loop ...
}
```

**Problem Flow**:
1. User clicks "×" button → `GUILayout.Button()` returns `true`
2. Deletion code executes (lines 307-317) ✅
3. `return` executes (line 319) → method exits ❌
4. `EditorGUILayout.EndHorizontal()` (line 322) is **never called** ❌
5. Unity GUI stack remains in "horizontal layout" state
6. Next GUI render cycle detects mismatched stack → throws error

### Call Stack Analysis

```
User clicks "×" button
  ↓
SceneEditorWindow.OnGUI() [DrawDialogue() called]
  ↓
DrawDialogue() → for loop iteration i
  ↓
EditorGUILayout.BeginHorizontal() [Stack: +1 horizontal]
  ↓
GUILayout.Button("×") returns true
  ↓
Deletion code executes
  ↓
return; ← EXIT METHOD [Stack: still +1 horizontal] ❌
  ↓
Unity GUI render cycle completes
  ↓
Unity.GUIUtility.ProcessEvent() detects mismatched stack
  ↓
GUI Error: "Invalid GUILayout state"
```

## Git History Insights

### Recent Changes

**Commit**: `7ef9131` - "fix(editor): delete sub-assets when removing DialogueLine/Choice" (2026-03-10, previous fix)
- **Suspicion Level**: **HIGH** (introduced `return` statement)
- **Analysis**: Previous fix added deletion logic (`RemoveObjectFromAsset`), но скопировал `return` statement из оригинального кода
- **Original Code** (before commit 7ef9131):
  ```csharp
  if (GUILayout.Button("×", GUILayout.Width(25)))
  {
      dialogueProperty.DeleteArrayElementAtIndex(i);
      serializedObject.ApplyModifiedProperties();
      return;  // ← This was ALREADY BUGGY in original code!
  }
  ```
- **Reason**: `return` was present in original code but didn't cause visible error because deletion was fast. After adding `RemoveObjectFromAsset()` + `SaveAssets()`, operations became slower → GUI error now visible.

**Root Cause Timing**: Regression introduced in commit 7ef9131 (но `return` существовал в оригинальном коде).

## Edge Cases Analysis

**None applicable** — это не edge case. Ошибка проявляется в **100% случаев** при нажатии кнопки "×".

## Confidence Score

**98%**

### Reasoning

1. **Root cause trivial and obvious**: Early `return` before `EndHorizontal()` — objectively verified
2. **100% reproducible**: Every "×" button click triggers error
3. **Direct evidence**: GUI error message explicitly states "Begin/End calls mismatch"
4. **Pattern repeated** 3 times (DialogueLine, Choice, Character deletion handlers)
5. **Unity IMGUI documentation** confirms Begin/End balance requirement

**Why not 100%?**
- 2% reserved for possibility that Unity has specific edge case where early return is acceptable (unlikely)

## Recommended Fix Approach

### Option 1: Remove `return` statement (SIMPLE FIX)

```csharp
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
    
    if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
    {
        AssetDatabase.RemoveObjectFromAsset(subAsset);
        EditorUtility.SetDirty(_currentScene);
    }
    
    dialogueProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    AssetDatabase.SaveAssets();
    
    // ✅ REMOVE return; statement
    // Let method continue to EndHorizontal()
}
```

**Issue with Option 1**: Loop continues after deletion → might cause index shifting issues or processing deleted item.

### Option 2: Break из loop вместо return (RECOMMENDED)

```csharp
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
    
    if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
    {
        AssetDatabase.RemoveObjectFromAsset(subAsset);
        EditorUtility.SetDirty(_currentScene);
    }
    
    dialogueProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    AssetDatabase.SaveAssets();
    
    break;  // ✅ EXIT LOOP (not method) → EndHorizontal() will be called
}
```

**Benefit**: Exits loop immediately (prevents processing deleted item), but allows `EndHorizontal()` to execute.

### Option 3: Call EndHorizontal() before return (VERBOSE)

```csharp
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    EditorGUILayout.EndHorizontal();  // ✅ BALANCE BEFORE RETURN
    
    var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
    
    if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
    {
        AssetDatabase.RemoveObjectFromAsset(subAsset);
        EditorUtility.SetDirty(_currentScene);
    }
    
    dialogueProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    AssetDatabase.SaveAssets();
    
    return;
}
```

**Issue**: Breaks semantic flow (EndHorizontal before deletion logic), more verbose.

### Recommended Fix: **Option 2** (`break` вместо `return`)

**Reasoning**:
- ✅ Maintains GUI balance (EndHorizontal called)
- ✅ Exits loop immediately (no index shifting issues)
- ✅ Minimal code change (1 word: `return` → `break`)
- ✅ Semantic clarity (exit loop, not method)

### Changes Required

**3 locations** (DialogueLine, Choice, Character):
1. Line 319: `return;` → `break;`
2. Line 424: `return;` → `break;`
3. Line 231: `return;` → `break;`

**Total**: 3 lines changed (1-word replacement each)

## Constitution Compliance

**After fix**:
- ✅ **Principle VI (Code Style)**: Allman braces maintained
- ✅ **Unity Best Practices**: GUI Begin/End balance enforced
- ✅ **AI Development Constraints**: Only .cs files modified

## Risk Assessment

**Risk Level**: **Very Low**

**Reasons**:
1. ✅ Trivial change (1 word per location)
2. ✅ Fix is well-understood (Unity GUI balance requirement)
3. ✅ No logic changes (just control flow adjustment)
4. ✅ Easy to test (click "×" → no GUI error)

**Potential Issues**: None identified.
