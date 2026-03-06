# Quick Start Guide: Visual Novel Constructor Development

**Feature**: Visual Novel Constructor  
**Date**: 2026-03-06  
**Audience**: Developers implementing the constructor

## Prerequisites

### Required Software

- **Unity 2022.3 LTS** (or later LTS version)
- **Visual Studio 2022** or **JetBrains Rider 2023.3+** (C# IDE)
- **Git** for version control
- **.NET SDK 6.0+** (for standalone tools/tests)

### Platform SDKs (for build targets)

- **Steam**: Steamworks SDK (via Steamworks.NET package)
- **iOS**: Xcode 14+ (macOS only), Apple Developer Account
- **Android**: Android SDK API 21+, Android NDK, JDK 11

### Unity Packages (install via Package Manager)

```
com.unity.addressables (1.21+)
com.unity.localization (1.4+)
com.unity.inputsystem (1.7+)
com.unity.test-framework (1.3+)
jp.hadashikick.vcontainer (1.14+) - Install via OpenUPM or git URL
```

**Spine-Unity**: Download from http://esotericsoftware.com/spine-unity-download (requires Spine license)

**Steamworks.NET**: Download from https://github.com/rlabrecque/Steamworks.NET

---

## Project Setup

### 1. Unity Project Location

The Unity project is located at the repository root:

```
novel-core/                 # Repository root = Unity project root
├── Assets/                 # Unity Assets folder
│   ├── Scripts/            # All C# code (AI-modifiable)
│   ├── Content/            # User content (Addressables source)
│   ├── Resources/          # Runtime-loaded assets
│   └── StreamingAssets/    # Addressables catalog
├── Packages/               # Unity packages
├── ProjectSettings/        # Unity project settings
├── .specify/               # Spec documentation
├── specs/                  # Feature specifications
└── README.md               # Project readme
```

**Important**: When opening in Unity, select the `novel-core` folder (repository root), not a subfolder.

### 2. Clone Repository

### 2. Clone Repository

```bash
git clone https://github.com/yourusername/novel-core.git
cd novel-core
git checkout 001-visual-novel-constructor
```

### 3. Open in Unity

### 3. Open in Unity

1. Launch Unity Hub
2. Click "Add" → select `novel-core` folder (repository root, not a subfolder)
3. Select Unity 2022.3 LTS version
4. Click project to open

**Note**: Unity will recognize the `Assets/`, `Packages/`, and `ProjectSettings/` folders and open as a Unity project.

### 4. Install Dependencies

### 4. Install Dependencies

**Via Package Manager** (Window → Package Manager):

1. Click "+" → "Add package by name"
2. Enter each package:
   - `com.unity.render-pipelines.universal` (URP - add first!)
   - `com.unity.addressables`
   - `com.unity.localization`
   - `com.unity.inputsystem`
   - `com.unity.test-framework`

**VContainer** (via git URL):

1. Click "+" → "Add package from git URL"
2. Enter: `https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer`

**Spine-Unity** (manual import):

1. Download `.unitypackage` from Spine website
2. Assets → Import Package → Custom Package → select downloaded file
3. Import all except Examples folder

**Steamworks.NET** (manual copy):

1. Download latest release from GitHub
2. Extract to `Assets/Plugins/Steamworks.NET/`

### 4. Configure Project Settings

**Rendering Pipeline** (Edit → Project Settings → Graphics):
- Install URP package via Package Manager first (see step 3)
- Create URP Asset: Right-click Project → Create → Rendering → URP Asset (2D Renderer)
- Go to Edit → Project Settings → Graphics
- Set "Scriptable Render Pipeline Settings" to your new URP Asset

**Scripting Backend** (Edit → Project Settings → Player):
- **Windows**: Player → Other Settings → Scripting Backend → **Mono**
- **macOS**: Player → Other Settings → Scripting Backend → IL2CPP
- **iOS**: IL2CPP (default, required by Apple)
- **Android**: IL2CPP

**Input System** (Edit → Project Settings → Player):
- Active Input Handling: "Input System Package (New)"
- Restart Unity when prompted

**Addressables** (Window → Asset Management → Addressables → Groups):
- Click "Create Addressables Settings" if first time
- Default settings are sufficient for now

---

## Project Structure Tour

### Code Organization

```
Assets/Scripts/NovelCore/
├── Runtime/              # Runtime code (included in builds)
│   ├── Core/             # Core systems (Dialogue, Scene, Save, Asset)
│   ├── Data/             # ScriptableObjects (data definitions)
│   ├── UI/               # UI components (dialogue box, choices)
│   ├── Animation/        # Character animation (Unity + Spine)
│   └── Platform/         # Platform abstractions (Steam, iOS, Android)
├── Editor/               # Editor-only code (tools, windows)
│   ├── Windows/          # Custom editor windows
│   ├── Inspectors/       # Custom property drawers
│   └── Tools/            # Utilities (validators, build pipeline)
└── Tests/                # Unit and integration tests
    ├── Runtime/          # PlayMode tests
    └── Editor/           # EditMode tests
```

### Content Organization

```
Assets/Content/           # User-created content (Addressables source)
├── Backgrounds/          # Background images
├── Characters/           # Character sprites
├── Audio/                # Music and SFX
│   ├── Music/
│   └── SFX/
├── Localization/         # Localization CSV files
└── Projects/             # Visual novel projects
    └── SampleProject/    # Example project
```

---

## Development Workflow

### Code Style Requirements

**IMPORTANT**: This project enforces strict C# coding standards (constitution requirement).

**Key Rules** (see `.specify/memory/constitution.md` for full details):

1. **Allman Style Braces** - Opening brace on new line:
   ```csharp
   // ✅ CORRECT
   if (condition)
   {
       DoSomething();
   }
   
   // ❌ WRONG
   if (condition) {
       DoSomething();
   }
   ```

2. **Always Use Braces** - Even for single-line statements:
   ```csharp
   // ✅ CORRECT
   if (condition)
   {
       DoSomething();
   }
   
   // ❌ WRONG
   if (condition)
       DoSomething();
   ```

3. **Var in Loops**:
   ```csharp
   // ✅ CORRECT
   foreach (var item in collection)
   {
       ProcessItem(item);
   }
   
   // ❌ WRONG
   foreach (SceneData item in collection)
   {
       ProcessItem(item);
   }
   ```

4. **Underscore Prefix for Private/Protected Fields**:
   ```csharp
   // ✅ CORRECT
   private int _count;
   protected IDialogueSystem _dialogueSystem;
   
   // ❌ WRONG
   private int count;
   protected IDialogueSystem dialogueSystem;
   ```

**Automatic Formatting**:
- `.editorconfig` file is configured with these rules
- Visual Studio / Rider will auto-format code on save
- Press `Ctrl+K, Ctrl+D` (VS) or `Ctrl+Alt+L` (Rider) to format file

### Phase 1: Core Systems (P1 - Basic Scene Playback)

**Goal**: Implement minimum viable dialogue system

**Steps**:

1. **Create ScriptableObject Definitions** (`Assets/Scripts/NovelCore/Runtime/Data/`)

   ```csharp
   // SceneData.cs
   [CreateAssetMenu(menuName = "NovelCore/Scene Data")]
   public class SceneData : ScriptableObject {
       public string sceneId;
       public string sceneName;
       public AssetReference<Sprite> backgroundImage;
       public List<DialogueLineData> dialogueLines;
   }
   ```

2. **Implement IDialogueSystem** (`Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/`)

   ```csharp
   public class DialogueSystem : IDialogueSystem {
       private readonly ILocalizationService _localization;
       private readonly IAudioService _audio;
       
       public DialogueSystem(ILocalizationService localization, IAudioService audio) {
           _localization = localization;
           _audio = audio;
       }
       
       public void PlayDialogue(IEnumerable<DialogueLineData> dialogueLines, Action onComplete) {
           // Implementation
       }
   }
   ```

3. **Create UI Prefab** (`Assets/Resources/NovelCore/UI/DialogueBox.prefab`)
   - Canvas with dialogue box panel
   - TextMeshPro for dialogue text
   - Continue indicator (arrow icon)

4. **Write PlayMode Test** (`Assets/Scripts/NovelCore/Tests/Runtime/DialogueSystemTests/`)

   ```csharp
   [UnityTest]
   public IEnumerator DialogueSystem_PlaysLine_WhenDialogueStarted() {
       var mockLocalization = new Mock<ILocalizationService>();
       var mockAudio = new Mock<IAudioService>();
       var dialogueSystem = new DialogueSystem(mockLocalization.Object, mockAudio.Object);
       
       bool completed = false;
       dialogueSystem.PlayDialogue(testLines, () => completed = true);
       
       yield return new WaitUntil(() => completed);
       Assert.IsTrue(completed);
   }
   ```

5. **Test in Play Mode**
   - Create sample SceneData asset
   - Add 3 test dialogue lines
   - Press Play, verify dialogue displays

**Success Criteria**: Can play dialogue from SceneData in Unity Play mode

---

### Phase 2: Editor Tools (P1 - Scene Editor)

**Goal**: Create editor window for non-programmers to author scenes

**Steps**:

1. **Create SceneEditorWindow** (`Assets/Scripts/NovelCore/Editor/Windows/`)

   ```csharp
   public class SceneEditorWindow : EditorWindow {
       [MenuItem("NovelCore/Scene Editor")]
       public static void ShowWindow() {
           GetWindow<SceneEditorWindow>("Scene Editor");
       }
       
       private void OnGUI() {
           // Draw scene editing UI
       }
   }
   ```

2. **Add Asset Picker Fields**
   - ObjectField for background sprite
   - List view for dialogue lines
   - Add/Remove dialogue buttons

3. **Implement Drag-and-Drop**
   - Drag sprite from Project window → background field
   - Auto-configure as Addressable

4. **Add Preview Button**
   - "Preview Scene" button → enters Play mode with scene loaded

**Success Criteria**: Non-programmer can create a scene via editor window

---

### Phase 3: Asset Management (P2 - Addressables Integration)

**Goal**: All assets loaded via Addressables, no broken references

**Steps**:

1. **Configure Addressable Groups** (Window → Asset Management → Addressables → Groups)
   - Create "Content_Backgrounds" group
   - Add `Assets/Content/Backgrounds/` folder to group
   - Repeat for Characters, Audio

2. **Implement IAssetManager** (`Assets/Scripts/NovelCore/Runtime/Core/AssetManagement/`)

   ```csharp
   public class AddressablesAssetManager : IAssetManager {
       public void LoadAssetAsync<T>(AssetReference<T> assetReference, Action<T> onComplete) 
           where T : UnityEngine.Object 
       {
           Addressables.LoadAssetAsync<T>(assetReference).Completed += handle => {
               onComplete?.Invoke(handle.Result);
           };
       }
   }
   ```

3. **Create AssetValidator Tool** (`Assets/Scripts/NovelCore/Editor/Tools/`)
   - Scan all SceneData assets
   - Validate each AssetReference resolves
   - Log errors for missing assets

4. **Write EditMode Test**

   ```csharp
   [Test]
   public void AssetValidator_DetectsMissingReferences() {
       var sceneData = CreateTestScene();
       sceneData.backgroundImage = default; // Invalid reference
       
       var validator = new AssetValidator();
       var errors = validator.ValidateScene(sceneData);
       
       Assert.AreEqual(1, errors.Count);
       Assert.IsTrue(errors[0].Contains("backgroundImage"));
   }
   ```

**Success Criteria**: Asset validator catches all broken references

---

### Phase 4: Save System (P2 - Save/Load)

**Goal**: Player progress persists across sessions

**Steps**:

1. **Implement ISaveSystem** (`Assets/Scripts/NovelCore/Runtime/Core/SaveSystem/`)

   ```csharp
   public class SaveSystem : ISaveSystem {
       public void SaveGame(int slotIndex, Action<bool> onComplete) {
           var saveData = new SaveData {
               version = "1.0.0",
               currentSceneId = _sceneManager.CurrentScene.sceneId,
               choiceHistory = _choiceHistory.ToList(),
               timestamp = DateTime.UtcNow
           };
           
           string json = JsonUtility.ToJson(saveData);
           string path = GetSavePath(slotIndex);
           File.WriteAllText(path, json);
           onComplete?.Invoke(true);
       }
   }
   ```

2. **Test Save/Load** (PlayMode test)

   ```csharp
   [UnityTest]
   public IEnumerator SaveSystem_PreservesProgress_AcrossSaveLoad() {
       _saveSystem.SaveGame(1, success => Assert.IsTrue(success));
       yield return null;
       
       _saveSystem.LoadGame(1, saveData => {
           Assert.AreEqual("scene_001", saveData.currentSceneId);
       });
   }
   ```

3. **Implement Auto-Save**
   - Hook into `ISceneManager.OnSceneLoadCompleted`
   - Trigger `SaveGame(0, null)` (slot 0 = auto-save)

**Success Criteria**: Save in scene A, quit Unity, reload, resume from scene A

---

### Phase 5: Build Pipeline (P5 - Multi-Platform Builds)

**Goal**: Generate builds for Windows, macOS, iOS, Android

**Steps**:

1. **Create BuildPipelineWindow** (`Assets/Scripts/NovelCore/Editor/Windows/`)

   ```csharp
   public class BuildPipelineWindow : EditorWindow {
       [MenuItem("NovelCore/Build Pipeline")]
       public static void ShowWindow() {
           GetWindow<BuildPipelineWindow>("Build Pipeline");
       }
       
       private void OnGUI() {
           if (GUILayout.Button("Build Windows x64")) {
               BuildWindows();
           }
           // Repeat for other platforms
       }
   }
   ```

2. **Implement BuildWindows()**

   ```csharp
   private void BuildWindows() {
       var options = new BuildPlayerOptions {
           scenes = new[] { "Assets/Scenes/Main.unity" },
           target = BuildTarget.StandaloneWindows64,
           locationPathName = "Builds/Windows/VisualNovel.exe",
           options = BuildOptions.None
       };
       
       var report = BuildPipeline.BuildPlayer(options);
       if (report.summary.result == BuildResult.Succeeded) {
           Debug.Log("Build succeeded: " + report.summary.totalSize + " bytes");
       }
   }
   ```

3. **Test Build**
   - Click "Build Windows x64"
   - Navigate to `Builds/Windows/`
   - Run `VisualNovel.exe`
   - Verify scene loads and dialogue plays

**Success Criteria**: Working executable for each platform

---

## Testing Guide

### Running Tests

**Via Test Runner** (Window → General → Test Runner):

1. Click "PlayMode" tab
2. Select test(s) to run
3. Click "Run Selected"

**Via Command Line** (for CI/CD):

```bash
# Run all PlayMode tests
/Applications/Unity/Hub/Editor/2022.3.XX/Unity.app/Contents/MacOS/Unity \
  -runTests \
  -batchmode \
  -projectPath $(pwd) \
  -testResults $(pwd)/test-results.xml \
  -testPlatform PlayMode

# Run specific test assembly
-assemblyNames "NovelCore.Tests.Runtime"
```

### Test Structure

- **PlayMode Tests**: Test runtime systems (Dialogue, Scene, Save)
- **EditMode Tests**: Test editor tools (AssetValidator, BuildPipeline)
- **Integration Tests**: Test cross-system interactions (Scene + Dialogue + Audio)

### Coverage Requirement

Constitution Principle VI requires >80% code coverage:

```bash
# Install OpenCover (Windows) or dotnet-coverage (macOS/Linux)
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## Debugging Tips

### Common Issues

**Issue**: "Assembly 'NovelCore.Runtime.Core' not found"  
**Fix**: Create assembly definition files:
- Right-click folder → Create → Assembly Definition
- Name: `NovelCore.Runtime.Core`
- Check "Auto Referenced" and "Override References"

**Issue**: "AssetReference not loading"  
**Fix**: Check Addressables Groups:
- Window → Asset Management → Addressables → Groups
- Ensure asset is in a group
- Click "Build → New Build → Default Build Script"

**Issue**: "VContainer injection failing"  
**Fix**: Ensure LifetimeScope in scene:
- Create GameObject → VContainer → LifetimeScope
- Attach `GameLifetimeScope` script

### Performance Profiling

**Unity Profiler** (Window → Analysis → Profiler):
- CPU Usage: Target 16ms per frame (60 FPS)
- Memory: Monitor Addressables allocations
- Rendering: Check draw calls (<50 for 2D dialogue scenes)

**Deep Profiling**:
- Enable "Deep Profile" in Profiler
- Identify bottlenecks in dialogue/scene systems
- Optimize hot paths (e.g., text rendering, sprite batching)

---

## VContainer Dependency Injection

### Registering Services

Edit `GameLifetimeScope.cs`:

```csharp
protected override void Configure(IContainerBuilder builder) {
    // Singleton: One instance for entire app lifetime
    builder.Register<IDialogueSystem, DialogueSystem>(Lifetime.Singleton);
    
    // Transient: New instance per injection
    builder.Register<ICharacterAnimator, UnityCharacterAnimator>(Lifetime.Transient);
    
    // Scoped: One instance per scene
    builder.Register<ISceneManager, SceneManager>(Lifetime.Scoped);
}
```

### Injecting Dependencies

**Constructor Injection** (preferred):

```csharp
public class SceneController : MonoBehaviour {
    private readonly IDialogueSystem _dialogueSystem;
    private readonly ISceneManager _sceneManager;
    
    [Inject]
    public SceneController(IDialogueSystem dialogueSystem, ISceneManager sceneManager) {
        _dialogueSystem = dialogueSystem;
        _sceneManager = sceneManager;
    }
}
```

**Field Injection** (for MonoBehaviours):

```csharp
public class DialogueUI : MonoBehaviour {
    [Inject] private ILocalizationService _localization;
    
    private void Start() {
        string text = _localization.GetLocalizedString("greeting");
    }
}
```

---

## Git Workflow

### Branch Strategy

- `main`: Stable releases only
- `001-visual-novel-constructor`: Active development (current)
- `feature/dialogue-system`: Sub-features branch from constructor branch

### Commit Guidelines

```bash
# Format: <type>: <description>
git commit -m "feat: implement dialogue system PlayDialogue method"
git commit -m "fix: resolve asset reference null exception"
git commit -m "test: add PlayMode tests for SaveSystem"
git commit -m "docs: update quickstart with VContainer setup"
```

**Types**: `feat`, `fix`, `test`, `docs`, `refactor`, `perf`, `chore`

---

## Next Steps

1. **Week 1-2**: Implement P1 (Basic Scene Playback)
   - Core systems: Dialogue, Scene, Asset
   - Basic editor window

2. **Week 3-4**: Implement P2 (Branching Narrative)
   - Choice system
   - Story flow graph
   - Save/Load

3. **Week 5-6**: Implement P3-P4 (Characters + Audio)
   - Character emotions
   - Animation system (Unity + Spine)
   - Audio wrapper

4. **Week 7-8**: Implement P5 (Build Pipeline)
   - Multi-platform builds
   - Platform-specific SDKs
   - Steam/iOS/Android integration

---

## Resources

- **Constitution**: `.specify/memory/constitution.md` (project principles)
- **Specification**: `specs/001-visual-novel-constructor/spec.md` (requirements)
- **Data Model**: `specs/001-visual-novel-constructor/data-model.md` (entities)
- **Contracts**: `specs/001-visual-novel-constructor/contracts/` (interfaces)
- **Unity Manual**: https://docs.unity3d.com/2022.3/Documentation/Manual/
- **VContainer Docs**: https://vcontainer.hadashikick.jp/

---

## Getting Help

- **Code Reviews**: Submit PRs to `001-visual-novel-constructor` branch
- **Questions**: Open GitHub Issues with `[Question]` tag
- **Bugs**: Open GitHub Issues with reproduction steps
- **Constitution Violations**: Flag in PR review with principle reference

---

Happy coding! 🎮
