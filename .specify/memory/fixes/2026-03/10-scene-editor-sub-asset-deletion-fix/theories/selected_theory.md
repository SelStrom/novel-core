# Selected Theory

## Decision

**Selected**: **IMPL_THEORY** (Implementation Theory)

## Reasoning

### Theory Comparison

| Aspect                      | Spec Theory                    | Impl Theory                    |
|-----------------------------|--------------------------------|--------------------------------|
| **Confidence Score**        | 35%                            | **90%**                        |
| **Evidence Quality**        | LOW (indirect violation)       | **HIGH (direct code evidence)**|
| **Failing Test Alignment**  | WEAK                           | **STRONG**                     |
| **Constitution Alignment**  | Indirect violation (spirit)    | **Direct alignment (fix required)**|
| **Root Cause Coverage**     | PARTIAL (incomplete spec)      | **COMPLETE (missing API call)**|
| **Fix Complexity**          | Medium (spec + impl update)    | **Low (6 lines per handler)**  |

### Decision Rationale

#### 1. Evidence Strength: IMPL_THEORY overwhelmingly stronger

**IMPL_THEORY**:
- ✅ **Direct code evidence**: Deletion handlers (lines 305-310, 400-404) do NOT contain `AssetDatabase.RemoveObjectFromAsset()` call
- ✅ **Symmetry violation**: Creation uses `AddObjectToAsset()`, deletion does NOT use `RemoveObjectFromAsset()`
- ✅ **100% reproducible**: Every deletion leaves orphaned sub-asset
- ✅ **Precedent**: Previous fix (commit 8f7eb74) fixed **creation** but missed **deletion** (incomplete fix pattern)

**SPEC_THEORY**:
- ⚠️ **Indirect evidence**: Constitution requires "asset pipeline integrity" but does NOT explicitly describe sub-asset lifecycle
- ⚠️ **Speculative**: "Spirit of Constitution violated" — но нет explicit requirement нарушен
- ⚠️ **Low confidence**: Only 35% because spec doesn't clearly describe expected behavior for sub-asset deletion

#### 2. Failing Test Alignment: IMPL_THEORY perfectly explains test failure

**Failing Test**: `DeleteDialogueLine_ShouldDeleteSubAsset_NotJustArrayReference()`

**Test Expectation**:
```csharp
// After deleteArrayElementAtIndex(0):
Object[] subAssetsAfter = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
Assert.That(subAssetsAfter.Length, Is.EqualTo(0), "Bug: sub-asset NOT deleted");
```

**IMPL_THEORY Explanation**:
- Test fails because `DeleteArrayElementAtIndex()` does NOT call `AssetDatabase.RemoveObjectFromAsset()`
- Fix: Add `RemoveObjectFromAsset()` → test PASSES ✅

**SPEC_THEORY Explanation**:
- Test fails because... Constitution doesn't describe sub-asset lifecycle? (weak explanation)
- Fix would require: Update Constitution → Update impl → test passes (unnecessary complexity)

**Alignment**: **IMPL_THEORY perfectly aligns** with test failure mechanism.

#### 3. Root Cause Coverage: IMPL_THEORY identifies exact missing code

**IMPL_THEORY**: Points to **exact location** (lines 305-310, 400-404) and **exact missing code** (`AssetDatabase.RemoveObjectFromAsset()`)

**SPEC_THEORY**: Points to "Constitution incomplete" but doesn't identify what code needs to change

**Actionability**: **IMPL_THEORY provides immediate actionable fix** (add 6 lines per handler).

#### 4. Constitution Compliance: Both theories acknowledge Principle III, but IMPL provides fix

**Constitution Principle III** (Asset Pipeline Integrity):
- Requirement: "No Broken References: Missing asset references MUST be detected"
- Requirement: "Dependency Tracking: Moving/renaming assets MUST automatically update all references"

**Analysis**:
- Orphaned sub-assets technically **не являются broken references** (sub-asset exists, но не referenced)
- Они нарушают **asset pipeline cleanliness** (мусорные assets)

**Both theories agree**: Bug violates **spirit** of Principle III.

**Difference**:
- SPEC_THEORY suggests: Update Constitution to explicitly require sub-asset lifecycle management
- IMPL_THEORY suggests: Fix implementation now, optionally enhance Constitution later

**Practical choice**: **IMPL_THEORY** — fix implementation first (blocks user pain immediately), Constitution update can follow (preventive measure).

### Rejected Theory

**Rejected**: **SPEC_THEORY** (Specification Theory)

#### Reason for Rejection

1. **Low Confidence (35%)**: Weak evidence that Constitution is root cause
2. **Indirect Violation**: Constitution doesn't explicitly describe sub-asset deletion behavior
3. **Unnecessary Complexity**: Fixing spec THEN impl is slower than fixing impl directly
4. **Missing Implementation Evidence**: Spec theory doesn't explain WHY code is missing `RemoveObjectFromAsset()` call
5. **Precedent Contra-Indicates Spec Issue**: Previous fix (8f7eb74) was pure implementation fix (no spec changes needed)

#### What Spec Theory Got Right

- ✅ Correct observation: Constitution COULD be enhanced with explicit sub-asset lifecycle guidance
- ✅ Correct long-term recommendation: Adding sub-asset lifecycle requirement to Principle III would prevent similar bugs

#### What Spec Theory Got Wrong

- ❌ Wrong primary root cause: Implementation bug, NOT spec bug
- ❌ Overstated confidence in spec incompleteness: Constitution's "Asset Pipeline Integrity" principle is sufficient guidance for implementer to infer sub-asset cleanup is needed

## Confidence in Decision

**95%**

### Justification

**Very high confidence** because:

1. **Code evidence is unambiguous**: Missing `RemoveObjectFromAsset()` call is **objectively true**
2. **Symmetry argument is strong**: Creation has `AddObjectToAsset()`, deletion MUST have `RemoveObjectFromAsset()`
3. **Previous fix pattern**: Commit 8f7eb74 fixed creation (impl-only, no spec changes) — current bug is symmetric deletion issue
4. **Test failure mechanism**: Failing test directly verifies that `AssetDatabase.LoadAllAssetRepresentationsAtPath()` still returns orphaned sub-asset
5. **Low risk fix**: Implementation fix is 12 lines total, well-understood Unity API, easy to test

**Why not 100%?**
- 5% possibility that there's a non-obvious reason why SceneEditorWindow intentionally doesn't delete sub-assets (unlikely, but can't be 100% certain without asking original implementer)

## Recommended Fix Strategy

### Primary Action: Fix Implementation (IMPL_THEORY)

**Changes Required**:

#### File: `novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`

**1. DialogueLine Deletion** (Lines 305-310):
```csharp
// BEFORE
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    dialogueProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    return;
}

// AFTER
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
    
    return;
}
```

**2. Choice Deletion** (Lines 400-404):
```csharp
// BEFORE
if (GUILayout.Button("×", GUILayout.Width(25)))
{
    choicesProperty.DeleteArrayElementAtIndex(i);
    serializedObject.ApplyModifiedProperties();
    return;
}

// AFTER
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

**Total Changes**: ~12 lines added (6 per handler), 0 lines removed

### Secondary Action: Enhance Constitution (Optional, Post-Fix)

**Recommendation**: After fix is validated, consider adding explicit sub-asset lifecycle guidance to Constitution.

**Proposed Addition to Principle III** (Asset Pipeline Integrity):

```markdown
### Sub-Asset Lifecycle Management

When using Unity sub-assets (`AssetDatabase.AddObjectToAsset()`), editor tools MUST maintain lifecycle symmetry:

- **Creation**: Use `AssetDatabase.AddObjectToAsset(subAsset, parentAsset)` to embed sub-asset
- **Deletion**: Use `AssetDatabase.RemoveObjectFromAsset(subAsset)` before removing reference from parent
- **Validation**: Check `AssetDatabase.IsSubAsset(asset)` to distinguish sub-assets from standalone assets
- **Orphaned Asset Prevention**: Removing a reference to a sub-asset from parent MUST also delete the sub-asset from AssetDatabase

**Rationale**: Orphaned sub-assets clutter the Project Browser, increase asset database size, and violate the principle of "scene/parent as atomic unit". Creator should not see unreferenced sub-assets when inspecting parent asset.
```

**Priority**: Low (Constitution update is **preventive**, fix is **corrective** — fix first, enhance Constitution later)

## Next Steps

### Immediate (FixAgent)

1. ✅ Apply implementation fix to `SceneEditorWindow.cs` (lines 305-310, 400-404)
2. ✅ Compile Unity project → verify no errors
3. ✅ Run failing tests → verify tests NOW PASS
4. ✅ Run full EditMode test suite → verify no regressions
5. ✅ Manual test: Create DialogueLine → Delete → Verify sub-asset removed from Inspector

### Post-Fix (Commit Stage)

1. ✅ Commit fix with message: `fix(editor): delete sub-assets when removing DialogueLine/Choice`
2. ✅ Archive fix documentation to `.specify/memory/fixes/2026-03/10-scene-editor-sub-asset-deletion/`
3. ⚠️ **Constitution Review** (MANDATORY after fix):
   - Check if 3+ similar "orphaned asset" fixes exist
   - If pattern detected → update Constitution Principle III with sub-asset lifecycle guidance

### Long-Term (Constitution Update)

- **Trigger**: If 3+ fixes related to sub-asset lifecycle accumulate
- **Action**: Update Constitution Principle III with explicit sub-asset lifecycle requirements
- **Rationale**: Prevent future developers from making same mistake

## Summary

**Primary Root Cause**: **Missing `AssetDatabase.RemoveObjectFromAsset()` call** in SceneEditorWindow deletion handlers.

**Fix Approach**: Add sub-asset deletion logic (6 lines per handler) before `DeleteArrayElementAtIndex()`.

**Constitution Status**: Current Constitution is sufficient (Asset Pipeline Integrity principle applies), but MAY be enhanced post-fix with explicit sub-asset lifecycle guidance.

**Confidence**: 95% that this fix resolves the bug without regressions.
