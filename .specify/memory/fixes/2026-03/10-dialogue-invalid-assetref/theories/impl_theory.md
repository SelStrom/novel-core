# Implementation Theory

## Hypothesis

Баг вызван **отсутствием валидации AssetReference** перед попыткой загрузки через Addressables API. Код проверяет `!= null`, но AssetReference может быть non-null с **пустым или невалидным RuntimeKey**, что приводит к `InvalidKeyException`.

## Location

### Primary Location

- **File**: `Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`
- **Namespace**: `NovelCore.Runtime.Core.DialogueSystem`
- **Class**: `DialogueSystem`
- **Method**: `CompleteDialogue()` (async void)
- **Line Range**: 338-372 (specifically lines 338-351)

### Secondary Locations

1. **File**: `Assets/Scripts/NovelCore/Runtime/Core/AssetManagement/AddressablesAssetManager.cs`
   - **Method**: `LoadAssetAsync<T>(object key)`
   - **Lines**: 35-70
   - **Reason**: Receives invalid key from DialogueSystem, doesn't validate AssetReference before passing to Addressables

## Root Cause Hypothesis

### Category

- [x] Logical error (incorrect condition check)
- [x] Edge case not handled (invalid AssetReference with empty RuntimeKey)
- [ ] Race condition
- [ ] State management error
- [ ] Memory/resource leak
- [x] API misuse (AssetReference.RuntimeKeyIsValid() not called)
- [ ] Other

### Detailed Explanation

**How the bug occurs**:

1. **User clicks on last dialogue** in a scene
   - `DialogueSystem.AdvanceDialogue()` is called (line 117)
   
2. **No more dialogue lines** → `CompleteDialogue()` is invoked (line 117)
   
3. **Scene transition check** (lines 338-342):
   ```csharp
   if (targetSceneRef == null && _currentScene.NextScene != null)
   {
       Debug.Log($"DialogueSystem: Using nextScene for linear progression");
       targetSceneRef = _currentScene.NextScene;
   }
   ```
   
   **Issue**: `_currentScene.NextScene` is a **non-null AssetReference** but has an **empty or invalid GUID/RuntimeKey**
   - Unity Inspector allows creating AssetReference without assigning an asset
   - AssetReference object exists (not null) but `RuntimeKey` is null or empty string
   
4. **Invalid null check** (line 345):
   ```csharp
   if (targetSceneRef != null)
   ```
   
   **Problem**: This check passes even when AssetReference is invalid!
   - Should be: `if (targetSceneRef != null && targetSceneRef.RuntimeKeyIsValid())`
   
5. **Attempt to load with invalid key** (line 351):
   ```csharp
   var nextScene = await _assetManager.LoadAssetAsync<SceneData>(targetSceneRef);
   ```
   
   **Problem**: AssetReference with empty RuntimeKey is passed to LoadAssetAsync
   
6. **AddressablesAssetManager receives invalid key** (lines 41-43):
   ```csharp
   if (key is AssetReference assetRef)
   {
       handle = Addressables.LoadAssetAsync<T>(assetRef);
   }
   ```
   
   **Problem**: No validation of `assetRef.RuntimeKeyIsValid()` before calling Addressables API
   
7. **Addressables.LoadAssetAsync throws InvalidKeyException**:
   ```
   UnityEngine.AddressableAssets.InvalidKeyException: 
   Exception of type 'UnityEngine.AddressableAssets.InvalidKeyException' was thrown. 
   No MergeMode is set to merge the multiple keys requested. 
   Keys=, Type=NovelCore.Runtime.Data.Scenes.SceneData
   ```
   
   **Root Cause**: Addressables API cannot load from an empty key

8. **Exception is caught** (lines 366-371) but:
   - Error is logged to console (confuses user)
   - No user-facing "End of Story" notification shown
   - User sees dialogue panel disappear with error messages

### Problematic Code

**DialogueSystem.cs** (lines 338-351):

```csharp
// If no rule matched, check for linear nextScene
if (targetSceneRef == null && _currentScene.NextScene != null)
{
    Debug.Log($"DialogueSystem: Using nextScene for linear progression");
    targetSceneRef = _currentScene.NextScene;
}

// Load target scene if determined
if (targetSceneRef != null) // ❌ INSUFFICIENT CHECK
{
    Debug.Log($"DialogueSystem: Loading target scene...");
    
    try
    {
        var nextScene = await _assetManager.LoadAssetAsync<SceneData>(targetSceneRef);
        // ... exception occurs here
```

**Issue**: Line 345 checks `!= null` but AssetReference requires additional validation.

**AddressablesAssetManager.cs** (lines 41-43):

```csharp
if (key is AssetReference assetRef)
{
    handle = Addressables.LoadAssetAsync<T>(assetRef); // ❌ NO VALIDATION
}
```

**Issue**: Should check `assetRef.RuntimeKeyIsValid()` before calling Addressables API.

### Call Stack Analysis

```
[Entry Point]
  ↓
UIManager.OnPrimaryInput()  (line 257)
  ↓
DialogueSystem.AdvanceDialogue()  (line 117)
  ↓
DialogueSystem.CompleteDialogue()  (line 289 - async void)
  ↓
[Check NextScene]
  targetSceneRef = _currentScene.NextScene  (line 341)
  ↓
[Invalid null check]
  if (targetSceneRef != null)  (line 345) ✓ passes (AssetReference is non-null)
  ↓
AddressablesAssetManager.LoadAssetAsync<SceneData>(targetSceneRef)  (line 351)
  ↓
Addressables.LoadAssetAsync<T>(assetRef)  (AddressablesAssetManager.cs:43)
  ↓
[Addressables Internal]
  InvalidKeyException thrown (empty key)
```

**Analysis**: The bug occurs because AssetReference null check is insufficient. The object exists but contains no valid asset reference.

## Git History Insights

### Recent Changes

1. **Commit**: Search needed (recent scene-transition feature merge)
   - **Suspicion Level**: HIGH
   - **Reason**: Feature `001-scene-transition` was recently merged (2026-03-07 based on spec date)
   - **Changes**: Added NextScene field to SceneData, linear progression logic to DialogueSystem
   - **Problem**: Implementation added NextScene support but didn't validate AssetReference properly

2. **Missing validation pattern**:
   - **Evidence**: Test `NextScene_WhenLoadFails_CompletesDialogueGracefully()` tests **missing asset** (null return from LoadAssetAsync)
   - **Gap**: No test for **invalid AssetReference** (empty GUID)
   - **Reason**: Developers likely tested null case and asset-not-found case, but missed empty-AssetReference case

## Edge Cases Analysis

### Edge Case 1: Empty AssetReference

- **Scenario**: SceneData.NextScene is assigned in Unity Inspector but no asset selected
- **Current handling**: AssetReference object created with empty GUID
- **Problem**: `!= null` check passes, `RuntimeKeyIsValid()` returns false, Addressables throws exception
- **Should handle**: Check `RuntimeKeyIsValid()` before attempting load

### Edge Case 2: Invalid GUID

- **Scenario**: AssetReference contains GUID of deleted/moved asset
- **Current handling**: Addressables attempts load, returns null (handled by existing code)
- **Status**: ✅ Already handled by null check (line 360)

### Edge Case 3: Null AssetReference

- **Scenario**: SceneData.NextScene is completely null
- **Current handling**: Line 338 check `_currentScene.NextScene != null` prevents issue
- **Status**: ✅ Correctly handled

**Summary**: Bug occurs specifically with **empty (but non-null) AssetReference** - the edge case that wasn't tested.

## Confidence Score

**95%** that this is an implementation bug

### Reasoning

1. ✅ **Very Strong Evidence (95%)**:
   - **Clear API misuse**: AssetReference requires `RuntimeKeyIsValid()` check, not just `!= null`
   - **Stack trace proof**: InvalidKeyException directly from Addressables.LoadAssetAsync
   - **Reproducible**: Can create failing test with empty AssetReference (already done)
   - **Recent feature**: Linear scene progression recently added, validation missed
   - **Missing test coverage**: No test for empty AssetReference edge case

2. ✅ **Code inspection confirms**:
   - Unity documentation for AssetReference states: "Use RuntimeKeyIsValid() to check if the reference is valid"
   - Existing code has precedent for this pattern (should be used but isn't)
   - Exception message explicitly mentions "Keys=" (empty key list)

3. ✅ **Defensive programming violation**:
   - Constitution Principle VI requires defensive programming
   - Missing validation at two levels:
     a. DialogueSystem before calling LoadAssetAsync
     b. AddressablesAssetManager before calling Addressables API

**Why not 100%**:
- Small chance (5%) that there's a Unity version-specific behavior or Addressables configuration issue
- However, exception message is unambiguous - the key is empty

## Recommended Fix Approach

### Fix Strategy

**Two-level fix** (defense in depth):

**Level 1: DialogueSystem.cs** (primary fix)

**Location**: Line 345
**Change**:
```csharp
// Before:
if (targetSceneRef != null)

// After:
if (targetSceneRef != null && targetSceneRef.RuntimeKeyIsValid())
```

**Rationale**: Prevent invalid AssetReference from reaching LoadAssetAsync

**Level 2: AddressablesAssetManager.cs** (defensive fix)

**Location**: Lines 41-43
**Change**:
```csharp
// Before:
if (key is AssetReference assetRef)
{
    handle = Addressables.LoadAssetAsync<T>(assetRef);
}

// After:
if (key is AssetReference assetRef)
{
    if (!assetRef.RuntimeKeyIsValid())
    {
        Debug.LogWarning($"AddressablesAssetManager: AssetReference has invalid RuntimeKey");
        return Task.FromResult<T>(null);
    }
    handle = Addressables.LoadAssetAsync<T>(assetRef);
}
```

**Rationale**: Defense in depth - prevent any invalid AssetReference from reaching Addressables API

### Conceptual Fix

```csharp
// DialogueSystem.cs, CompleteDialogue() method
private async void CompleteDialogue()
{
    // ... existing code ...

    // If no rule matched, check for linear nextScene
    if (targetSceneRef == null && _currentScene.NextScene != null)
    {
        Debug.Log($"DialogueSystem: Using nextScene for linear progression");
        targetSceneRef = _currentScene.NextScene;
    }

    // ✅ FIX: Validate AssetReference before attempting load
    if (targetSceneRef != null && targetSceneRef.RuntimeKeyIsValid())
    {
        Debug.Log($"DialogueSystem: Loading target scene...");
        
        try
        {
            var nextScene = await _assetManager.LoadAssetAsync<SceneData>(targetSceneRef);
            
            if (nextScene != null)
            {
                Debug.Log($"DialogueSystem: ✓ Successfully loaded next scene: {nextScene.SceneName}");
                _isPlaying = false;
                OnSceneNavigationRequested?.Invoke(nextScene);
            }
            else
            {
                Debug.LogError($"DialogueSystem: ✗ Failed to load target scene (returned null)");
                _isPlaying = false;
                OnDialogueComplete?.Invoke();
                // TODO: Show EndOfStoryPanel
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"DialogueSystem: ✗ Exception loading target scene: {ex.Message}");
            _isPlaying = false;
            OnDialogueComplete?.Invoke();
            // TODO: Show EndOfStoryPanel
        }
    }
    else
    {
        // ✅ FIX: This path now includes invalid AssetReference case
        if (targetSceneRef != null && !targetSceneRef.RuntimeKeyIsValid())
        {
            Debug.LogWarning("DialogueSystem: NextScene AssetReference is invalid (empty or missing asset)");
        }
        
        // No valid target scene determined, complete dialogue normally
        Debug.Log("DialogueSystem: No target scene determined, dialogue ending normally");
        _isPlaying = false;
        OnDialogueComplete?.Invoke();
        // TODO: Show EndOfStoryPanel (user requirement: "немодальное окно о том, что сюжет закончился")
    }
}
```

### Additional Fix: EndOfStoryPanel UI

**Note**: This is required by spec (FR-005) but is a **separate feature**, not part of the InvalidKeyException fix.

**Location**: New file `Assets/Scripts/NovelCore/Runtime/UI/EndOfStoryPanel/EndOfStoryPanel.cs`

**Purpose**: Show non-modal notification when story completes (no next scene)

**Integration**: Subscribe to `OnDialogueComplete` event and check if navigation occurred

## Constitution Compliance

After fix:

- ✅ **Principle VI (Defensive Programming)**: AssetReference validated before use
- ✅ **Principle VI (Testing)**: Edge case test added for invalid AssetReference
- ✅ **Code Style**: Defensive pattern aligns with existing null-check conventions
- ✅ **FR-005 (Spec)**: Graceful handling of end-of-story scenario (after EndOfStoryPanel implementation)

## Summary

**Root Cause**: Missing `RuntimeKeyIsValid()` check for AssetReference before attempting Addressables load

**Fix Complexity**: LOW (2 lines of code change in DialogueSystem.cs)

**Secondary Feature**: EndOfStoryPanel UI (spec requirement, moderate complexity)

**Test Coverage**: Add regression test for empty AssetReference scenario
