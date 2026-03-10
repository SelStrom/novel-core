# Manual Test Instructions for Sub-Asset Deletion Fix

## Test Scenario 1: Create and Delete DialogueLine

1. **Open Unity Editor** (6000.0.69f1)
2. **Open Scene Editor Window**: `Window > NovelCore > Scene Editor`
3. **Load Sample Scene**: Select `Sample/Scenes/Scene01_Introduction.asset`
4. **Create New DialogueLine**:
   - Click "+ Add Dialogue Line"
   - New DialogueLine should appear in list
5. **Verify Sub-Asset Creation**:
   - In Project Browser, expand `Scene01_Introduction.asset`
   - Should see new sub-asset `dialog_line_XXX` (snake_case, zero-padded)
   - Should NOT see standalone `DialogueLine N.asset` file
6. **Delete DialogueLine**:
   - Click "×" button next to the newly created DialogueLine
   - DialogueLine disappears from list ✅
7. **Verify Sub-Asset Deletion** (THIS IS THE FIX):
   - In Project Browser, expand `Scene01_Introduction.asset` again
   - ✅ **EXPECTED (AFTER FIX)**: `dialog_line_XXX` sub-asset SHOULD BE GONE
   - ❌ **BUG (BEFORE FIX)**: sub-asset would still exist as "orphaned" asset

## Test Scenario 2: Create and Delete Choice

1. **Same steps as Scenario 1**, but:
   - Use "+ Add Choice" button
   - Verify `choice_XXX` sub-asset created
   - Delete choice via "×" button
   - Verify `choice_XXX` sub-asset deleted ✅

## Test Scenario 3: Delete Pre-Existing DialogueLine

1. **Load Scene with Existing DialogueLines**: `Sample/Scenes/Scene01_Introduction.asset`
2. **Expand Scene in Project Browser**: Should see existing `dialog_line_001`, `dialog_line_002`, etc.
3. **Delete One DialogueLine**: Click "×" on any line
4. **Verify Sub-Asset Deleted**: Expand scene again, deleted sub-asset should be GONE ✅

## Test Scenario 4: Edge Case - Standalone Asset Reference (Should NOT Delete)

1. **Create Standalone DialogueLineData**:
   - Right-click in Project Browser → Create → NovelCore → Dialogue Line
   - Name it `TestStandaloneDialogueLine.asset`
2. **Reference It in SceneData** (manual edit):
   - Select `Scene01_Introduction.asset` in Inspector
   - Manually drag `TestStandaloneDialogueLine.asset` into `Dialogue Lines` array
3. **Delete Reference via Scene Editor**:
   - Open Scene Editor, load scene
   - Click "×" to remove the reference
4. **Verify Standalone Asset NOT Deleted**:
   - ✅ **EXPECTED**: `TestStandaloneDialogueLine.asset` file STILL EXISTS in Project Browser
   - Reason: `AssetDatabase.IsSubAsset()` check prevents deleting standalone assets

## Expected Results Summary

| Action                          | Before Fix          | After Fix           |
|---------------------------------|---------------------|---------------------|
| Delete DialogueLine sub-asset   | ❌ Orphaned asset   | ✅ Asset deleted    |
| Delete Choice sub-asset         | ❌ Orphaned asset   | ✅ Asset deleted    |
| Delete standalone asset ref     | ✅ Asset remains    | ✅ Asset remains    |

## Automated Test Execution (Alternative)

If Unity Test Runner works:

```bash
# Run failing tests (should NOW PASS)
/Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
  -runTests -batchmode \
  -projectPath "/Users/selstrom/work/projects/novel-core/novel-core/novel-core" \
  -testPlatform EditMode \
  -testFilter "SceneEditorWindowSubAssetDeletionReproductionTests" \
  -testResults "./test_results.xml" \
  -logFile -
```

**Expected**: All tests PASS ✅
- `DeleteDialogueLine_ShouldDeleteSubAsset_NotJustArrayReference` → PASS
- `DeleteChoice_ShouldDeleteSubAsset_NotJustArrayReference` → PASS
- `DeleteMultipleDialogueLines_ShouldDeleteAllSubAssets` → PASS
