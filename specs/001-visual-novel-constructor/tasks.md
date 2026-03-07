# Tasks: Visual Novel Constructor

**Input**: Design documents from `/specs/001-visual-novel-constructor/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

**Tests**: Automated tests deferred to post-MVP per Constitution Principle VI (MVP Exception). MVP (v0.1.0-v0.3.0) will use manual testing and creator dogfooding. Automated test suite (>80% coverage) will be implemented incrementally starting v0.4.0 before production release (v1.0.0). Tasks below focus on implementation only.

**Organization**: Tasks are grouped by user story (P1-P5) to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3, US4, US5)
- Include exact file paths in descriptions

## Path Conventions

- **Unity Project Root**: `./novel-core`
- **Scripts**: `Assets/Scripts/NovelCore/`
- **Runtime**: `Assets/Scripts/NovelCore/Runtime/`
- **Editor**: `Assets/Scripts/NovelCore/Editor/`
- **Content**: `Assets/Content/` (user content, NOT AI-modifiable)
- **Resources**: `Assets/Resources/NovelCore/`
- **Tests**: `Assets/Scripts/NovelCore/Tests/` (not included - tests not requested)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Unity project initialization and basic structure

- [ ] T001 Create Unity 6 project at `./novel-core` with URP 2D renderer
- [ ] T002 [P] Install Unity packages via Package Manager: Addressables 2.0+, URP, Localization 2.0+, Input System 1.8+
- [ ] T003 [P] Configure URP Asset (2D Renderer) in `Assets/Settings/UniversalRP-2DRenderer.asset`
- [ ] T004 [P] Create assembly definition files: `Assets/Scripts/NovelCore/Runtime/NovelCore.Runtime.asmdef`
- [ ] T005 [P] Create assembly definition: `Assets/Scripts/NovelCore/Editor/NovelCore.Editor.asmdef` with Editor platform
- [ ] T006 Create `.editorconfig` file at project root (already exists with C# code style rules)
- [ ] T007 [P] Create folder structure: `Assets/Content/{Backgrounds,Characters,Audio,Localization,Projects}`
- [ ] T008 [P] Configure project settings: Scripting Backend (Mono for Windows, IL2CPP for others)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T009 Setup Addressables groups in `Assets/AddressableAssets/Settings/` for Content folders
- [ ] T010 [P] Create `GlobalUsings.cs` in `Assets/Scripts/NovelCore/Runtime/` with C# 10 global usings
- [ ] T011 [P] Create base ScriptableObject classes in `Assets/Scripts/NovelCore/Runtime/Data/BaseScriptableObject.cs`
- [ ] T012 [P] Implement VContainer LifetimeScope in `Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs`
- [ ] T013 [P] Create platform abstraction interfaces in `Assets/Scripts/NovelCore/Runtime/Platform/Interfaces/IPlatformService.cs`
- [ ] T014 [P] Create core data models: `SceneData.cs`, `CharacterData.cs`, `DialogueLineData.cs` in `Assets/Scripts/NovelCore/Runtime/Data/`
- [ ] T015 [P] Implement IAssetManager interface in `Assets/Scripts/NovelCore/Runtime/Core/AssetManagement/IAssetManager.cs`
- [ ] T016 Implement AddressablesAssetManager in `Assets/Scripts/NovelCore/Runtime/Core/AssetManagement/AddressablesAssetManager.cs`
- [ ] T017 [P] Create IAudioService interface in `Assets/Scripts/NovelCore/Runtime/Core/AudioSystem/IAudioService.cs`
- [ ] T018 Implement UnityAudioService wrapper in `Assets/Scripts/NovelCore/Runtime/Core/AudioSystem/UnityAudioService.cs`
- [ ] T019 [P] Create IInputService interface in `Assets/Scripts/NovelCore/Runtime/Core/InputHandling/IInputService.cs`
- [ ] T020 Implement UnityInputService in `Assets/Scripts/NovelCore/Runtime/Core/InputHandling/UnityInputService.cs`
- [ ] T021 [P] Register all services in VContainer GameLifetimeScope

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Create Basic Visual Novel Scene (Priority: P1) 🎯 MVP

**Goal**: Enable non-programmers to create a simple scene with background, character, and dialogue that plays in preview mode

**Independent Test**: Create new project, add background image, add character sprite, write 3 dialogue lines, preview in Play mode

### Implementation for User Story 1

- [ ] T022 [P] [US1] Create SceneData ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/Scenes/SceneData.cs`
- [ ] T023 [P] [US1] Create CharacterPlacement struct in `Assets/Scripts/NovelCore/Runtime/Data/Scenes/CharacterPlacement.cs`
- [ ] T024 [P] [US1] Create DialogueLineData ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/Dialogue/DialogueLineData.cs`
- [ ] T025 [P] [US1] Create IDialogueSystem interface in `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/IDialogueSystem.cs`
- [ ] T026 [US1] Implement DialogueSystem class in `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`
- [ ] T027 [P] [US1] Create DialogueBox UI prefab in `Assets/Resources/NovelCore/UI/DialogueBox.prefab` (manual in Unity Editor)
- [ ] T028 [P] [US1] Create DialogueBoxController script in `Assets/Scripts/NovelCore/Runtime/UI/DialogueBox/DialogueBoxController.cs`
- [ ] T029 [US1] Implement dialogue text rendering with TextMeshPro in DialogueBoxController
- [ ] T030 [P] [US1] Create ISceneManager interface in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/ISceneManager.cs`
- [ ] T031 [US1] Implement SceneManager in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/SceneManager.cs`
- [ ] T032 [US1] Implement background rendering in SceneManager using SpriteRenderer
- [ ] T033 [US1] Implement character sprite positioning in SceneManager
- [ ] T034 [P] [US1] Create SceneEditorWindow in `Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- [ ] T035 [US1] Implement background image drag-and-drop in SceneEditorWindow
- [ ] T036 [US1] Implement dialogue line editor UI in SceneEditorWindow
- [ ] T037 [US1] Implement character placement UI in SceneEditorWindow
- [ ] T038 [US1] Add "Preview Scene" button to SceneEditorWindow that enters Play mode
- [ ] T039 [US1] Register DialogueSystem and SceneManager in VContainer
- [ ] T040 [US1] Create sample SceneData asset for testing in `Assets/Content/Projects/Sample/`

**Checkpoint**: At this point, users can create a basic scene with background, character, and dialogue, and preview it

---

## Phase 4: User Story 2 - Create Branching Narrative (Priority: P2)

**Goal**: Enable users to add choice points that branch the story into different paths

**Independent Test**: Create 2 scenes, add choice in scene 1 with 2 options, test both paths in preview

### Implementation for User Story 2

- [ ] T041 [P] [US2] Create ChoiceData ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/Choices/ChoiceData.cs`
- [ ] T042 [P] [US2] Create ChoiceOption struct in `Assets/Scripts/NovelCore/Runtime/Data/Choices/ChoiceOption.cs`
- [ ] T043 [US2] Add choices list to SceneData in `Assets/Scripts/NovelCore/Runtime/Data/Scenes/SceneData.cs`
- [ ] T044 [P] [US2] Create ChoiceUI prefab in `Assets/Resources/NovelCore/UI/ChoiceButton.prefab` (manual in Unity Editor)
- [ ] T045 [P] [US2] Create ChoiceUIController script in `Assets/Scripts/NovelCore/Runtime/UI/ChoiceButtons/ChoiceUIController.cs`
- [ ] T046 [US2] Implement choice display logic in DialogueSystem
- [ ] T047 [US2] Implement choice selection and scene navigation in SceneManager
- [ ] T048 [US2] Track choice history for conditional branching in DialogueSystem
- [ ] T049 [P] [US2] Create StoryFlowWindow in `Assets/Scripts/NovelCore/Editor/Windows/StoryFlowWindow.cs`
- [ ] T050 [US2] Implement graph visualization using Unity GraphView in StoryFlowWindow
- [ ] T051 [US2] Display scene nodes and choice connections in StoryFlowWindow
- [ ] T052 [US2] Add choice editor UI to SceneEditorWindow
- [ ] T053 [US2] Implement choice validation (detect broken links) in SceneEditorWindow
- [ ] T054 [US2] Add warning indicators for scenes with no paths to endings

**Checkpoint**: Users can create branching narratives with choices, and visualize story flow

---

## Phase 5: User Story 3 - Manage Characters and Emotions (Priority: P3)

**Goal**: Enable users to create characters with multiple emotions and switch expressions during dialogue

**Independent Test**: Create character with 3 emotions, write dialogue switching emotions, preview and verify changes

### Implementation for User Story 3

- [ ] T055 [P] [US3] Create CharacterData ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/Characters/CharacterData.cs`
- [ ] T056 [P] [US3] Create CharacterEmotion struct in `Assets/Scripts/NovelCore/Runtime/Data/Characters/CharacterEmotion.cs`
- [ ] T057 [US3] Add emotions dictionary to CharacterData
- [ ] T058 [P] [US3] Create ICharacterAnimator interface in `Assets/Scripts/NovelCore/Runtime/Animation/ICharacterAnimator.cs`
- [ ] T059 [P] [US3] Implement UnityCharacterAnimator in `Assets/Scripts/NovelCore/Runtime/Animation/UnityCharacterAnimator.cs`
- [ ] T060 [P] [US3] Implement SpineCharacterAnimator in `Assets/Scripts/NovelCore/Runtime/Animation/SpineCharacterAnimator.cs`
- [ ] T061 [US3] Add emotion switching logic to DialogueSystem
- [ ] T062 [US3] Implement character sprite swapping in SceneManager
- [ ] T063 [P] [US3] Create CharacterEditorWindow in `Assets/Scripts/NovelCore/Editor/Windows/CharacterEditorWindow.cs`
- [ ] T064 [US3] Implement emotion sprite upload UI in CharacterEditorWindow
- [ ] T065 [US3] Add emotion dropdown to dialogue line editor in SceneEditorWindow
- [ ] T066 [US3] Implement active speaker highlighting in DialogueBoxController
- [ ] T067 [US3] Register character animators in VContainer with factory pattern

**Checkpoint**: Users can create characters with emotions and see expressions change during dialogue

---

## Phase 6: User Story 4 - Add Audio and Visual Effects (Priority: P4)

**Goal**: Enable users to add background music, sound effects, and scene transitions

**Independent Test**: Add music to scene, add SFX to dialogue, add fade transition, preview and verify

### Implementation for User Story 4

- [ ] T068 [US4] Add backgroundMusic field to SceneData
- [ ] T069 [US4] Add soundEffect field to DialogueLineData
- [ ] T070 [US4] Implement music playback with fade in/out in UnityAudioService
- [ ] T071 [US4] Implement SFX playback in DialogueSystem
- [ ] T072 [P] [US4] Create TransitionType enum in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/TransitionType.cs`
- [ ] T073 [P] [US4] Create transition shaders (Fade, Slide) in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/Transitions/`
- [ ] T074 [US4] Implement scene transition effects in SceneManager
- [ ] T075 [P] [US4] Create character entrance/exit animations (Slide, Fade) in UnityCharacterAnimator
- [ ] T076 [US4] Add music drag-and-drop to SceneEditorWindow
- [ ] T077 [US4] Add SFX drag-and-drop to dialogue line editor in SceneEditorWindow
- [ ] T078 [US4] Add transition type dropdown to SceneEditorWindow
- [ ] T079 [US4] Add entrance animation controls to character placement UI

**Checkpoint**: Users can add audio and visual effects to enhance their visual novels

---

## Phase 7: User Story 5 - Build and Publish Visual Novel (Priority: P5)

**Goal**: Enable users to build executables for Windows, macOS, iOS, Android with platform-specific optimizations

**Independent Test**: Create 2-scene project, build for all platforms, verify builds launch and play correctly

### Implementation for User Story 5

- [ ] T080 [P] [US5] Create BuildConfig ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/BuildConfig.cs`
- [ ] T081 [P] [US5] Create BuildPipelineWindow in `Assets/Scripts/NovelCore/Editor/Windows/BuildPipelineWindow.cs`
- [ ] T082 [US5] Implement Windows build automation in `Assets/Scripts/NovelCore/Editor/Tools/BuildPipeline/WindowsBuilder.cs`
- [ ] T083 [P] [US5] Implement macOS build automation in `Assets/Scripts/NovelCore/Editor/Tools/BuildPipeline/MacOSBuilder.cs`
- [ ] T084 [P] [US5] Implement iOS build automation in `Assets/Scripts/NovelCore/Editor/Tools/BuildPipeline/iOSBuilder.cs`
- [ ] T085 [P] [US5] Implement Android build automation in `Assets/Scripts/NovelCore/Editor/Tools/BuildPipeline/AndroidBuilder.cs`
- [ ] T086 [US5] Add platform-specific asset optimization (texture compression) in build pipeline
- [ ] T087 [P] [US5] Implement SteamPlatformService in `Assets/Scripts/NovelCore/Runtime/Platform/Steam/SteamPlatformService.cs`
- [ ] T088 [P] [US5] Implement iOSPlatformService in `Assets/Scripts/NovelCore/Runtime/Platform/iOS/iOSPlatformService.cs`
- [ ] T089 [P] [US5] Implement AndroidPlatformService in `Assets/Scripts/NovelCore/Runtime/Platform/Android/AndroidPlatformService.cs`
- [ ] T090 [US5] Integrate Steamworks.NET for achievements and cloud saves
- [ ] T091 [US5] Add build configuration UI to BuildPipelineWindow
- [ ] T092 [US5] Implement "Build All Platforms" button with sequential builds
- [ ] T093 [US5] Add build error reporting with actionable messages
- [ ] T094 [P] [US5] Create AssetValidator tool in `Assets/Scripts/NovelCore/Editor/Tools/AssetValidator.cs`
- [ ] T095 [US5] Implement pre-build validation (missing assets, broken links) in BuildPipelineWindow
- [ ] T096 [US5] Register platform services conditionally in VContainer based on build target

**Checkpoint**: Users can build and publish their visual novels to all target platforms

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T097 [P] Implement ISaveSystem interface in `Assets/Scripts/NovelCore/Runtime/Core/SaveSystem/ISaveSystem.cs`
- [ ] T098 Implement SaveSystem with JSON serialization in `Assets/Scripts/NovelCore/Runtime/Core/SaveSystem/SaveSystem.cs`
- [ ] T099 Add auto-save on scene transitions and choices in SceneManager
- [ ] T100 Implement save/load UI in `Assets/Resources/NovelCore/UI/SaveLoadUI.prefab` (manual in Unity Editor)
- [ ] T101 [P] Implement ILocalizationService interface in `Assets/Scripts/NovelCore/Runtime/Core/Localization/ILocalizationService.cs`
- [ ] T102 Implement UnityLocalizationService wrapper in `Assets/Scripts/NovelCore/Runtime/Core/Localization/UnityLocalizationService.cs`
- [ ] T103 Add localization key support to DialogueLineData
- [ ] T104 [P] Create ProjectSetup wizard in `Assets/Scripts/NovelCore/Editor/Tools/ProjectSetup/ProjectSetupWizard.cs`
- [ ] T105 Implement new project creation workflow in ProjectSetupWizard
- [ ] T106 [P] Add NovelCore menu items to Unity Editor menu bar (Window → NovelCore)
- [ ] T107 Performance profiling: Optimize sprite batching for 60 FPS target
- [ ] T108 Memory profiling: Ensure <512MB RAM usage on mobile
- [ ] T109 [P] Code cleanup: Apply C# 10 features (file-scoped namespaces, record types)
- [ ] T110 [P] Documentation: Update README.md with setup instructions
- [ ] T111 Create sample visual novel project in `Assets/Content/Projects/SampleProject/`
- [ ] T112 Run platform build validation on Windows, macOS, iOS, Android

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - **BLOCKS** all user stories
- **User Stories (Phase 3-7)**: All depend on Foundational phase completion
  - User stories can proceed in parallel (if team capacity allows)
  - Or sequentially in priority order (P1 → P2 → P3 → P4 → P5)
- **Polish (Phase 8)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - **No dependencies** on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - Uses SceneData from US1 but independently testable
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - Uses DialogueSystem from US1 but independently testable
- **User Story 4 (P4)**: Can start after Foundational (Phase 2) - Enhances US1-US3 but independently testable
- **User Story 5 (P5)**: Can start after Foundational (Phase 2) - Independent build system

**Key Insight**: After Foundational phase, all 5 user stories can be developed in parallel by different team members!

### Within Each User Story

- Data models (ScriptableObjects) before systems (managers, services)
- Systems before UI components
- UI components before editor windows
- Editor windows before integration
- Story complete before moving to next priority

### Parallel Opportunities

- **Setup Phase**: T002, T003, T004, T005, T007, T008 can run in parallel
- **Foundational Phase**: T010-T021 (most tasks) can run in parallel after T009
- **Within US1**: T022-T024, T027-T028, T030, T034 can run in parallel at various stages
- **Within US2**: T041-T042, T044-T045, T049 can run in parallel
- **Within US3**: T055-T056, T058-T060, T063 can run in parallel
- **Within US5**: T080-T081, T083-T085, T087-T089, T094 can run in parallel
- **Polish Phase**: T097, T101, T104, T106, T109, T110 can run in parallel

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (8 tasks)
2. Complete Phase 2: Foundational (13 tasks) - **CRITICAL**
3. Complete Phase 3: User Story 1 (19 tasks)
4. **STOP and VALIDATE**: Test User Story 1 independently
   - Create scene with background
   - Add character sprite
   - Write dialogue
   - Preview in Play mode
5. Deploy/demo if ready - **This is your MVP!**

**MVP Total**: 40 tasks to working visual novel creator

### Incremental Delivery

1. Complete Setup + Foundational → **Foundation ready**
2. Add User Story 1 → Test independently → Deploy/Demo (**MVP!**)
3. Add User Story 2 → Test independently → Deploy/Demo (branching added)
4. Add User Story 3 → Test independently → Deploy/Demo (emotions added)
5. Add User Story 4 → Test independently → Deploy/Demo (audio/effects added)
6. Add User Story 5 → Test independently → Deploy/Demo (publishing added)
7. Add Polish → Final release

Each story adds value without breaking previous stories!

### Parallel Team Strategy

With 3-5 developers:

1. **Week 1-2**: Team completes Setup + Foundational together (21 tasks)
2. **Week 3-6**: Once Foundational is done, split:
   - Developer A: User Story 1 (19 tasks) - **Priority**
   - Developer B: User Story 2 (14 tasks)
   - Developer C: User Story 3 (13 tasks)
3. **Week 7-9**: Continue parallel:
   - Developer A: User Story 4 (12 tasks)
   - Developer B: User Story 5 (17 tasks)
   - Developer C: Polish (16 tasks)
4. **Week 10**: Integration testing and final polish

---

## Task Summary

- **Total Tasks**: 112
- **Phase 1 (Setup)**: 8 tasks
- **Phase 2 (Foundational)**: 13 tasks (**BLOCKS all user stories**)
- **Phase 3 (US1 - Basic Scene)**: 19 tasks 🎯 **MVP**
- **Phase 4 (US2 - Branching)**: 14 tasks
- **Phase 5 (US3 - Emotions)**: 13 tasks
- **Phase 6 (US4 - Audio/Effects)**: 12 tasks
- **Phase 7 (US5 - Build/Publish)**: 17 tasks
- **Phase 8 (Polish)**: 16 tasks

**MVP Scope** (Recommended for first milestone):
- Setup + Foundational + User Story 1 = **40 tasks**
- Delivers: Working visual novel creator with scene/dialogue/character support

**Parallel Opportunities**: 45 tasks marked [P] can run in parallel within their phases

---

## Notes

- **[P] tasks** = Different files, no dependencies, can run in parallel
- **[Story] label** = Maps task to specific user story for traceability
- **AI Constraint**: Only modify files in `Assets/Scripts/NovelCore/` directory
- **Manual Unity Work**: Prefabs (T027, T044, T100) must be created in Unity Editor (drag-and-drop UI)
- **Code Style**: Follow .editorconfig rules (Allman braces, underscore private fields, var in loops)
- **Tests Not Included**: Specification doesn't explicitly request tests - focus on implementation
- **Constitution Compliance**: All tasks align with 7 constitution principles
- **Commit Strategy**: Commit after each logical group (e.g., after completing US1 data models)
- **Stop Points**: Test each user story independently before proceeding to next

---

**Ready to Start**: Begin with Phase 1 (Setup) tasks T001-T008 🚀
