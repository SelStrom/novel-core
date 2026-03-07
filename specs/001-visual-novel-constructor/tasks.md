# Tasks: Visual Novel Constructor

**Input**: Design documents from `/specs/001-visual-novel-constructor/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

**Tests**: Automated tests deferred to post-MVP per Constitution Principle VI (MVP Exception). MVP (v0.1.0-v0.3.0) will use manual testing and creator dogfooding. Automated test suite (>80% coverage) will be implemented incrementally starting v0.4.0 before production release (v1.0.0). Tasks below focus on implementation only.

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
- [ ] T002 [P] Install Unity packages via Package Manager: Addressables 2.0+, URP, Localization 2.0+, Input System 1.8+ (MANUAL - requires Unity Editor)
- [X] T003 [P] Configure URP Asset (2D Renderer) in `Assets/Settings/UniversalRP-2DRenderer.asset`
- [X] T004 [P] Create assembly definition files: `Assets/Scripts/NovelCore/Runtime/NovelCore.Runtime.asmdef`
- [X] T005 [P] Create assembly definition: `Assets/Scripts/NovelCore/Editor/NovelCore.Editor.asmdef` with Editor platform
- [X] T006 Create `.editorconfig` file at project root (already exists with C# code style rules)
- [X] T007 [P] Create folder structure: `Assets/Content/{Backgrounds,Characters,Audio,Localization,Projects}`
- [ ] T008 [P] Configure project settings: Scripting Backend (Mono for Windows, IL2CPP for others) (MANUAL - requires Unity Editor)

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
- [ ] T027 [P] [US1] Create DialogueBox UI prefab in `Assets/Resources/NovelCore/UI/DialogueBox.prefab` (manual in Unity Editor)
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
- [ ] T034 [P] [US1] Create SceneEditorWindow in `Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs`
- [ ] T035 [US1] Implement background image drag-and-drop in SceneEditorWindow
- [ ] T036 [US1] Implement dialogue line editor UI in SceneEditorWindow
- [ ] T037 [US1] Implement character placement UI in SceneEditorWindow

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
- [ ] T038 [US1] Add "Preview Scene" button to SceneEditorWindow that enters Play mode
- [ ] T040 [US1] Create sample SceneData asset for testing in `Assets/Content/Projects/Sample/`

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
- [ ] T044 [P] [US2] Create ChoiceUI prefab in `Assets/Resources/NovelCore/UI/ChoiceButton.prefab` (manual in Unity Editor)
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
- [ ] T049 [P] [US2] Create StoryFlowWindow in `Assets/Scripts/NovelCore/Editor/Windows/StoryFlowWindow.cs`
- [ ] T050 [US2] Implement graph visualization using Unity GraphView in StoryFlowWindow
- [ ] T051 [US2] Display scene nodes and choice connections in StoryFlowWindow
- [ ] T052 [US2] Add choice editor UI to SceneEditorWindow
- [ ] T053 [US2] Implement choice validation (detect broken links) in SceneEditorWindow
- [ ] T054 [US2] Add warning indicators for scenes with no paths to endings

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
- [ ] T063 [P] [US3] Create CharacterEditorWindow in `Assets/Scripts/NovelCore/Editor/Windows/CharacterEditorWindow.cs`
- [ ] T064 [US3] Implement emotion sprite upload UI in CharacterEditorWindow
- [ ] T065 [US3] Add emotion dropdown to dialogue line editor in SceneEditorWindow
- [ ] T066 [US3] Implement active speaker highlighting in DialogueBoxController

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
- [ ] T073 [P] [US4] Create transition shaders (Fade, Slide) in `Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/Transitions/`
- [ ] T074 [US4] Implement scene transition effects in SceneManager
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
- [ ] T076 [US4] Add music drag-and-drop to SceneEditorWindow
- [ ] T077 [US4] Add SFX drag-and-drop to dialogue line editor in SceneEditorWindow
- [ ] T078 [US4] Add transition type dropdown to SceneEditorWindow
- [ ] T079 [US4] Add entrance animation controls to character placement UI

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
- [ ] T097 [P] Implement ISaveSystem interface in `Assets/Scripts/NovelCore/Runtime/Core/SaveSystem/ISaveSystem.cs`
- [ ] T098 Implement SaveSystem with JSON serialization in `Assets/Scripts/NovelCore/Runtime/Core/SaveSystem/SaveSystem.cs`
- [ ] T099 Add auto-save on scene transitions and choices in SceneManager
- [ ] T100 Implement save/load UI in `Assets/Resources/NovelCore/UI/SaveLoadUI.prefab` (manual in Unity Editor)

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

# ITERATION SUMMARY

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
| 27 | Final Validation | 2-3h | **✅ Full release validation** |

**Total Iterations**: 27  
**Estimated Solo Duration**: 65-85 hours  
**MVP Checkpoint**: Iteration 7 (first 19 hours)  
**Production Ready**: Iteration 27

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
ITERATION 27: Final Validation ✅
```

---

# KEY MILESTONES

- **After Iteration 3**: Foundation complete, can start user story work
- **After Iteration 7**: 🎯 **MVP READY** - Can create basic visual novel scene with dialogue
- **After Iteration 10**: Branching narratives work
- **After Iteration 13**: Characters with emotions work
- **After Iteration 16**: Audio and effects work
- **After Iteration 21**: Can build and publish to all platforms
- **After Iteration 27**: ✅ **PRODUCTION READY** - Full feature set validated

---

# NOTES

- Each iteration is **independently testable** - you get visible results at the end
- **Iteration 7 is your MVP checkpoint** - stop here for first demo
- Iterations can be **parallelized** within user stories if you have team members
- **Stop and validate** after each iteration before proceeding
- **Commit after each iteration** - you have a working state
- Constitution Principle VI MVP Exception applies: manual testing for iterations 0-21

---

**Ready to Start**: Begin with Iteration 0 (Project Bootstrap) 🚀
