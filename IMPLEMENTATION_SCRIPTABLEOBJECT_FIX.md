# Implementation Summary: ScriptableObject Asset Creation Fix

**Date**: 2026-03-07  
**Issue**: Dialog Line Data references missing in Sample Scene Project  
**Status**: ✅ COMPLETED

## Problem Description

### Root Cause
`DialogueLineData` ScriptableObjects were created with `ScriptableObject.CreateInstance<DialogueLineData>()` but never saved as asset files to disk. This caused:

- **Missing references** in Unity Inspector when viewing SceneData assets
- **Null references** at runtime when trying to access dialogue lines
- **Data loss** after Unity Editor restart (in-memory objects not persisted)

### Affected Code
**File**: `novel-core/Assets/Scripts/NovelCore/Editor/Tools/Generators/SampleProjectGenerator.cs`

**Problem locations**:
1. **Line 324**: `DialogueLineData` created but not saved as asset
2. **Line 302-307**: DialogueLineData instances added to SceneData before SceneData was saved

**Why it failed**:
```csharp
// ❌ BEFORE (INCORRECT)
private static DialogueLineData CreateDialogueLine(DialogueContent content)
{
    DialogueLineData line = ScriptableObject.CreateInstance<DialogueLineData>();
    // ... set fields ...
    return line; // NOT SAVED TO DISK!
}

private static SceneData CreateScene(...)
{
    SceneData scene = ScriptableObject.CreateInstance<SceneData>();
    // ... set fields ...
    
    // Create dialogue lines (in-memory only)
    var dialogueLines = new List<DialogueLineData>();
    foreach (var content in dialogueContents)
    {
        var line = CreateDialogueLine(content); // Returns unsaved object
        dialogueLines.Add(line);
    }
    
    SetPrivateField(scene, "_dialogueLines", dialogueLines);
    
    // Save scene AFTER adding unsaved references
    AssetDatabase.CreateAsset(scene, path); // ⚠️ References will be null!
    
    return scene;
}
```

**Unity behavior**:
- ScriptableObject instances created with `CreateInstance` exist only in memory
- When saved to an asset, Unity cannot serialize references to unsaved ScriptableObjects
- Inspector shows "Missing (ScriptableObject)" for these references
- After Editor restart, references become null

## Solution

### Approach: Sub-Asset Pattern

ScriptableObjects that are "owned" by a parent asset should be saved as **sub-assets** using `AssetDatabase.AddObjectToAsset()`.

**Benefits**:
- ✅ All objects saved to disk with proper references
- ✅ Hierarchical structure visible in Project window
- ✅ Single file contains parent + children (easier to manage)
- ✅ References preserved across Editor restarts

### Implementation

#### 1. Fixed CreateScene() Method

**Changes**:
```csharp
// ✅ AFTER (CORRECT)
private static SceneData CreateScene(
    string fileName,
    string sceneId,
    string sceneName,
    List<DialogueContent> dialogueContents,
    string backgroundName)
{
    // Create SceneData asset
    SceneData scene = ScriptableObject.CreateInstance<SceneData>();
    
    // ... set fields ...
    
    // ✅ CRITICAL: Save scene asset FIRST (required before adding sub-assets)
    string path = $"{SCENES_DIR}/{fileName}.asset";
    AssetDatabase.CreateAsset(scene, path);

    // ✅ Create dialogue lines as sub-assets
    var dialogueLines = new List<DialogueLineData>();
    int lineIndex = 0;
    foreach (var content in dialogueContents)
    {
        var line = CreateDialogueLine(content, fileName, lineIndex);
        dialogueLines.Add(line);
        
        // ✅ Add as sub-asset to the SceneData asset
        AssetDatabase.AddObjectToAsset(line, scene);
        lineIndex++;
    }

    SetPrivateField(scene, "_dialogueLines", dialogueLines);
    SetPrivateField(scene, "_characters", new List<CharacterPlacement>());
    SetPrivateField(scene, "_choices", new List<ChoiceData>());

    // ✅ Mark scene as dirty and save
    EditorUtility.SetDirty(scene);
    AssetDatabase.SaveAssets();
    
    Debug.Log($"[SampleProjectGenerator] Created scene: {path} with {dialogueLines.Count} dialogue lines");
    
    return scene;
}
```

**Key changes**:
1. **Save parent first**: `AssetDatabase.CreateAsset(scene, path)` must happen before adding sub-assets
2. **Create sub-assets**: Each `DialogueLineData` is created and added as sub-asset
3. **Set relationships**: After all sub-assets added, update parent's references
4. **Save all**: `EditorUtility.SetDirty()` + `AssetDatabase.SaveAssets()` persists everything

#### 2. Fixed CreateDialogueLine() Method

**Changes**:
```csharp
// ✅ AFTER (CORRECT)
private static DialogueLineData CreateDialogueLine(
    DialogueContent content, 
    string sceneFileName, 
    int lineIndex)
{
    DialogueLineData line = ScriptableObject.CreateInstance<DialogueLineData>();
    
    // ✅ Set name for better visibility in Project window
    line.name = $"{sceneFileName}_Line{lineIndex:D2}";
    
    SetPrivateField(line, "_lineId", content.lineId);
    SetPrivateField(line, "_emotion", content.emotion);
    SetPrivateField(line, "_textKey", content.lineId);
    SetPrivateField(line, "_fallbackText", content.fallbackText);
    SetPrivateField(line, "_displayDuration", -1f);
    SetPrivateField(line, "_characterAction", CharacterAction.None);
    
    // Note: Asset saving happens in CreateScene() via AddObjectToAsset()
    return line;
}
```

**Key changes**:
1. **Added parameters**: `sceneFileName` and `lineIndex` for naming
2. **Set object name**: `line.name` for Inspector/Project window display
3. **Return unsaved object**: Caller will add as sub-asset

#### 3. Fixed LinkScenesWithChoices() Method

**Changes**:
```csharp
private static void LinkScenesWithChoices(Dictionary<string, SceneData> scenes)
{
    // Create choice for Scene 2
    ChoiceData choice = ScriptableObject.CreateInstance<ChoiceData>();
    
    // ✅ Set name for better visibility
    choice.name = "Choice_MainDecision";
    
    // ... set fields ...

    // ✅ Save choice as a separate asset (ChoiceData can be reused across scenes)
    string choicePath = $"{SCENES_DIR}/Choice_MainDecision.asset";
    AssetDatabase.CreateAsset(choice, choicePath);
    AssetDatabase.SaveAssets();
    
    Debug.Log($"[SampleProjectGenerator] Created choice: {choicePath}");

    // Add choice to Scene 2
    var scene2Choices = new List<ChoiceData> { choice };
    SetPrivateField(scenes["scene2"], "_choices", scene2Choices);
    
    EditorUtility.SetDirty(scenes["scene2"]);
    
    // ✅ Save scene after updating choices
    AssetDatabase.SaveAssets();
}
```

**Key changes**:
1. **Set object name**: `choice.name` for clarity
2. **Save as separate asset**: ChoiceData saved independently (can be reused)
3. **Save parent scene**: After adding choice reference to scene

## Verification

### Code Review Checklist

✅ **SampleProjectGenerator.cs**: DialogueLineData saved as sub-assets  
✅ **SampleProjectGenerator.cs**: ChoiceData saved as separate asset  
✅ **SceneEditorWindow.cs**: Already correct (saves DialogueLineData as separate assets)  
✅ **CharacterEditorWindow.cs**: Already correct (saves CharacterData as separate asset)  
✅ **Test files**: Correct (CreateInstance without saving is OK for tests)

### Search Results

Searched for all `CreateInstance` calls with ScriptableObjects:

```bash
Grep: "CreateInstance.*(?:SceneData|DialogueLineData|ChoiceData|CharacterData)"
```

**Results**:
1. ✅ **SampleProjectGenerator.cs**: Fixed in this implementation
2. ✅ **SceneEditorWindow.cs**: Already correct (lines 575-578, 595-598)
3. ✅ **CharacterEditorWindow.cs**: Already correct (lines 365-368)
4. ✅ **Test files**: Correct pattern for tests (no asset saving needed)

### Manual Testing Steps

To verify the fix works:

1. **Delete existing Sample Project**:
   ```bash
   rm -rf Assets/Content/Projects/Sample/
   ```

2. **Regenerate Sample Project**:
   - Unity Editor → `NovelCore → Generate Sample Project`

3. **Verify in Project Window**:
   - Navigate to `Assets/Content/Projects/Sample/Scenes/`
   - Select `Scene01_Introduction.asset`
   - Should see sub-assets in Project window (foldout arrow next to asset)
   - Expand to see: `Scene01_Introduction_Line00`, `Scene01_Introduction_Line01`, etc.

4. **Verify in Inspector**:
   - Select `Scene01_Introduction.asset`
   - Expand "Narrative Content → Dialogue Lines"
   - All array elements should show DialogueLineData references (not "Missing")
   - Click each reference → should open in Inspector

5. **Test Runtime**:
   - Press Play ▶️ in Unity Editor
   - Game should load and display dialogue correctly
   - No null reference exceptions in Console

## Architecture Patterns

### When to Use Sub-Assets vs Separate Assets

**Use Sub-Assets (`AddObjectToAsset`)** when:
- ✅ Child objects are "owned" by parent (e.g., DialogueLineData owned by SceneData)
- ✅ Child objects are not reused across multiple parents
- ✅ Deleting parent should delete children
- ✅ Example: DialogueLineData → SceneData relationship

**Use Separate Assets (`CreateAsset`)** when:
- ✅ Objects can be reused across multiple parents (e.g., ChoiceData shared by scenes)
- ✅ Objects have independent lifecycle
- ✅ Users need to select/reference objects independently
- ✅ Example: ChoiceData, CharacterData

### ScriptableObject Creation Order

```csharp
// ✅ CORRECT ORDER for parent-child relationship:

// 1. Create parent ScriptableObject
var parent = ScriptableObject.CreateInstance<ParentType>();
parent.name = "ParentName";

// 2. Save parent as main asset
AssetDatabase.CreateAsset(parent, "Assets/Path/Parent.asset");

// 3. Create child ScriptableObjects
var child1 = ScriptableObject.CreateInstance<ChildType>();
child1.name = "Child1";
var child2 = ScriptableObject.CreateInstance<ChildType>();
child2.name = "Child2";

// 4. Add children as sub-assets
AssetDatabase.AddObjectToAsset(child1, parent);
AssetDatabase.AddObjectToAsset(child2, parent);

// 5. Update parent references
parent.children = new List<ChildType> { child1, child2 };

// 6. Mark dirty and save
EditorUtility.SetDirty(parent);
AssetDatabase.SaveAssets();
AssetDatabase.Refresh(); // Optional: Force Unity to update Project window
```

## Documentation Updates

### Files Updated

1. ✅ **SampleProjectGenerator.cs**: Code fixed
2. 📝 **IMPLEMENTATION_SCRIPTABLEOBJECT_FIX.md**: This document (new)
3. 📝 **specs/001-visual-novel-constructor/tasks.md**: Add note about ScriptableObject asset creation
4. 📝 **SAMPLE_PROJECT_GUIDE.md**: Update known limitations section

### Specification Notes

**Location**: `specs/001-visual-novel-constructor/tasks.md`

**Add to T040 notes**:
```markdown
**Important**: DialogueLineData must be saved as sub-assets of SceneData using 
`AssetDatabase.AddObjectToAsset()`. Do not use `CreateInstance` without saving to disk.

See `IMPLEMENTATION_SCRIPTABLEOBJECT_FIX.md` for details on ScriptableObject asset creation patterns.
```

### User Manual Updates

**Location**: `SAMPLE_PROJECT_GUIDE.md`

**Update "Known Limitations" section**:
```diff
-### 0. Нет автоматического запуска игры
+### ~~0. Нет автоматического запуска игры~~ ✅ FIXED (T040.3)

-**Проблема**: При нажатии Play ничего не происходит...
+**Статус**: Исправлено. GameStarter теперь создаётся автоматически.

+### ~~Dialog Line Data missing~~ ✅ FIXED (2026-03-07)
+
+**Проблема** (старая версия): В Inspector SceneData показывались "Missing (ScriptableObject)" 
+для диалоговых реплик.
+
+**Причина**: DialogueLineData создавались в памяти, но не сохранялись на диск.
+
+**Решение**: DialogueLineData теперь сохраняются как sub-assets SceneData. Проблема устранена.
```

## Testing Recommendations

### Automated Tests (Future Work)

Add integration test for Sample Project generation:

**File**: `Assets/Scripts/NovelCore/Tests/Editor/Tools/SampleProjectGeneratorTests.cs` (new)

```csharp
[Test]
public void GenerateSampleProject_CreatesValidDialogueLineAssets()
{
    // Arrange
    SampleProjectGenerator.GenerateSampleProject();
    
    // Act
    var scene = AssetDatabase.LoadAssetAtPath<SceneData>(
        "Assets/Content/Projects/Sample/Scenes/Scene01_Introduction.asset"
    );
    
    // Assert
    Assert.IsNotNull(scene, "Scene asset should exist");
    Assert.IsNotNull(scene.DialogueLines, "Dialogue lines should not be null");
    Assert.Greater(scene.DialogueLines.Count, 0, "Should have dialogue lines");
    
    foreach (var line in scene.DialogueLines)
    {
        Assert.IsNotNull(line, "Dialogue line reference should not be null");
        Assert.IsFalse(string.IsNullOrEmpty(line.LineId), "Line should have ID");
        Assert.IsFalse(string.IsNullOrEmpty(line.FallbackText), "Line should have text");
    }
}

[Test]
public void GenerateSampleProject_DialogueLinesAreSubAssets()
{
    // Arrange
    SampleProjectGenerator.GenerateSampleProject();
    
    // Act
    var scene = AssetDatabase.LoadAssetAtPath<SceneData>(
        "Assets/Content/Projects/Sample/Scenes/Scene01_Introduction.asset"
    );
    var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(
        AssetDatabase.GetAssetPath(scene)
    );
    
    // Assert
    Assert.Greater(subAssets.Length, 0, "Scene should have sub-assets");
    
    var dialogueSubAssets = subAssets.OfType<DialogueLineData>().ToList();
    Assert.AreEqual(
        scene.DialogueLines.Count, 
        dialogueSubAssets.Count,
        "Number of dialogue sub-assets should match scene dialogue lines"
    );
}
```

### Manual Regression Test

**Test case**: Sample Project regeneration after Editor restart

1. Generate Sample Project
2. Close Unity Editor
3. Reopen Unity Editor
4. Open `Scene01_Introduction.asset` in Inspector
5. Verify: All dialogue line references still valid (not missing)
6. Press Play ▶️
7. Verify: Dialogue displays correctly

## Related Issues

### Similar Patterns in Codebase

**SceneEditorWindow.cs** already implements correct pattern:

```csharp
// Lines 575-578 (already correct)
var newLine = CreateInstance<DialogueLineData>();
string linePath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/DialogueLine.asset");
AssetDatabase.CreateAsset(newLine, linePath); // ✅ Saved as separate asset
AssetDatabase.SaveAssets();
```

**Difference**:
- SceneEditorWindow creates DialogueLineData as **separate assets** (for manual editing)
- SampleProjectGenerator creates DialogueLineData as **sub-assets** (generated, not manually edited)

Both approaches are valid depending on use case.

## Constitution Compliance

✅ **Principle III - Pragmatic Problem Solving**: Fixed root cause (missing asset creation) rather than workaround

✅ **Principle VI - Modular Architecture**: Proper asset relationships ensure data integrity

✅ **Principle II - Data-Driven Content Creation**: ScriptableObject assets correctly saved for runtime use

## Commit Message

```
fix(sample): save DialogueLineData as sub-assets to prevent missing references

Problem: DialogueLineData created with CreateInstance() but never saved to disk,
causing "Missing (ScriptableObject)" references in Inspector.

Solution:
- Save SceneData asset BEFORE creating dialogue lines
- Use AssetDatabase.AddObjectToAsset() to save DialogueLineData as sub-assets
- Set object names for better Project window visibility
- Add AssetDatabase.SaveAssets() after all modifications

Changes:
- CreateScene(): Save scene first, then add dialogue line sub-assets
- CreateDialogueLine(): Add parameters for naming, return unsaved object
- LinkScenesWithChoices(): Save ChoiceData and scene after modifications

Testing:
- Verified all CreateInstance calls in codebase for correct asset saving
- SceneEditorWindow and CharacterEditorWindow already correct
- Test files correctly use CreateInstance without saving (expected)

See IMPLEMENTATION_SCRIPTABLEOBJECT_FIX.md for detailed analysis.

Closes #SCRIPTABLEOBJECT_MISSING_REFERENCES
```

---

**Implementation Time**: ~30 minutes  
**Complexity**: Medium (Unity asset management, sub-asset pattern)  
**Risk**: Low (isolated to SampleProjectGenerator, doesn't affect existing code)
