# Implementation Plan: Visual Novel Constructor

**Branch**: `001-visual-novel-constructor` | **Date**: 2026-03-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-visual-novel-constructor/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Build a Unity-based visual novel constructor enabling non-programmers to create professional visual novels with branching narratives, character emotions, audio, and cross-platform publishing to Steam and mobile. The system will provide a visual editor for scene creation, dialogue management, and story flow design, with a build pipeline generating standalone executables and mobile packages. Technical approach uses Unity 6 (LTS) with Universal Render Pipeline (URP) 2D, Addressables for asset management, VContainer for dependency injection, and Spine for advanced character animations.

## Technical Context

> **📘 Centralized Documentation**: For complete technical stack details, see [`.specify/memory/tech-stack.md`](../../.specify/memory/tech-stack.md)

**Feature-Specific Details**:
- **Unity Project Location**: `./novel-core` (repository root)
- **Project Type**: Hybrid editor tool + runtime system (creator uses Unity Editor, end-users run published games)
- **Scale/Scope**: Support 200+ scenes, 50+ branching choice points, 500+ assets per project, 10+ hour visual novels

## Constitution Check

> **📘 Project Constitution**: For complete principle definitions, see [`.specify/memory/constitution.md`](../../.specify/memory/constitution.md)

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Feature Compliance Summary

✅ **I. Creator-First Design** - Custom editor windows, story flow graph, Play Mode preview, user-friendly validators  
✅ **II. Cross-Platform Parity** - Addressables + IL2CPP ensure Windows/macOS/iOS/Android parity  
✅ **III. Asset Pipeline Integrity** - Addressables mandatory, asset validators, migration scripts  
✅ **IV. Runtime Performance Guarantees** - 60 FPS target, memory budgets met, async loading  
✅ **V. Save System Reliability** - Auto-save on state changes, versioned format, cloud sync  
✅ **VI. Modular Architecture & Testing** - Assembly definitions, DI via VContainer, >80% coverage target  
✅ **VII. AI Development Constraints** - Code limited to Assets/Scripts/, no .meta modification  
✅ **VIII. Editor-Runtime Bridge** - EditorPrefs bridge for Scene Editor preview mode  
✅ **IX. User Documentation Language** - Russian for end-user docs, English for technical docs  

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
