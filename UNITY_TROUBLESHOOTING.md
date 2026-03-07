# Unity Troubleshooting Checklist - UI Manager Fix

**Date**: 2026-03-07  
**Issue**: Checking for Unity compilation errors after UI Manager implementation

## Pre-Flight Checklist

### 1. Required Files Created ✅
- [x] `Assets/Scripts/NovelCore/Runtime/UI/UIManager.cs`
- [x] `Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs` (modified)
- [x] `Assets/Scripts/NovelCore/Runtime/UI/DialogueBox/DialogueBoxController.cs` (existing)
- [x] `Assets/Scripts/NovelCore/Editor/Tools/Generators/SampleProjectGenerator.cs` (modified)

### 2. Dependencies Check

**UIManager.cs** requires:
- ✅ `UnityEngine` (built-in)
- ✅ `VContainer` (should be installed)
- ✅ `VContainer.Unity` (should be installed)
- ✅ `NovelCore.Runtime.Core.DialogueSystem`
- ✅ `NovelCore.Runtime.Core.InputHandling`
- ✅ `NovelCore.Runtime.Core.Localization`
- ✅ `NovelCore.Runtime.UI.DialogueBox`

**Check VContainer Package**:
```bash
# In Unity: Window → Package Manager → Packages: In Project
# Look for: VContainer (com.vrchat.vcontainer or similar)
```

### 3. Common Unity Errors & Fixes

#### Error: "Type or namespace 'VContainer' could not be found"

**Fix**:
1. Open Unity Package Manager
2. Add VContainer if missing:
   - Git URL: `https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.14.0`
   - Or via OpenUPM

#### Error: "FindFirstObjectByType does not exist"

**Fix**: Update Unity to 2022.3+ or replace with:
```csharp
// Replace:
FindFirstObjectByType<T>()

// With (Unity 2021):
FindObjectOfType<T>()
```

#### Error: "The type or namespace name 'DialogueBoxController' could not be found"

**Fix**:
1. Check file location: `Assets/Scripts/NovelCore/Runtime/UI/DialogueBox/DialogueBoxController.cs`
2. Ensure namespace is `NovelCore.Runtime.UI.DialogueBox`
3. Force reimport: Right-click folder → Reimport

#### Error: "Method 'Initialize' not found on 'DialogueBoxController'"

**Fix**: Verify DialogueBoxController has:
```csharp
public void Initialize(IDialogueSystem dialogueSystem, ILocalizationService localizationService)
```

### 4. Unity Editor Actions

**To force recompile**:
1. Unity Editor → Assets → Refresh (Cmd/Ctrl + R)
2. Or: Assets → Reimport All (nuclear option, slow)

**To check for compilation errors**:
1. Unity Editor → Window → Console
2. Filter by "Error" (red icon)
3. Look for script compilation errors

**To regenerate .csproj files** (for IDE):
```
Unity Editor → Edit → Preferences → External Tools → Regenerate project files
```

### 5. Testing Steps

#### Step 1: Verify Compilation
1. Open Unity Editor
2. Wait for compilation to complete (bottom-right spinner)
3. Check Console for errors (should be 0)

#### Step 2: Verify UIManager in Scene
1. Generate Sample Project: `NovelCore → Generate Sample Project`
2. Open `Assets/Scenes/SampleScene.unity`
3. Check Hierarchy:
   - ✅ GameLifetimeScope
   - ✅ GameStarter
   - ✅ UIManager ← should exist
   - ✅ UnityInputService (child of GameLifetimeScope)

#### Step 3: Verify Inspector Fields
1. Select UIManager GameObject
2. Inspector should show:
   - UI References section (Canvas, DialogueBoxController - both optional)
   - DialogueBox Prefab field (optional)
   - Script component should not show "Missing Script" error

#### Step 4: Play Mode Test
1. Press Play ▶️
2. Wait 0.5 seconds for auto-start
3. Check Console output:
   ```
   NovelCore: Initializing GameLifetimeScope...
   UnityInputService: Initialized
   UIManager: Initializing UI...
   UIManager: UI initialization complete
   GameStarter: Starting game with scene: Введение
   DialogueSystem: Starting scene Введение
   DialogueSystem: Displaying line line_intro_001
   ```

4. Check Game view:
   - ✅ Dialogue box visible at bottom
   - ✅ Text: "Привет! Это демонстрационная визуальная новелла."
   - ✅ Continue indicator (▼) after typewriter

5. Click/press Space:
   - ✅ Text changes to next line
   - ✅ Console: "UnityInputService: Primary action performed"

### 6. Common Runtime Errors

#### Console: "UIManager: DialogueBox prefab not found!"

**Fix**:
1. Generate DialogueBox prefab: `NovelCore → Generate UI Prefabs → Dialogue Box`
2. Verify file exists: `Assets/Resources/NovelCore/UI/DialogueBox.prefab`

#### Console: "UIManager: DialogueSystem not injected!"

**Fix**:
1. Ensure GameLifetimeScope exists in scene
2. Check GameLifetimeScope.cs has DialogueSystem registration
3. Verify UIManager is in the same scene as GameLifetimeScope

#### Console: "DialogueBoxController: Dialogue text reference not set!"

**Fix**:
1. Regenerate DialogueBox prefab (see above)
2. Or manually add TextMeshPro component to DialogueBox/DialogueText

### 7. Files to Check if Errors Persist

```bash
# Check these files have correct structure:
cat Assets/Scripts/NovelCore/Runtime/UI/UIManager.cs | head -50
cat Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs
cat Assets/Scripts/NovelCore/Runtime/UI/DialogueBox/DialogueBoxController.cs | grep "public void Initialize"

# Check Unity meta files exist:
ls Assets/Scripts/NovelCore/Runtime/UI/UIManager.cs.meta
```

### 8. Nuclear Option (if all else fails)

**Full Unity project reimport**:
1. Close Unity Editor
2. Delete `Library/` folder (Unity will regenerate)
3. Delete `Temp/` folder
4. Reopen Unity Editor
5. Wait for full reimport (can take 5-10 minutes)

⚠️ **Warning**: Only use if other solutions fail. This forces full recompilation.

---

## Expected Result After Fix

✅ Unity compiles without errors  
✅ UIManager GameObject exists in scene  
✅ Press Play → dialogue appears  
✅ Click/Space → dialogue advances  
✅ Scene transitions work  

## If Problems Persist

Please provide:
1. Unity Console error messages (exact text)
2. Unity version (Help → About Unity)
3. VContainer package version (Window → Package Manager)
4. Screenshot of Hierarchy (showing GameObjects)
5. Screenshot of UIManager Inspector

---

**Related Documentation**:
- `IMPLEMENTATION_UI_MANAGER_FIX.md` - Full implementation details
- `SAMPLE_PROJECT_GUIDE.md` - User testing guide
- `PACKAGE_INSTALLATION.md` - Dependency setup
