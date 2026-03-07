<!--
Sync Impact Report:
- Version: 1.1.2 → 1.2.0 (Unity version upgrade)
- Modified Principles: None
- Added Principles: None
- Removed Principles: None
- Modified Sections:
  - Technical Requirements: Unity 2022.3 LTS → Unity 6 (LTS), updated minimum iOS/Android versions
- Added Sections: None
- Removed Sections: None
- Templates Status:
  ✅ plan-template.md - no changes needed
  ✅ spec-template.md - no changes needed
  ✅ tasks-template.md - no changes needed
- Follow-up TODOs: None (all updates complete)
-->

# Novel Core Constructor Constitution

## Core Principles

### I. Creator-First Design

The visual novel constructor MUST prioritize the content creator experience above technical implementation convenience.

- **Editor Interface**: All core features MUST be accessible through Unity Editor GUI without requiring code
- **Visual Scripting**: Dialogue, branching narratives, and scene transitions MUST use node-based or visual scripting
- **Immediate Feedback**: Changes in the editor MUST reflect immediately in preview mode without requiring project rebuilds
- **Asset Import**: Common formats (PNG, JPG, MP3, OGG, TTF) MUST import with sensible defaults requiring zero configuration
- **Error Messages**: All validation errors MUST be presented in plain language with actionable solutions, not technical stack traces

**Rationale**: Content creators (writers, artists, game designers) are the primary users. Technical barriers reduce adoption and limit creative expression. A tool that requires programming knowledge contradicts the "constructor" value proposition.

### II. Cross-Platform Parity (NON-NEGOTIABLE)

All runtime features MUST work identically on Windows (Steam), macOS (Steam), iOS, and Android platforms.

- **Feature Parity**: No platform-specific features unless technically impossible (e.g., haptic feedback)
- **Visual Consistency**: UI layouts, fonts, and rendering MUST produce identical visual results across platforms within 5% tolerance
- **Input Abstraction**: Touch, mouse, and keyboard inputs MUST be abstracted to unified interaction events
- **Testing Requirements**: Every feature MUST include automated tests on at least three target platforms before merge
- **Build Validation**: CI/CD MUST produce and validate builds for all four platforms on every release candidate

**Rationale**: Visual novels are narrative experiences where parity is critical. A choice appearing on one platform but not another breaks the narrative contract. Creators expect "build once, publish everywhere" without per-platform debugging.

### III. Asset Pipeline Integrity

Content assets (sprites, audio, fonts) MUST maintain referential integrity throughout the project lifecycle.

- **No Broken References**: Missing asset references MUST be detected at import-time and edit-time with clear warnings
- **Addressable System**: All game assets MUST use Unity Addressables or equivalent persistent reference system
- **Version Compatibility**: Asset format changes MUST include automatic migration scripts for existing projects
- **Dependency Tracking**: Moving/renaming assets MUST automatically update all references across scenes, prefabs, and scripts
- **External Assets**: Support for AssetBundle DLC workflow MUST maintain reference integrity across base game and DLC

**Rationale**: Broken asset references are the #1 cause of visual novel runtime failures. Creators work iteratively, replacing placeholder art. Manual reference fixing is error-prone and wastes creative time.

### IV. Runtime Performance Guarantees

The player experience MUST meet performance targets across target platforms.

- **Mobile Targets**: 60 FPS on iPhone 12 / Samsung Galaxy S21 or equivalent midrange devices
- **Desktop Targets**: 60 FPS on Intel HD 620 / AMD Vega 8 integrated graphics
- **Memory Budget**: Maximum 512MB RAM usage on mobile, 1GB on desktop (excluding asset streaming)
- **Load Times**: Scene transitions < 1 second on target hardware
- **Battery Impact**: Mobile builds MUST NOT drain > 10% battery per hour of gameplay
- **Profiling Requirement**: Performance regressions detected via automated profiling MUST block release

**Rationale**: Visual novels compete with professionally published games on Steam and mobile stores. Poor performance reduces ratings, increases refund rates, and damages creator reputation. Performance issues discovered post-launch are expensive to fix.

### V. Save System Reliability (NON-NEGOTIABLE)

Player progress MUST be preserved reliably across sessions and platform-specific constraints.

- **Auto-Save**: Progress MUST auto-save after every story branch decision or scene transition
- **Cloud Sync**: Steam Cloud and mobile cloud saves (iCloud/Google Play) MUST be supported with conflict resolution
- **Save Integrity**: Corrupted saves MUST be detected and fallback to previous valid save without data loss
- **Format Stability**: Save format changes MUST include automatic migration for backward compatibility
- **Testing**: Save/load MUST be tested across version upgrades, platform switches, and storage failures

**Rationale**: Lost progress is the #1 cause of negative user reviews for story-driven games. Visual novels are multi-hour experiences where progress loss is unacceptable. Platform holders (Apple, Google) can terminate apps mid-session; auto-save is critical.

### VI. Modular Architecture

The constructor MUST be built as composable, independently testable modules.

- **Core Systems**: Dialogue engine, asset manager, save system, input handler MUST be separate assemblies
- **Editor Extensions**: Custom editors MUST be optional and not required for runtime functionality
- **Platform Abstraction**: Platform-specific code (Steam, iOS, Android APIs) MUST be isolated behind interfaces
- **Dependency Injection**: Systems MUST use constructor injection or ScriptableObject configuration, avoiding singletons
- **Testing**: Each module MUST have unit tests achieving >80% code coverage and integration tests for cross-module contracts

**Rationale**: Monolithic architecture prevents parallel development, makes debugging difficult, and increases regression risk. Modular design enables: faster iteration, easier onboarding, isolated bug fixes, and potential open-source component extraction.

### VII. AI Development Constraints (NON-NEGOTIABLE)

AI-assisted development tools MUST operate within strict boundaries to prevent Unity project corruption and maintain engine integrity.

- **Script-Only Modifications**: AI tools MUST only create or modify files within `Assets/Scripts/` directory
- **Meta File Prohibition**: AI tools MUST NOT generate, edit, or delete `.meta` files - these are managed exclusively by Unity Engine
- **Asset Generation**: AI tools MUST NOT create or modify asset files (prefabs, scenes, materials, textures) directly
- **Project Settings**: AI tools MUST NOT modify `ProjectSettings/` files or Unity package manifests
- **Scene Editing**: AI tools MUST NOT edit `.unity` scene files or `.prefab` files in binary or YAML format

**Rationale**: Unity's `.meta` files contain critical GUID mappings that ensure asset reference integrity. Manual `.meta` editing causes reference breakage, missing script errors, and project corruption. AI-generated asset files bypass Unity's import pipeline, leading to incorrect serialization, missing dependencies, and platform-specific build failures. Restricting AI to code generation in `Assets/Scripts/` provides value (scaffolding, boilerplate) while preventing engine-level corruption that requires manual recovery.

## Platform & Distribution Requirements

### Steam Integration

- **Steamworks SDK**: MUST integrate Steamworks for achievements, cloud saves, and workshop support
- **Steam Input**: MUST support Steam Input API for controller remapping
- **Overlay Compatibility**: Constructor-generated games MUST be compatible with Steam Overlay
- **Workshop Support**: MUST provide framework for mod/content Workshop submissions (optional for creators)

### Mobile Distribution

- **App Store Guidelines**: MUST comply with Apple App Store Review Guidelines (especially 4.2 - Minimum Functionality)
- **Google Play Policies**: MUST comply with Google Play Content Policies
- **GDPR Compliance**: Data collection (analytics, crash reports) MUST be opt-in with privacy policy template
- **Localization**: UI and template content MUST support UTF-8 and right-to-left languages
- **Accessibility**: MUST support platform accessibility features (VoiceOver, TalkBack, font scaling)

### Technical Requirements

- **Unity Version**: Unity 6 (LTS)
- **Rendering**: Universal Render Pipeline (URP) 2D for cross-platform performance
- **Scripting Backend**: Mono for Windows, IL2CPP for macOS/iOS/Android
- **Asset Optimization**: Automatic texture compression, audio compression, mesh optimization per platform
- **Build Size**: Target < 50MB initial download for mobile (streaming additional assets)

## Quality Assurance Standards

### Pre-Release Gates

Every release candidate MUST pass:

1. **Automated Tests**: 100% pass rate on unit, integration, and UI automation tests
2. **Platform Builds**: Successful builds and smoke tests on all four target platforms
3. **Performance Profiling**: No regressions vs. previous release (memory, FPS, load times)
4. **Sample Project**: At least one complete demo visual novel builds and runs on all platforms
5. **Documentation Review**: All new features documented with tutorials and API references

### Content Creator Testing

Before major releases (0.X.0, X.0.0):

- **Beta Program**: Minimum 10 external creators test new features for 2 weeks
- **Feedback Integration**: Critical bugs and UX issues MUST be addressed before release
- **Migration Testing**: At least 3 existing projects from previous version MUST migrate successfully

### Regression Prevention

- **Change Detection**: New features MUST NOT break existing sample projects (automated regression tests)
- **Asset Compatibility**: New versions MUST load projects created in previous MINOR version without errors
- **Editor Stability**: No crashes during 8-hour stress test of typical editing workflows

## Code Style Standards

All C# code MUST follow these formatting and naming conventions to ensure consistency and readability.

### Formatting Rules

**Braces (Allman Style)**:
- Opening brace MUST always be on a new line
- Braces MUST be used for all control structures (if, else, for, while, foreach), even single-line statements

```csharp
// ✅ CORRECT
if (condition)
{
    DoSomething();
}

// ❌ INCORRECT (brace on same line)
if (condition) {
    DoSomething();
}

// ❌ INCORRECT (missing braces)
if (condition)
    DoSomething();
```

**Loops and Type Inference**:
- Use `var` for type declaration in loops (foreach, for)
- Use explicit types for clarity in other contexts (optional, but recommended for complex types)

```csharp
// ✅ CORRECT
foreach (var item in collection)
{
    ProcessItem(item);
}

for (var i = 0; i < count; i++)
{
    ProcessIndex(i);
}

// ✅ ACCEPTABLE (explicit type if needed for clarity)
IEnumerable<ComplexType> items = GetItems();
```

### Naming Conventions

**Private and Protected Fields**:
- MUST use underscore prefix: `_fieldName`
- MUST use camelCase after underscore
- Applies to both private and protected instance fields

```csharp
// ✅ CORRECT
public class Example
{
    private int _count;
    private string _userName;
    protected IDialogueSystem _dialogueSystem;
    
    public Example(IDialogueSystem dialogueSystem)
    {
        _dialogueSystem = dialogueSystem;
        _count = 0;
    }
}

// ❌ INCORRECT (no underscore)
private int count;
private string userName;
```

**Other Naming Rules**:
- Public properties: PascalCase (`PlayerName`, `SceneData`)
- Methods: PascalCase (`LoadScene`, `SaveGame`)
- Local variables: camelCase (`sceneId`, `dialogueLine`)
- Constants: PascalCase (`MaxSceneCount`, `DefaultVolume`)
- Interfaces: PascalCase with I prefix (`IDialogueSystem`, `ISaveSystem`)

### Code Organization

**Ordering**:
1. Private fields (with underscore)
2. Protected fields (with underscore)
3. Public properties
4. Constructors
5. Public methods
6. Protected methods
7. Private methods

**Example**:
```csharp
public class SceneManager : ISceneManager
{
    // Private fields
    private readonly IAssetManager _assetManager;
    private readonly IAudioService _audioService;
    private SceneData _currentScene;
    
    // Protected fields
    protected List<SceneData> _sceneHistory;
    
    // Public properties
    public SceneData CurrentScene => _currentScene;
    public bool IsLoading { get; private set; }
    
    // Constructor
    public SceneManager(IAssetManager assetManager, IAudioService audioService)
    {
        _assetManager = assetManager;
        _audioService = audioService;
        _sceneHistory = new List<SceneData>();
    }
    
    // Public methods
    public void LoadScene(SceneData sceneData, TransitionType transition, Action onComplete)
    {
        if (sceneData == null)
        {
            throw new ArgumentNullException(nameof(sceneData));
        }
        
        StartLoadingScene(sceneData, transition, onComplete);
    }
    
    // Private methods
    private void StartLoadingScene(SceneData sceneData, TransitionType transition, Action onComplete)
    {
        IsLoading = true;
        _sceneHistory.Add(sceneData);
        // ... implementation
    }
}
```

### Rationale

- **Allman Braces**: Improves readability by clearly separating control structure from body, consistent with Unity C# conventions
- **Mandatory Braces**: Prevents bugs from accidental scope misunderstanding, especially when adding statements later
- **Underscore Prefix**: Immediately distinguishes fields from local variables and parameters, reducing naming conflicts
- **Var in Loops**: Reduces verbosity without sacrificing clarity (loop variable type is obvious from context)
- **Consistent Ordering**: Makes code predictable and easier to navigate in large files

### Enforcement

- Code reviews MUST verify adherence to these standards
- `.editorconfig` file SHOULD be configured with these rules for automatic formatting
- CI/CD pipeline MAY include linting step to flag violations (warning, not error)

## Governance

This constitution defines the non-negotiable principles for Novel Core Constructor development. Any feature, refactor, or technical decision MUST comply with these principles.

### Amendment Process

1. **Proposal**: Document proposed amendment with rationale and impact analysis
2. **Review**: Team review of constitution alignment and migration cost
3. **Approval**: Unanimous approval required for MAJOR version changes, majority for MINOR
4. **Migration Plan**: Document breaking changes, migration scripts, and deprecation timeline
5. **Communication**: Announce changes to creator community with migration guides

### Compliance Review

- **Pull Requests**: All PRs MUST include constitution compliance checklist (auto-generated)
- **Design Documents**: All feature specs MUST reference applicable constitution principles
- **Quarterly Audits**: Review codebase for principle violations and technical debt

### Complexity Budget

Violations of simplicity/modularity principles (Principle VI) MUST be justified in design documents:

- **Justification Required**: Why simpler alternatives insufficient
- **Debt Tracking**: Document as technical debt with remediation timeline
- **Review Cadence**: Quarterly review of accumulated complexity debt

**Version**: 1.2.0 | **Ratified**: 2026-03-06 | **Last Amended**: 2026-03-06
