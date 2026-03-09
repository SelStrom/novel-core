# Navigation Buttons Troubleshooting Guide

**Date**: 2026-03-09  
**Issue**: Navigation buttons don't work  
**Status**: Investigating

## Quick Fix Applied

Fixed the async/await pattern in `SceneManager.cs` navigation methods. The issue was that `.ContinueWith()` doesn't properly execute on Unity's main thread, which could cause navigation to fail silently.

### What Was Changed

**File**: `novel-core/Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/SceneManager.cs`

**Before**:
```csharp
_isNavigating = true;
LoadSceneAsync(previousEntry.sceneData).ContinueWith(task =>
{
    _isNavigating = false;
});
```

**After**:
```csharp
_isNavigating = true;
NavigateToSceneAsync(previousEntry.sceneData);

// New helper method:
private async void NavigateToSceneAsync(SceneData sceneData)
{
    try
    {
        await LoadSceneAsync(sceneData);
    }
    finally
    {
        _isNavigating = false;
    }
}
```

This ensures the `_isNavigating` flag is properly reset after navigation completes.

## How Navigation Buttons Should Work

### Expected Behavior

1. **On Game Start** (first scene loaded):
   - Back button: **DISABLED** (grey) - no history yet
   - Forward button: **DISABLED** (grey) - no forward history

2. **After Navigating to Second Scene** (via dialogue choices or scene transitions):
   - Back button: **ENABLED** (visible) - can go back to first scene
   - Forward button: **DISABLED** (grey) - no forward history

3. **After Clicking Back Button**:
   - Returns to previous scene
   - Dialogue restarts from beginning
   - Back button: state depends on whether there are more scenes behind
   - Forward button: **ENABLED** - can go forward to the scene we just came from

4. **After Clicking Forward Button**:
   - Goes to next scene in history
   - Dialogue restarts from beginning
   - Back button: **ENABLED** - can go back again
   - Forward button: state depends on whether there are more scenes ahead

## Testing Steps

### Test 1: Initial State

1. Open `Assets/Scenes/SampleScene.unity`
2. Press Play ▶️
3. Check Console for initialization messages:
   ```
   NovelCore: Initializing GameLifetimeScope...
   NovelCore: GameLifetimeScope initialized successfully
   NavigationUIManager: Successfully initialized SceneNavigationUI
   SceneNavigationUI: Back button listener added
   SceneNavigationUI: Forward button listener added
   SceneNavigationUI: Initialized successfully
   ```

4. **Check buttons at bottom of screen**:
   - Are they visible? ✅/❌
   - Back button is grey/disabled? ✅/❌
   - Forward button is grey/disabled? ✅/❌

**Expected**: Both buttons visible but disabled (grey)

---

### Test 2: Navigate to Second Scene

1. Continue from Test 1
2. Click through dialogue (Space or Click on screen)
3. When choice appears, select any option
4. Scene should transition to next scene
5. Check Console for:
   ```
   SceneManager: Added scene '[scene_id]' to navigation history
   ```

6. **Check buttons**:
   - Back button is now enabled (darker)? ✅/❌
   - Forward button still disabled (grey)? ✅/❌

**Expected**: Back button enabled, Forward button disabled

---

### Test 3: Click Back Button

1. Continue from Test 2
2. Click the Back button
3. Check Console for:
   ```
   SceneNavigationUI: Back button clicked!
   SceneManager: Navigating back to scene '[scene_id]' at line 0
   SceneNavigationHistory: Navigated back to scene '[scene_id]' (index: 0)
   NavigationUIManager: Restarting dialogue for scene '[scene_name]'
   ```

4. **Verify**:
   - Scene changed back to previous scene? ✅/❌
   - Dialogue restarted from beginning? ✅/❌
   - Back button state updated correctly? ✅/❌
   - Forward button is now enabled? ✅/❌

**Expected**: Scene goes back, dialogue restarts, Forward button enabled

---

### Test 4: Click Forward Button

1. Continue from Test 3
2. Click the Forward button
3. Check Console for:
   ```
   SceneNavigationUI: Forward button clicked!
   SceneManager: Navigating forward to scene '[scene_id]' at line 0
   SceneNavigationHistory: Navigated forward to scene '[scene_id]' (index: 1)
   NavigationUIManager: Restarting dialogue for scene '[scene_name]'
   ```

4. **Verify**:
   - Scene changed forward? ✅/❌
   - Dialogue restarted? ✅/❌
   - Button states correct? ✅/❌

**Expected**: Scene goes forward, dialogue restarts

---

## Common Issues & Solutions

### Issue 1: Buttons Not Visible

**Symptom**: Can't see navigation buttons at all

**Possible Causes**:
- NavigationUI GameObject is inactive
- Canvas is missing or not rendering
- UI is positioned off-screen

**Solution**:
1. In Hierarchy, find `Canvas → NavigationUI`
2. Make sure all parent objects are active
3. Check Inspector for `SceneNavigationUI` component
4. Verify button references are assigned

---

### Issue 2: Buttons Visible But Always Disabled

**Symptom**: Buttons are grey even after navigating to second scene

**Possible Causes**:
- NavigationUIManager not initialized
- ISceneManager not injected
- History not being populated

**Diagnostic Steps**:
1. Check Console for initialization errors:
   ```
   NavigationUIManager: ISceneManager not injected!
   ```
   
2. If error appears, check:
   - Is `GameLifetimeScope` in the scene?
   - Is `NavigationUIManager` in `autoInjectGameObjects` list?

3. Check if history is being populated:
   - Navigate to second scene
   - Look for: `SceneManager: Added scene to navigation history`
   - If missing, the scene isn't being added to history

**Solution**:
- Regenerate Sample Project: `Window → NovelCore → Generate Sample Project`
- This will recreate all UI components with correct setup

---

### Issue 3: Buttons Click But Nothing Happens

**Symptom**: Button click registered in Console, but scene doesn't change

**Diagnostic Steps**:
1. Click Back button
2. Check Console for error messages
3. Look for these specific logs:

**Expected logs**:
```
SceneNavigationUI: Back button clicked!
SceneManager: Navigating back to scene '...'
SceneNavigationHistory: Navigated back to scene '...'
SceneManager: Loading scene ...
NavigationUIManager: Restarting dialogue for scene '...'
```

**If you see**:
```
SceneNavigationUI: Cannot navigate back - no previous scenes in history
```
→ This means the history is empty. You need to navigate to at least one other scene first.

**If you see**:
```
SceneManager: Failed to get previous scene from history
```
→ The history entry is invalid or missing `sceneData` reference.

**Solution**: This was the bug fixed in this update. Try again after the fix.

---

### Issue 4: Navigation Works But Dialogue Doesn't Restart

**Symptom**: Scene changes but dialogue UI doesn't update

**Possible Causes**:
- NavigationUIManager not subscribed to events
- IDialogueSystem not injected
- Event handler not working

**Diagnostic Steps**:
1. Check for initialization log:
   ```
   NavigationUIManager: Subscribed to scene navigation events
   ```

2. After navigation, check for:
   ```
   NavigationUIManager: Restarting dialogue for scene '...'
   ```

3. If missing, check:
   - Is `IDialogueSystem` being injected into NavigationUIManager?
   - Is the event subscription in `Start()` method?

**Solution**: Code already includes this fix. If issue persists, regenerate Sample Project.

---

## Debug Commands

### Check Navigation History State

Add this temporary debug code to `SceneNavigationUI.cs` Update method:

```csharp
private void Update()
{
    if (_sceneManager == null) return;
    
    // Temporary debug (press H key)
    if (Input.GetKeyDown(KeyCode.H))
    {
        Debug.Log($"=== Navigation History Debug ===");
        Debug.Log($"Can Navigate Back: {_sceneManager.CanNavigateBack()}");
        Debug.Log($"Can Navigate Forward: {_sceneManager.CanNavigateForward()}");
        // Add more debug info as needed
    }
    
    UpdateButtonStates();
}
```

Press `H` key during gameplay to print navigation state.

---

## Architecture Overview

```
User Click → Button.onClick
    ↓
SceneNavigationUI.OnBackButtonClicked()
    ↓
SceneManager.NavigateBack()
    ↓
SceneNavigationHistory.NavigateBack() → returns SceneHistoryEntry
    ↓
SceneManager.NavigateToSceneAsync(sceneData)
    ↓
SceneManager.LoadSceneAsync(sceneData) → loads scene
    ↓
SceneManager.OnSceneLoadComplete event fires
    ↓
NavigationUIManager.OnSceneNavigated()
    ↓
DialogueSystem.StartScene(sceneData) → restarts dialogue
```

All steps should log to Console. If a step is missing in Console, that's where the problem is.

---

## Recent Fixes Applied

### 2026-03-09: Fixed Async Navigation

- **Problem**: `.ContinueWith()` doesn't properly execute on Unity main thread
- **Solution**: Created `NavigateToSceneAsync()` helper with `async void` pattern
- **Impact**: Navigation should now properly load scenes and reset `_isNavigating` flag

### Previous Fixes (from NAVIGATION_FIXES_REPORT.md)

- ✅ Registered `ISceneNavigationHistory` in DI container
- ✅ Added `sceneData` reference to `SceneHistoryEntry`
- ✅ Implemented actual scene loading in `NavigateBack/Forward`
- ✅ Added `IDialogueSystem` integration in `NavigationUIManager`
- ✅ Fixed `_isNavigating` flag to prevent duplicate history entries

---

## Next Steps

1. **Test in Unity Editor** with the steps above
2. **Check Console logs** to identify where the issue occurs
3. **Report findings**:
   - Which test fails?
   - What are the Console logs?
   - Are buttons visible/clickable?

This will help pinpoint the exact issue and guide further fixes.
