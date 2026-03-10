<!--
Sync Impact Report:
- Version: 1.16.0 → 1.17.0 (MINOR: Added Principle XI - File Naming Conventions)
- Modified Principles: None
- Added Sections:
  - Principle XI (File Naming Conventions): C# files use PascalCase, Unity assets use snake_case
- Removed Sections: None
- Templates Requiring Updates:
  ✅ plan-template.md: Constitution Check section references principles - no changes needed
  ✅ spec-template.md: No naming convention references - no changes needed
  ✅ tasks-template.md: No naming convention references - no changes needed
  ⚠ Developer documentation: Should add file naming examples to quickstart.md
- Follow-up TODOs:
  - Add file naming examples to quickstart.md
  - Update existing assets to snake_case if needed (audit required)
  - Consider adding file naming linter/validator
- Source: User requirement (2026-03-10)
-->

# Novel Core Constructor Constitution

**Version**: 1.17.0 | **Ratified**: 2026-03-06 | **Last Amended**: 2026-03-10

## Core Principles

### I. Creator-First Design

The visual novel constructor MUST prioritize the content creator experience above technical implementation convenience.

- **Editor Interface**: All core features MUST be accessible through Unity Editor GUI without requiring code
- **Visual Scripting**: Dialogue, branching narratives, and scene transitions MUST use node-based or visual scripting
- **Immediate Feedback**: Changes in the editor MUST reflect immediately in preview mode without requiring project rebuilds
- **Dual-Mode Preview**: System MUST support two preview modes:
  - **Play Mode Full Start**: Pressing Play ▶️ starts the game from current scene (or configured starting scene) via GameStarter component
  - **Scene Editor Preview**: Scene Editor window MUST provide "Preview Scene" button to test individual scenes in isolation without full game initialization
- **Asset Import**: Common formats (PNG, JPG, MP3, OGG, TTF) MUST import with sensible defaults requiring zero configuration
- **Error Messages**: All validation errors MUST be presented in plain language with actionable solutions, not technical stack traces

**Rationale**: Content creators (writers, artists, game designers) are the primary users. Technical barriers reduce adoption and limit creative expression. A tool that requires programming knowledge contradicts the "constructor" value proposition. Dual preview modes enable both rapid iteration (Scene Editor preview for individual scenes) and full validation (Play Mode for complete game flow testing with proper initialization).

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

### VI. Modular Architecture & Testing (NON-NEGOTIABLE)

The constructor MUST be built as composable, independently testable modules with comprehensive test coverage.

- **Core Systems**: Dialogue engine, asset manager, save system, input handler MUST be separate assemblies
- **Game Entry Point**: System MUST have explicit entry point (GameStarter component) that:
  - Initializes VContainer dependency injection
  - Loads starting scene from configuration
  - Starts DialogueSystem and SceneManager
  - Supports both Play Mode full start and Scene Editor preview mode
- **Call Stack Analysis** (MANDATORY for modifications): When modifying existing logic, developers MUST:
  - Analyze the complete call stack (all callers of the modified method/class)
  - Identify all systems that depend on the current behavior
  - Verify that changes do not break dependent logic or introduce unintended side effects
  - Update all affected call sites if method signatures or behavior contracts change
  - Run integration tests covering all identified call paths before committing
- **Editor Extensions**: Custom editors MUST be optional and not required for runtime functionality
- **Platform Abstraction**: Platform-specific code (Steam, iOS, Android APIs) MUST be isolated behind interfaces
- **Dependency Injection**: Systems MUST use constructor injection or ScriptableObject configuration, avoiding singletons
- **VContainer MonoBehaviour Registration**: MonoBehaviour services MUST be registered using `RegisterComponentOnNewGameObject<T>().AsImplementedInterfaces()`, NOT `Register<TInterface, TImplementation>()`. VContainer cannot instantiate MonoBehaviour via constructor. Example:
  ```csharp
  // ✅ CORRECT (MonoBehaviour)
  builder.RegisterComponentOnNewGameObject<UnityAudioService>(Lifetime.Singleton)
      .AsImplementedInterfaces();
  
  // ❌ WRONG (MonoBehaviour - will return null)
  builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);
  
  // ✅ CORRECT (POCO class)
  builder.Register<IAssetManager, AddressablesAssetManager>(Lifetime.Singleton);
  ```
- **DI Singleton GameObject Lifecycle**: Singleton services that create Unity GameObject MUST call `UnityEngine.Object.DontDestroyOnLoad(gameObject)` immediately after GameObject creation to prevent Unity from destroying them on scene transitions. GameObject lifecycle MUST match service lifetime (Singleton → persists until app shutdown). Example:
  ```csharp
  // In SceneManager constructor or initialization
  _backgroundContainer = new GameObject("Background");
  UnityEngine.Object.DontDestroyOnLoad(_backgroundContainer); // ← REQUIRED for Singleton
  ```
- **Unit Testing**: Each module MUST have unit tests achieving >80% code coverage of business logic
- **Integration Testing**: Cross-module contracts (e.g., dialogue system + save system) MUST have integration tests covering:
  - Data flow between modules
  - State transitions across system boundaries
  - Error handling and recovery scenarios
  - Platform-specific implementations against common interfaces
  - **Game Initialization**: GameStarter → VContainer → DialogueSystem/SceneManager initialization sequence
- **Test Organization**: Tests MUST be organized in separate assemblies (`NovelCore.Tests.Runtime`, `NovelCore.Tests.Editor`) with clear naming conventions
- **EditMode-First Strategy**: Unit tests for ScriptableObjects, data validation, pure C# logic, and builders MUST be implemented as EditMode tests. PlayMode tests are ONLY for async operations, file I/O, runtime-specific Unity APIs (e.g., `Application.persistentDataPath`), and integration tests requiring game loop
- **Test Platform Selection**:
  - **EditMode** (preferred): ScriptableObject creation/validation, data model tests, builders, pure C# business logic, synchronous operations
  - **PlayMode** (only when required): `async`/`await` operations, file system I/O, `Application.persistentDataPath`, `Directory`/`File` APIs, integration tests needing runtime environment, GameStarter initialization tests
- **Test-First Development**: For critical systems (save system, dialogue branching, asset management), tests MUST be written before implementation to validate requirements
- **Immediate Test Coverage**: After implementing new functionality, tests MUST be written immediately to cover the new code before moving to the next feature. Tests MUST be run and all errors fixed before proceeding.
- **Test Execution Workflow**: After writing tests, run Unity Test Runner in EditMode first (`-testPlatform EditMode`), then PlayMode if needed. Fix all compilation and runtime errors before committing
- **Continuous Validation**: Test suite MUST run automatically on pre-commit and CI/CD pipeline to prevent regressions
- **Test Execution Requirement** (MANDATORY after task completion):
  - After completing ANY group of related tasks (user story, feature phase, bug fix), developers MUST run full test suite
  - **EditMode Tests**: Run first with `-testPlatform EditMode -testResults ./test-results-editmode.xml`
  - **PlayMode Tests**: Run second with `-testPlatform PlayMode -testResults ./test-results-playmode.xml`
  - **Batch Mode Execution**: Tests MUST be runnable in batch mode for CI/CD integration
  - **Zero Failures Required**: ALL tests MUST pass (exit code 0) before considering task complete
  - **Failure Handling**: If tests fail, developer MUST fix failures before moving to next task or committing
  - **Bug Fix Validation**: After fixing any bug, developer MUST run full test suite to verify fix and prevent regressions
  - **Test Execution Command Template**:
    ```bash
    # EditMode tests
    /Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
      -runTests -batchmode -projectPath "$(pwd)" \
      -testPlatform EditMode -testResults "./test-results-editmode.xml" \
      -logFile - 2>&1
    
    # PlayMode tests (only if EditMode passes)
    /Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
      -runTests -batchmode -projectPath "$(pwd)" \
      -testPlatform PlayMode -testResults "./test-results-playmode.xml" \
      -logFile - 2>&1
    ```
- **MVP Exception**: Initial MVP release (v0.1.0-v0.3.0) MAY rely on manual testing only. Automated test suite MUST be implemented incrementally post-MVP, with >80% coverage achieved before production release (v1.0.0)
- **Naming Conventions** (MANDATORY):
  - **C# Files**: MUST use PascalCase (e.g., `SceneEditorWindow.cs`, `DialogueLineData.cs`)
  - **Asset Files**: MUST use snake_case (e.g., `scene_01_introduction.asset`, `character_hero.asset`)
  - **Sub-Assets**: MUST use snake_case with zero-padded index:
    - DialogueLineData: `dialog_line_001`, `dialog_line_002`, etc.
    - ChoiceData: `choice_001`, `choice_002`, etc.
    - Pattern: `{type}_{index:D3}` where index is 1-based and zero-padded to 3 digits
  - **Prefabs**: MUST use PascalCase (e.g., `DialogueBox.prefab`, `CharacterSprite.prefab`)
  - **Scenes**: MUST use PascalCase (e.g., `MainMenu.unity`, `Gameplay.unity`)
  - **Rationale**: Consistent naming improves discoverability, reduces cognitive load, and prevents file conflicts. PascalCase for code aligns with C# conventions. snake_case for assets improves readability in Unity Project Browser and aligns with common asset naming practices. Zero-padded indices ensure correct alphabetical sorting in Inspector and Project Browser.

**Rationale**: Modular architecture without tests prevents parallel development, makes debugging difficult, and increases regression risk. Unit tests validate individual components in isolation, while integration tests catch cross-system bugs that unit tests miss (e.g., save system serializing data the dialogue system cannot deserialize). **Call stack analysis prevents regression bugs by ensuring developers understand the full impact of their changes before implementation** - a method might be called from multiple places with different assumptions about its behavior, and changing it without analyzing all call sites can break distant, seemingly unrelated functionality. **Explicit entry point (GameStarter) ensures predictable initialization order, proper dependency injection, and enables both full game testing (Play Mode) and rapid scene iteration (Scene Editor preview)**. Test-first development for critical paths ensures requirements are understood before implementation, reducing rework. EditMode tests are preferred because they execute faster (no Play Mode initialization), are more reliable (no Unity runtime variability), and provide immediate feedback during development. PlayMode tests are reserved for scenarios that genuinely require runtime environment (async I/O, Application APIs, integration tests, game initialization). This strategy reduces test execution time by 60-80% and eliminates PlayMode test flakiness for pure logic. Comprehensive testing enables: faster iteration cycles, confident refactoring, automated regression prevention, and easier onboarding (tests document expected behavior). **Mandatory test execution after task completion prevents regression bugs by validating that new code doesn't break existing functionality** - running the full test suite after each user story, feature phase, or bug fix catches integration issues early when they're cheap to fix. Batch mode test execution enables CI/CD automation and ensures tests can run in headless environments. Zero-tolerance for test failures ensures code quality gates are enforced consistently. This workflow transforms tests from optional validation into a required checkpoint, making regression prevention automatic rather than relying on developer discipline. MVP exception acknowledges that proving core functionality to stakeholders takes precedence over test infrastructure, while maintaining long-term quality standards for production releases.

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

When the user explicitly requests creation or modification of Unity assets (prefabs, ScriptableObjects, materials, etc.), AI tools MAY create Editor scripts that programmatically generate or modify these assets:

- **Editor Script Creation**: AI MAY create C# Editor scripts in `Assets/Scripts/NovelCore/Editor/Tools/Generators/` that use Unity Editor APIs to generate or modify assets
- **Programmatic Asset Generation**: Editor scripts MAY use Unity APIs (`PrefabUtility`, `AssetDatabase`, `EditorUtility`) to create prefabs, materials, ScriptableObjects
- **Programmatic Asset Modification**: Editor scripts MAY use Unity APIs (`PrefabUtility.LoadPrefabContents`, `PrefabUtility.SaveAsPrefabAsset`) to modify existing prefabs
- **Menu Integration**: Editor scripts MAY add menu items (e.g., `NovelCore → Generate Dialogue Prefab`, `NovelCore → Update Scene Prefabs`) for user-triggered generation or modification
- **One-Time Execution**: Scripts SHOULD be designed for one-time or on-demand execution, not automatic generation on every compile
- **Asset Output Location**: Generated assets MUST be placed in appropriate directories (`Assets/Resources/`, `Assets/Content/`) as specified by project structure

**Editor Script Generator Requirements (for new asset creation)**:

- User MUST explicitly request asset generation (e.g., "create DialogueBox prefab", "generate default materials")
- AI MUST create Editor script in `Assets/Scripts/NovelCore/Editor/Tools/Generators/` directory only
- Script MUST use Unity Editor APIs (not direct file I/O for .prefab/.asset files)
- Script MUST include clear menu item or button for user to trigger generation
- Script MUST log success/failure messages to Unity Console
- Script MUST check if asset already exists and prompt user before overwriting
- Generated assets MUST follow project naming conventions and structure from plan.md

**Editor Script Modification Requirements (for existing asset modification)**:

- User MUST explicitly request asset modification (e.g., "update all scene prefabs to include AudioSource", "add BoxCollider to all character prefabs")
- AI MUST create Editor script in `Assets/Scripts/NovelCore/Editor/Tools/Generators/` directory only
- Script MUST use Unity Editor APIs (`PrefabUtility.LoadPrefabContents`, `PrefabUtility.SaveAsPrefabAsset`, `AssetDatabase.SaveAssets`) exclusively
- Script MUST create backup of modified assets before making changes (via `AssetDatabase.CopyAsset` or version control commit)
- Script MUST validate asset paths and types before modification (e.g., verify prefab exists and is a valid prefab asset)
- Script MUST log all modifications to Unity Console (file path, changes made, success/failure status)
- Script MUST include safety constraints:
  - **Namespace Scope**: Only modify assets under specific namespace/folder (e.g., `Assets/NovelCore/Runtime/Prefabs/`)
  - **Type Validation**: Verify asset type matches expected type before modification
  - **Dry-Run Mode**: Optionally support preview mode that logs planned changes without applying them
- Script MUST handle errors gracefully (e.g., missing prefab, invalid GameObject hierarchy) and report failures clearly
- Script MUST NOT modify Unity built-in assets (e.g., `Assets/Standard Assets/`, packages from Package Manager)
- Script SHOULD batch modifications using `AssetDatabase.StartAssetEditing()` / `AssetDatabase.StopAssetEditing()` for performance

**Rationale**: Unity's `.meta` files contain critical GUID mappings that ensure asset reference integrity. Manual `.meta` editing causes reference breakage, missing script errors, and project corruption. AI-generated asset files bypass Unity's import pipeline, leading to incorrect serialization, missing dependencies, and platform-specific build failures. Restricting AI to code generation in `Assets/Scripts/` provides value (scaffolding, boilerplate) while preventing engine-level corruption that requires manual recovery.

Package management is explicitly permitted because: (1) Unity Package Manager provides safe APIs via `manifest.json` modifications, (2) package installation is a common development workflow that benefits from AI assistance, (3) package compatibility verification prevents breaking changes, (4) user must explicitly request package operations, eliminating speculative modifications. This permission enables AI to assist with dependency management while maintaining project integrity through validation and reporting requirements.

Editor script generators and modifiers are explicitly permitted because: (1) Unity Editor APIs (`PrefabUtility`, `AssetDatabase`) provide safe programmatic asset creation and modification with proper GUID generation and .meta file management, (2) user explicitly triggers generation/modification via menu commands, eliminating accidental modifications, (3) scripts are auditable C# code that can be reviewed and version-controlled, (4) Unity Editor validates generated/modified assets through its import pipeline, (5) this approach automates repetitive manual work (e.g., batch prefab updates across hundreds of assets) while maintaining Unity's asset integrity guarantees, (6) backup requirements and dry-run mode enable safe experimentation, (7) namespace/type validation prevents accidental modification of critical assets. Prefab modification is especially valuable for visual novel constructor development where batch updates to scene prefabs (adding components, updating references, refactoring hierarchies) are common maintenance tasks that would be prohibitively time-consuming to perform manually.

**Unity Compilation Validation** (REQUIRED for error checking):

When AI tools need to validate Unity project compilation after code changes, they MUST use the following workflow:

- **Batch Mode Execution**: Unity MUST be launched with `-batchmode`, `-nographics`, and `-quit` flags
- **Project Path**: Unity MUST be invoked from the **parent directory** of the Unity project folder
- **Relative Path**: Project path MUST be specified as `"$(pwd)/novel-core"` (relative to parent directory)
- **Log File Output**: Compilation logs MUST be written to a file in the parent directory using `-logFile "$(pwd)/unity_<task_name>.log"`
- **Error Detection**: After Unity exits, AI MUST parse the log file for `error CS` patterns to detect compilation failures
- **No GUI Launch**: AI MUST NOT launch Unity with GUI (`open -a Unity.app`) for compilation validation

**Required Unity Command Template**:
```bash
cd /Users/selstrom/work/projects/novel-core/novel-core && \
/Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
  -quit \
  -batchmode \
  -nographics \
  -projectPath "$(pwd)/novel-core" \
  -logFile "$(pwd)/unity_<iteration_name>.log" 2>&1
```

**Rationale**: Unity batch mode with `-nographics` avoids licensing issues that occur with GUI-based launches in automated contexts. Running from the parent directory with relative paths ensures consistent project resolution across different environments. File-based logging (`-logFile`) captures full compilation output including compiler errors, warnings, and Unity initialization messages, which are essential for debugging. Parsing log files for `error CS` patterns provides reliable compilation failure detection without depending on exit codes, which may not reflect C# compilation errors due to Unity's multi-stage initialization process. This workflow has been validated to work reliably for automated compilation checks in CI/CD pipelines and AI-assisted development.

**Unity PlayMode Test Execution** (REQUIRED for running tests):

When AI tools need to run PlayMode tests after implementing new functionality, they MUST use the following workflow:

- **Batch Mode Execution**: Unity MUST be launched with `-batchmode`, `-nographics`, `-quit`, and `-runTests` flags
- **Test Platform**: MUST specify `-testPlatform PlayMode` for runtime tests or `-testPlatform EditMode` for editor tests
- **Test Results Output**: Test results MUST be written to XML file using `-testResults ./test-results.xml`
- **Log File Output**: Test execution logs MUST be written using `-logFile ./unity-tests.log`
- **Project Path**: Use absolute path to Unity project directory

**Required Unity PlayMode Test Command Template**:
```bash
/Applications/Unity/Hub/Editor/2022.3.XXf1/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -projectPath /path/to/project \
  -runTests \
  -testPlatform PlayMode \
  -testResults ./results.xml \
  -logFile ./unity.log \
```

**Required Unity EditMode Test Command Template**:
```bash
/Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -projectPath /Users/selstrom/work/projects/novel-core/novel-core/novel-core \
  -runTests \
  -testPlatform EditMode \
  -testResults /Users/selstrom/work/projects/novel-core/novel-core/test-results.xml \
  -logFile /Users/selstrom/work/projects/novel-core/novel-core/unity-tests.log \
```
** If unable to run tests from command line don't use no graphics. It will able to start unit tests in Edit mode.

**Alternative: Run tests from Unity Test Runner GUI**:
- Open Unity Editor with the project
- Go to Window → General → Test Runner
- Select PlayMode or EditMode tab
- Click "Run All" to execute tests
- Review results in the Test Runner window

**Rationale**: PlayMode tests require Unity's runtime environment and cannot be executed outside Unity. Batch mode test execution enables automated testing in CI/CD pipelines and allows AI tools to validate test coverage after implementing features. The `-runTests` flag triggers Unity Test Framework execution, `-testResults` outputs NUnit XML format for test result parsing, and `-testPlatform` specifies whether to run runtime (PlayMode) or editor (EditMode) tests. GUI-based Test Runner is recommended for interactive development and debugging, while batch mode is required for automation. This workflow ensures tests are executed immediately after writing them, following Principle VI's "Immediate Test Coverage" requirement.

### VIII. Editor-Runtime Bridge (NON-NEGOTIABLE)

Editor tools and runtime systems MUST be integrated to enable seamless preview and testing workflows.

- **Preview State Transfer**: Editor tools (Scene Editor, Dialogue Editor) MUST be able to transfer preview state to runtime Play Mode
- **EditorPrefs Bridge**: Preview data MUST be stored in EditorPrefs (or equivalent) for Editor→Runtime communication
- **GameStarter Integration**: GameStarter (runtime entry point) MUST check for preview state on initialization and load preview scene if present
- **Fallback Behavior**: If preview state is invalid or missing, GameStarter MUST load default starting scene without errors
- **State Cleanup**: Preview state MUST be cleared after being consumed by runtime to prevent stale data
- **Preview Manager (Recommended)**: Centralized PreviewManager class SHOULD encapsulate preview state management for reusability

**Rationale**: The Scene Editor provides a "Preview Scene" button that should allow creators to test individual scenes in isolation without manually configuring GameStarter's starting scene. Without Editor-Runtime bridge, preview functionality is non-functional: clicking "Preview Scene" starts Play Mode but loads the wrong scene because GameStarter ignores the preview request. This creates a broken user experience where editors provide preview buttons that don't work. The bridge ensures: (1) rapid iteration - creators can test scenes immediately from the editor, (2) workflow consistency - "Preview" button does what creators expect, (3) no manual setup - no need to change Inspector settings for testing, (4) isolation - preview doesn't affect project configuration. EditorPrefs provides reliable Editor→Runtime communication (available in Unity Editor context, persists across Play Mode transitions, automatically scoped to project). Fallback ensures runtime never breaks due to invalid preview state. Cleanup prevents stale preview data from interfering with normal game launches. This pattern enables dual-mode testing: (a) full game flow via Play Mode with configured starting scene, (b) isolated scene testing via Scene Editor preview with temporary override.

### IX. User Documentation Language (NON-NEGOTIABLE)

All end-user documentation MUST be written in Russian as the primary language.

- **User Manuals**: All user-facing documentation (user manuals, tutorials, quickstart guides) MUST be written in Russian
- **Error Messages**: All user-facing error messages and warnings MUST be in Russian with clear, actionable explanations
- **UI Labels**: Unity Editor extension menus and windows MUST have Russian labels and tooltips
- **Help System**: In-editor help and documentation links MUST direct to Russian-language resources
- **Technical Documentation Exception**: Developer/contributor documentation (API docs, architecture docs, code comments) MAY remain in English for international collaboration
- **Translation Quality**: Russian documentation MUST be written by native speakers or professional translators, not machine translation

**Rationale**: Content creators (primary users) are Russian-speaking. Technical English documentation creates barriers to entry, reduces adoption, and increases support burden. Native-language documentation is standard for professional creative tools (Adobe Creative Suite, Unreal Engine localization, Unity Hub Russian interface). Clear Russian error messages prevent creator frustration and reduce developer support load. English technical documentation is retained for potential open-source contributors and international development team collaboration.

### X. File Organization & Temporary Artifacts

Temporary output files and intermediate documentation MUST be organized in designated locations to maintain project cleanliness.

- **Temporary Output Directory**: All temporary output files (logs, test reports, build artifacts) MUST be placed in `./temp/` directory
  - Unity compilation logs (e.g., `unity_clean.log`, `unity_tests.log`)
  - Test result XML files (e.g., `test-results-editmode.xml`, `test-results-playmode.xml`)
  - Build logs and intermediate build artifacts
  - Profiling reports and performance metrics
  - CI/CD pipeline artifacts
- **Intermediate Documentation**: All intermediate MD files documenting bugfixes, behavior analysis, or implementation notes MUST be placed in `.specify/memory/`
  - Bug analysis documents (e.g., `navigation-fix-2026-03-09.md`)
  - Implementation decision records
  - Architecture analysis notes
  - Refactoring reports with historical value
  - NOT temporary status updates (those should be deleted after merge)
- **Version Control Exclusion**: `./temp/` directory MUST be excluded from version control via `.gitignore`
- **Cleanup Policy**: Files in `./temp/` MAY be deleted at any time without data loss concerns
- **Documentation Retention**: Files in `.specify/memory/` SHOULD be retained for historical context and decision tracking

**Rationale**: Separating temporary artifacts from permanent documentation prevents repository clutter and makes it clear which files are ephemeral vs. valuable knowledge. The `./temp/` directory provides a predictable location for automated tools (CI/CD, test runners) to write output without polluting the repository root. The `.specify/memory/` directory serves as a knowledge base for understanding past decisions, debugging patterns, and architectural evolution. This organization follows industry best practices (e.g., `target/` in Maven, `build/` in Gradle) and enables easy cleanup without risk of deleting important documentation.

### XI. File Naming Conventions (NON-NEGOTIABLE)

All files in the project MUST follow strict naming conventions to ensure consistency, readability, and compatibility with version control systems.

- **C# Source Files**: MUST use PascalCase (UpperCamelCase) naming
  - Class files: `DialogueSystem.cs`, `SceneManager.cs`, `CharacterData.cs`
  - Interface files: `IDialogueSystem.cs`, `ISaveSystem.cs`
  - Test files: `DialogueSystemTests.cs`, `SceneManagerTests.cs`
  - Editor scripts: `SceneEditorWindow.cs`, `DialogueInspector.cs`
  - All C# files MUST match the primary type name within (Unity requirement)
- **Unity Asset Files**: MUST use snake_case (lowercase with underscores) naming
  - Scenes: `scene_01_introduction.unity`, `scene_02_choice_point.unity`
  - Prefabs: `dialogue_box.prefab`, `character_protagonist.prefab`, `ui_button_primary.prefab`
  - ScriptableObjects: `character_data_protagonist.asset`, `scene_data_intro.asset`
  - Materials: `material_ui_background.mat`, `material_sprite_default.mat`
  - Textures/Sprites: `sprite_character_idle.png`, `background_forest.jpg`
  - Audio: `audio_bgm_menu.mp3`, `audio_sfx_click.ogg`
  - Fonts: `font_liberation_sans.ttf`
  - Animation: `animation_fade_in.anim`, `animator_character.controller`
- **Asset Naming Structure** (RECOMMENDED): Use `[category]_[descriptor]_[variant]` pattern
  - Examples: `character_data_protagonist.asset`, `scene_data_intro.asset`, `sprite_ui_button_hover.png`
  - Benefits: Natural sorting, clear categorization, easy bulk operations
- **Meta Files**: Automatically generated by Unity - NEVER manually rename or create
- **Directory Names**: MUST use PascalCase for consistency with C# namespaces
  - `Assets/Scripts/NovelCore/Runtime/DialogueSystem/`
  - `Assets/Content/Projects/Sample/Scenes/`
  - `Assets/Resources/Prefabs/`
- **Special Cases**:
  - Unity package files: Follow Unity convention (`package.json`, `README.md`)
  - Documentation files: Follow markdown convention (`quickstart.md`, `user-manual-ru.md`)
  - Configuration files: Follow tool convention (`.gitignore`, `.editorconfig`)

**Enforcement**:

- Code reviews MUST reject PRs with incorrect file naming
- Asset import pipeline SHOULD validate naming conventions and warn on violations
- Rename refactoring MUST update all references (Unity handles this automatically for assets)

**Rationale**: Consistent file naming improves:

1. **Developer Experience**: Predictable naming reduces cognitive load when navigating codebase
2. **Version Control**: snake_case for assets avoids Windows/macOS case sensitivity issues in git
3. **Searchability**: Consistent patterns enable glob/regex filtering (`find . -name "character_*.asset"`)
4. **Unity Best Practices**: PascalCase for C# matches Unity's coding standards and type naming requirements
5. **Asset Organization**: snake_case with category prefixes groups related assets naturally in Project window
6. **Cross-Platform Compatibility**: Lowercase asset names avoid issues with case-insensitive filesystems
7. **Automation**: Structured naming enables scripted batch operations (e.g., "rename all `sprite_ui_*` textures")

This dual convention (PascalCase for code, snake_case for assets) is industry-standard in Unity projects and prevents common pitfalls like type name mismatches, broken asset references on case-sensitive filesystems, and poor asset discoverability.

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
   - **Unit Tests**: >80% code coverage of business logic (service classes, managers, data processors)
   - **Integration Tests**: All cross-module contracts tested (dialogue + save, asset + scene, input + UI)
   - **Regression Tests**: Critical user workflows automated (create scene, add dialogue, make choice, save/load)
2. **Platform Builds**: Successful builds and smoke tests on all four target platforms
3. **Performance Profiling**: No regressions vs. previous release (memory, FPS, load times)
4. **Sample Project**: At least one complete demo visual novel builds and runs on all platforms
5. **Documentation Review**: All new features documented with tutorials and API references

### MVP Testing Strategy (v0.1.0 - v0.3.0)

For initial MVP releases:

- **Manual Testing**: Feature validation via manual testing of user stories in Unity Editor and builds
- **Smoke Tests**: Basic "does it launch and run" validation on target platforms
- **Creator Dogfooding**: Internal team creates sample visual novels to validate workflows
- **Critical Path Coverage**: Even without full automation, critical user paths (create scene → add dialogue → preview) MUST be tested manually before each release
- **Incremental Automation**: Test infrastructure MUST be implemented alongside feature development, targeting >80% coverage by 0.4.0

### Post-MVP Testing Requirements (v0.4.0+)

Starting from v0.4.0, automated testing becomes mandatory:

- **Unit Test Coverage**: >80% code coverage for all Runtime systems (DialogueSystem, SaveSystem, AssetManager, SceneManager, etc.)
- **Integration Test Suite**: Minimum 20 integration tests covering:
  - Dialogue system → Save system: Save/load dialogue state mid-conversation
  - Asset system → Scene system: Load scene assets via Addressables
  - Input system → UI system: Click handling for dialogue advance and choices
  - Save system → Platform services: Cloud save integration (Steam, iCloud, Google Play)
  - Localization system → Dialogue system: Language switching updates dialogue text
- **Regression Test Automation**: All P1 and P2 user stories MUST have automated PlayMode tests
- **CI/CD Integration**: Test suite MUST run on every commit, blocking merges if tests fail
- **Test Naming Convention**: Tests MUST follow pattern `[System]_[Method]_[Scenario]_[Expected]` (e.g., `DialogueSystem_AdvanceDialogue_AtLastLine_CompletesScene`)

### Test Organization Standards

- **Separate Assemblies**: Runtime tests in `NovelCore.Tests.Runtime.asmdef`, Editor tests in `NovelCore.Tests.Editor.asmdef`
- **Test Fixtures**: Group related tests in test fixtures with `[TestFixture]` and shared setup/teardown
- **Test Data Builders**: Use builder pattern for complex test data (SceneData, CharacterData) to improve readability
- **Mock Abstractions**: Use interfaces (IAssetManager, ISaveSystem) with mock implementations for unit tests
- **PlayMode vs EditMode**: Use PlayMode tests for runtime behavior, EditMode tests for editor tools and validators

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

**Braces (Allman Style)** (MANDATORY - ZERO TOLERANCE):
- Opening brace MUST always be on a new line
- Braces MUST be used for ALL control structures (if, else, for, while, foreach, do-while), even single-line statements
- **NO EXCEPTIONS**: Single-line statements without braces are PROHIBITED
- Code reviews MUST reject any PR with missing braces

```csharp
// ✅ CORRECT
if (condition)
{
    DoSomething();
}

// ✅ CORRECT (single statement still requires braces)
if (condition)
{
    return true;
}

// ✅ CORRECT (empty else block - braces still required)
if (condition)
{
    DoSomething();
}
else
{
    // No action needed
}

// ❌ INCORRECT (brace on same line)
if (condition) {
    DoSomething();
}

// ❌ INCORRECT (missing braces - PROHIBITED)
if (condition)
    DoSomething();

// ❌ INCORRECT (inline single statement - PROHIBITED)
if (condition) DoSomething();

// ❌ INCORRECT (ternary operator abuse - use if/else with braces for complex logic)
var result = condition ? CallMethod() : CallOtherMethod(); // Only acceptable for simple assignments

// ✅ CORRECT (loop with single statement)
foreach (var item in collection)
{
    ProcessItem(item);
}

// ❌ INCORRECT (loop missing braces - PROHIBITED)
foreach (var item in collection)
    ProcessItem(item);

// ✅ CORRECT (nested conditions with braces)
if (outerCondition)
{
    if (innerCondition)
    {
        DoNestedAction();
    }
}

// ❌ INCORRECT (nested conditions without braces - PROHIBITED)
if (outerCondition)
    if (innerCondition)
        DoNestedAction();
```

**Rationale**: Mandatory braces prevent entire categories of bugs:
1. **Accidental Scope Errors**: Adding statements later without realizing they're outside the conditional
2. **Merge Conflicts**: Git merges can create invalid code when statements are added/removed without braces
3. **Readability**: Braces make code structure immediately obvious, especially in deeply nested logic
4. **"Apple goto fail" Bug Prevention**: The infamous SSL bug (CVE-2014-1266) was caused by missing braces allowing unintended code execution
5. **Team Consistency**: Eliminates style debates and ensures all code follows the same structure

This is a **zero-tolerance rule** because the cost of a single missing-brace bug (hours of debugging, production incidents) far exceeds the minor inconvenience of typing two extra characters.

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
- **Mandatory Braces (Zero Tolerance)**: Prevents entire bug categories including scope errors, merge conflicts, and the infamous "Apple goto fail" SSL vulnerability (CVE-2014-1266). The minor typing inconvenience is vastly outweighed by preventing hours of debugging production incidents.
- **Underscore Prefix**: Immediately distinguishes fields from local variables and parameters, reducing naming conflicts
- **Var in Loops**: Reduces verbosity without sacrificing clarity (loop variable type is obvious from context)
- **Consistent Ordering**: Makes code predictable and easier to navigate in large files

### Enforcement

- Code reviews MUST reject PRs with missing braces (zero tolerance)
- `.editorconfig` file MUST be configured with `csharp_prefer_braces = true:error` for automatic enforcement
- CI/CD pipeline SHOULD include linting step to flag violations as build errors (not warnings)

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

**Version**: 1.17.0 | **Ratified**: 2026-03-06 | **Last Amended**: 2026-03-10
