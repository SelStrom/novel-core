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
**Testing**: Unity Test Framework (UTF), NUnit, PlayMode tests for runtime validation, EditMode tests for editor tools  
**Target Platforms**: Windows x64, macOS (Intel + Apple Silicon), iOS 15+, Android API 24+ (Nougat)  
**Scripting Backend**: Mono (Windows), IL2CPP (macOS, iOS, Android)  
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

✅ **VI. Modular Architecture**
   - [x] Feature implemented as separate assembly/module - Assembly definitions per system (Dialogue, Save, Asset, etc.)
   - [x] Interfaces used for platform-specific code - IPlatformService for Steam/iOS/Android abstractions
   - [x] Unit tests achieve >80% coverage - UTF test assemblies per module
   - [x] Integration tests for cross-module contracts - PlayMode tests for system interactions

✅ **VII. AI Development Constraints (NON-NEGOTIABLE)**
   - [x] AI modifications limited to Assets/Scripts/ directory only - All code in Assets/Scripts/NovelCore/
   - [x] No .meta file generation or modification by AI - Unity generates .meta files
   - [x] No direct asset file creation (prefabs, scenes, materials) - Created via Unity Editor only
   - [x] No ProjectSettings/ or package manifest changes by AI - Manual configuration documented

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
│       │   │   ├── DialogueSystem/     # Dialogue engine, text rendering
│       │   │   ├── SceneManagement/    # Scene loading, transitions
│       │   │   ├── SaveSystem/         # Save/load, cloud sync
│       │   │   ├── AssetManagement/    # Addressables wrappers
│       │   │   ├── InputHandling/      # Input abstraction layer
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
- **Assets/Content**: User content folder (Addressables source, not AI-modifiable)
- **Runtime/**: Modular systems with assembly definitions per subsystem (Principle VI)
- **Editor/**: Custom editor tools for creator-first UX (Principle I)
- **Tests/**: Separate assemblies for runtime vs editor tests (Principle VI)
- **Platform/**: Abstraction layer for Steam/iOS/Android (Principle II)

## Complexity Tracking

No constitution violations - all principles satisfied with current architecture.
