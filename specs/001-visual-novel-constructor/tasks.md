# Tasks: Visual Novel Constructor

**Input**: Design documents from `/specs/001-visual-novel-constructor/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

**Tests**: Per Constitution Principle VI (Modular Architecture & Testing), functionality MUST be covered by unit and integration tests. MVP (v0.1.0-v0.3.0) MAY use manual testing during initial implementation. Automated test suite with >80% code coverage MUST be implemented starting v0.4.0 before production release. Tasks below include dedicated test iterations for post-MVP phase.

**Organization**: Tasks are grouped into **iterations** - small groups where each iteration delivers a visible, testable result. Each iteration can be completed and validated before moving to the next.

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
- **Tests**: `Assets/Scripts/NovelCore/Tests/` (not included - tests deferred to post-MVP)

---

# ITERATION STRUCTURE

Each iteration is a complete mini-milestone with:
- **Goal**: What visible result you'll see
- **Duration**: Estimated time (solo developer)
- **Validation**: How to test the iteration result
- **Tasks**: Checklist of work to complete

---

## 🔧 ITERATION 0: Project Bootstrap (1-2 hours)

**Goal**: Unity project exists with correct configuration and folder structure visible in Project window

**Visible Result**: Open Unity Hub, see "novel-core" project with URP 2D, organized folders

**Validation**:
- Unity project opens without errors
- Project window shows `Assets/Scripts/NovelCore/`, `Assets/Content/` folders
- URP 2D renderer visible in Graphics settings

**Tasks**:
- [X] T001 Create Unity 6 project at `./novel-core` with URP 2D renderer
- [X] T002 [P] Install Unity packages via Package Manager: Addressables 2.0+, URP, Localization 2.0+, Input System 1.8+ (Can be automated by AI when user-specified, per Constitution VII)
- [X] T003 [P] Configure URP Asset (2D Renderer) in `Assets/Settings/UniversalRP-2DRenderer.asset`
- [X] T004 [P] Create assembly definition files: `Assets/Scripts/NovelCore/Runtime/NovelCore.Runtime.asmdef`
- [X] T005 [P] Create assembly definition: `Assets/Scripts/NovelCore/Editor/NovelCore.Editor.asmdef` with Editor platform
- [X] T006 Create `.editorconfig` file at project root (already exists with C# code style rules)
- [X] T007 [P] Create folder structure: `Assets/Content/{Backgrounds,Characters,Audio,Localization,Projects}`
- [X] T008 [P] Configure project settings: Scripting Backend (IL2CPP for all platforms) (MANUAL - requires Unity Editor)

---

## 🏗️ ITERATION 1: Dependency Injection Infrastructure (2-3 hours)

**Goal**: VContainer DI system working - can register and resolve services

**Visible Result**: Play mode starts with no errors, console shows "GameLifetimeScope initialized" log

**Validation**:
- Enter Play mode - no compilation errors
- Console shows VContainer initialization logs
- Create test MonoBehaviour that resolves IAssetManager - works

**Tasks**:
- [ ] T009 Setup Addressables groups in `Assets/AddressableAssets/Settings/` for Content folders (MANUAL - see ADDRESSABLES_SETUP.md)
- [X] T010 [P] Create `GlobalUsings.cs` in `Assets/Scripts/NovelCore/Runtime/` with C# 10 global usings
- [X] T011 [P] Create base ScriptableObject classes in `Assets/Scripts/NovelCore/Runtime/Data/BaseScriptableObject.cs`
- [X] T012 [P] Implement VContainer LifetimeScope in `Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs`
- [X] T013 [P] Create platform abstraction interfaces in `Assets/Scripts/NovelCore/Runtime/Platform/Interfaces/IPlatformService.cs`
- [X] T021 [P] Register all services in VContainer GameLifetimeScope (placeholder registrations for now)

---

## 🎨 ITERATION 2: Asset Management System (2-3 hours)

**Goal**: Can load sprites via Addressables in Play mode

**Visible Result**: Test scene with background sprite loaded from Addressables, displayed on screen

**Validation**:
- Create test background image in `Assets/Content/Backgrounds/`, mark as Addressable
- Play mode: background displays fullscreen
- Addressables Profiler shows asset loaded correctly

**Tasks**:
- [X] T014 [P] Create core data models: `SceneData.cs`, `CharacterData.cs`, `DialogueLineData.cs` in `Assets/Scripts/NovelCore/Runtime/Data/`
- [X] T015 [P] Implement IAssetManager interface in `Assets/Scripts/NovelCore/Runtime/Core/AssetManagement/IAssetManager.cs`
- [X] T016 Implement AddressablesAssetManager in `Assets/Scripts/NovelCore/Runtime/Core/AssetManagement/AddressablesAssetManager.cs`

---

## 🎵 ITERATION 3: Audio & Input Services (1-2 hours)

**Goal**: Can play audio and handle click/tap input in Play mode

**Visible Result**: Test scene with "Click to play sound" - clicking plays audio clip

**Validation**:
- Play mode: click anywhere on screen → hear test sound effect
- Test on mobile: tap works
- Audio volume adjustable via IAudioService

**Tasks**:
- [X] T017 [P] Create IAudioService interface in `Assets/Scripts/NovelCore/Runtime/Core/AudioSystem/IAudioService.cs`
- [X] T018 Implement UnityAudioService wrapper in `Assets/Scripts/NovelCore/Runtime/Core/AudioSystem/UnityAudioService.cs`
- [X] T019 [P] Create IInputService interface in `Assets/Scripts/NovelCore/Runtime/Core/InputHandling/IInputService.cs`
- [X] T020 Implement UnityInputService in `Assets/Scripts/NovelCore/Runtime/Core/InputHandling/UnityInputService.cs`

---

## 📝 ITERATION 4: Basic Dialogue Display (3-4 hours)

**Goal**: See text dialogue box on screen with dialogue advancing on click

**Visible Result**: Play mode shows dialogue box UI with "Hello, world!" text, clicking advances to next line

**Validation**:
- Play mode: dialogue box visible at screen bottom
- Shows 3 hardcoded dialogue lines
- Clicking advances line-by-line
- TextMeshPro renders text correctly

**Tasks**:
- [X] T022 [P] [US1] Create SceneData ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/Scenes/SceneData.cs`
- [X] T024 [P] [US1] Create DialogueLineData ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/Dialogue/DialogueLineData.cs`
- [X] T025 [P] [US1] Create IDialogueSystem interface in `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/IDialogueSystem.cs`
- [X] T026 [US1] Implement DialogueSystem class in `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`
- [X] T027 [P] [US1] Create DialogueBox UI prefab in `Assets/Resources/NovelCore/UI/DialogueBox.prefab` (Generated via Editor script)
- [X] T028 [P] [US1] Create DialogueBoxController script in `Assets/Scripts/NovelCore/Runtime/UI/DialogueBox/DialogueBoxController.cs`
- [X] T029 [US1] Implement dialogue text rendering with TextMeshPro in DialogueBoxController
- [X] T039 [US1] Register DialogueSystem and SceneManager in VContainer

---

## 🖼️ ITERATION 5: Scene Rendering (Background + Character) (2-3 hours)

**Goal**: See background image and character sprite displayed together in Play mode

**Visible Result**: Play mode shows fullscreen background with character sprite positioned on top

**Validation**:
- Play mode: background covers screen
- Character sprite visible at specified position
- Correct layering (background behind, character on top, UI above)

**Tasks**:
- [X] T023 [P] [US1] Create CharacterPlacement struct in `Assets/Scripts/NovelCore/Runtime/Data/Scenes/CharacterPlacement.cs`
- [X] T030 [P] [US1] Create ISceneManager interface in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/ISceneManager.cs`
- [X] T031 [US1] Implement SceneManager in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/SceneManager.cs`
- [X] T032 [US1] Implement background rendering in SceneManager using SpriteRenderer
- [X] T033 [US1] Implement character sprite positioning in SceneManager

---

## 🛠️ ITERATION 6: Scene Editor Window (3-4 hours)

**Goal**: Unity Editor menu "Window → NovelCore → Scene Editor" opens, can drag-and-drop assets

**Visible Result**: Open Scene Editor window, drag background image from Project → see it assigned to SceneData

**Validation**:
- Menu item "Window → NovelCore → Scene Editor" exists
- Window opens with UI for background, characters, dialogue
- Drag-and-drop image → updates SceneData asset
- Changes persist after closing/reopening Unity

**Tasks**:
- [X] T034 [P] [US1] Create SceneEditorWindow in `Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- [X] T035 [US1] Implement background image drag-and-drop in SceneEditorWindow
- [X] T036 [US1] Implement dialogue line editor UI in SceneEditorWindow
- [X] T037 [US1] Implement character placement UI in SceneEditorWindow

---

## ▶️ ITERATION 7: Preview Mode + Sample Scene (1-2 hours)

**Goal**: Click "Preview Scene" button → Unity enters Play mode and shows complete scene

**Visible Result**: Create sample scene in editor, click Preview → see background, character, dialogue playing

**Validation**:
- Create SceneData asset with background, character, 3 dialogue lines
- Click "Preview Scene" button in Scene Editor
- Play mode: scene renders correctly, dialogue advances
- **🎯 MVP CHECKPOINT**: First complete visual novel scene works end-to-end!

**Tasks**:
- [X] T038 [US1] Add "Preview Scene" button to SceneEditorWindow that enters Play mode
- [X] T040 [US1] Create sample SceneData asset for testing in `Assets/Content/Projects/Sample/`

---

## 🎯 ITERATION 7.5: Game Entry Point (GameStarter) (2-3 hours)

**Goal**: GameStarter component initializes game and loads starting scene automatically in Play Mode

**Visible Result**: Unity scene has GameStarter GameObject. Press Play → game auto-loads configured starting scene

**Validation**:
- Add GameStarter GameObject to main Unity scene (SampleScene.unity)
- Assign starting scene (Scene01_Introduction.asset) in Inspector
- Press Play → GameStarter logs initialization, scene loads automatically
- Dialogue starts without manual intervention
- **🎯 ARCHITECTURE CHECKPOINT**: Explicit entry point establishes proper initialization sequence

**Tasks**:
- [X] T039 [P] [US1] Create GameStarter component in `Assets/Scripts/NovelCore/Runtime/Core/GameStarter.cs`
  - Inject IDialogueSystem and ISceneManager via VContainer
  - Load starting scene from SceneData field
  - Call SceneManager.LoadScene() and DialogueSystem.StartScene()
  - Support auto-start with configurable delay
  - Provide RestartGame() method for full reset
- [ ] T040.1 [P] [US1] Add GameStarter integration test in `Assets/Scripts/NovelCore/Tests/Runtime/Core/GameStarterTests.cs`
  - Test initialization sequence (VContainer → SceneManager → DialogueSystem)
  - Test Play Mode full start vs Scene Editor preview
  - Test error handling (missing starting scene, failed DI injection)
- [ ] T040.2 [US1] Update Sample Project setup instructions in SAMPLE_PROJECT_QUICKSTART.md
  - Add step for creating GameStarter GameObject
  - Document Inspector configuration (starting scene, auto-start, delay)
  - Add troubleshooting section for common setup errors
- [ ] T040.3 [P] [US1] Create SampleProjectGenerator update in `Assets/Scripts/NovelCore/Editor/Tools/Generators/SampleProjectGenerator.cs`
  - Auto-create GameStarter GameObject in SampleScene.unity if missing
  - Auto-assign Scene01_Introduction.asset as starting scene
  - Log setup completion with validation checks

**Note**: This iteration implements Constitution Principle VI requirement for explicit game entry point. GameStarter ensures predictable initialization order: VContainer DI → Services registered → Starting scene loaded → DialogueSystem started.

---

## 🔀 ITERATION 8: Choice System - Data Models (2 hours)

**Goal**: Can create ChoiceData ScriptableObject with multiple options

**Visible Result**: Inspector shows ChoiceData asset with 2-6 choice options, each linking to target scene

**Validation**:
- Right-click in Project → Create → NovelCore → ChoiceData
- Inspector shows list of ChoiceOptions
- Each option has text and target SceneData reference
- Data persists correctly

**Tasks**:
- [X] T041 [P] [US2] Create ChoiceData ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/Choices/ChoiceData.cs`
- [X] T042 [P] [US2] Create ChoiceOption struct in `Assets/Scripts/NovelCore/Runtime/Data/Choices/ChoiceOption.cs`
- [X] T043 [US2] Add choices list to SceneData in `Assets/Scripts/NovelCore/Runtime/Data/Scenes/SceneData.cs`

---

## 🔘 ITERATION 9: Choice UI Display (3 hours)

**Goal**: Choice buttons appear on screen during dialogue, clicking navigates to target scene

**Visible Result**: Play mode reaches choice point → 2 buttons appear, clicking "Option A" loads Scene A

**Validation**:
- Create 2 scenes: Scene1 (with choice), Scene2a, Scene2b
- Play mode: Scene1 dialogue finishes → choice buttons appear
- Click button → loads target scene
- Choice history tracked (console logs)

**Tasks**:
- [X] T044 [P] [US2] Create ChoiceUI prefab in `Assets/Resources/NovelCore/UI/ChoiceButton.prefab` (Generated via Editor script)
- [X] T045 [P] [US2] Create ChoiceUIController script in `Assets/Scripts/NovelCore/Runtime/UI/ChoiceButtons/ChoiceUIController.cs`
- [X] T046 [US2] Implement choice display logic in DialogueSystem
- [X] T047 [US2] Implement choice selection and scene navigation in SceneManager
- [X] T048 [US2] Track choice history for conditional branching in DialogueSystem

---

## 🗺️ ITERATION 10: Story Flow Visualization (3-4 hours)

**Goal**: Open Story Flow window → see graph of scene connections with choice branches

**Visible Result**: Unity Editor shows graph with scene nodes connected by choice arrows

**Validation**:
- Menu "Window → NovelCore → Story Flow"
- Graph shows all SceneData assets as nodes
- Arrows show choice connections
- Click node → opens Scene Editor for that scene
- Visual warnings for broken links

**Tasks**:
- [X] T049 [P] [US2] Create StoryFlowWindow in `Assets/Scripts/NovelCore/Editor/Windows/StoryFlowWindow.cs`
- [X] T050 [US2] Implement graph visualization using Unity GraphView in StoryFlowWindow
- [X] T051 [US2] Display scene nodes and choice connections in StoryFlowWindow
- [X] T052 [US2] Add choice editor UI to SceneEditorWindow
- [X] T053 [US2] Implement choice validation (detect broken links) in SceneEditorWindow
- [X] T054 [US2] Add warning indicators for scenes with no paths to endings

---

## 😊 ITERATION 11: Character System - Data Models (2 hours)

**Goal**: Can create CharacterData with multiple emotion sprites

**Visible Result**: Inspector shows CharacterData with emotions dictionary (happy, sad, angry sprites)

**Validation**:
- Right-click → Create → NovelCore → CharacterData
- Inspector shows emotions dictionary
- Add 3 emotions with different sprites
- Data persists correctly

**Tasks**:
- [X] T055 [P] [US3] Create CharacterData ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/Characters/CharacterData.cs`
- [X] T056 [P] [US3] Create CharacterEmotion struct in `Assets/Scripts/NovelCore/Runtime/Data/Characters/CharacterEmotion.cs`
- [X] T057 [US3] Add emotions dictionary to CharacterData

---

## 🎭 ITERATION 12: Character Animation System (3 hours)

**Goal**: Character sprite changes during dialogue based on assigned emotion

**Visible Result**: Play mode: character shows "happy" sprite, dialogue advances → switches to "sad" sprite

**Validation**:
- Create CharacterData with 3 emotions
- Create scene with 3 dialogue lines, each using different emotion
- Play mode: character sprite updates as dialogue advances
- Smooth transitions between emotions

**Tasks**:
- [X] T058 [P] [US3] Create ICharacterAnimator interface in `Assets/Scripts/NovelCore/Runtime/Animation/ICharacterAnimator.cs`
- [X] T059 [P] [US3] Implement UnityCharacterAnimator in `Assets/Scripts/NovelCore/Runtime/Animation/UnityCharacterAnimator.cs`
- [X] T060 [P] [US3] Implement SpineCharacterAnimator in `Assets/Scripts/NovelCore/Runtime/Animation/SpineCharacterAnimator.cs`
- [X] T061 [US3] Add emotion switching logic to DialogueSystem
- [X] T062 [US3] Implement character sprite swapping in SceneManager
- [X] T067 [US3] Register character animators in VContainer with factory pattern

---

## 👤 ITERATION 13: Character Editor Window (3 hours)

**Goal**: Unity Editor window for creating characters and uploading emotion sprites

**Visible Result**: Menu "Window → NovelCore → Character Editor" → upload sprites, assign to emotions

**Validation**:
- Open Character Editor window
- Create new character
- Upload 3 different sprites
- Assign to emotions (happy, sad, angry)
- Character available in Scene Editor dropdown

**Tasks**:
- [X] T063 [P] [US3] Create CharacterEditorWindow in `Assets/Scripts/NovelCore/Editor/Windows/CharacterEditorWindow.cs`
- [X] T064 [US3] Implement emotion sprite upload UI in CharacterEditorWindow
- [X] T065 [US3] Add emotion dropdown to dialogue line editor in SceneEditorWindow
- [X] T066 [US3] Implement active speaker highlighting in DialogueBoxController

---

## 🎵 ITERATION 14: Audio Integration (2-3 hours)

**Goal**: Background music plays during scene, sound effects trigger on dialogue lines

**Visible Result**: Play mode: hear looping background music, hear "door slam" SFX on specific dialogue line

**Validation**:
- Add music to SceneData
- Add SFX to DialogueLineData
- Play mode: music starts when scene loads
- SFX plays when dialogue line appears
- Music fades in/out on scene transitions

**Tasks**:
- [X] T068 [US4] Add backgroundMusic field to SceneData
- [X] T069 [US4] Add soundEffect field to DialogueLineData
- [X] T070 [US4] Implement music playback with fade in/out in UnityAudioService
- [X] T071 [US4] Implement SFX playback in DialogueSystem

---

## ✨ ITERATION 15: Scene Transitions (3 hours)

**Goal**: Scenes transition with fade/slide effects

**Visible Result**: Play mode: scene fades out → fades into next scene (smooth visual transition)

**Validation**:
- Create 2 scenes with choice linking them
- Set transition type to "Fade"
- Play mode: smooth fade transition between scenes
- Try different transition types (Slide, Cut)

**Tasks**:
- [X] T072 [P] [US4] Create TransitionType enum in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/TransitionType.cs`
- [X] T073 [P] [US4] Create transition shaders (Fade, Slide) in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/Transitions/`
- [X] T074 [US4] Implement scene transition effects in SceneManager
- [X] T075 [P] [US4] Create character entrance/exit animations (Slide, Fade) in UnityCharacterAnimator

---

## 🎨 ITERATION 16: Audio/Effects Editor UI (2 hours)

**Goal**: Scene Editor window has drag-and-drop for music/SFX, transition dropdowns

**Visible Result**: Scene Editor shows music field (drag audio clip), transition type dropdown

**Validation**:
- Open Scene Editor
- See "Background Music" field → drag AudioClip from Project
- See "Transition Type" dropdown → select Fade/Slide/Cut
- Dialogue line editor has SFX field
- Changes persist and work in Play mode

**Tasks**:
- [X] T076 [US4] Add music drag-and-drop to SceneEditorWindow
- [X] T077 [US4] Add SFX drag-and-drop to dialogue line editor in SceneEditorWindow
- [X] T078 [US4] Add transition type dropdown to SceneEditorWindow
- [X] T079 [US4] Add entrance animation controls to character placement UI

---

## 🏗️ ITERATION 17: Build Pipeline - Data Models (2 hours)

**Goal**: Can create BuildConfig ScriptableObject with platform-specific settings

**Visible Result**: Inspector shows BuildConfig with platform dropdown, paths, SDK settings

**Validation**:
- Right-click → Create → NovelCore → BuildConfig
- Inspector shows platform, bundle ID, scripting backend
- Create separate configs for Windows/macOS/iOS/Android
- Data persists

**Tasks**:
- [ ] T080 [P] [US5] Create BuildConfig ScriptableObject in `Assets/Scripts/NovelCore/Runtime/Data/BuildConfig.cs`
- [ ] T081 [P] [US5] Create BuildPipelineWindow in `Assets/Scripts/NovelCore/Editor/Windows/BuildPipelineWindow.cs`

---

## 🔨 ITERATION 18: Platform Builders (4-5 hours)

**Goal**: Build Pipeline window can build executables for all platforms

**Visible Result**: Click "Build Windows" → Unity builds .exe in Builds/ folder, runs successfully

**Validation**:
- Open Build Pipeline window
- Select Windows platform → click Build
- Verify .exe created in Builds/Windows/
- Run .exe → visual novel plays correctly
- Repeat for macOS, iOS, Android

**Tasks**:
- [ ] T082 [US5] Implement Windows build automation in `Assets/Scripts/NovelCore/Editor/Tools/BuildPipeline/WindowsBuilder.cs`
- [ ] T083 [P] [US5] Implement macOS build automation in `Assets/Scripts/NovelCore/Editor/Tools/BuildPipeline/MacOSBuilder.cs`
- [ ] T084 [P] [US5] Implement iOS build automation in `Assets/Scripts/NovelCore/Editor/Tools/BuildPipeline/iOSBuilder.cs`
- [ ] T085 [P] [US5] Implement Android build automation in `Assets/Scripts/NovelCore/Editor/Tools/BuildPipeline/AndroidBuilder.cs`
- [ ] T086 [US5] Add platform-specific asset optimization (texture compression) in build pipeline

---

## 🎮 ITERATION 19: Platform Services (3-4 hours)

**Goal**: Steam SDK integration works (achievements, cloud saves)

**Visible Result**: Windows build shows Steam overlay, saving triggers cloud save upload

**Validation**:
- Implement SteamPlatformService
- Build Windows with Steam integration
- Run with Steam client → overlay works
- Save game → verify Steam cloud upload
- Test iOS/Android platform services

**Tasks**:
- [ ] T087 [P] [US5] Implement SteamPlatformService in `Assets/Scripts/NovelCore/Runtime/Platform/Steam/SteamPlatformService.cs`
- [ ] T088 [P] [US5] Implement iOSPlatformService in `Assets/Scripts/NovelCore/Runtime/Platform/iOS/iOSPlatformService.cs`
- [ ] T089 [P] [US5] Implement AndroidPlatformService in `Assets/Scripts/NovelCore/Runtime/Platform/Android/AndroidPlatformService.cs`
- [ ] T090 [US5] Integrate Steamworks.NET for achievements and cloud saves
- [ ] T096 [US5] Register platform services conditionally in VContainer based on build target

---

## 🎛️ ITERATION 20: Build Pipeline UI (2-3 hours)

**Goal**: Build Pipeline window with full UI for configuration and batch builds

**Visible Result**: Window shows platform checkboxes, config settings, "Build All" button

**Validation**:
- Open Build Pipeline window
- See UI for each platform (enable/disable checkboxes)
- Configure bundle ID, version, etc.
- Click "Build All Platforms" → builds sequentially
- Build errors show in window with actionable messages

**Tasks**:
- [ ] T091 [US5] Add build configuration UI to BuildPipelineWindow
- [ ] T092 [US5] Implement "Build All Platforms" button with sequential builds
- [ ] T093 [US5] Add build error reporting with actionable messages

---

## ✅ ITERATION 21: Pre-Build Validation (2 hours)

**Goal**: Asset Validator detects errors before build (missing assets, broken links)

**Visible Result**: Click Build → validator runs, shows warnings for missing background in Scene 3

**Validation**:
- Create scene with missing background reference
- Click Build → validation stops build
- Window shows "Scene 3: Missing background asset" error
- Fix issue → build proceeds
- Validator detects circular loops, broken choice links

**Tasks**:
- [ ] T094 [P] [US5] Create AssetValidator tool in `Assets/Scripts/NovelCore/Editor/Tools/AssetValidator.cs`
- [ ] T095 [US5] Implement pre-build validation (missing assets, broken links) in BuildPipelineWindow

---

## 💾 ITERATION 22: Save System (3-4 hours)

**Goal**: Can save/load visual novel progress with multiple save slots

**Visible Result**: Play mode: reach choice, auto-save triggers, load save → resumes from choice point

**Validation**:
- Play through 2 scenes, make choice
- Auto-save triggers (console log)
- Quit Play mode, re-enter
- Load save slot → resumes at correct scene/choice
- Multiple manual save slots work

**Tasks**:
- [X] T097 [P] Implement ISaveSystem interface in `Assets/Scripts/NovelCore/Runtime/Core/SaveSystem/ISaveSystem.cs`
- [X] T098 Implement SaveSystem with JSON serialization in `Assets/Scripts/NovelCore/Runtime/Core/SaveSystem/SaveSystem.cs`
- [ ] T099 Add auto-save on scene transitions and choices in SceneManager
- [X] T100 Implement save/load UI in `Assets/Resources/NovelCore/UI/SaveLoadUI.prefab` (Generated via Editor script)

---

## 🌍 ITERATION 23: Localization System (2-3 hours)

**Goal**: Dialogue text supports multiple languages (English, Russian, Japanese)

**Visible Result**: Settings menu has language dropdown → switch to Russian → all dialogue in Russian

**Validation**:
- Add localization keys to DialogueLineData
- Create Unity Localization tables (EN, RU, JA)
- Add translations
- Play mode: switch language → dialogue updates
- Build includes all languages

**Tasks**:
- [ ] T101 [P] Implement ILocalizationService interface in `Assets/Scripts/NovelCore/Runtime/Core/Localization/ILocalizationService.cs`
- [ ] T102 Implement UnityLocalizationService wrapper in `Assets/Scripts/NovelCore/Runtime/Core/Localization/UnityLocalizationService.cs`
- [ ] T103 Add localization key support to DialogueLineData

---

## 🎬 ITERATION 24: Project Setup Wizard (2 hours)

**Goal**: First-time setup wizard guides creator through project creation

**Visible Result**: Menu "NovelCore → New Project" → wizard with steps (name, platforms, default assets)

**Validation**:
- Open wizard from menu
- Enter project name, select platforms
- Wizard creates folder structure in Assets/Content/Projects/[Name]
- Default UI assets copied
- Sample scenes created
- Ready to start creating

**Tasks**:
- [ ] T104 [P] Create ProjectSetup wizard in `Assets/Scripts/NovelCore/Editor/Tools/ProjectSetup/ProjectSetupWizard.cs`
- [ ] T105 Implement new project creation workflow in ProjectSetupWizard
- [ ] T106 [P] Add NovelCore menu items to Unity Editor menu bar (Window → NovelCore)

---

## ⚡ ITERATION 25: Performance Optimization (3-4 hours)

**Goal**: 60 FPS on target hardware, <512MB RAM mobile, <1GB desktop

**Visible Result**: Unity Profiler shows 60 FPS during dialogue/transitions, Memory <512MB

**Validation**:
- Open Unity Profiler
- Play through 10-scene visual novel
- Verify 60 FPS maintained
- Check Memory Profiler: <512MB on mobile
- Optimize sprite batching if needed
- Test on iPhone 12 / Intel HD 620

**Tasks**:
- [ ] T107 Performance profiling: Optimize sprite batching for 60 FPS target
- [ ] T108 Memory profiling: Ensure <512MB RAM usage on mobile

---

## 🧹 ITERATION 26: Code Cleanup & Documentation (2-3 hours)

**Goal**: Code uses C# 10 features, README updated, sample project complete

**Visible Result**: Repository README shows setup instructions, sample project playable

**Validation**:
- All code uses file-scoped namespaces
- Record types for data structs where appropriate
- README.md has installation/setup guide
- Sample visual novel project in Assets/Content/Projects/SampleProject/
- 5-minute tutorial playable start to finish

**Tasks**:
- [ ] T109 [P] Code cleanup: Apply C# 10 features (file-scoped namespaces, record types)
- [ ] T110 [P] Documentation: Update README.md with setup instructions
- [ ] T111 Create sample visual novel project in `Assets/Content/Projects/SampleProject/`

---

## 🚀 ITERATION 27: Final Validation (2-3 hours)

**Goal**: All platforms build successfully, cross-platform parity verified

**Visible Result**: 4 builds (Windows .exe, macOS .app, iOS .ipa, Android .apk) all run sample project identically

**Validation**:
- Build for Windows, macOS, iOS, Android
- Test sample project on each platform
- Verify identical behavior:
  - Same scenes, dialogue, choices
  - Same visual rendering (within 5% tolerance)
  - Same audio playback
  - Save/load works on all platforms
- Performance meets targets on all platforms

**Tasks**:
- [ ] T112 Run platform build validation on Windows, macOS, iOS, Android

---

## 🧪 POST-MVP PHASE: Automated Testing (v0.4.0+)

**CRITICAL**: Starting from v0.4.0, automated testing becomes MANDATORY per Constitution Principle VI. The iterations below MUST be completed to achieve >80% code coverage and comprehensive integration test coverage before v1.0.0 production release.

---

## 🔬 ITERATION 28: Test Infrastructure Setup (3-4 hours)

**Goal**: Unity Test Framework configured, test assemblies created, mock framework integrated

**Visible Result**: Unity Test Runner window shows test assemblies, can run empty tests successfully

**Validation**:
- Open Unity Test Runner window (Window → General → Test Runner)
- See NovelCore.Tests.Runtime and NovelCore.Tests.Editor assemblies
- Run tests → all pass (even if empty initially)
- Test assemblies reference runtime assemblies correctly
- Mock framework (NSubstitute or similar) available for interface mocking

**Tasks**:
- [X] T113 [P] Create test assembly definitions: `Assets/Scripts/NovelCore/Tests/Runtime/NovelCore.Tests.Runtime.asmdef`
- [X] T114 [P] Create test assembly definitions: `Assets/Scripts/NovelCore/Tests/Editor/NovelCore.Tests.Editor.asmdef`
- [X] T115 [P] Configure test assembly references to NovelCore.Runtime and NovelCore.Editor assemblies
- [X] T116 Install NSubstitute (or NUnit mocking framework) via Package Manager for interface mocking
- [X] T117 [P] Create test fixtures base classes in `Assets/Scripts/NovelCore/Tests/Runtime/BaseTestFixture.cs`
- [X] T118 [P] Create test data builders in `Assets/Scripts/NovelCore/Tests/Runtime/Builders/` for SceneData, CharacterData, DialogueLineData
- [X] T119 Create sample test to verify infrastructure in `Assets/Scripts/NovelCore/Tests/Runtime/SampleTest.cs`

---

## 🔄 ITERATION 28.5: Test Refactoring - EditMode Migration (2-3 hours)

**Goal**: Migrate unit tests from PlayMode to EditMode for faster execution and better reliability

**Visible Result**: Most unit tests run in EditMode (faster), only async/I/O tests remain in PlayMode

**Validation**:
- Open Unity Test Runner → EditMode tab shows 60+ tests
- Open Unity Test Runner → PlayMode tab shows <15 tests (only SaveSystem and integration tests)
- EditMode tests execute in <5 seconds (vs 30+ seconds for PlayMode)
- All tests pass after migration
- No runtime initialization required for data model tests

**Tasks**:
- [X] T112a [P] Create `BaseTestFixture` for EditMode tests in `Assets/Scripts/NovelCore/Tests/Editor/BaseTestFixture.cs`
- [X] T112b [P] Copy test data builders to Editor folder: `Assets/Scripts/NovelCore/Tests/Editor/Builders/TestDataBuilders.cs`
- [X] T112c [P] Move data model tests to EditMode: `SceneDataTests`, `CharacterDataTests`, `DialogueLineDataTests`, `ChoiceDataTests`, `CharacterPlacementTests`
- [X] T112d [P] Move DialogueSystem tests to EditMode: `DialogueSystemTests.cs` (pure C# logic, no Unity runtime needed)
- [X] T112e Move builder tests (`SampleTest.cs`) to EditMode
- [X] T112f Update `NovelCore.Tests.Editor.asmdef` to reference Addressables, NovelCore.Runtime, TestFramework
- [X] T112g [P] Run EditMode tests to verify migration successful
- [X] T112h Clean up old Runtime test files after confirming EditMode tests pass

**Rationale**: EditMode tests for ScriptableObject creation, data validation, and pure C# logic execute 60-80% faster than PlayMode tests and are more reliable (no Unity runtime variability). Only SaveSystemTests require PlayMode due to async/await operations and file I/O (`Application.persistentDataPath`, `Directory`/`File` APIs).

---

## 🧩 ITERATION 29: Unit Tests - Core Data Models (2-3 hours)

**Goal**: Unit tests for SceneData, CharacterData, DialogueLineData, ChoiceData validation

**Visible Result**: Test Runner shows 20+ passing unit tests for data models

**Validation**:
- Run Test Runner → see tests for all data models
- Tests verify data validation (null checks, required fields)
- Tests verify serialization/deserialization
- Tests verify edge cases (empty dialogue, missing emotions)
- Code coverage report shows >80% for data model classes

**Tasks**:
- [X] T120 [P] Write unit tests for SceneData in `Assets/Scripts/NovelCore/Tests/Runtime/Data/SceneDataTests.cs`
- [X] T121 [P] Write unit tests for CharacterData in `Assets/Scripts/NovelCore/Tests/Runtime/Data/CharacterDataTests.cs`
- [X] T122 [P] Write unit tests for DialogueLineData in `Assets/Scripts/NovelCore/Tests/Runtime/Data/DialogueLineDataTests.cs`
- [X] T123 [P] Write unit tests for ChoiceData in `Assets/Scripts/NovelCore/Tests/Runtime/Data/ChoiceDataTests.cs`
- [X] T124 [P] Write unit tests for CharacterPlacement in `Assets/Scripts/NovelCore/Tests/Runtime/Data/CharacterPlacementTests.cs`

---

## 🗣️ ITERATION 30: Unit Tests - Dialogue System (3-4 hours)

**Goal**: Unit tests for DialogueSystem covering dialogue flow, state management, choice handling

**Visible Result**: Test Runner shows 30+ passing tests for DialogueSystem

**Validation**:
- Run Test Runner → see DialogueSystem tests
- Tests verify dialogue advance, speaker changes, emotion switching
- Tests verify choice point detection and navigation
- Tests verify dialogue completion and scene transitions
- Mocked dependencies (IAssetManager, ISceneManager) isolate DialogueSystem logic
- Code coverage >80% for DialogueSystem

**Tasks**:
- [X] T125 [P] Write unit tests for DialogueSystem initialization in `Assets/Scripts/NovelCore/Tests/Runtime/Core/DialogueSystemTests.cs`
- [X] T126 Write unit tests for dialogue advance logic (AdvanceDialogue, previous line, completion)
- [X] T127 Write unit tests for choice point handling (ShowChoices, SelectChoice, track history)
- [X] T128 Write unit tests for emotion switching during dialogue
- [X] T129 Write unit tests for dialogue state serialization (for save system integration)
- [X] T130 [P] Create mock implementations: MockAssetManager, MockSceneManager for test isolation

---

## 💾 ITERATION 31: Unit Tests - Save System (3-4 hours)

**Goal**: Unit tests for SaveSystem covering save/load, versioning, corruption handling

**Visible Result**: Test Runner shows 25+ passing tests for SaveSystem

**Validation**:
- Run Test Runner → see SaveSystem tests
- Tests verify save/load with JSON serialization
- Tests verify save format versioning and migration
- Tests verify corrupted save detection and recovery
- Tests verify multiple save slots management
- Code coverage >80% for SaveSystem

**Tasks**:
- [X] T131 [P] Write unit tests for SaveSystem save operations in `Assets/Scripts/NovelCore/Tests/Runtime/Core/SaveSystemTests.cs`
- [X] T132 Write unit tests for SaveSystem load operations (valid saves, missing saves, corrupted saves)
- [X] T133 Write unit tests for save format versioning (v1 → v2 migration scenarios)
- [X] T134 Write unit tests for auto-save triggers (scene transitions, choice points)
- [X] T135 Write unit tests for save slot management (create, delete, list slots)

---

## 🎨 ITERATION 32: Unit Tests - Asset & Scene Management (3-4 hours)

**Goal**: Unit tests for AddressablesAssetManager, SceneManager covering asset loading, scene transitions

**Visible Result**: Test Runner shows 30+ passing tests for asset and scene systems

**Validation**:
- Run Test Runner → see AssetManager and SceneManager tests
- Tests verify asset loading via Addressables (with mock Addressables operations)
- Tests verify missing asset detection and error handling
- Tests verify scene loading, background rendering, character placement
- Tests verify scene transitions with effects
- Code coverage >80% for AssetManager and SceneManager

**Tasks**:
- [ ] T136 [P] Write unit tests for AddressablesAssetManager in `Assets/Scripts/NovelCore/Tests/Runtime/Core/AssetManagementTests.cs`
- [ ] T137 Write unit tests for asset loading (success, missing asset, invalid format)
- [ ] T138 Write unit tests for asset reference tracking and cleanup
- [ ] T139 [P] Write unit tests for SceneManager in `Assets/Scripts/NovelCore/Tests/Runtime/Core/SceneManagementTests.cs`
- [ ] T140 Write unit tests for scene loading, background rendering, character placement
- [ ] T141 Write unit tests for scene transitions (Fade, Slide, Cut)

---

## 🎭 ITERATION 33: Unit Tests - Character Animation (2-3 hours)

**Goal**: Unit tests for UnityCharacterAnimator and SpineCharacterAnimator

**Visible Result**: Test Runner shows 20+ passing tests for character animation

**Validation**:
- Run Test Runner → see CharacterAnimator tests
- Tests verify emotion switching (happy → sad → angry)
- Tests verify entrance/exit animations
- Tests verify Spine animation integration (if Spine used)
- Code coverage >80% for character animators

**Tasks**:
- [ ] T142 [P] Write unit tests for UnityCharacterAnimator in `Assets/Scripts/NovelCore/Tests/Runtime/Animation/UnityCharacterAnimatorTests.cs`
- [ ] T143 Write unit tests for emotion switching logic
- [ ] T144 Write unit tests for character entrance/exit animations
- [ ] T145 [P] Write unit tests for SpineCharacterAnimator in `Assets/Scripts/NovelCore/Tests/Runtime/Animation/SpineCharacterAnimatorTests.cs`

---

## 🔗 ITERATION 34: Integration Tests - Dialogue + Save (3-4 hours)

**Goal**: Integration tests for dialogue system saving/loading mid-conversation

**Visible Result**: PlayMode tests show dialogue state persists across save/load

**Validation**:
- Run PlayMode Test Runner → see integration tests
- Tests verify saving dialogue state (current line, speaker, emotion)
- Tests verify loading saved dialogue resumes correctly
- Tests verify choice history persists across saves
- Tests verify auto-save triggers at choice points

**Tasks**:
- [ ] T146 [P] Write integration test: Save dialogue state mid-conversation in `Assets/Scripts/NovelCore/Tests/Runtime/Integration/DialogueSaveIntegrationTests.cs`
- [ ] T147 Write integration test: Load saved dialogue and verify state restoration
- [ ] T148 Write integration test: Choice history persists across save/load
- [ ] T149 Write integration test: Auto-save triggers on choice selection
- [ ] T150 Write integration test: Multiple save slots maintain separate dialogue states

---

## 🖼️ ITERATION 35: Integration Tests - Asset + Scene (3-4 hours)

**Goal**: Integration tests for scene loading assets via Addressables

**Visible Result**: PlayMode tests show scenes load with correct assets

**Validation**:
- Run PlayMode Test Runner → see asset + scene integration tests
- Tests verify scene loads background via Addressables
- Tests verify scene loads character sprites via Addressables
- Tests verify missing asset warnings displayed correctly
- Tests verify asset cleanup on scene unload

**Tasks**:
- [ ] T151 [P] Write integration test: Load scene with background asset in `Assets/Scripts/NovelCore/Tests/Runtime/Integration/AssetSceneIntegrationTests.cs`
- [ ] T152 Write integration test: Load scene with character sprites
- [ ] T153 Write integration test: Missing asset detection and error handling
- [ ] T154 Write integration test: Asset cleanup on scene transition
- [ ] T155 Write integration test: Multiple scenes reference same asset (reference counting)

---

## 🎵 ITERATION 36: Integration Tests - Audio + Dialogue (2-3 hours)

**Goal**: Integration tests for audio playback during dialogue and scenes

**Visible Result**: PlayMode tests show audio plays at correct moments

**Validation**:
- Run PlayMode Test Runner → see audio integration tests
- Tests verify background music starts on scene load
- Tests verify SFX plays on specific dialogue lines
- Tests verify music fades on scene transitions
- Tests verify audio stops on game quit/pause

**Tasks**:
- [ ] T156 [P] Write integration test: Background music plays on scene load in `Assets/Scripts/NovelCore/Tests/Runtime/Integration/AudioDialogueIntegrationTests.cs`
- [ ] T157 Write integration test: SFX triggers on dialogue line
- [ ] T158 Write integration test: Music fade-out on scene transition
- [ ] T159 Write integration test: Audio volume controls affect playback
- [ ] T160 Write integration test: Audio stops on game pause/quit

---

## 🖱️ ITERATION 37: Integration Tests - Input + UI (3-4 hours)

**Goal**: Integration tests for input handling (clicks/taps) updating UI

**Visible Result**: PlayMode tests show clicking advances dialogue, selects choices

**Validation**:
- Run PlayMode Test Runner → see input + UI integration tests
- Tests verify clicking/tapping advances dialogue
- Tests verify clicking choice button navigates to target scene
- Tests verify keyboard shortcuts work (space advances, ESC opens menu)
- Tests verify touch input works identically to mouse (cross-platform parity)

**Tasks**:
- [ ] T161 [P] Write integration test: Click advances dialogue in `Assets/Scripts/NovelCore/Tests/Runtime/Integration/InputUIIntegrationTests.cs`
- [ ] T162 Write integration test: Click choice button navigates to scene
- [ ] T163 Write integration test: Keyboard shortcuts (Space, ESC) work
- [ ] T164 Write integration test: Touch input advances dialogue (mobile)
- [ ] T165 Write integration test: Input blocked during scene transitions

---

## 🌐 ITERATION 38: Integration Tests - Platform Services (3-4 hours)

**Goal**: Integration tests for platform-specific features (Steam, iCloud, Google Play)

**Visible Result**: PlayMode tests show cloud save integration works

**Validation**:
- Run PlayMode Test Runner → see platform service integration tests
- Tests verify Steam cloud save upload/download (with mock Steamworks)
- Tests verify iOS iCloud save sync (with mock iCloud APIs)
- Tests verify Android Google Play save sync (with mock Google Play Services)
- Tests verify platform service fallback if APIs unavailable

**Tasks**:
- [ ] T166 [P] Write integration test: Steam cloud save upload in `Assets/Scripts/NovelCore/Tests/Runtime/Integration/PlatformServicesIntegrationTests.cs`
- [ ] T167 Write integration test: Steam cloud save download and conflict resolution
- [ ] T168 [P] Write integration test: iOS iCloud save sync
- [ ] T169 [P] Write integration test: Android Google Play save sync
- [ ] T170 Write integration test: Platform service fallback (offline mode)

---

## 🌍 ITERATION 39: Integration Tests - Localization (2-3 hours)

**Goal**: Integration tests for language switching updating dialogue text

**Visible Result**: PlayMode tests show dialogue updates when language changes

**Validation**:
- Run PlayMode Test Runner → see localization integration tests
- Tests verify switching language updates dialogue text
- Tests verify switching language updates UI labels
- Tests verify localization keys resolve correctly
- Tests verify missing translations show fallback text

**Tasks**:
- [ ] T171 [P] Write integration test: Switch language updates dialogue in `Assets/Scripts/NovelCore/Tests/Runtime/Integration/LocalizationIntegrationTests.cs`
- [ ] T172 Write integration test: Switch language updates UI labels
- [ ] T173 Write integration test: Missing translation shows fallback
- [ ] T174 Write integration test: Localization persists across saves

---

## 📊 ITERATION 40: Test Coverage & Regression Suite (3-4 hours)

**Goal**: Achieve >80% code coverage, automate regression tests for P1/P2 user stories

**Visible Result**: Coverage report shows >80% for all core systems, regression suite runs in CI/CD

**Validation**:
- Run Unity Test Framework with code coverage plugin
- Coverage report shows:
  - DialogueSystem: >80% coverage
  - SaveSystem: >80% coverage
  - AssetManager: >80% coverage
  - SceneManager: >80% coverage
  - CharacterAnimator: >80% coverage
- Regression suite (P1/P2 user stories) runs automatically on every commit
- CI/CD pipeline blocks merges if tests fail

**Tasks**:
- [ ] T175 Install Unity Code Coverage package via Package Manager
- [ ] T176 Configure code coverage for test assemblies in Unity Test Framework settings
- [ ] T177 Run coverage report and identify gaps (<80% coverage) in core systems
- [ ] T178 Write additional unit tests to fill coverage gaps
- [ ] T179 Create regression test suite for P1 user story (Create Basic Scene) in `Assets/Scripts/NovelCore/Tests/Runtime/Regression/US1_BasicSceneRegressionTests.cs`
- [ ] T180 Create regression test suite for P2 user story (Branching Narrative) in `Assets/Scripts/NovelCore/Tests/Runtime/Regression/US2_BranchingRegressionTests.cs`
- [ ] T181 Configure CI/CD pipeline to run test suite on every commit (GitHub Actions or similar)
- [ ] T182 Document test execution and coverage reporting in README.md

---

# ITERATION SUMMARY

## MVP Phase (v0.1.0 - v0.3.0)

| Iteration | Goal | Duration | Deliverable |
|-----------|------|----------|-------------|
| 0 | Project Bootstrap | 1-2h | Unity project with folders |
| 1 | DI Infrastructure | 2-3h | VContainer working |
| 2 | Asset Management | 2-3h | Load sprites via Addressables |
| 3 | Audio & Input | 1-2h | Play sounds, handle clicks |
| 4 | Basic Dialogue | 3-4h | Dialogue box with text |
| 5 | Scene Rendering | 2-3h | Background + character display |
| 6 | Scene Editor | 3-4h | Editor window with drag-and-drop |
| 7 | Preview Mode | 1-2h | **🎯 MVP: First complete scene!** |
| 8 | Choice Data | 2h | ChoiceData ScriptableObjects |
| 9 | Choice UI | 3h | Clickable choice buttons |
| 10 | Story Flow | 3-4h | Visual graph of story structure |
| 11 | Character Data | 2h | CharacterData with emotions |
| 12 | Character Animation | 3h | Emotion switching |
| 13 | Character Editor | 3h | Editor window for characters |
| 14 | Audio Integration | 2-3h | Music + SFX in scenes |
| 15 | Scene Transitions | 3h | Fade/slide effects |
| 16 | Audio/Effects UI | 2h | Drag-and-drop audio in editor |
| 17 | Build Config | 2h | BuildConfig data models |
| 18 | Platform Builders | 4-5h | Build .exe/.app/.apk |
| 19 | Platform Services | 3-4h | Steam/iOS/Android integration |
| 20 | Build Pipeline UI | 2-3h | Build window with UI |
| 21 | Pre-Build Validation | 2h | Asset validator |
| 22 | Save System | 3-4h | Save/load with cloud sync |
| 23 | Localization | 2-3h | Multi-language support |
| 24 | Project Wizard | 2h | New project setup wizard |
| 25 | Performance | 3-4h | 60 FPS optimization |
| 26 | Cleanup & Docs | 2-3h | Code polish, README |
| 27 | Final Validation | 2-3h | **✅ MVP feature-complete** |

**MVP Phase Total**: 27 iterations, 65-85 hours

## Post-MVP Testing Phase (v0.4.0+) - MANDATORY

| Iteration | Goal | Duration | Deliverable |
|-----------|------|----------|-------------|
| 28 | Test Infrastructure | 3-4h | UTF configured, test assemblies |
| 29 | Unit Tests - Data Models | 2-3h | 20+ tests for SceneData, CharacterData |
| 30 | Unit Tests - Dialogue System | 3-4h | 30+ tests for DialogueSystem |
| 31 | Unit Tests - Save System | 3-4h | 25+ tests for SaveSystem |
| 32 | Unit Tests - Asset & Scene | 3-4h | 30+ tests for AssetManager, SceneManager |
| 33 | Unit Tests - Character Animation | 2-3h | 20+ tests for CharacterAnimator |
| 34 | Integration - Dialogue + Save | 3-4h | Save/load dialogue state tests |
| 35 | Integration - Asset + Scene | 3-4h | Scene asset loading tests |
| 36 | Integration - Audio + Dialogue | 2-3h | Audio playback tests |
| 37 | Integration - Input + UI | 3-4h | Click/tap interaction tests |
| 38 | Integration - Platform Services | 3-4h | Cloud save integration tests |
| 39 | Integration - Localization | 2-3h | Language switching tests |
| 40 | Coverage & Regression Suite | 3-4h | **✅ >80% coverage, CI/CD** |

**Testing Phase Total**: 13 iterations, 35-47 hours

## Combined Total

**Total Iterations**: 40 (MVP: 27, Testing: 13)  
**Estimated Solo Duration**: 100-132 hours  
**MVP Checkpoint**: Iteration 7 (first 19 hours)  
**MVP Feature-Complete**: Iteration 27 (65-85 hours)  
**Production Ready (v1.0.0)**: Iteration 40 (100-132 hours, >80% test coverage)

---

# ITERATION FLOW

```
ITERATION 0-3: Foundation (Infrastructure)
    ↓
ITERATION 4-7: MVP - User Story 1 (Basic Scene) 🎯
    ↓
ITERATION 8-10: User Story 2 (Branching)
    ↓
ITERATION 11-13: User Story 3 (Characters/Emotions)
    ↓
ITERATION 14-16: User Story 4 (Audio/Effects)
    ↓
ITERATION 17-21: User Story 5 (Build/Publish)
    ↓
ITERATION 22-26: Polish & Cross-Cutting
    ↓
ITERATION 27: MVP Feature-Complete ✅
    ↓
========== POST-MVP TESTING PHASE (v0.4.0+) ==========
    ↓
ITERATION 28: Test Infrastructure Setup 🧪
    ↓
ITERATION 29-33: Unit Tests (All Core Systems)
    ↓
ITERATION 34-39: Integration Tests (Cross-Module Contracts)
    ↓
ITERATION 40: Coverage & Regression Suite ✅
    ↓
========== PRODUCTION READY (v1.0.0) ==========
```

---

# KEY MILESTONES

## MVP Phase (v0.1.0 - v0.3.0)

- **After Iteration 3**: Foundation complete, can start user story work
- **After Iteration 7**: 🎯 **MVP READY** - Can create basic visual novel scene with dialogue
- **After Iteration 10**: Branching narratives work
- **After Iteration 13**: Characters with emotions work
- **After Iteration 16**: Audio and effects work
- **After Iteration 21**: Can build and publish to all platforms
- **After Iteration 27**: ✅ **MVP FEATURE-COMPLETE** - Full feature set validated (manual testing)

## Post-MVP Testing Phase (v0.4.0+)

- **After Iteration 28**: Test infrastructure ready (UTF, assemblies, mocks)
- **After Iteration 33**: All core systems have >80% unit test coverage
- **After Iteration 39**: All cross-module contracts have integration tests
- **After Iteration 40**: ✅ **PRODUCTION READY (v1.0.0)** - >80% coverage, regression suite, CI/CD validation

---

# NOTES

## MVP Phase (Iterations 0-27)

- Each iteration is **independently testable** - you get visible results at the end
- **Iteration 7 is your MVP checkpoint** - stop here for first demo
- Iterations can be **parallelized** within user stories if you have team members
- **Stop and validate** after each iteration before proceeding
- **Commit after each iteration** - you have a working state
- Constitution Principle VI MVP Exception applies: manual testing for MVP phase

## Post-MVP Testing Phase (Iterations 28-40)

- **MANDATORY for v1.0.0 production release** - cannot ship without >80% test coverage
- **Start immediately after Iteration 27** (MVP feature-complete)
- Test iterations 28-40 MUST be completed before v1.0.0 release
- **Test-first development**: For critical systems (save, dialogue branching), write tests before implementation where possible
- **Code coverage target**: >80% for all core systems (DialogueSystem, SaveSystem, AssetManager, SceneManager, CharacterAnimator)
- **Integration test priority**: Focus on cross-module contracts that unit tests cannot catch:
  - Dialogue + Save (state persistence)
  - Asset + Scene (Addressables loading)
  - Audio + Dialogue (timing and triggers)
  - Input + UI (interaction flows)
  - Platform Services (cloud saves)
  - Localization (language switching)
- **CI/CD integration**: Test suite MUST run automatically on every commit starting v0.4.0
- **Regression prevention**: P1 and P2 user stories MUST have automated regression tests

---

**Ready to Start**: 
- **MVP Phase**: Begin with Iteration 0 (Project Bootstrap) 🚀
- **Testing Phase**: Begin with Iteration 28 after completing Iteration 27 🧪
