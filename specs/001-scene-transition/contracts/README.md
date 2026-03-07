# Interface Contracts: Scene Transition System

**Date**: 2026-03-07  
**Purpose**: Define API contracts for scene transition interfaces

---

## IDialogueSystem (Extended)

**File**: `Runtime/Core/DialogueSystem/IDialogueSystem.cs`

### Existing Events

```csharp
public interface IDialogueSystem
{
    // Scene navigation events
    event Action<SceneData> OnSceneNavigationRequested;
    event Action OnDialogueComplete;
    event Action<ChoiceData> OnChoicePointReached;
    event Action<DialogueLineData> OnDialogueLineDisplayed;
    
    // ... other members
}
```

### Event Contracts

**OnSceneNavigationRequested**:
- **When**: Fired when nextScene should be loaded (from CompleteDialogue or SelectChoice)
- **Payload**: SceneData to load
- **Subscribers**: SceneManager (listens and calls LoadScene)
- **Guarantees**: SceneData is never null, already validated

**OnDialogueComplete**:
- **When**: Fired when dialogue completes with no nextScene
- **Payload**: None
- **Subscribers**: UI components (DialogueBoxController, ChoiceUIController)
- **Guarantees**: All dialogue exhausted, no choices, no nextScene

**OnChoicePointReached**:
- **When**: Fired when dialogue exhausts and choices exist
- **Payload**: ChoiceData to display
- **Subscribers**: ChoiceUIController (displays choice buttons)
- **Guarantees**: ChoiceData validated, has at least 1 option

---

## ISceneManager (Future Extensions)

**File**: `Runtime/Core/SceneManagement/ISceneManager.cs`

### Planned Methods (US3, US5)

```csharp
public interface ISceneManager
{
    // Existing
    void LoadScene(SceneData sceneData);
    void UnloadCurrentScene();
    event Action<SceneData> OnSceneLoadStarted;
    event Action<SceneData> OnSceneLoadComplete;
    
    // US3: Navigation History
    void NavigateBack();
    void NavigateForward();
    bool CanNavigateBack();
    bool CanNavigateForward();
    
    // US5: Preloading
    Task PreloadSceneAsync(AssetReference sceneRef);
    event Action<SceneData> OnScenePreloadStarted;
    event Action<SceneData> OnScenePreloadComplete;
}
```

### Method Contracts (Future)

**NavigateBack()**:
- **Preconditions**: CanNavigateBack() == true
- **Behavior**: Pops history stack, loads previous scene
- **Postconditions**: Current scene is previous scene, dialogue restored to saved index
- **Throws**: InvalidOperationException if no history exists

**PreloadSceneAsync(AssetReference)**:
- **Preconditions**: sceneRef.RuntimeKeyIsValid() == true
- **Behavior**: Asynchronously loads scene assets without displaying
- **Postconditions**: Assets in memory, ready for instant transition
- **Returns**: Task completing when preload finishes
- **Throws**: ArgumentException if invalid reference

---

## ISceneNavigationHistory (US3)

**File**: `Runtime/Core/SceneManagement/ISceneNavigationHistory.cs`

```csharp
public interface ISceneNavigationHistory
{
    // Stack operations
    void Push(SceneHistoryEntry entry);
    SceneHistoryEntry Pop();
    SceneHistoryEntry Peek();
    void Clear();
    
    // Navigation queries
    bool CanNavigateBack();
    bool CanNavigateForward();
    int Count { get; }
    int MaxSize { get; set; }
}
```

### Contract Details

**Push(SceneHistoryEntry)**:
- **Preconditions**: entry != null, entry.sceneId valid
- **Behavior**: Adds entry to history stack, enforces max size limit
- **Postconditions**: Count incremented (or oldest entry removed if at max)
- **Throws**: ArgumentNullException if entry is null

**Pop()**:
- **Preconditions**: Count > 0
- **Behavior**: Removes and returns most recent entry
- **Postconditions**: Count decremented
- **Returns**: Most recent SceneHistoryEntry
- **Throws**: InvalidOperationException if stack empty

**MaxSize**:
- **Default**: 50 entries
- **Range**: 1-100
- **Behavior**: When set, immediately trims stack if exceeds new size

---

## IGameStateManager (US4)

**File**: `Runtime/Data/GameState/IGameStateManager.cs`

```csharp
public interface IGameStateManager
{
    // Flag operations
    void SetFlag(string key, bool value);
    bool GetFlag(string key, bool defaultValue = false);
    bool HasFlag(string key);
    
    // Variable operations
    void SetVariable(string key, int value);
    int GetVariable(string key, int defaultValue = 0);
    bool HasVariable(string key);
    
    // Condition evaluation
    bool EvaluateCondition(string expression);
    
    // State management
    Dictionary<string, object> CreateSnapshot();
    void RestoreSnapshot(Dictionary<string, object> snapshot);
    void Clear();
}
```

### Contract Details

**EvaluateCondition(string)**:
- **Preconditions**: expression is valid syntax (e.g., "flagName == true")
- **Behavior**: Parses and evaluates expression against current state
- **Returns**: true if condition matches, false otherwise
- **Throws**: ArgumentException if invalid syntax
- **Supported Syntax**: 
  - `"flagName == true"` (flag checks)
  - `"variableName >= 5"` (variable comparisons)
  - Operators: `==`, `!=`, `>`, `<`, `>=`, `<=`

**CreateSnapshot()**:
- **Preconditions**: None
- **Behavior**: Creates deep copy of current state
- **Returns**: Dictionary with all flags/variables
- **Postconditions**: Original state unchanged
- **Thread Safety**: Safe to call from any thread

---

## Event Ordering Guarantees

### Scene Transition Lifecycle

```
1. OnSceneNavigationRequested (DialogueSystem → SceneManager)
2. OnSceneLoadStarted (SceneManager)
3. [Asset Loading Phase]
4. OnSceneTransitionStart (SceneManager)
5. [Transition Animation]
6. OnSceneTransitionComplete (SceneManager)
7. OnSceneLoadComplete (SceneManager)
8. DialogueSystem.StartScene(newScene)
9. OnDialogueLineDisplayed (first line)
```

**Guarantees**:
- Events fire in this exact order
- No events skipped (all fire or none fire)
- If loading fails, OnSceneLoadComplete still fires (with error logged)

### Choice Selection Lifecycle

```
1. OnChoicePointReached (DialogueSystem → ChoiceUIController)
2. [Player Interaction]
3. SelectChoice(index) called
4. [Choice history tracked]
5. OnSceneNavigationRequested (if targetScene defined)
6. ... or OnDialogueComplete (if no targetScene)
```

---

## Thread Safety

### Main Thread Only

All interface methods MUST be called from Unity main thread:
- IDialogueSystem methods
- ISceneManager methods
- IGameStateManager methods (except CreateSnapshot)

### Async Methods

Methods returning Task are safe to await from any context:
- `PreloadSceneAsync()` - returns Task, safe to await
- `LoadAssetAsync()` - returns Task, safe to await

---

## Error Handling Contracts

### Validation Failures

**Invalid Scene Reference**:
- **When**: nextScene or targetScene points to missing asset
- **Behavior**: Log error, fire OnDialogueComplete (no crash)
- **User Impact**: Dialogue ends gracefully, no navigation

**Circular References**:
- **When**: Scene A → Scene B → Scene A detected
- **Behavior**: Editor validation prevents save, runtime depth limit enforced
- **Runtime**: Max depth 100, then force OnDialogueComplete

### Asset Loading Failures

**Background Load Failure**:
- **When**: AssetReference.LoadAssetAsync fails
- **Behavior**: Log error, continue with null fallback
- **User Impact**: Scene loads without background (or uses fallback)

**Complete Scene Load Failure**:
- **When**: Target SceneData fails to load
- **Behavior**: Log error, stay on current scene, show error UI
- **User Impact**: Can retry or return to menu

---

## Performance Contracts

### Timing Guarantees

- `LoadScene()` completes in <1 second (target hardware)
- `AdvanceDialogue()` completes in <16ms (60 FPS)
- `EvaluateCondition()` completes in <1ms (simple expressions)
- `NavigateBack()` completes in <100ms (history lookup + scene load)

### Memory Guarantees

- History size limited to 50 entries (~100KB)
- Preloaded scenes limited to 5 (~20MB)
- No memory leaks on scene transitions (verified via profiler)

---

**Contracts Status**: ✅ Defined for US1-US2, Planned for US3-US5  
**Implementation**: US1 (Complete), US2 (Validation), US3-US5 (Future)
