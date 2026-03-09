# Testing Instructions: Preview Bridge Implementation

**Date**: 2026-03-07  
**Feature**: Editor-Runtime Preview Bridge (Iteration 7.6)  
**Status**: ✅ Implemented, Ready for Testing

## ✅ What Was Implemented

### 1. PreviewManager.cs (NEW)
**File**: `Assets/Scripts/NovelCore/Runtime/Core/PreviewManager.cs`

**Functionality**:
- Static API for managing preview state
- `SetPreviewScene(SceneData)` - Called by Scene Editor before Play Mode
- `GetPreviewScene()` - Called by GameStarter on initialization
- `IsPreviewMode` - Check if preview is active
- `ClearPreviewData()` - Cleanup after use
- EditorPrefs bridge for Editor→Runtime communication
- #if UNITY_EDITOR guards (no overhead in builds)

### 2. GameStarter.cs (UPDATED)
**File**: `Assets/Scripts/NovelCore/Runtime/Core/GameStarter.cs`

**Changes**:
- Added `GetSceneToLoad()` method
- Checks `PreviewManager.GetPreviewScene()` first
- Falls back to `_startingScene` if no preview
- Logs preview mode vs normal mode

### 3. Documentation Updates
- **Constitution**: Added Principle VIII (Editor-Runtime Bridge)
- **plan.md**: Added preview architecture section
- **tasks.md**: Added Iteration 7.6 with detailed tasks

---

## 🧪 Manual Testing Steps

### Test Case 1: Preview Scene from Scene Editor (Primary Workflow)

**Setup**:
1. Open Unity Editor with novel-core project
2. Open Scene Editor: `Window → NovelCore → Scene Editor`
3. Create or select 3 test scenes:
   - `Scene01_Introduction` (default starting scene in GameStarter Inspector)
   - `Scene02_Middle`
   - `Scene03_Ending`

**Test Steps**:
1. **Select Scene02_Middle** in Project window
   - Scene Editor should auto-load it
2. **Click "Preview Scene" button** in Scene Editor toolbar
   - Unity enters Play Mode
3. **Expected Result**: 
   - Console shows: `[Preview Mode] Loading preview scene: Scene02_Middle`
   - Scene02 dialogue/characters/background display
   - **NOT** Scene01_Introduction
4. **Exit Play Mode**
   - Preview state should be cleared automatically

**Success Criteria**:
- ✅ Correct scene loads (Scene02, not Scene01)
- ✅ Console logs show preview mode
- ✅ EditorPrefs cleared after use (check with `EditorPrefs.HasKey("NovelCore_PreviewScene")` in console)

---

### Test Case 2: Normal Play Mode (Default Behavior)

**Setup**:
1. Ensure `Scene01_Introduction` is assigned in GameStarter Inspector
2. Do NOT click "Preview Scene" button

**Test Steps**:
1. **Press Play ▶️ button** (normal Unity Play Mode)
2. **Expected Result**:
   - Console shows: `GameStarter: Loading default starting scene: Scene01_Introduction`
   - Scene01 loads normally
   - No preview mode messages

**Success Criteria**:
- ✅ Default scene loads (Scene01)
- ✅ No interference from preview system
- ✅ Game starts normally

---

### Test Case 3: Preview with Invalid Scene (Error Handling)

**Setup**:
1. Manually set invalid EditorPrefs (simulate corruption)
   - Open Unity Console
   - Execute: `EditorPrefs.SetString("NovelCore_PreviewScene", "invalid/path")`
2. Press Play ▶️

**Test Steps**:
1. **Enter Play Mode**
2. **Expected Result**:
   - Console warning: `PreviewManager: Failed to load preview scene at path: invalid/path`
   - Falls back to Scene01_Introduction
   - No crash or null reference errors

**Success Criteria**:
- ✅ Graceful fallback to default scene
- ✅ Warning logged (not error)
- ✅ Game continues normally

---

### Test Case 4: Multiple Preview Attempts (State Cleanup)

**Setup**:
1. Select Scene02 in Scene Editor
2. Click "Preview Scene" → Exit Play Mode
3. Select Scene03 in Scene Editor
4. Click "Preview Scene" → Enter Play Mode

**Test Steps**:
1. **First preview**: Scene02 should load
2. **Exit Play Mode**
3. **Second preview**: Scene03 should load (not Scene02)

**Expected Result**:
- First preview loads Scene02
- EditorPrefs cleared after first use
- Second preview loads Scene03 (fresh state)

**Success Criteria**:
- ✅ No stale data from previous preview
- ✅ Each preview loads correct scene
- ✅ State cleanup works properly

---

## 🔍 Verification Checklist

### Code Verification
- [x] PreviewManager.cs compiles without errors
- [x] GameStarter.cs compiles without errors
- [x] No linter warnings
- [x] #if UNITY_EDITOR guards present (build safety)

### Constitution Compliance
- [x] Principle I: "Preview Scene" button provides immediate feedback
- [x] Principle VIII: Editor-Runtime bridge implemented correctly
- [x] Principle VI: Modular architecture (PreviewManager isolated)

### Documentation
- [x] Constitution updated with Principle VIII
- [x] plan.md includes preview architecture diagram
- [x] tasks.md includes Iteration 7.6
- [x] ARCHITECTURE_ANALYSIS_PREVIEW.md documents problem/solution

---

## 🐛 Known Issues / Future Work

### Not Yet Implemented (Deferred to Future Iterations)
- [ ] Preview from specific dialogue line (T040.7 integration tests)
- [ ] Preview with choice point override
- [ ] Visual indicator in Scene Editor showing preview status
- [ ] Auto-stop Play Mode after preview scene completes

### Integration Tests (Post-MVP)
- [ ] Automated test for preview workflow
- [ ] Test preview state cleanup
- [ ] Test fallback behavior
- [ ] Test normal vs preview mode distinction

---

## 📝 Testing Notes

### If Preview Doesn't Work

**Problem**: Clicking "Preview Scene" loads Scene01 instead of selected scene

**Diagnosis**:
1. Check Console for logs:
   - Should see: `[Preview Mode] Set preview scene: SceneName`
   - Then: `[Preview Mode] Loading preview scene: SceneName`
2. Check EditorPrefs manually:
   ```csharp
   // In Unity Console window
   Debug.Log(EditorPrefs.GetString("NovelCore_PreviewScene"));
   ```

**Common Causes**:
- Unity instance was already running when code changed (restart Unity)
- GameStarter not present in scene (add GameStarter GameObject)
- GameStarter not injected correctly (check GameLifetimeScope exists)
- SceneEditorWindow.PreviewScene() not setting EditorPrefs (code issue)

### If Normal Play Mode Breaks

**Problem**: Normal Play Mode (▶️ button) shows preview logs

**Diagnosis**:
1. EditorPrefs not cleared properly
2. Manual fix:
   ```csharp
   // In Unity Console
   EditorPrefs.DeleteKey("NovelCore_PreviewScene");
   ```

---

## ✅ Success Indicators

The feature is working correctly if:

1. **Scene Editor "Preview Scene" button loads the selected scene** (not default)
2. **Normal Play Mode loads default starting scene** (unchanged behavior)
3. **Console logs clearly show preview vs normal mode**
4. **EditorPrefs cleared automatically after preview**
5. **No errors or warnings during preview workflow**
6. **Graceful fallback if preview scene invalid**

---

## 🚀 Next Steps After Testing

If all tests pass:
1. Update tasks.md: Mark T040.5 as complete
2. Commit changes with message: `feat: implement Editor-Runtime preview bridge (Iteration 7.6)`
3. Update user-manual.md with preview workflow documentation
4. Consider adding visual feedback to Scene Editor (preview mode indicator)

If tests fail:
1. Document failure scenario in this file
2. Add debug logs to identify root cause
3. Fix issue and re-test
4. Update ARCHITECTURE_ANALYSIS_PREVIEW.md with lessons learned

---

**Status**: ⏳ Awaiting manual testing in Unity Editor  
**Tester**: User (selstrom)  
**Estimated Testing Time**: 10-15 minutes for all test cases
