---

description: "Task list template for feature implementation"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: The examples below include test tasks. Tests are OPTIONAL - only include them if explicitly requested in the feature specification.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Unity Editor Tool**: `Assets/NovelCore/Editor/`, `Assets/NovelCore/Runtime/`
- **Unity Package**: `Packages/com.novelcore.[feature]/Runtime/`, `Packages/com.novelcore.[feature]/Editor/`
- **Tests**: `Assets/NovelCore/Tests/EditMode/`, `Assets/NovelCore/Tests/PlayMode/` or `Tests/` at repository root
- **Resources**: `Assets/NovelCore/Resources/` for runtime-loaded assets
- **Addressables**: `Assets/AddressableAssets/` for Addressable system content
- Paths shown below assume Hybrid Runtime + Editor structure - adjust based on plan.md structure

<!-- 
  ============================================================================
  IMPORTANT: The tasks below are SAMPLE TASKS for illustration purposes only.
  
  The /speckit.tasks command MUST replace these with actual tasks based on:
  - User stories from spec.md (with their priorities P1, P2, P3...)
  - Feature requirements from plan.md
  - Entities from data-model.md
  - Endpoints from contracts/
  
  Tasks MUST be organized by user story so each story can be:
  - Implemented independently
  - Tested independently
  - Delivered as an MVP increment
  
  DO NOT keep these sample tasks in the generated tasks.md file.
  ============================================================================
-->

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create Unity project structure per implementation plan
- [ ] T002 Initialize Unity project with required packages (Addressables, TextMeshPro, Input System)
- [ ] T003 [P] Configure assembly definitions and test assemblies

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

Examples of foundational tasks (adjust based on your project):

- [ ] T004 Setup Unity Addressables system and asset labeling strategy
- [ ] T005 [P] Create base ScriptableObject architecture for configuration
- [ ] T006 [P] Implement platform abstraction layer (Steam/iOS/Android APIs)
- [ ] T007 Create core data models and serialization framework
- [ ] T008 Configure error handling and debug logging infrastructure
- [ ] T009 Setup Input System integration (touch/mouse/keyboard abstraction)
- [ ] T010 Implement save system foundation with cloud sync interfaces

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - [Title] (Priority: P1) 🎯 MVP

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 1 (OPTIONAL - only if tests requested) ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T010 [P] [US1] Play mode test for [feature workflow] in Assets/NovelCore/Tests/PlayMode/Test[Name].cs
- [ ] T011 [P] [US1] Edit mode test for [editor functionality] in Assets/NovelCore/Tests/EditMode/Test[Name].cs

### Implementation for User Story 1

- [ ] T012 [P] [US1] Create [ScriptableObject/Data] in Assets/NovelCore/Runtime/Data/[Name].cs
- [ ] T013 [P] [US1] Create [MonoBehaviour/Component] in Assets/NovelCore/Runtime/[Name].cs
- [ ] T014 [US1] Implement [System/Manager] in Assets/NovelCore/Runtime/Core/[Name].cs (depends on T012, T013)
- [ ] T015 [US1] Create custom editor window in Assets/NovelCore/Editor/Windows/[Name]Window.cs
- [ ] T016 [US1] Add validation and error reporting to editor
- [ ] T017 [US1] Add debug logging for user story 1 operations

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

### Test Execution for User Story 1 (MANDATORY) ✅

> **REQUIRED by Constitution Principle VI: Test Execution Requirement**
>
> After completing User Story 1 implementation, ALL tests MUST pass before proceeding.

- [ ] T018 [US1] **Run EditMode tests** in batch mode:
  ```bash
  /Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
    -runTests -batchmode -projectPath "$(pwd)" \
    -testPlatform EditMode -testResults "./test-results-editmode.xml" \
    -logFile - 2>&1
  ```
  - Verify exit code 0 (all tests passed)
  - Fix any test failures before proceeding

- [ ] T019 [US1] **Run PlayMode tests** in batch mode (only if EditMode passes):
  ```bash
  /Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
    -runTests -batchmode -projectPath "$(pwd)" \
    -testPlatform PlayMode -testResults "./test-results-playmode.xml" \
    -logFile - 2>&1
  ```
  - Verify exit code 0 (all tests passed)
  - Fix any test failures before proceeding

**Test Gate**: User Story 1 is NOT complete until all tests pass ✅

---

## Phase 4: User Story 2 - [Title] (Priority: P2)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 2 (OPTIONAL - only if tests requested) ⚠️

- [ ] T018 [P] [US2] Play mode test for [feature workflow] in Assets/NovelCore/Tests/PlayMode/Test[Name].cs
- [ ] T019 [P] [US2] Edit mode test for [editor functionality] in Assets/NovelCore/Tests/EditMode/Test[Name].cs

### Implementation for User Story 2

- [ ] T020 [P] [US2] Create [ScriptableObject/Data] in Assets/NovelCore/Runtime/Data/[Name].cs
- [ ] T021 [US2] Implement [System/Manager] in Assets/NovelCore/Runtime/Core/[Name].cs
- [ ] T022 [US2] Create custom editor/inspector in Assets/NovelCore/Editor/[Name].cs
- [ ] T023 [US2] Integrate with User Story 1 components (if needed)

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

### Test Execution for User Story 2 (MANDATORY) ✅

- [ ] T024 [US2] **Run EditMode tests** - verify all tests pass (exit code 0)
- [ ] T025 [US2] **Run PlayMode tests** - verify all tests pass (exit code 0)

**Test Gate**: User Story 2 is NOT complete until all tests pass ✅

---

## Phase 5: User Story 3 - [Title] (Priority: P3)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 3 (OPTIONAL - only if tests requested) ⚠️

- [ ] T024 [P] [US3] Play mode test for [feature workflow] in Assets/NovelCore/Tests/PlayMode/Test[Name].cs
- [ ] T025 [P] [US3] Edit mode test for [editor functionality] in Assets/NovelCore/Tests/EditMode/Test[Name].cs

### Implementation for User Story 3

- [ ] T026 [P] [US3] Create [ScriptableObject/Data] in Assets/NovelCore/Runtime/Data/[Name].cs
- [ ] T027 [US3] Implement [System/Manager] in Assets/NovelCore/Runtime/Core/[Name].cs
- [ ] T028 [US3] Create custom editor/inspector in Assets/NovelCore/Editor/[Name].cs

**Checkpoint**: All user stories should now be independently functional

### Test Execution for User Story 3 (MANDATORY) ✅

- [ ] T029 [US3] **Run EditMode tests** - verify all tests pass (exit code 0)
- [ ] T030 [US3] **Run PlayMode tests** - verify all tests pass (exit code 0)

**Test Gate**: User Story 3 is NOT complete until all tests pass ✅

---

[Add more user story phases as needed, following the same pattern]

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] TXXX [P] Documentation updates in Documentation~/
- [ ] TXXX Code cleanup and refactoring
- [ ] TXXX Performance optimization and profiling across all stories
- [ ] TXXX [P] Additional unit tests (if requested) in Assets/NovelCore/Tests/
- [ ] TXXX Platform build validation (Windows, Mac, iOS, Android)
- [ ] TXXX Run quickstart.md validation
- [ ] TXXX Asset reference integrity check

### Final Test Execution (MANDATORY before release) ✅

- [ ] TXXX **Run full EditMode test suite** - verify 100% pass rate
- [ ] TXXX **Run full PlayMode test suite** - verify 100% pass rate
- [ ] TXXX **Verify test results XML files** - confirm zero failures
- [ ] TXXX **Document test coverage** - confirm >80% coverage (post-MVP requirement)

**Release Gate**: Feature is NOT ready for release until all tests pass ✅

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1 but should be independently testable
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - May integrate with US1/US2 but should be independently testable

### Within Each User Story

- Tests (if included) MUST be written and FAIL before implementation
- Models before services
- Services before endpoints
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- Models within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together (if tests requested):
Task: "Play mode test for [feature workflow] in Assets/NovelCore/Tests/PlayMode/Test[Name].cs"
Task: "Edit mode test for [editor functionality] in Assets/NovelCore/Tests/EditMode/Test[Name].cs"

# Launch all data models for User Story 1 together:
Task: "Create [ScriptableObject1] in Assets/NovelCore/Runtime/Data/[Name1].cs"
Task: "Create [ScriptableObject2] in Assets/NovelCore/Runtime/Data/[Name2].cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
