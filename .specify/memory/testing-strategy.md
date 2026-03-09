# Novel Core - Testing Strategy

**Last Updated**: 2026-03-09

## Test Organization

### Assembly Structure

```
NovelCore.Tests.Runtime.asmdef  # PlayMode tests
NovelCore.Tests.Editor.asmdef   # EditMode tests
```

### Naming Convention

Tests MUST follow pattern: `[System]_[Method]_[Scenario]_[Expected]`

**Examples:**
- `DialogueSystem_AdvanceDialogue_AtLastLine_CompletesScene`
- `SaveSystem_LoadGame_WithCorruptedData_FallsBackToPreviousSave`
- `SceneManager_NavigateBack_WithEmptyHistory_ReturnsFalse`

## Test Mode Selection (EditMode vs PlayMode)

### EditMode Tests (Preferred)

Use EditMode for:
- ✅ ScriptableObject creation/validation
- ✅ Data model tests
- ✅ Builders and factory classes
- ✅ Pure C# business logic
- ✅ Synchronous operations
- ✅ Validators and utility classes

**Benefits:**
- 60-80% faster execution (no Play Mode initialization)
- More reliable (no Unity runtime variability)
- Immediate feedback during development

### PlayMode Tests (Only When Required)

Use PlayMode for:
- ✅ `async`/`await` operations
- ✅ File system I/O
- ✅ `Application.persistentDataPath`
- ✅ `Directory`/`File` APIs
- ✅ Integration tests requiring game loop
- ✅ GameStarter initialization tests
- ✅ Scene loading/unloading
- ✅ Addressables asset loading

## Test Coverage Targets

### MVP Phase (v0.1.0 - v0.3.0)
- **Functional Requirements**: ≥80% coverage
- **P1 User Stories**: 100% coverage (manual testing acceptable)
- **Automated Tests**: Optional
- **Testing Method**: Manual validation + dogfooding

### Post-MVP Phase (v0.4.0 - v0.9.0)
- **Functional Requirements**: ≥95% coverage
- **Code Coverage**: >80% automated test coverage
- **Unit Tests**: All Runtime systems (DialogueSystem, SaveSystem, SceneManager, etc.)
- **Integration Tests**: Minimum 20 integration tests (see below)
- **Edge Cases**: All documented edge cases handled

### Production (v1.0.0+)
- **Functional Requirements**: 100% coverage
- **Code Coverage**: >80% automated test coverage
- **Regression Tests**: All P1/P2 user stories automated
- **CI/CD**: Test suite blocks merges if tests fail

## Minimum Integration Test Suite (v0.4.0+)

Required integration tests:

1. **Dialogue System ↔ Save System**: Save/load dialogue state mid-conversation
2. **Asset System ↔ Scene System**: Load scene assets via Addressables
3. **Input System ↔ UI System**: Click handling for dialogue advance and choices
4. **Save System ↔ Platform Services**: Cloud save integration (Steam, iCloud, Google Play)
5. **Localization System ↔ Dialogue System**: Language switching updates dialogue text
6. **GameStarter Initialization**: VContainer → DialogueSystem/SceneManager startup
7. **Scene Navigation**: Linear scene progression (Scene1 → Scene2 → Scene3)
8. **Choice-Based Branching**: Choice selection → target scene loading
9. **Scene History**: Back/Forward navigation with state restoration
10. **Conditional Transitions**: Game state evaluation → correct scene loading
11. **Asset Preloading**: Preload next scene → instant transition performance
12. **Save Migration**: Old save format → new format upgrade
13. **Character Animation**: Spine animation state transitions
14. **Audio Transitions**: Music crossfade during scene transitions
15. **Multi-Platform Input**: Touch/Mouse/Keyboard input equivalence
16. **Addressables Memory**: Asset loading → memory cleanup on scene change
17. **Auto-Save Trigger**: Scene transition → auto-save execution
18. **Error Recovery**: Missing asset → fallback asset loading
19. **Localization Loading**: Language switch → asset bundle reload
20. **Platform Service Initialization**: Steam/iOS/Android SDK startup

## Test Execution Workflow

### After Writing Tests

1. **Run EditMode Tests First**:
   ```bash
   /Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
     -runTests -batchmode -nographics -projectPath "$(pwd)" \
     -testPlatform EditMode -testResults "./test-results-editmode.xml" \
     -logFile - 2>&1
   ```

2. **Run PlayMode Tests (if EditMode passes)**:
   ```bash
   /Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
     -runTests -batchmode -projectPath "$(pwd)" \
     -testPlatform PlayMode -testResults "./test-results-playmode.xml" \
     -logFile - 2>&1
   ```

3. **Fix All Errors Before Proceeding**: Zero-tolerance for test failures

### After Task Completion (MANDATORY)

After completing ANY group of related tasks (user story, feature phase, bug fix):

1. Run full test suite (EditMode + PlayMode)
2. Fix ALL test failures before moving to next task
3. Verify exit code 0 (all tests passed)
4. Commit changes only after tests pass

## Test Best Practices

### Test Fixtures

```csharp
[TestFixture]
public class DialogueSystemTests
{
    private DialogueSystem _dialogueSystem;
    private Mock<IAudioService> _mockAudio;

    [SetUp]
    public void Setup()
    {
        _mockAudio = new Mock<IAudioService>();
        _dialogueSystem = new DialogueSystem(_mockAudio.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _dialogueSystem = null;
    }
}
```

### Test Data Builders

```csharp
public class SceneDataBuilder
{
    private string _sceneName = "TestScene";
    private List<DialogueLineData> _dialogueLines = new();

    public SceneDataBuilder WithName(string name)
    {
        _sceneName = name;
        return this;
    }

    public SceneDataBuilder AddDialogueLine(string text)
    {
        _dialogueLines.Add(new DialogueLineData { text = text });
        return this;
    }

    public SceneData Build()
    {
        var scene = ScriptableObject.CreateInstance<SceneData>();
        scene.sceneName = _sceneName;
        scene.dialogueLines = _dialogueLines;
        return scene;
    }
}
```

### Mock Abstractions

Use interfaces for platform-specific code:

```csharp
[Test]
public void SaveSystem_SaveGame_CallsPlatformService()
{
    // Arrange
    var mockPlatform = new Mock<IPlatformService>();
    var saveSystem = new SaveSystem(mockPlatform.Object);

    // Act
    saveSystem.SaveGame(1, success => {});

    // Assert
    mockPlatform.Verify(p => p.UploadSaveToCloud(
        It.IsAny<string>(), 
        It.IsAny<byte[]>(), 
        It.IsAny<Action<bool>>()), 
        Times.Once);
}
```

## Regression Prevention

### Change Detection
- New features MUST NOT break existing sample projects
- Automated regression tests run on every commit

### Asset Compatibility
- New versions MUST load projects from previous MINOR version
- Save format migrations tested via integration tests

### Editor Stability
- No crashes during 8-hour stress test of editing workflows

## CI/CD Integration

### Pre-Commit Hooks
- Run EditMode tests locally before commit
- Block commit if tests fail

### Pipeline Stages
1. **Unit Tests** (EditMode): Fast validation (<2 minutes)
2. **Integration Tests** (PlayMode): Cross-module validation (<5 minutes)
3. **Platform Builds**: Windows/Mac/iOS/Android smoke tests
4. **Performance Profiling**: Memory/FPS regression detection

### Merge Blocking
- Pull requests MUST pass 100% of tests
- Code coverage MUST NOT decrease
- Performance MUST NOT regress vs. baseline

## Test Naming Examples

```csharp
// ✅ Good names (clear scenario and expected result)
[Test] public void DialogueSystem_AdvanceDialogue_WithChoices_ShowsChoiceUI() {}
[Test] public void SaveSystem_LoadGame_WithMissingFile_ReturnsNull() {}
[Test] public void SceneManager_PreloadScene_WithValidReference_LoadsAssets() {}

// ❌ Bad names (vague, unclear expected result)
[Test] public void TestDialogue() {}
[Test] public void SaveTest1() {}
[Test] public void SceneLoading() {}
```

## Enforcement

- **Code Reviews**: Require test coverage for all new features
- **CI/CD Gates**: Block merges if coverage drops below target
- **Quarterly Audits**: Review test suite health and coverage gaps

## Manual Testing Procedures

### Running Tests in Unity Editor

Due to platform-specific restrictions (macOS permission issues with Unity Package Manager in batch mode), tests should primarily be run through Unity Editor GUI.

#### Unity Test Runner Workflow

1. **Open Unity Project**:
   ```bash
   open -a "Unity Hub" novel-core/
   ```

2. **Open Test Runner Window**:
   - Menu: `Window → General → Test Runner`

3. **Run EditMode Tests**:
   - In Test Runner window, select **EditMode** tab
   - Click **Run All** button
   - All tests should pass (green checkmarks)

4. **Run PlayMode Tests**:
   - In Test Runner window, select **PlayMode** tab
   - Click **Run All** button
   - All tests should pass (green checkmarks)

#### Batch Mode Test Execution (CI/CD)

For automated testing environments, use Unity command-line interface:

**EditMode Tests**:
```bash
/Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
  -runTests -batchmode -nographics -projectPath "$(pwd)" \
  -testPlatform EditMode -testResults "./test-results-editmode.xml" \
  -logFile - 2>&1
```

**PlayMode Tests**:
```bash
/Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
  -runTests -batchmode -projectPath "$(pwd)" \
  -testPlatform PlayMode -testResults "./test-results-playmode.xml" \
  -logFile - 2>&1
```

### Feature-Specific Manual Testing

#### Linear Scene Progression

1. Open SampleScene in Unity
2. Ensure GameStarter.StartingScene points to Scene01_Introduction
3. Press Play
4. Click through dialogue
5. **Expected**: Scene automatically transitions to Scene02
6. Click through Scene02 dialogue
7. **Expected**: Scene automatically transitions to Scene03

#### Choice-Based Branching

1. Create test scene with choices (or use Scene02_ChoicePoint)
2. Press Play
3. Reach choice point
4. Select Option A
5. **Expected**: Loads correct target scene for Option A
6. Restart and select Option B
7. **Expected**: Loads correct target scene for Option B

#### Scene Navigation (Back/Forward)

1. Load sample project with multiple scenes
2. Press Play
3. Navigate through 2-3 scenes
4. Click "Back" button (◀ Назад)
5. **Expected**: Returns to previous scene, dialogue restarts
6. Click "Forward" button (Вперёд →)
7. **Expected**: Goes to next scene in history

### Troubleshooting Test Failures

#### Tests Not Appearing in Test Runner

**Symptom**: Test Runner shows "No tests found"

**Solution**:
1. Close Unity
2. Delete `Library/` folder in project
3. Reopen Unity and wait for recompilation
4. Open Test Runner again

#### Compilation Errors

**Symptom**: Red errors in Console

**Solution**:
1. Check error messages for missing namespaces or types
2. Ensure all test files are in correct folders:
   - EditMode tests: `Assets/Scripts/NovelCore/Tests/Editor/`
   - PlayMode tests: `Assets/Scripts/NovelCore/Tests/Runtime/`
3. Verify assembly references in `.asmdef` files

#### Package Manager Errors in Batch Mode

**Symptom**: Unity hangs with "Failed to start Unity Package Manager" error

**Workaround**:
- This is a known macOS permission issue
- Use Unity Editor GUI Test Runner instead of batch mode
- Tests will run successfully in editor environment

### Test Coverage Verification

After completing feature implementation:

1. Run EditMode tests → verify 100% pass rate
2. Run PlayMode tests → verify 100% pass rate
3. Check test coverage report (if available)
4. Ensure coverage meets target for current phase:
   - MVP: >80% functional coverage
   - Post-MVP: >80% code coverage
   - Production: >80% code coverage + 100% functional coverage
