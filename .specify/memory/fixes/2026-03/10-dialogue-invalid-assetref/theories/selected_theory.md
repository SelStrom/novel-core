# Selected Theory

## Decision

**Selected**: IMPL_THEORY

## Reasoning

### Theory Comparison

| Aspect                     | Spec Theory      | Impl Theory      |
|----------------------------|------------------|------------------|
| Confidence Score           | 65%              | 95%              |
| Evidence Quality           | MEDIUM           | HIGH             |
| Failing Test Alignment     | WEAK             | STRONG           |
| Constitution Alignment     | PARTIAL          | ALIGNED          |
| Immediate Fix Complexity   | HIGH (spec update + UI) | LOW (2-line validation) |
| Root Cause Coverage        | Partial (missing UI) | Complete (validation bug) |

### Decision Rationale

1. **Evidence Strength**:
   - **Impl Theory** has direct stack trace proof pointing to exact line of code (DialogueSystem.cs:351)
   - **Spec Theory** identifies a real gap (missing EndOfStoryPanel) but this is a **separate feature**, not the cause of InvalidKeyException
   - InvalidKeyException occurs **before** any "show ending screen" logic could execute
   - Stack trace shows: `Addressables.LoadAssetAsync()` → `InvalidKeyException`, clearly an API misuse

2. **Test Alignment**:
   - **Failing test** specifically reproduces InvalidKeyException with empty AssetReference
   - **Impl Theory** explains exactly why test fails: missing `RuntimeKeyIsValid()` check
   - **Spec Theory** explains missing UI feature, but UI is irrelevant to the exception
   - Test demonstrates: `AssetReference("")` → InvalidKeyException, not "missing UI"

3. **Root Cause Coverage**:
   - **Impl Theory** explains the **entire bug**: 
     - Why InvalidKeyException occurs (invalid AssetReference not validated)
     - Why it happens on last dialogue (NextScene field used without validation)
     - How to fix it (add `RuntimeKeyIsValid()` check)
   - **Spec Theory** explains a **secondary issue**: missing EndOfStoryPanel UI
     - This is real but **does not cause the exception**
     - Spec requirement (FR-005) exists but is not the root cause of the bug
     - Even with EndOfStoryPanel implemented, InvalidKeyException would still occur

4. **Fix Complexity**:
   - **Impl Theory fix**: 2 lines of code
     ```csharp
     // Line 345: Add RuntimeKeyIsValid() check
     if (targetSceneRef != null && targetSceneRef.RuntimeKeyIsValid())
     ```
   - **Spec Theory fix**: Requires:
     - Clarify spec with UI mockup
     - Implement EndOfStoryPanel component (~100+ lines)
     - Wire up events
     - Add localization
     - **Still wouldn't fix InvalidKeyException**

5. **Constitution Alignment**:
   - **Impl Theory** directly addresses Principle VI (Defensive Programming):
     > "Null checks before dereferencing"
     > "Graceful degradation on errors"
   - AssetReference is a Unity API type that requires `RuntimeKeyIsValid()` - this is documented best practice
   - Missing this check violates defensive programming principle
   - **Spec Theory** addresses FR-005 but doesn't explain the exception

### Rejected Theory

**Rejected**: SPEC_THEORY (partially)

**Reason for Rejection as Primary Theory**:

Spec Theory is **not rejected entirely** - it identifies a **real gap**:
- Spec requires "show ending screen" (FR-005, User Story 1)
- Implementation missing EndOfStoryPanel UI
- This is a **legitimate spec incompleteness**

However, Spec Theory is rejected as the **primary root cause** of the InvalidKeyException bug because:

1. **InvalidKeyException occurs regardless of UI**:
   - Exception thrown at line 351 (LoadAssetAsync call)
   - UI notification would be shown later (after exception)
   - Even with EndOfStoryPanel implemented, exception still occurs

2. **Spec does not explain the exception**:
   - FR-005 says "handle gracefully"
   - InvalidKeyException is **not graceful handling**
   - Exception violates the spec requirement
   - Therefore, spec incompleteness didn't **cause** the exception - implementation did

3. **Hybrid Nature**:
   - This bug has **two components**:
     a. **Primary (90%)**: InvalidKeyException from missing validation (IMPL bug)
     b. **Secondary (10%)**: Missing EndOfStoryPanel UI (SPEC gap)
   - Impl Theory addresses (a), Spec Theory addresses (b)
   - User complaint focuses on (a): "возникает ряд экспшенов"

**Spec Theory is valid for follow-up work**:
- After fixing InvalidKeyException (Impl fix)
- Implement EndOfStoryPanel to fulfill FR-005
- Update spec with explicit UI requirements
- This is a **separate task**, not part of exception fix

## Confidence in Decision

**95%** confident that IMPL_THEORY is correct

### Justification

**Why so confident**:

1. ✅ **Stack trace is unambiguous**:
   ```
   UnityEngine.AddressableAssets.InvalidKeyException
   Keys=, Type=NovelCore.Runtime.Data.Scenes.SceneData
   ```
   - "Keys=" means empty key list
   - Direct result of passing invalid AssetReference to Addressables
   - No other interpretation possible

2. ✅ **Reproducible failing test**:
   - Test creates `AssetReference("")` (empty key)
   - Passes to DialogueSystem
   - DialogueSystem doesn't validate, passes to LoadAssetAsync
   - Exception thrown
   - **Impl Theory predicts this exactly**

3. ✅ **Unity API documentation confirms**:
   - AssetReference API docs: "Use RuntimeKeyIsValid() before loading"
   - Code violates this documented requirement
   - This is textbook API misuse

4. ✅ **Constitution compliance**:
   - Principle VI explicitly requires defensive programming
   - Missing RuntimeKeyIsValid() check violates this
   - Fix aligns code with constitution

5. ⚠️ **Only 95% not 100% because**:
   - Small chance (5%) of Unity/Addressables configuration issue
   - Possible that Addressables settings could prevent exception
   - However, exception message is too specific to be anything else

**Why Spec Theory scored 65%**:
- It correctly identifies missing EndOfStoryPanel (real issue)
- But this doesn't explain InvalidKeyException
- Spec gap is a **consequence** not a **cause**

## Recommended Fix Strategy

### Phase 1: Fix InvalidKeyException (IMPL fix) - IMMEDIATE

**Priority**: P0 (blocker bug)

**Changes**:

1. **DialogueSystem.cs**, line 345:
   ```csharp
   if (targetSceneRef != null && targetSceneRef.RuntimeKeyIsValid())
   ```

2. **AddressablesAssetManager.cs**, lines 41-43 (defensive):
   ```csharp
   if (key is AssetReference assetRef)
   {
       if (!assetRef.RuntimeKeyIsValid())
       {
           Debug.LogWarning($"AddressablesAssetManager: Invalid AssetReference");
           return Task.FromResult<T>(null);
       }
       handle = Addressables.LoadAssetAsync<T>(assetRef);
   }
   ```

3. **Add failing test** (already created in `.fix/bug_context/failing_test.cs`):
   - `EndOfStory_WithEmptyNextSceneReference_ShouldShowNotificationWithoutException()`
   - `EndOfStory_WithInvalidAssetReference_ShouldValidateBeforeLoad()`

**Expected Result**:
- ✅ No InvalidKeyException
- ✅ Dialogue completes gracefully
- ✅ OnDialogueComplete event fires
- ❌ Still no EndOfStoryPanel (that's Phase 2)

### Phase 2: Implement EndOfStoryPanel (SPEC fulfillment) - FOLLOW-UP

**Priority**: P1 (spec requirement, not blocker)

**Changes**:

1. Create `EndOfStoryPanel.cs`:
   - Non-modal UI panel with "Сюжет завершён" message
   - Subscribe to OnDialogueComplete
   - Show when story ends (no next scene)
   - Buttons: Return to Menu, Replay (optional)

2. Update spec:
   - Add explicit UI mockup for "ending screen"
   - Clarify FR-005 with UI requirements
   - Document non-modal vs modal behavior

3. Add UI tests:
   - Verify EndOfStoryPanel shows when last scene completes
   - Verify panel doesn't show for normal scene transitions

**This phase can be a separate task/PR** - it doesn't block the exception fix.

## Next Steps

1. ✅ **Proceed with FixAgent using IMPL_THEORY**
2. ✅ **Apply RuntimeKeyIsValid() validation**
3. ✅ **Run failing tests** - should pass after fix
4. ✅ **Run break tests** - ensure no regressions
5. ⚠️ **Create follow-up task**: Implement EndOfStoryPanel (spec requirement)

## Decision Log

**Date**: 2026-03-10
**Decision Maker**: TheoryArbitratorAgent
**Decision**: Select IMPL_THEORY as primary root cause
**Rationale**: Direct stack trace evidence, reproducible test, clear API misuse
**Follow-up**: Create task for EndOfStoryPanel implementation (SPEC_THEORY secondary issue)
