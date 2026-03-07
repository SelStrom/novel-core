# Testing Guide: Scene Transition MVP

**Date**: 2026-03-07  
**Status**: Ready for Testing  
**Feature**: Linear Scene Progression (User Story 1)

---

## 🧪 Testing Checklist

### Part 1: Create Test Scenes (Automated)

**Status**: ⏳ Ready to Execute

**Steps**:
1. ✅ Open Unity Editor (already running)
2. In Unity menu: **NovelCore → Testing → Create Linear Test Scenes**
3. Wait for success dialog
4. Verify 3 test scenes created in `Assets/Content/Projects/Test/Scenes/`

**Expected Result**:
- TestScene01_Start
- TestScene02_Middle  
- TestScene03_End

**Created By**: `LinearSceneTestGenerator.cs`

---

### Part 2: Verify Inspector UI

**Status**: ⏳ Pending User Action

**Steps**:
1. In Unity Project window, select `TestScene01_Start`
2. In Inspector, scroll to **Scene Transition** section
3. Verify **Next Scene** field shows `TestScene02_Middle`
4. Look for info box: "✓ Linear progression enabled"

**What to Check**:
- [x] Next Scene field is visible
- [ ] Next Scene field shows correct reference
- [ ] Info box appears with correct message
- [ ] No warning boxes (no choices defined)
- [ ] **Validate Scene** button at bottom works

**Screenshots Needed**: Inspector view showing nextScene field

---

### Part 3: Test Warning Messages

**Status**: ⏳ Pending User Action

**Steps**:
1. Select `TestScene01_Start`
2. In Inspector, **Narrative Content** → **Choices**
3. Add a dummy ChoiceData (click + button)
4. Verify **warning** appears: "⚠️ Both choices and nextScene are defined"

**Expected Warnings**:
- Yellow box when both choices AND nextScene exist
- Message explains choices take priority
- No errors, just warning

**Cleanup**: Remove the dummy ChoiceData after testing

---

### Part 4: Play Mode Test (Linear Progression)

**Status**: ⏳ Pending User Action

**Prerequisites**:
- Test scenes created (Part 1)
- GameStarter configured

**Setup Steps**:
1. Open `SampleScene` (or main game scene)
2. Find `GameStarter` GameObject in Hierarchy
3. In Inspector, set **Starting Scene** to `TestScene01_Start`
4. Save scene

**Test Steps**:
1. Press **Play ▶️**
2. Game should start with TestScene01
3. Read dialogue line 1: "Welcome to the linear progression test!"
4. **Click** to advance (or press Space)
5. Read lines 2-3
6. **Click** after final line

**Expected Results**:
- ✅ After line 3 of Scene01, **automatically** transitions to Scene02
- ✅ No manual scene selection needed
- ✅ Scene02 loads with its dialogue
- ✅ After Scene02, **automatically** transitions to Scene03
- ✅ After Scene03, dialogue ends (no nextScene defined)

**Console Output to Verify**:
```
DialogueSystem: Dialogue complete
DialogueSystem: Loading next scene via nextScene reference
DialogueSystem: Navigating to next scene: Test Scene 2: Middle
SceneManager: Loading scene Test Scene 2: Middle
DialogueSystem: Starting scene Test Scene 2: Middle
```

**Failure Cases** (should NOT happen):
- ❌ Scene01 ends with no transition
- ❌ Error: "Failed to load next scene"
- ❌ Scene transitions to wrong scene
- ❌ Game crashes or freezes

---

### Part 5: Run EditMode Tests

**Status**: ⏳ Pending User Action

**Steps**:
1. In Unity menu: **Window → General → Test Runner**
2. Click **EditMode** tab
3. Find `NovelCore.Tests.Editor.Data` folder
4. Should see:
   - `SceneDataValidationTests` (5 tests)
   - `SceneDataNextSceneTests` (5 tests)
5. Click **Run All**

**Expected Results**:
- ✅ All 10 tests **PASS** (green checkmarks)
- ⏱ Execution time: <1 second

**Tests Included**:

**SceneDataValidationTests**:
1. ✓ SceneData_WithValidNextScene_PassesValidation
2. ✓ SceneData_WithoutSceneId_FailsValidation
3. ✓ SceneData_WithBothChoicesAndNextScene_ShowsWarning
4. ✓ SceneData_WithNoDialogueOrChoices_ShowsWarning
5. ✓ SceneData_NextSceneProperty_ReturnsCorrectValue

**SceneDataNextSceneTests**:
1. ✓ NextScene_WhenSet_ReturnsCorrectReference
2. ✓ NextScene_WhenNotSet_ReturnsNull
3. ✓ SceneData_WithNextScene_HasCorrectProperty
4. ✓ SceneData_CanHaveNullNextScene_WithoutErrors
5. ✓ SceneData_NextSceneProperty_IsReadOnly

**Failure Scenarios** (if tests fail):
- Check Console for error details
- Verify Unity compilation completed successfully
- Restart Unity Editor if needed

---

### Part 6: Edge Case Testing

**Status**: ⏳ Optional

**Test Case 1: Scene with no nextScene**
- Select `TestScene03_End`
- Verify **No nextScene** defined in Inspector
- Info box should say: "ℹ️ No nextScene or choices defined"
- Play and verify dialogue just ends (no crash)

**Test Case 2: Invalid nextScene reference**
- Create new test scene
- Set nextScene to a scene, then delete that scene
- Save and try to play
- Should see error: "Failed to load next scene" (but no crash)

**Test Case 3: Circular reference (manual)**
- Create SceneA → SceneB → SceneA
- Try to play
- Should eventually hit max depth limit or runtime protection

---

## 📊 Test Results Template

**Date Tested**: _____________  
**Unity Version**: 6000.0.69f1  
**Tester**: _____________

### Results

| Test Part | Status | Notes |
|-----------|--------|-------|
| 1. Create Test Scenes | ⬜ Pass / ⬜ Fail | |
| 2. Inspector UI | ⬜ Pass / ⬜ Fail | |
| 3. Warning Messages | ⬜ Pass / ⬜ Fail | |
| 4. Play Mode (Linear) | ⬜ Pass / ⬜ Fail | |
| 5. EditMode Tests | ⬜ Pass / ⬜ Fail | |
| 6. Edge Cases | ⬜ Pass / ⬜ Fail | |

### Issues Found

1. _____________________________________________
2. _____________________________________________
3. _____________________________________________

### Console Errors

```
(paste any errors here)
```

### Screenshots

- [ ] Inspector with nextScene field
- [ ] Warning message (choices + nextScene)
- [ ] Play Mode showing Scene01
- [ ] Automatic transition to Scene02
- [ ] Test Runner results

---

## 🎯 Success Criteria

**Minimum to Pass**:
- ✅ Test scenes created successfully
- ✅ nextScene field visible in Inspector
- ✅ Play Mode: Scene01 → Scene02 → Scene03 automatically
- ✅ All 10 EditMode tests PASS
- ✅ No crashes or compilation errors

**Optional (Nice to Have)**:
- ✅ Warning messages display correctly
- ✅ Edge cases handled gracefully
- ✅ Console messages are clear and helpful

---

## 🐛 Known Issues

*None yet - this is the first test run!*

---

## 📝 Next Steps After Testing

**If All Tests Pass**:
1. Mark T021, T022 as complete in tasks.md
2. Create git commit with test results
3. Proceed to User Story 2 (Choice Validation)
4. Continue with User Story 3 (Navigation History)

**If Tests Fail**:
1. Document failure details
2. Check Console for errors
3. Fix issues in code
4. Recompile Unity
5. Re-run tests

---

**Testing Guide Status**: ✅ Ready  
**Automated Tools**: LinearSceneTestGenerator.cs created  
**Manual Testing**: Requires user interaction in Unity Editor  
**Estimated Time**: 10-15 minutes for full test suite
