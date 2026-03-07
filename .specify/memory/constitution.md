<!--
Sync Impact Report:
- Version: 1.6.0 → 1.7.0 (Editor script generators for asset creation)
- Modified Principles:
  - VII. AI Development Constraints: Added explicit permissions for Editor script generators
- Added Principles: None
- Removed Principles: None
- Modified Sections:
  - AI Development Constraints: Added "Explicit Permissions for Editor Script Generators" section
- Added Sections:
  - Editor Script Generators permissions and requirements
- Removed Sections: None
- Templates Status:
  ✅ Constitution updated with Editor script generator permissions
  ✅ Principle VII expanded with programmatic asset generation via Editor scripts
- Follow-up TODOs: None
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
- **MVP Exception**: Initial MVP release (first working version of User Story 1) MAY rely on manual testing only. Automated test suite MUST be implemented incrementally post-MVP, with full coverage achieved before production release (1.0.0)

**Rationale**: Modular architecture prevents parallel development, makes debugging difficult, and increases regression risk. Modular design enables: faster iteration, easier onboarding, isolated bug fixes, and potential open-source component extraction. MVP exception acknowledges that proving core functionality to stakeholders takes precedence over test infrastructure, while maintaining long-term quality standards for production releases.

### VII. AI Development Constraints (NON-NEGOTIABLE)

AI-assisted development tools MUST operate within strict boundaries to prevent Unity project corruption and maintain engine integrity.

- **Script-Only Modifications**: AI tools MUST only create or modify files within `Assets/Scripts/` directory
- **Meta File Prohibition**: AI tools MUST NOT generate, edit, or delete `.meta` files - these are managed exclusively by Unity Engine
- **Asset Generation**: AI tools MUST NOT create or modify asset files (prefabs, scenes, materials, textures) directly
- **Project Settings**: AI tools MUST NOT modify `ProjectSettings/` files except where explicitly permitted below
- **Scene Editing**: AI tools MUST NOT edit `.unity` scene files or `.prefab` files in binary or YAML format

**Explicit Permissions for Package Management** (when user-specified):

When the user explicitly requests package operations by name, version, or configuration, AI tools MAY:

- **Package Installation**: Install Unity packages via Package Manager (`Packages/manifest.json` modifications)
- **Package Updates**: Update existing packages to specified versions
- **Package Configuration**: Modify package-specific settings and asset configurations
- **Package Removal**: Remove packages when explicitly requested
- **Dependency Resolution**: Resolve and install package dependencies automatically

**Package Management Requirements**:

- User MUST explicitly specify package name, operation, or configuration intent
- AI MUST verify package compatibility with Unity version before installation
- AI MUST backup `Packages/manifest.json` before modifications
- AI MUST report package changes to user (added/updated/removed packages with versions)
- AI MUST NOT install packages speculatively or without user request
- AI MUST NOT modify `Packages/packages-lock.json` directly (Unity regenerates this automatically)

**Explicit Permissions for Editor Script Generators** (when user-specified):

When the user explicitly requests creation of Unity assets (prefabs, ScriptableObjects, materials, etc.), AI tools MAY create Editor scripts that programmatically generate these assets:

- **Editor Script Creation**: AI MAY create C# Editor scripts in `Assets/Scripts/NovelCore/Editor/Tools/Generators/` that use Unity Editor APIs to generate assets
- **Programmatic Asset Generation**: Editor scripts MAY use Unity APIs (`PrefabUtility`, `AssetDatabase`, `EditorUtility`) to create prefabs, materials, ScriptableObjects
- **Menu Integration**: Editor scripts MAY add menu items (e.g., `NovelCore → Generate Dialogue Prefab`) for user-triggered generation
- **One-Time Execution**: Scripts SHOULD be designed for one-time or on-demand execution, not automatic generation on every compile
- **Asset Output Location**: Generated assets MUST be placed in appropriate directories (`Assets/Resources/`, `Assets/Content/`) as specified by project structure

**Editor Script Generator Requirements**:

- User MUST explicitly request asset generation (e.g., "create DialogueBox prefab", "generate default materials")
- AI MUST create Editor script in `Assets/Scripts/NovelCore/Editor/Tools/Generators/` directory only
- Script MUST use Unity Editor APIs (not direct file I/O for .prefab/.asset files)
- Script MUST include clear menu item or button for user to trigger generation
- Script MUST log success/failure messages to Unity Console
- Script MUST check if asset already exists and prompt user before overwriting
- Generated assets MUST follow project naming conventions and structure from plan.md
- AI MUST NOT create Editor scripts that modify existing prefabs/assets - only generate new ones

**Rationale**: Unity's `.meta` files contain critical GUID mappings that ensure asset reference integrity. Manual `.meta` editing causes reference breakage, missing script errors, and project corruption. AI-generated asset files bypass Unity's import pipeline, leading to incorrect serialization, missing dependencies, and platform-specific build failures. Restricting AI to code generation in `Assets/Scripts/` provides value (scaffolding, boilerplate) while preventing engine-level corruption that requires manual recovery.

Package management is explicitly permitted because: (1) Unity Package Manager provides safe APIs via `manifest.json` modifications, (2) package installation is a common development workflow that benefits from AI assistance, (3) package compatibility verification prevents breaking changes, (4) user must explicitly request package operations, eliminating speculative modifications. This permission enables AI to assist with dependency management while maintaining project integrity through validation and reporting requirements.

Editor script generators are explicitly permitted because: (1) Unity Editor APIs (`PrefabUtility`, `AssetDatabase`) provide safe programmatic asset creation with proper GUID generation and .meta file management, (2) user explicitly triggers generation via menu commands, eliminating accidental modifications, (3) scripts are auditable C# code that can be reviewed and version-controlled, (4) Unity Editor validates generated assets through its import pipeline, (5) this approach automates repetitive manual work while maintaining Unity's asset integrity guarantees.

### VIII. User Documentation Language (NON-NEGOTIABLE)

All end-user documentation MUST be written in Russian as the primary language.

- **User Manuals**: All user-facing documentation (user manuals, tutorials, quickstart guides) MUST be written in Russian
- **Error Messages**: All user-facing error messages and warnings MUST be in Russian with clear, actionable explanations
- **UI Labels**: Unity Editor extension menus and windows MUST have Russian labels and tooltips
- **Help System**: In-editor help and documentation links MUST direct to Russian-language resources
- **Technical Documentation Exception**: Developer/contributor documentation (API docs, architecture docs, code comments) MAY remain in English for international collaboration
- **Translation Quality**: Russian documentation MUST be written by native speakers or professional translators, not machine translation

**Rationale**: Content creators (primary users) are Russian-speaking. Technical English documentation creates barriers to entry, reduces adoption, and increases support burden. Native-language documentation is standard for professional creative tools (Adobe Creative Suite, Unreal Engine localization, Unity Hub Russian interface). Clear Russian error messages prevent creator frustration and reduce developer support load. English technical documentation is retained for potential open-source contributors and international development team collaboration.

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
- **Scripting Backend**: IL2CPP for all platforms (Windows, macOS, iOS, Android)
- **Asset Optimization**: Automatic texture compression, audio compression, mesh optimization per platform
- **Build Size**: Target < 50MB initial download for mobile (streaming additional assets)

## Quality Assurance Standards

### Pre-Release Gates

Every release candidate MUST pass:

1. **Automated Tests**: 100% pass rate on unit, integration, and UI automation tests *(MVP Exception: 0.1.0-0.3.0 may use manual testing only; automated tests required starting 0.4.0)*
2. **Platform Builds**: Successful builds and smoke tests on all four target platforms
3. **Performance Profiling**: No regressions vs. previous release (memory, FPS, load times)
4. **Sample Project**: At least one complete demo visual novel builds and runs on all platforms
5. **Documentation Review**: All new features documented with tutorials and API references

### MVP Testing Strategy

For initial MVP releases (0.1.0 through 0.3.0):

- **Manual Testing**: Feature validation via manual testing of user stories in Unity Editor and builds
- **Smoke Tests**: Basic "does it launch and run" validation on target platforms
- **Creator Dogfooding**: Internal team creates sample visual novels to validate workflows
- **Incremental Automation**: Test infrastructure MUST be implemented alongside feature development, targeting >80% coverage by 0.4.0

### MVP Scope Guidance

For initial MVP releases (0.1.0 through 0.3.0):

- **Functional Requirement Coverage**: Minimum 80% coverage of functional requirements is acceptable for MVP validation
- **Deferred Features**: Non-critical features (e.g., dialogue history, undo/redo, custom asset browsers) MAY be deferred to post-MVP iterations
- **Core User Stories**: All P1 (Priority 1) user stories MUST be fully implemented and manually tested
- **Edge Case Handling**: Critical edge cases (broken links, missing assets, empty projects) MUST be covered; minor edge cases (large images, Unicode rendering) MAY be deferred with explicit documentation
- **Coverage Targets by Phase**:
  - **MVP (v0.1.0-v0.3.0)**: ≥80% functional requirement coverage, 100% P1 user story coverage, manual testing
  - **Post-MVP (v0.4.0-v0.9.0)**: ≥95% functional requirement coverage, >80% automated test coverage, all edge cases addressed
  - **Production (v1.0.0+)**: 100% functional requirement coverage, >80% automated test coverage, all documented edge cases handled

**Rationale**: MVP focuses on proving core value proposition (creators can build visual novels without code) rather than feature completeness. 80% coverage ensures essential workflows work while acknowledging that perfect feature parity is iterative. Explicit deferred feature tracking prevents scope creep and enables rapid stakeholder validation.

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

**Namespaces (Traditional Block-Scoped)**:
- MUST use traditional namespace blocks with braces (NOT file-scoped namespaces)
- Opening brace MUST be on a new line (Allman style)
- All type declarations MUST be nested inside the namespace block
- One blank line after namespace declaration before first type

```csharp
// ✅ CORRECT (traditional namespace)
namespace NovelCore.Runtime.Core.DialogueSystem
{
    /// <summary>
    /// Dialogue system implementation.
    /// </summary>
    public class DialogueSystem : IDialogueSystem
    {
        // Implementation
    }
}

// ❌ INCORRECT (file-scoped namespace - C# 10 feature not used)
namespace NovelCore.Runtime.Core.DialogueSystem;

public class DialogueSystem : IDialogueSystem
{
    // Implementation
}
```

**Rationale**: Traditional namespaces provide clear scope boundaries, consistent with C# 9.0 language features available in Unity 6. File-scoped namespaces (C# 10) are not used to maintain compatibility and readability standards.

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

**Version**: 1.7.0 | **Ratified**: 2026-03-06 | **Last Amended**: 2026-03-07
