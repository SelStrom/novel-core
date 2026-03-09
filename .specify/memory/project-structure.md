# Novel Core - Project Structure

**Last Updated**: 2026-03-09

## Canonical Directory Structure

```text
novel-core/                     # Unity project root
├── Assets/
│   ├── Scripts/                # All C# code (AI can modify this directory only)
│   │   └── NovelCore/
│   │       ├── Runtime/        # Runtime systems (included in builds)
│   │       │   ├── Core/
│   │       │   │   ├── GameStarter.cs          # Entry point component
│   │       │   │   ├── GameLifetimeScope.cs    # VContainer DI root scope
│   │       │   │   ├── DialogueSystem/         # Dialogue engine
│   │       │   │   ├── SceneManagement/        # Scene loading, transitions
│   │       │   │   ├── SaveSystem/             # Save/load, cloud sync
│   │       │   │   ├── AssetManagement/        # Addressables wrappers
│   │       │   │   ├── InputHandling/          # Input abstraction
│   │       │   │   ├── Localization/           # Localization service
│   │       │   │   └── AudioSystem/            # Audio wrapper
│   │       │   ├── Data/                       # ScriptableObjects, data models
│   │       │   │   ├── Scenes/                 # Scene data definitions
│   │       │   │   ├── Characters/             # Character configurations
│   │       │   │   ├── Dialogue/               # Dialogue line data
│   │       │   │   └── Choices/                # Choice point definitions
│   │       │   ├── UI/                         # Runtime UI components
│   │       │   │   ├── DialogueBox/            # Dialogue display
│   │       │   │   ├── ChoiceButtons/          # Choice UI
│   │       │   │   ├── SaveLoadUI/             # Save/load screens
│   │       │   │   └── SettingsUI/             # Settings menu
│   │       │   ├── Animation/                  # Animation controllers
│   │       │   │   ├── CharacterAnimator/      # Character transitions
│   │       │   │   └── SpineIntegration/       # Spine animation wrapper
│   │       │   └── Platform/                   # Platform abstractions
│   │       │       ├── Interfaces/             # IPlatformService, ISaveProvider
│   │       │       ├── Steam/                  # Steamworks integration
│   │       │       ├── iOS/                    # iOS-specific (iCloud, etc.)
│   │       │       └── Android/                # Android-specific (Google Play)
│   │       ├── Editor/                         # Editor-only tools (excluded from builds)
│   │       │   ├── Windows/
│   │       │   │   ├── SceneEditorWindow/      # Scene creation UI
│   │       │   │   ├── DialogueEditorWindow/   # Dialogue editor
│   │       │   │   ├── CharacterEditorWindow/  # Character config
│   │       │   │   ├── StoryFlowWindow/        # Story graph visualization
│   │       │   │   └── BuildPipelineWindow/    # Build configuration
│   │       │   ├── Inspectors/
│   │       │   │   ├── SceneDataInspector/     # Custom scene inspector
│   │       │   │   ├── CharacterInspector/     # Custom character inspector
│   │       │   │   └── DialogueInspector/      # Custom dialogue inspector
│   │       │   ├── Tools/
│   │       │   │   ├── AssetValidator/         # Validate asset references
│   │       │   │   ├── ProjectSetup/           # Initial project setup wizard
│   │       │   │   ├── Generators/             # Editor script generators (AI-created)
│   │       │   │   └── BuildPipeline/          # Multi-platform build automation
│   │       │   └── Importers/
│   │       │       ├── LocalizationImporter/   # CSV → Unity Localization
│   │       │       └── SpineAssetProcessor/    # Spine import validation
│   │       └── Tests/
│   │           ├── Runtime/                    # PlayMode tests
│   │           │   ├── DialogueSystemTests/
│   │           │   ├── SaveSystemTests/
│   │           │   ├── SceneManagementTests/
│   │           │   └── IntegrationTests/
│   │           └── Editor/                     # EditMode tests
│   │               ├── AssetValidatorTests/
│   │               ├── EditorWindowTests/
│   │               └── BuildPipelineTests/
│   ├── Content/                                # User-created content (Addressables source)
│   │   ├── Backgrounds/                        # Background images
│   │   ├── Characters/                         # Character sprites
│   │   ├── Audio/                              # Music and SFX
│   │   │   ├── Music/
│   │   │   └── SFX/
│   │   ├── Localization/                       # Localization tables
│   │   └── Projects/                           # User visual novel projects
│   │       └── [ProjectName]/                  # Each project is a folder
│   │           ├── Scenes/
│   │           ├── Characters/
│   │           └── Data/
│   ├── Resources/                              # Runtime-loaded assets (fallbacks, UI prefabs)
│   │   └── NovelCore/
│   │       ├── UI/                             # UI prefabs (dialogue box, buttons)
│   │       ├── DefaultAssets/                  # Fallback sprites, fonts
│   │       └── Configuration/                  # Default ScriptableObjects
│   ├── StreamingAssets/                        # Addressables catalog, platform-specific data
│   │   └── aa/                                 # Addressables build output
│   └── AddressableAssets/                      # Addressables asset groups
│       ├── Settings/
│       └── Groups/
├── Packages/
│   ├── manifest.json                           # Package dependencies (AI can modify)
│   └── packages-lock.json                      # Generated lock file (Unity manages)
├── ProjectSettings/                            # Unity project settings (AI cannot modify)
└── .specify/                                   # Project documentation
    ├── memory/                                 # Centralized documentation
    │   ├── constitution.md                     # Project principles (SINGLE SOURCE OF TRUTH)
    │   ├── tech-stack.md                       # Technical stack (THIS FILE'S PARENT)
    │   ├── testing-strategy.md                 # Testing guidelines
    │   └── project-structure.md                # This file
    └── templates/                              # Document templates

specs/                                          # Feature specifications
├── 001-visual-novel-constructor/               # Core feature
│   ├── plan.md
│   ├── contracts/
│   │   └── runtime-contracts.md
│   ├── quickstart.md
│   └── ...
└── 001-scene-transition/                       # Scene transition feature
    ├── plan.md
    ├── spec.md
    ├── tasks.md
    ├── contracts/
    └── ...
```

## AI Modification Boundaries (Constitution Principle VII)

### ✅ AI CAN MODIFY
- `Assets/Scripts/NovelCore/` (all C# code)
- `Packages/manifest.json` (when user explicitly requests package operations)
- `.specify/` documentation files

### ❌ AI CANNOT MODIFY
- `.meta` files (Unity manages these)
- `.unity` scene files (created via Unity Editor only)
- `.prefab` files (created via Unity Editor only, but can be modified via Editor scripts)
- `ProjectSettings/` files (except when explicitly permitted)
- `Packages/packages-lock.json` (Unity regenerates automatically)
- `Assets/Content/` (user-created content)
- `Assets/Resources/` (created via Unity Editor only)

### 🔧 AI CAN CREATE EDITOR SCRIPTS TO GENERATE
- Prefabs (via `PrefabUtility` API)
- ScriptableObjects (via `AssetDatabase` API)
- Materials (via `AssetDatabase` API)
- Editor menu items (via `[MenuItem]` attribute)

## Assembly Definitions

Runtime systems use separate assemblies per Constitution Principle VI (Modular Architecture):

- `NovelCore.Runtime.asmdef` (main runtime assembly)
- `NovelCore.Runtime.DialogueSystem.asmdef`
- `NovelCore.Runtime.SceneManagement.asmdef`
- `NovelCore.Runtime.SaveSystem.asmdef`
- `NovelCore.Runtime.AssetManagement.asmdef`
- `NovelCore.Editor.asmdef` (editor tools)
- `NovelCore.Tests.Runtime.asmdef` (PlayMode tests)
- `NovelCore.Tests.Editor.asmdef` (EditMode tests)

## Entry Point Architecture

### GameStarter Component
- Location: `Assets/Scripts/NovelCore/Runtime/Core/GameStarter.cs`
- Purpose: Initialize VContainer, load starting scene, start DialogueSystem/SceneManager
- Play Modes:
  - **Full Start**: Play ▶️ runs complete game initialization
  - **Scene Preview**: Scene Editor preview button for isolated testing (via EditorPrefs bridge)

### VContainer Scope
- Location: `Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs`
- Purpose: DI container root scope registering all services

## Content Organization

### User Projects
- Each visual novel project = folder in `Assets/Content/Projects/[ProjectName]/`
- Contains: Scenes, Characters, Data (isolated per project)

### Sample Projects
- Location: `Assets/Content/Projects/Sample/`
- Purpose: Demo visual novel for validation and tutorial

## Documentation Organization

### Centralized Documentation (`.specify/memory/`)
- **constitution.md**: Project principles (SINGLE SOURCE OF TRUTH)
- **tech-stack.md**: Technical stack (this file)
- **testing-strategy.md**: Testing guidelines
- **project-structure.md**: Directory structure (this file)

### Feature Specifications (`specs/[###-feature]/`)
- **plan.md**: Implementation plan (references centralized docs, no duplication)
- **spec.md**: Feature specification
- **tasks.md**: Task breakdown
- **contracts/**: Feature-specific interfaces
- **quickstart.md**: Usage guide for creators
- **research.md**: Implementation research

## Build Output (Not in Repository)

- `Library/` (Unity cache)
- `Temp/` (Unity temporary files)
- `Logs/` (Unity logs)
- `obj/`, `bin/` (C# build artifacts)
- `Builds/` (platform builds)

## Version Control Exclusions (.gitignore)

```
/Library/
/Temp/
/Logs/
/obj/
/Builds/
*.csproj
*.sln
*.suo
*.user
*.tmp
*.log
/test-results*.xml
/unity*.log
```

## Reference Patterns

### From Feature Plans
Feature plans (e.g., `specs/001-scene-transition/plan.md`) should:
- **Reference** centralized docs: "See `.specify/memory/tech-stack.md`"
- **Not duplicate** constitution principles, tech stack, testing strategy
- **Only include** feature-specific architecture decisions

### From Code
Code should follow structure defined in:
- `.specify/memory/project-structure.md` (directory placement)
- `.specify/memory/constitution.md` (coding standards, naming conventions)
