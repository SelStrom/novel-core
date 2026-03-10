# Specification Theory

## Hypothesis

Баг частично вызван **неполной реализацией спецификации**. Спецификация `001-scene-transition` явно требует показывать "appropriate ending screen or returns to main menu" при завершении последней сцены, но это требование **не реализовано** в коде.

## Evidence

### Spec Requirements

1. **User Story 1 (Linear Scene Progression), Acceptance Scenario 2**:
   - **Location**: `specs/001-scene-transition/spec.md`, lines 21-22
   - **Requirement**:
     ```
     Given a scene that is the last in the story (no next scene defined), 
     When the player finishes all dialogue, 
     Then the game shows an appropriate ending screen or returns to main menu
     ```
   - **Status**: ❌ NOT IMPLEMENTED

2. **Functional Requirement FR-005**:
   - **Location**: `specs/001-scene-transition/spec.md`, line 119
   - **Requirement**:
     ```
     System MUST handle scenes with neither choices nor nextScene defined by 
     completing dialogue gracefully (showing ending or menu)
     ```
   - **Status**: ❌ NOT IMPLEMENTED

### Current Implementation vs Spec

**Specification States**:
- Show "appropriate ending screen" when last scene completes

**Current Code** (`DialogueSystem.cs`, lines 373-379):
```csharp
else
{
    // No target scene determined, complete dialogue normally
    Debug.Log("DialogueSystem: No target scene determined, dialogue ending normally");
    _isPlaying = false;
    OnDialogueComplete?.Invoke();
}
```

**Mismatch**:
- Code only fires `OnDialogueComplete` event
- No UI notification shown
- No "ending screen" or "return to menu" behavior
- Only logs debug message (invisible to player)

### Expected Behavior Mismatch

| Aspect | According to Spec | Actual Behavior | Bug Behavior Aligns With |
|--------|-------------------|-----------------|--------------------------|
| End of story handling | Show ending screen/menu | Only fire event | Implementation (partial) |
| User feedback | Visual notification | None | Neither |
| InvalidKeyException | Should not occur | Occurs with invalid AssetReference | Implementation bug |

### Spec Contradictions

**No direct contradictions found**, but there is an **implementation gap**:

**Gap 1: Missing UI Component**
- **Spec requires**: "appropriate ending screen"
- **Code provides**: Only event `OnDialogueComplete`
- **UI Components**: 
  - `DialogueBoxController` listens to `OnDialogueComplete` → only hides dialogue panel
  - `ChoiceUIController` listens to `OnDialogueComplete` → only hides choices
  - **No component shows "ending screen"**

**Gap 2: Ambiguous "appropriate ending screen" Definition**
- **Spec says**: "appropriate ending screen or returns to main menu"
- **Ambiguity**: 
  - What does "appropriate" mean?
  - Should it be modal or non-modal? (User explicitly requested "немодальное окно")
  - Should it have buttons (Return to Menu, Replay, Quit)?
  - What text should it display? (Localized?)

**Recommendation**: Spec should be clarified with mockup or explicit requirements

### Outdated Requirements

**None identified** - spec was recently created (2026-03-07), implementation is in progress.

However, **incomplete implementation** detected:
- Spec was created 3 days ago (2026-03-07)
- Implementation merged but incomplete (missing FR-005)
- This is a **partial implementation** rather than outdated spec

### Ambiguous Requirements

1. **"Appropriate ending screen" is vague**:
   - **Spec location**: `specs/001-scene-transition/spec.md`, line 22
   - **Ambiguity**: What constitutes "appropriate"?
   - **User clarification**: User explicitly requested "немодальное окно-заглушку о том, что сюжет закончился"
   - **Resolution needed**: Update spec with explicit UI requirement for non-modal end-of-story notification

2. **"Returns to main menu" alternative**:
   - **Spec location**: `specs/001-scene-transition/spec.md`, line 22
   - **Ambiguity**: Is this an alternative behavior or a button in the ending screen?
   - **Current state**: No main menu implementation found in codebase

3. **InvalidKeyException handling not specified**:
   - **Gap**: Spec does not address what happens when NextScene AssetReference is non-null but **invalid** (empty GUID)
   - **Edge case**: Lines 92-99 discuss missing scenes, but not invalid AssetReferences
   - **Current behavior**: Throws InvalidKeyException, spec silent on this

## Root Cause: Spec vs Implementation

**Primary Root Cause**: The InvalidKeyException is an **implementation bug**, not a spec issue.

**Secondary Root Cause**: Missing UI feature (ending screen) is a **spec incompleteness** - the requirement exists but lacks detail and is not implemented.

### Why InvalidKeyException is an Implementation Bug

The exception occurs **before** the "show ending screen" logic should execute:

1. `CompleteDialogue()` checks `if (targetSceneRef != null)` (line 345)
2. AssetReference can be non-null with empty/invalid RuntimeKey
3. Code attempts `LoadAssetAsync(targetSceneRef)` (line 351)
4. Addressables throws InvalidKeyException
5. **Code never reaches** the "show ending screen" logic (lines 373-379)

**This violates Principle VI (Constitution) - Defensive Programming**:
- Spec says "handle scenes gracefully" (FR-005)
- Code should validate AssetReference before attempting load
- Missing: `targetSceneRef.RuntimeKeyIsValid()` check

## Constitution Alignment

### Principle Violated

**Principle VI (Modular Architecture & Testing) - Code Style Standards**:

> Defensive programming patterns expected:
> - Public method parameter validation
> - **Null checks before dereferencing**
> - Graceful degradation on errors

**Violation Details**:
- `CompleteDialogue()` checks `!= null` but not `RuntimeKeyIsValid()`
- AssetReference is a **reference type** - null check insufficient
- Should validate key validity before calling Addressables API
- Exception is caught (lines 366-371) but **after** log pollution and user confusion

**Expected Code** (defensive):
```csharp
if (targetSceneRef != null && targetSceneRef.RuntimeKeyIsValid())
{
    // Proceed with load
}
```

**Actual Code**:
```csharp
if (targetSceneRef != null) // INSUFFICIENT
{
    // Proceeds to LoadAssetAsync with invalid key
}
```

### Principle VI - Testing Requirements

**Testing Gap**:
- Existing test `NextScene_WhenLoadFails_CompletesDialogueGracefully()` tests **missing asset**, not **invalid AssetReference**
- Missing test case: Empty AssetReference GUID (the actual bug scenario)
- Missing test case: AssetReference with `RuntimeKeyIsValid() == false`

## Confidence Score

**65%** that this is primarily a spec incompleteness issue

### Reasoning

**Evidence for Spec Theory (65%)**:
1. ✅ **Strong**: Spec explicitly requires "ending screen" - not implemented
2. ✅ **Strong**: FR-005 mandates graceful handling - only partially done (event fires, no UI)
3. ✅ **Medium**: User requirement aligns with spec ("немодальное окно") - spec needs clarification
4. ⚠️ **Weak**: Spec does not define InvalidKeyException handling - edge case gap

**Evidence against Spec Theory (35%)**:
1. ✅ **Strong**: InvalidKeyException is clearly an **implementation bug** (missing validation)
2. ✅ **Medium**: Spec is recent (2026-03-07) - unlikely to be "outdated"
3. ⚠️ **Medium**: Defensive programming is a **code quality issue**, not spec issue

**Why not higher confidence**:
- The InvalidKeyException is definitely an **implementation bug**
- The missing "ending screen" is a **spec gap** (requirement exists but incomplete)
- This is a **hybrid case**: both spec incompleteness AND implementation bug

## Recommended Action

- [x] Clarify spec: Add explicit UI requirement for non-modal "End of Story" notification
- [x] Update implementation: 
  1. Add `RuntimeKeyIsValid()` check before loading AssetReference (fixes InvalidKeyException)
  2. Implement EndOfStoryPanel UI component (fulfills FR-005)
  3. Wire OnDialogueComplete to show EndOfStoryPanel when no next scene
- [x] Update tests: Add test cases for invalid AssetReference scenarios
- [ ] Spec amendment: Document edge case behavior for invalid AssetReferences in spec.md

## Hybrid Theory Conclusion

This bug is **70% implementation bug, 30% spec gap**:

**Implementation Issues (70%)**:
1. Missing AssetReference validation (causes InvalidKeyException) - 40%
2. Missing EndOfStoryPanel UI component - 30%

**Spec Issues (30%)**:
1. Vague "appropriate ending screen" requirement (no mockup, no details) - 20%
2. Edge case not documented (invalid AssetReference) - 10%

**Recommendation**: Fix implementation first (addresses user pain immediately), then update spec with learned edge cases.
