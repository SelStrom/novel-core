# Bug Description

При удалении DialogueLine или Choice через кнопку "×" в SceneEditorWindow возникает GUI error:

```
GUI Error: Invalid GUILayout state in SceneEditorWindow view. Verify that all layout Begin/End calls match
UnityEngine.GUIUtility:ProcessEvent (int,intptr,bool&)
```

## Expected Behavior

При нажатии кнопки "×":
1. Sub-asset должен быть удален из AssetDatabase
2. Ссылка должна быть удалена из массива
3. GUI должен корректно обновиться **без ошибок**
4. `BeginHorizontal` и `EndHorizontal` вызовы должны быть сбалансированы

## Actual Behavior

При нажатии кнопки "×":
1. ✅ Sub-asset удаляется корректно (fix из commit 7ef9131)
2. ✅ Ссылка удаляется из массива
3. ❌ **GUI Error**: "Invalid GUILayout state" появляется в Console
4. ❌ `EndHorizontal()` не вызывается из-за `return` statement

## Steps to Reproduce

1. Открыть Scene Editor Window (`Window > NovelCore > Scene Editor`)
2. Загрузить любую SceneData с DialogueLines
3. Нажать кнопку "×" рядом с любой DialogueLine
4. **Observe**: DialogueLine удаляется, но в Console появляется GUI Error

## Root Cause

**File**: `SceneEditorWindow.cs`, method `DrawDialogue()`

**Problem**: `return` statement (line 319) выполняется **до** `EditorGUILayout.EndHorizontal()` (line 322), нарушая баланс Begin/End calls.

```csharp
EditorGUILayout.BeginHorizontal();  // Line 285

if (GUILayout.Button("×", GUILayout.Width(25)))
{
    // ... deletion code ...
    return;  // ❌ Line 319 - EXIT BEFORE EndHorizontal()!
}

EditorGUILayout.EndHorizontal();  // ❌ Line 322 - NEVER REACHED if button clicked
```

**Same Issue**: Choice deletion handler (lines 410-425) имеет ту же проблему.

## Environment

- Unity Version: 6000.0.69f1 (Unity 6 Long Term Support)
- Platform: macOS (darwin 25.2.0)
- NovelCore Version: dev (commit 7ef9131)
- Affected Components: SceneEditorWindow (DrawDialogue, DrawChoices methods)
