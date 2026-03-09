# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: [e.g., C# 9.0, Unity 2022.3 LTS or NEEDS CLARIFICATION]  
**Primary Dependencies**: [e.g., Unity Addressables, TextMeshPro, DOTween or NEEDS CLARIFICATION]  
**Rendering Pipeline**: [e.g., URP (Universal Render Pipeline), Built-in or NEEDS CLARIFICATION]  
**Storage**: [if applicable, e.g., JSON save files, PlayerPrefs, SQLite or N/A]  
**Testing**: [e.g., Unity Test Framework, NUnit, PlayMode tests or NEEDS CLARIFICATION]  
**Target Platforms**: [Windows/Mac (Steam), iOS, Android or NEEDS CLARIFICATION]  
**Scripting Backend**: [e.g., IL2CPP (all platforms recommended for cross-platform parity) or NEEDS CLARIFICATION]
**Project Type**: [e.g., editor-tool/runtime-system/visual-scripting-node/UI-component or NEEDS CLARIFICATION]  
**Performance Goals**: [60 FPS on iPhone 12/Intel HD 620, <1s load times or NEEDS CLARIFICATION]  
**Constraints**: [Mobile: ≤512MB RAM, Desktop: ≤1GB RAM, Steam/App Store compliance or NEEDS CLARIFICATION]  
**Scale/Scope**: [e.g., 10-hour story, 100 scenes, 500 assets or NEEDS CLARIFICATION]

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

<!--
  Review against .specify/memory/constitution.md principles:
  
  ✅ I. Creator-First Design
     - [ ] Feature accessible via Unity Editor GUI (no code required)
     - [ ] Visual/node-based interface where applicable
     - [ ] Preview mode provides immediate feedback
     - [ ] Error messages in plain language with solutions
  
  ✅ II. Cross-Platform Parity (NON-NEGOTIABLE)
     - [ ] Feature works identically on Windows, macOS, iOS, Android
     - [ ] Automated tests on at least 3 platforms
     - [ ] No platform-specific logic unless technically impossible
  
  ✅ III. Asset Pipeline Integrity
     - [ ] All asset references use Addressables/persistent IDs
     - [ ] Missing assets detected at import/edit time
     - [ ] Migration script included for format changes
  
  ✅ IV. Runtime Performance Guarantees
     - [ ] 60 FPS on target hardware (iPhone 12 / Intel HD 620)
     - [ ] Mobile: ≤512MB RAM, Desktop: ≤1GB RAM
     - [ ] Scene transitions <1 second
     - [ ] Performance profiling included
  
  ✅ V. Save System Reliability (NON-NEGOTIABLE)
     - [ ] Auto-save after state changes (if affects player progress)
     - [ ] Save format backward compatible or includes migration
     - [ ] Cloud sync supported (Steam/iCloud/Google Play)
  
  ✅ VI. Modular Architecture & Testing
     - [ ] Feature implemented as separate assembly/module
     - [ ] Interfaces used for platform-specific code
     - [ ] Unit tests achieve >80% coverage
     - [ ] Integration tests for cross-module contracts
     - [ ] Test execution workflow defined (EditMode → PlayMode)
     - [ ] Test execution after task groups/bug fixes is mandatory
     - [ ] Batch mode test execution supported for CI/CD
     - [ ] Zero-tolerance for test failures before task completion
  
  ✅ VII. AI Development Constraints (NON-NEGOTIABLE)
     - [ ] AI modifications limited to Assets/Scripts/ directory only
     - [ ] No .meta file generation or modification by AI
     - [ ] No direct asset file creation (prefabs, scenes, materials)
     - [ ] Package management permitted ONLY when user explicitly specifies
     - [ ] Package operations require compatibility verification and backup
  
  Document any principle violations with justification in Complexity Tracking section.
-->

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
# [REMOVE IF UNUSED] Option 1: Unity Editor Tool (DEFAULT for editor-only features)
Assets/
├── NovelCore/
│   ├── Editor/              # Editor-only scripts
│   │   ├── Windows/         # Custom editor windows
│   │   ├── Inspectors/      # Custom inspectors
│   │   └── Tools/           # Editor utilities
│   ├── Runtime/             # Runtime scripts (included in builds)
│   │   ├── Core/            # Core systems
│   │   ├── UI/              # UI components
│   │   └── Data/            # Data models & ScriptableObjects
│   └── Tests/
│       ├── Editor/          # Edit mode tests
│       └── Runtime/         # Play mode tests

# [REMOVE IF UNUSED] Option 2: Unity Package (for modular/distributable features)
Packages/
└── com.novelcore.[feature]/
    ├── Runtime/
    ├── Editor/
    ├── Tests/
    ├── Documentation~/
    └── package.json

# [REMOVE IF UNUSED] Option 3: Hybrid Runtime + Editor (most common for constructor features)
Assets/
├── NovelCore/
│   ├── Runtime/
│   │   ├── DialogueSystem/
│   │   ├── AssetManagement/
│   │   ├── SaveSystem/
│   │   └── InputHandling/
│   ├── Editor/
│   │   ├── DialogueEditor/
│   │   ├── SceneFlowEditor/
│   │   └── AssetValidators/
│   └── Resources/           # Runtime-loaded assets
│       ├── Prefabs/
│       └── ScriptableObjects/
└── StreamingAssets/         # Platform-specific or addressables catalog

Tests/
├── EditMode/                # Editor & non-runtime tests
└── PlayMode/                # Runtime integration tests

# [REMOVE IF UNUSED] Option 4: Multi-platform with Native Plugins
Assets/NovelCore/
├── Runtime/
├── Editor/
└── Plugins/
    ├── iOS/                 # iOS native code
    ├── Android/             # Android AAR/JAR
    └── x86_64/              # Native desktop libraries
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
