# Relevant Files

## Primary Files (directly affected)

### 1. `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`

**Location 1**: Method `DrawDialogue()` (Lines 270-344)
- **Issue**: `return` at line 319 exits before `EditorGUILayout.EndHorizontal()` at line 322
- **Buggy Code**:
  ```csharp
  EditorGUILayout.BeginHorizontal();  // Line 285
  // ... GUI elements ...
  if (GUILayout.Button("×", GUILayout.Width(25)))
  {
      // ... deletion logic ...
      return;  // ❌ Line 319 - EXITS EARLY
  }
  EditorGUILayout.EndHorizontal();  // Line 322 - UNREACHABLE if button clicked
  ```

**Location 2**: Method `DrawChoices()` (Lines 365-449)
- **Issue**: `return` at line 424 exits before `EditorGUILayout.EndHorizontal()` at line 427
- **Buggy Code**: Identical pattern to DrawDialogue()
  ```csharp
  EditorGUILayout.BeginHorizontal();  // Line 390
  // ... GUI elements ...
  if (GUILayout.Button("×", GUILayout.Width(25)))
  {
      // ... deletion logic ...
      return;  // ❌ Line 424 - EXITS EARLY
  }
  EditorGUILayout.EndHorizontal();  // Line 427 - UNREACHABLE if button clicked
  ```

## Secondary Files (context)

### 2. `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`

**Location**: Method `DrawCharacters()` (Lines 195-268)
- **Comparison**: Character deletion (lines 227-232) has SAME pattern:
  ```csharp
  if (GUILayout.Button("×", GUILayout.Width(25)))
  {
      charactersProperty.DeleteArrayElementAtIndex(i);
      serializedObject.ApplyModifiedProperties();
      return;  // ❌ Same issue!
  }
  ```
- **Note**: Character deletion также имеет эту проблему (но не упомянут в user bug report)

## Pattern Analysis

**Common Pattern** (3 locations):
1. `EditorGUILayout.BeginHorizontal()`
2. GUI elements (Toggle, Button)
3. `if (GUILayout.Button("×")) { ... return; }`
4. `EditorGUILayout.EndHorizontal()` — **UNREACHABLE**

**Impact**: All 3 deletion handlers (DialogueLine, Choice, Character) имеют баланс Begin/End проблему.
