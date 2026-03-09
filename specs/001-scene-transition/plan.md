# Implementation Plan: Scene Transition Mechanics

**Branch**: `001-scene-transition` | **Date**: 2026-03-07 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `/specs/001-scene-transition/spec.md`

## Summary

This plan implements comprehensive scene transition mechanics for the visual novel constructor, addressing the critical gap where scenes without choices currently have no way to progress. The implementation adds five key capabilities: (1) automatic linear scene progression via nextScene references, (2) validation of existing choice-based branching, (3) scene navigation history for back/forward traversal, (4) conditional transitions based on game state, and (5) asset preloading for seamless transitions. The technical approach extends existing `SceneData`, `DialogueSystem`, and `SceneManager` infrastructure while maintaining backward compatibility.

## Technical Context

> **📘 Centralized Documentation**: 
> - Technical stack: [`.specify/memory/tech-stack.md`](../../.specify/memory/tech-stack.md)
> - Testing strategy: [`.specify/memory/testing-strategy.md`](../../.specify/memory/testing-strategy.md)
> - Project structure: [`.specify/memory/project-structure.md`](../../.specify/memory/project-structure.md)

**Feature-Specific Details**:
- **Storage**: JSON for scene navigation state (serialized via JsonUtility), integrated with existing SaveSystem
- **Constraints**: Scene history limited to 50 entries (memory management)
- **Scale/Scope**: Support for stories with 100+ scenes, complex branching narratives with 50+ decision points

## Constitution Check

> **📘 Project Constitution**: For complete principle definitions, see [`.specify/memory/constitution.md`](../../.specify/memory/constitution.md)

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Feature Compliance Summary

✅ **I. Creator-First Design** - SceneData Inspector nextScene field, Scene Editor preview, plain language validation  
✅ **II. Cross-Platform Parity** - Pure C# logic, no platform-specific code, automated cross-platform tests  
✅ **III. Asset Pipeline Integrity** - AssetReference for nextScene, editor-time validation, migration support  
✅ **IV. Runtime Performance Guarantees** - No performance overhead, scene history capped at 50 entries  
✅ **V. Save System Reliability** - Navigation history persisted, backward compatible format, cloud sync  
✅ **VI. Modular Architecture & Testing** - Extends existing assemblies, >80% coverage, mandatory test execution after each user story  
✅ **VII. AI Development Constraints** - All changes in Assets/Scripts/, no .meta modification, no new packages  

**Constitution Compliance**: ✅ All principles satisfied. No violations requiring justification.

## Project Structure

### Documentation (this feature)

```text
specs/001-scene-transition/
├── plan.md              # This file
├── spec.md              # Feature specification (already created)
├── checklists/
│   └── requirements.md  # Specification quality checklist (already created)
├── research.md          # Phase 0: Current implementation analysis (to be created)
├── data-model.md        # Phase 1: Data structures and contracts (to be created)
├── quickstart.md        # Phase 1: Usage guide for creators (to be created)
├── contracts/           # Phase 1: Interface definitions (to be created)
└── tasks.md             # Phase 2: Implementation tasks (created via /speckit.tasks)
```

### Source Code (repository root)

```text
novel-core/Assets/Scripts/NovelCore/
├── Runtime/
│   ├── Core/
│   │   ├── DialogueSystem/
│   │   │   ├── DialogueSystem.cs              # MODIFY: Add nextScene transition logic
│   │   │   └── IDialogueSystem.cs             # MODIFY: Add navigation events
│   │   ├── SceneManagement/
│   │   │   ├── SceneManager.cs                # MODIFY: Add preloading support
│   │   │   ├── ISceneManager.cs               # MODIFY: Add navigation methods
│   │   │   ├── SceneNavigationHistory.cs      # CREATE: History management
│   │   │   └── SceneTransitionContext.cs      # CREATE: Transition state tracking
│   │   ├── SaveSystem/
│   │   │   ├── SaveSystem.cs                  # MODIFY: Persist navigation state
│   │   │   └── ISaveSystem.cs                 # MODIFY: Add navigation state methods
│   │   └── GameStarter.cs                     # No changes needed (initialization is correct)
│   ├── Data/
│   │   ├── Scenes/
│   │   │   ├── SceneData.cs                   # MODIFY: Add nextScene, transitionRules
│   │   │   ├── SceneTransitionRule.cs         # CREATE: Conditional transition data
│   │   │   └── SceneNavigationState.cs        # CREATE: Serializable history state
│   │   └── GameState/
│   │       ├── GameStateManager.cs            # CREATE: Flags/variables system
│   │       └── IGameStateManager.cs           # CREATE: Game state interface
│   └── UI/
│       └── NavigationControls/
│           └── SceneNavigationUI.cs           # CREATE: Back/forward UI controls
├── Editor/
│   ├── SceneData/
│   │   ├── SceneDataEditor.cs                 # MODIFY: Add nextScene field editor
│   │   └── SceneFlowValidator.cs              # CREATE: Validate scene graph
│   └── Tools/
│       └── SceneGraphVisualizer.cs            # CREATE: Visual scene flow editor (optional, P3)
└── Tests/
    ├── Editor/
    │   ├── Data/
    │   │   ├── SceneDataTests.cs              # MODIFY: Add nextScene validation tests
    │   │   └── SceneTransitionRuleTests.cs    # CREATE: Test conditional logic
    │   └── Validation/
    │       └── SceneGraphValidatorTests.cs    # CREATE: Test circular reference detection
    └── Runtime/
        ├── Core/
        │   ├── DialogueSystem/
        │   │   └── DialogueSystemTransitionTests.cs  # CREATE: Test scene transitions
        │   ├── SceneManagement/
        │   │   ├── SceneNavigationHistoryTests.cs    # CREATE: Test history stack
        │   │   └── ScenePreloadingTests.cs           # CREATE: Test preloading
        │   └── SaveSystem/
        │       └── NavigationStatePersistenceTests.cs # CREATE: Test save/load
        └── Integration/
            └── SceneTransitionIntegrationTests.cs    # CREATE: End-to-end flow tests
```

**Structure Decision**: Hybrid Runtime + Editor approach (Option 3 from template). The feature extends existing runtime systems (DialogueSystem, SceneManager, SaveSystem) with new data models and navigation logic. Editor tools validate scene graphs and provide visual feedback. This aligns with the existing NovelCore architecture pattern.

## Complexity Tracking

*No Constitution violations requiring justification. All implementation fits within existing architectural patterns.*

## Architecture Overview

### Core Systems to Modify

1. **SceneData** (`Data/Scenes/SceneData.cs`)
   - Add `AssetReference nextScene` field (linear progression)
   - Add `List<SceneTransitionRule> transitionRules` (conditional branching)
   - Extend `Validate()` to check nextScene references and detect circular dependencies
   - Maintain backward compatibility (existing scenes without nextScene still work)

2. **DialogueSystem** (`Core/DialogueSystem/DialogueSystem.cs`)
   - Modify `AdvanceDialogue()` to check for nextScene after dialogue completes
   - Add `OnSceneNavigationRequested` event (already exists, verify usage)
   - Integrate with GameStateManager to evaluate conditional transitions
   - Handle edge case: scenes with both choices and nextScene (choices take priority)

3. **SceneManager** (`Core/SceneManagement/SceneManager.cs`)
   - Add `NavigateBack()` and `NavigateForward()` methods
   - Integrate `SceneNavigationHistory` to track visited scenes
   - Add preloading API: `PreloadSceneAsync(AssetReference scene)`
   - Emit lifecycle events: `OnScenePreloadStarted`, `OnScenePreloadComplete`

4. **SaveSystem** (`Core/SaveSystem/SaveSystem.cs`)
   - Add `SceneNavigationState` to save data structure
   - Serialize/deserialize scene history stack
   - Ensure backward compatibility (old saves without navigation state still load)

### New Components

1. **SceneNavigationHistory** (Core/SceneManagement/)
   - Stack-based history (List<SceneHistoryEntry> with max 50 entries)
   - Stores: scene reference, dialogue index, character positions, game state snapshot
   - Methods: `Push()`, `Pop()`, `CanNavigateBack()`, `CanNavigateForward()`, `Clear()`
   - Memory management: Remove oldest entries when limit exceeded

2. **SceneTransitionRule** (Data/Scenes/)
   - Fields: `int priority`, `string conditionExpression`, `AssetReference targetScene`
   - Validation: Ensure priority is unique within scene, targetScene is valid
   - Condition syntax: Simple flag checks (e.g., "metCharacter == true", "chapter >= 2")

3. **GameStateManager** (Data/GameState/)
   - Dictionary-based flag/variable storage: `Dictionary<string, object> state`
   - Methods: `SetFlag(string key, bool value)`, `GetFlag(string key)`, `SetVariable(string key, int value)`
   - Integrated with SaveSystem for persistence
   - Evaluated by DialogueSystem when checking transition rules

4. **SceneNavigationUI** (UI/NavigationControls/)
   - Back/Forward buttons (optional, can be implemented by game creators)
   - Disabled state when history is empty
   - Hooks into SceneManager navigation methods

### Data Flow

```
Player advances dialogue
    ↓
DialogueSystem.AdvanceDialogue()
    ↓
All dialogue exhausted?
    ↓
Has choices? → Show choices → Player selects → Load targetScene (existing flow)
    ↓
No choices?
    ↓
Has transitionRules? → Evaluate conditions → Load matching rule's targetScene
    ↓
No matching rules?
    ↓
Has nextScene? → Load nextScene (NEW)
    ↓
No nextScene? → CompleteDialogue() → Show ending/menu (existing behavior)
    ↓
SceneManager.LoadScene() is called
    ↓
SceneNavigationHistory.Push(currentScene state)
    ↓
Preload next scene assets (if enabled)
    ↓
Apply transition (existing TransitionType system)
    ↓
DialogueSystem.StartScene(newScene)
```

### Backward Compatibility Strategy

- **Existing scenes without nextScene**: Continue to work as before (dialogue completes, no auto-transition)
- **Existing choice-based scenes**: No changes needed, choices still work via targetScene
- **Old save files**: NavigationState defaults to empty history, game continues from last scene
- **SceneData version field**: Increment to 1.1.0, future migrations can detect old format

## Phase 0: Research & Current State Analysis

**Deliverable**: `research.md` documenting current implementation gaps

### Key Questions to Answer

1. **DialogueSystem.CompleteDialogue() behavior**: What currently happens when dialogue ends without choices?
   - Analysis: Calls `OnDialogueComplete` event, which hides UI but doesn't transition
   - Gap: No logic to check for or load nextScene

2. **SceneManager.LoadScene() integration**: How is it currently called, and can it accept navigation context?
   - Analysis: Called from GameStarter (initial scene) and DialogueSystem.SelectChoice()
   - Modification needed: Accept optional `SceneNavigationHistory.Entry` to restore state

3. **SaveSystem structure**: What is the current save data format, and where should navigation state fit?
   - Analysis: Uses JSON serialization of game state
   - Integration point: Add `SceneNavigationState navigationState` field to SaveData class

4. **Addressables loading**: Can we preload AssetReferences without blocking current scene?
   - Analysis: `AddressablesAssetManager.LoadAssetAsync<T>()` supports async loading
   - Strategy: Call `LoadAssetAsync()` for next scene's background/characters during current scene

5. **Circular reference detection**: Best algorithm for scene graph validation?
   - Algorithm: Depth-first search (DFS) with visited set, detect back edges
   - Implementation: EditorMode validation runs on SceneData save

### Research Tasks

- [ ] Analyze complete call stack for `DialogueSystem.AdvanceDialogue()` and `CompleteDialogue()`
- [ ] Document all current callers of `SceneManager.LoadScene()`
- [ ] Review SaveSystem serialization format and extension points
- [ ] Test Addressables async loading behavior (load multiple scenes, check memory)
- [ ] Prototype simple condition expression evaluator (flag checks only, no full parser)

## Phase 1: Design & Contracts

**Deliverables**: `data-model.md`, `quickstart.md`, `contracts/` directory

### Data Model Design

**File**: `data-model.md`

1. **SceneData Extensions**
   ```csharp
   [SerializeField] private AssetReference _nextScene;
   [SerializeField] private List<SceneTransitionRule> _transitionRules = new();
   ```

2. **SceneTransitionRule**
   ```csharp
   [Serializable]
   public class SceneTransitionRule
   {
       public int priority;
       public string conditionExpression;
       public AssetReference targetScene;
   }
   ```

3. **SceneNavigationState** (Serializable)
   ```csharp
   [Serializable]
   public class SceneNavigationState
   {
       public List<SceneHistoryEntry> history;
       public int currentIndex;
   }
   
   [Serializable]
   public class SceneHistoryEntry
   {
       public string sceneId;
       public int dialogueLineIndex;
       public Dictionary<string, object> gameStateSnapshot;
   }
   ```

### Interface Contracts

**File**: `contracts/IGameStateManager.cs`

```csharp
public interface IGameStateManager
{
    void SetFlag(string key, bool value);
    bool GetFlag(string key, bool defaultValue = false);
    void SetVariable(string key, int value);
    int GetVariable(string key, int defaultValue = 0);
    bool EvaluateCondition(string expression);
    Dictionary<string, object> CreateSnapshot();
    void RestoreSnapshot(Dictionary<string, object> snapshot);
}
```

**File**: `contracts/ISceneNavigationHistory.cs`

```csharp
public interface ISceneNavigationHistory
{
    void Push(SceneHistoryEntry entry);
    SceneHistoryEntry Pop();
    SceneHistoryEntry Peek();
    bool CanNavigateBack();
    bool CanNavigateForward();
    void Clear();
    int Count { get; }
}
```

### Creator Quickstart Guide

**File**: `quickstart.md`

Content overview:
1. How to set up linear scene progression (add nextScene in Inspector)
2. How to create conditional transitions (add rules with priorities)
3. How to test scene flow (Scene Editor preview + Play Mode)
4. Common patterns: hub scenes, chapter structures, branching rejoins
5. Troubleshooting: circular references, missing scenes, condition errors

## Phase 2: Implementation Tasks

**Deliverable**: `tasks.md` (created via `/speckit.tasks` command after this plan is approved)

### Task Breakdown (Summary)

Implementation will be organized into 5 phases matching user story priorities:

**Phase 2.1 - Linear Progression (P1)** - 8 tasks
- Add nextScene field to SceneData
- Modify DialogueSystem.CompleteDialogue() to check nextScene
- Update SceneData.Validate() to verify nextScene references
- Write EditMode tests for SceneData validation
- Write PlayMode tests for linear scene transitions
- Update SceneDataEditor Inspector to show nextScene field
- Create sample linear story (3 scenes) for manual testing
- Integration test: GameStarter → Scene1 → Scene2 → Scene3 flow

**Phase 2.2 - Choice Validation (P1)** - 4 tasks
- Verify existing choice-based branching still works
- Add tests for choice priority over nextScene (when both exist)
- Test timed choice default behavior
- Integration test: Choice → targetScene transitions

**Phase 2.3 - Navigation History (P2)** - 10 tasks
- Create SceneNavigationHistory class with stack implementation
- Create SceneHistoryEntry data model
- Integrate history push into SceneManager.LoadScene()
- Implement NavigateBack() and NavigateForward() methods
- Add SceneNavigationState serialization
- Integrate with SaveSystem for persistence
- Write EditMode tests for history stack logic
- Write PlayMode tests for back/forward navigation
- Create SceneNavigationUI component (optional)
- Integration test: Save → Load with navigation history

**Phase 2.4 - Conditional Transitions (P3)** - 12 tasks
- Create SceneTransitionRule data model
- Create GameStateManager with flag/variable storage
- Implement condition expression evaluator (simple parser)
- Add transitionRules field to SceneData
- Modify DialogueSystem to evaluate rules before nextScene
- Integrate GameStateManager with SaveSystem
- Write EditMode tests for rule validation and evaluation
- Write PlayMode tests for conditional transitions
- Update SceneDataEditor to show transition rules UI
- Create sample conditional story for testing
- Add SceneFlowValidator (circular reference detection)
- Integration test: Complex branching with state-based transitions

**Phase 2.5 - Preloading (P3)** - 8 tasks
- Add PreloadSceneAsync() to ISceneManager
- Implement preloading in AddressablesAssetManager
- Add preload lifecycle events (OnPreloadStarted, OnPreloadComplete)
- Trigger preload at scene start (for nextScene/choice targets)
- Manage preloaded asset memory (release on scene change)
- Write PlayMode tests for preloading behavior
- Profile memory usage and load times
- Integration test: Measure transition performance with/without preload

**Total: 42 tasks** across 5 implementation phases

### Testing Strategy

1. **EditMode Tests** (preferred for speed)
   - SceneData validation (nextScene, rules)
   - GameStateManager flag/variable operations
   - SceneNavigationHistory stack operations
   - Condition expression evaluation
   - Circular reference detection

2. **PlayMode Tests** (for runtime integration)
   - DialogueSystem scene transitions
   - SceneManager navigation (back/forward)
   - SaveSystem persistence of navigation state
   - Preloading async behavior
   - End-to-end transition flows

3. **Integration Tests** (critical paths)
   - GameStarter → linear scene progression
   - Choice selection → target scene loading
   - Save → Load with navigation history
   - Conditional transition evaluation → correct scene
   - Preloading → instant transition performance

### Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|-----------|
| Circular scene references cause infinite loops | Medium | High | DFS validation in editor, runtime max depth check |
| Navigation history causes memory growth | Medium | Medium | Cap at 50 entries, profile memory usage |
| Preloading degrades current scene performance | Low | Medium | Background async loading, cancel if needed |
| Conditional expression parsing is complex | Low | Low | Start with simple flag checks only (v1) |
| Backward compatibility breaks old saves | Low | High | Version detection, default empty history |
| Choice + nextScene ambiguity confuses creators | Medium | Low | Document priority, validate in editor |

### Success Metrics

- [ ] All 18 functional requirements (FR-001 to FR-018) implemented and tested
- [ ] All 7 success criteria (SC-001 to SC-007) validated
- [ ] Unit test coverage >80% for new code
- [ ] All integration tests passing on 3+ platforms
- [ ] Zero compilation errors or warnings
- [ ] Constitution compliance verified (all 7 principles)
- [ ] Manual testing: Sample 10-scene story with branching works flawlessly
- [ ] Performance: Scene transitions <1 second on iPhone 12 and Intel HD 620

## Next Steps

1. **Approve this plan** - Review architecture and task breakdown
2. **Run Phase 0 Research** - Create `research.md` with current state analysis
3. **Run Phase 1 Design** - Create `data-model.md`, `quickstart.md`, `contracts/`
4. **Generate tasks** - Run `/speckit.tasks` to create detailed task list
5. **Begin implementation** - Start with Phase 2.1 (Linear Progression - P1)

## Dependencies

- Existing systems: DialogueSystem, SceneManager, SaveSystem, Addressables
- No new Unity packages required
- No external libraries needed
- Editor scripts will use Unity Editor APIs only

## Open Questions

1. **Condition expression syntax**: Should we support complex expressions (AND/OR logic) in v1, or just simple flag checks?
   - **Recommendation**: Start with simple flag checks (`flagName == true`), add complexity in v2 if needed

2. **Navigation UI placement**: Should back/forward buttons be part of core system or optional game-specific UI?
   - **Recommendation**: Provide `SceneNavigationUI` as optional component, let creators customize

3. **Preloading strategy**: Preload all choice targets, or only most likely next scene?
   - **Recommendation**: Preload all (memory allows it for visual novel assets), improves worst-case performance

4. **History snapshot granularity**: What game state should be captured in history entries?
   - **Recommendation**: Dialogue index + game flags/variables, skip character animation state (can be recomputed)

---

**Plan Status**: ✅ Ready for Phase 0 Research  
**Estimated Complexity**: Medium (42 tasks, ~2-3 weeks for full implementation)  
**Constitution Compliance**: ✅ All principles satisfied
