# Implementation Plan: Visual Novel Constructor

**Branch**: `001-visual-novel-constructor` | **Date**: 2026-03-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-visual-novel-constructor/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Build a Unity-based visual novel constructor enabling non-programmers to create professional visual novels with branching narratives, character emotions, audio, and cross-platform publishing to Steam and mobile. The system will provide a visual editor for scene creation, dialogue management, and story flow design, with a build pipeline generating standalone executables and mobile packages. Technical approach uses Unity 6 (LTS) with Universal Render Pipeline (URP) 2D, Addressables for asset management, VContainer for dependency injection, and Spine for advanced character animations.

## Technical Context

**Unity Project Location**: `./novel-core` (repository root)  
**Language/Version**: C# 9.0 / Unity 6 (Long Term Support)  
**Primary Dependencies**: Unity Addressables 2.0+, VContainer 1.14+, Spine-Unity 4.2+, Steamworks.NET 20.2+, Unity Localization 2.0+  
**Rendering Pipeline**: Universal Render Pipeline (URP) 2D  
**Storage**: JSON for save files (serialized via JsonUtility), Addressables for asset catalogs, PlayerPrefs for lightweight settings  
**Game Entry Point**: GameStarter component initializes VContainer, loads starting scene, starts DialogueSystem/SceneManager. Supports Play Mode full start and Scene Editor preview modes.  
**Testing**: Unity Test Framework (UTF), NUnit, **EditMode-first strategy** (unit tests for ScriptableObjects, data validation, pure C# logic), PlayMode tests only for async operations, file I/O, runtime-specific Unity APIs, integration tests requiring game loop (including GameStarter initialization), >80% code coverage target (post-MVP v0.4.0+), integration tests for cross-module contracts  
**Target Platforms**: Windows x64, macOS (Intel + Apple Silicon), iOS 15+, Android API 24+ (Nougat)  
**Scripting Backend**: IL2CPP (all platforms)  
**Project Type**: Hybrid editor tool + runtime system (creator uses Unity Editor, end-users run published games)  
**Performance Goals**: 60 FPS on iPhone 13/Intel HD 620, scene transitions <1 second, <512MB RAM mobile / <1GB desktop  
**Constraints**: Steam/App Store/Google Play compliance, GDPR opt-in analytics, UTF-8 localization support  
**Scale/Scope**: Support 200+ scenes, 50+ branching choice points, 500+ assets per project, 10+ hour visual novels

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

✅ **I. Creator-First Design**
   - [x] Feature accessible via Unity Editor GUI (no code required) - Custom editor windows for scene/dialogue/character management
   - [x] Visual/node-based interface where applicable - Story flow graph for branching visualization
   - [x] Preview mode provides immediate feedback - Play mode preview without build required
   - [x] Error messages in plain language with solutions - Asset validators with user-friendly warnings

✅ **II. Cross-Platform Parity (NON-NEGOTIABLE)**
   - [x] Feature works identically on Windows, macOS, iOS, Android - Addressables + IL2CPP ensure parity
   - [x] Automated tests on at least 3 platforms - UTF PlayMode tests on Windows/Mac/iOS required
   - [x] No platform-specific logic unless technically impossible - Input abstraction via new Input System

✅ **III. Asset Pipeline Integrity**
   - [x] All asset references use Addressables/persistent IDs - Addressables mandatory per user requirement
   - [x] Missing assets detected at import/edit time - AssetImportProcessor + custom validators
   - [x] Migration script included for format changes - ScriptableObject versioning + upgrade paths

✅ **IV. Runtime Performance Guarantees**
   - [x] 60 FPS on target hardware (iPhone 12 / Intel HD 620) - 2D sprite rendering, optimized draw calls
   - [x] Mobile: ≤512MB RAM, Desktop: ≤1GB RAM - Addressables streaming, unload unused assets
   - [x] Scene transitions <1 second - Async scene loading, preload critical assets
   - [x] Performance profiling included - Unity Profiler integration, automated performance tests

✅ **V. Save System Reliability (NON-NEGOTIABLE)**
   - [x] Auto-save after state changes (if affects player progress) - Save on choice selection + scene transitions
   - [x] Save format backward compatible or includes migration - Versioned save format with upgrade handlers
   - [x] Cloud sync supported (Steam/iCloud/Google Play) - Steamworks.NET + platform-specific APIs

✅ **VI. Modular Architecture & Testing (NON-NEGOTIABLE)**
   - [x] Feature implemented as separate assembly/module - Assembly definitions per system (Dialogue, Save, Asset, etc.)
   - [x] Interfaces used for platform-specific code - IPlatformService for Steam/iOS/Android abstractions
   - [x] Unit tests achieve >80% coverage - UTF test assemblies per module (post-MVP v0.4.0+)
   - [x] Integration tests for cross-module contracts - PlayMode tests for system interactions (post-MVP v0.4.0+)
   - [x] Test organization in separate assemblies - NovelCore.Tests.Runtime, NovelCore.Tests.Editor
   - [x] EditMode-first test strategy - Data models, validation, builders in EditMode; async/I/O in PlayMode
   - [x] Test-first development for critical systems - Save system, dialogue branching, asset management
   - [x] Call stack analysis for modifications - Analyze all callers before modifying existing logic to prevent regressions
   - [x] Continuous validation via CI/CD - Test suite runs on pre-commit and pipeline

✅ **VII. AI Development Constraints (NON-NEGOTIABLE)**
   - [x] AI modifications limited to Assets/Scripts/ directory only - All code in Assets/Scripts/NovelCore/
   - [x] No .meta file generation or modification by AI - Unity generates .meta files
   - [x] No direct asset file creation (prefabs, scenes, materials) - Created via Unity Editor only
   - [x] Package management permitted when user-specified - AI can install/update packages via manifest.json modifications

✅ **VIII. Editor-Runtime Bridge (NON-NEGOTIABLE)**
   - [x] Preview state transfer from Editor to Runtime - SceneEditorWindow → EditorPrefs → GameStarter
   - [x] GameStarter checks preview state on initialization - GetSceneToLoad() method with EditorPrefs check
   - [x] Fallback to default starting scene if preview invalid - Robust error handling
   - [x] Preview state cleanup after consumption - Prevents stale data
   - [x] PreviewManager recommended for centralized management - Encapsulates preview logic

**GATE RESULT**: ✅ PASSED - All constitution principles satisfied

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command - for developers)
├── user-manual.md       # Phase 1 output (/speckit.plan command - for content creators)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
Assets/
├── Scripts/                    # All C# code (AI can modify this directory only)
│   └── NovelCore/
│       ├── Runtime/            # Runtime systems (included in builds)
│       │   ├── Core/
│       │   │   ├── GameStarter.cs          # Entry point: initializes DI, loads starting scene
│       │   │   ├── GameLifetimeScope.cs    # VContainer DI root scope
│       │   │   ├── DialogueSystem/     # Dialogue engine, text rendering
│       │   │   ├── SceneManagement/    # Scene loading, transitions
│       │   │   ├── SaveSystem/         # Save/load, cloud sync
│       │   │   ├── AssetManagement/    # Addressables wrappers
│       │   │   ├── InputHandling/      # Input abstraction layer
│       │   │   ├── Localization/       # Localization service
│       │   │   └── AudioSystem/        # Audio wrapper (extensible)
│       │   ├── Data/                   # ScriptableObjects, data models
│       │   │   ├── Scenes/             # Scene data definitions
│       │   │   ├── Characters/         # Character configurations
│       │   │   ├── Dialogue/           # Dialogue line data
│       │   │   └── Choices/            # Choice point definitions
│       │   ├── UI/                     # Runtime UI components
│       │   │   ├── DialogueBox/        # Dialogue display
│       │   │   ├── ChoiceButtons/      # Choice UI
│       │   │   ├── SaveLoadUI/         # Save/load screens
│       │   │   └── SettingsUI/         # Settings menu
│       │   ├── Animation/              # Animation controllers
│       │   │   ├── CharacterAnimator/  # Character transitions
│       │   │   └── SpineIntegration/   # Spine animation wrapper
│       │   └── Platform/               # Platform abstractions
│       │       ├── Interfaces/         # IPlatformService, ISaveProvider
│       │       ├── Steam/              # Steamworks integration
│       │       ├── iOS/                # iOS-specific (iCloud, etc.)
│       │       └── Android/            # Android-specific (Google Play)
│       ├── Editor/                     # Editor-only tools (excluded from builds)
│       │   ├── Windows/
│       │   │   ├── SceneEditorWindow/      # Scene creation UI
│       │   │   ├── DialogueEditorWindow/   # Dialogue editor
│       │   │   ├── CharacterEditorWindow/  # Character config
│       │   │   ├── StoryFlowWindow/        # Story graph visualization
│       │   │   └── BuildPipelineWindow/    # Build configuration
│       │   ├── Inspectors/
│       │   │   ├── SceneDataInspector/     # Custom scene inspector
│       │   │   ├── CharacterInspector/     # Custom character inspector
│       │   │   └── DialogueInspector/      # Custom dialogue inspector
│       │   ├── Tools/
│       │   │   ├── AssetValidator/         # Validate asset references
│       │   │   ├── ProjectSetup/           # Initial project setup wizard
│       │   │   └── BuildPipeline/          # Multi-platform build automation
│       │   └── Importers/
│       │       ├── LocalizationImporter/   # CSV → Unity Localization
│       │       └── SpineAssetProcessor/    # Spine import validation
│       └── Tests/
│           ├── Runtime/                # PlayMode tests
│           │   ├── DialogueSystemTests/
│           │   ├── SaveSystemTests/
│           │   ├── SceneManagementTests/
│           │   └── IntegrationTests/
│           └── Editor/                 # EditMode tests
│               ├── AssetValidatorTests/
│               ├── EditorWindowTests/
│               └── BuildPipelineTests/
├── Content/                    # User-created content (per user requirement)
│   ├── Backgrounds/            # Background images
│   ├── Characters/             # Character sprites
│   ├── Audio/                  # Music and SFX
│   │   ├── Music/
│   │   └── SFX/
│   ├── Localization/           # Localization tables
│   └── Projects/               # User visual novel projects
│       └── [ProjectName]/      # Each project is a folder
│           ├── Scenes/
│           ├── Characters/
│           └── Data/
├── Resources/                  # Runtime-loaded assets (fallbacks, UI prefabs)
│   └── NovelCore/
│       ├── UI/                 # UI prefabs (dialogue box, buttons)
│       ├── DefaultAssets/      # Fallback sprites, fonts
│       └── Configuration/      # Default ScriptableObjects
├── StreamingAssets/            # Addressables catalog, platform-specific data
│   └── aa/                     # Addressables build output
└── AddressableAssets/          # Addressables asset groups (Content folder references)
    ├── Settings/
    └── Groups/

Packages/
├── manifest.json               # Package dependencies (manual config)
└── packages-lock.json          # Generated lock file
```

**Structure Decision**: Hybrid Runtime + Editor with strict separation:
- **Assets/Scripts/NovelCore**: All code lives here (AI-modifiable per Principle VII)
- **GameStarter.cs**: Entry point component attached to Unity scene, initializes VContainer, loads starting scene (with preview override support per Principle VIII)
- **GameLifetimeScope.cs**: VContainer root scope registering all services
- **PreviewManager.cs** (optional, recommended): Centralized preview state management for Editor-Runtime bridge
- **Assets/Content**: User content folder (Addressables source, not AI-modifiable)
- **Runtime/**: Modular systems with assembly definitions per subsystem (Principle VI)
- **Editor/**: Custom editor tools for creator-first UX (Principle I), including Scene Editor with preview functionality
- **Tests/**: Separate assemblies for runtime vs editor tests (Principle VI)
- **Platform/**: Abstraction layer for Steam/iOS/Android (Principle II)

## Preview Architecture (Principle VIII: Editor-Runtime Bridge)

### Preview Workflow

```
1. Creator clicks "Preview Scene" in SceneEditorWindow
   ↓
2. SceneEditorWindow.PreviewScene():
   - Stores scene path in EditorPrefs["NovelCore_PreviewScene"]
   - Enters Play Mode (EditorApplication.isPlaying = true)
   ↓
3. Unity starts Play Mode → GameStarter.Start() executes
   ↓
4. GameStarter.GetSceneToLoad():
   - #if UNITY_EDITOR: Checks EditorPrefs["NovelCore_PreviewScene"]
   - If preview scene found: Load preview scene, clear EditorPrefs
   - Else: Load _startingScene (default from Inspector)
   ↓
5. SceneManager.LoadScene(scene) → DialogueSystem.StartScene(scene)
   ↓
6. Creator sees preview scene in Play Mode (isolated testing)
```

### Key Components

**SceneEditorWindow** (Editor):
- Provides "Preview Scene" button
- Writes preview state to EditorPrefs
- Validates scene path before entering Play Mode

**GameStarter** (Runtime):
- Checks for preview state on initialization
- Falls back to default scene if preview missing/invalid
- Clears preview state after consumption

**PreviewManager** (Optional, Runtime):
- Centralized API: `SetPreviewScene()`, `GetPreviewScene()`, `IsPreviewMode`
- Encapsulates EditorPrefs key management
- Reusable for future preview features (dialogue line preview, choice preview)

## Complexity Tracking

No constitution violations - all principles satisfied with current architecture.
