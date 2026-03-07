# Research: Scene Transition Mechanics - Current Implementation Analysis

**Date**: 2026-03-07  
**Status**: Phase 0 Complete  
**Purpose**: Document current implementation gaps and technical constraints

## Executive Summary

**Critical Gap Identified**: Scenes without choices currently have no mechanism to progress to the next scene. When `DialogueSystem.CompleteDialogue()` is called and no choices exist, the dialogue simply ends with no transition logic.

**Impact**: This breaks the fundamental flow of visual novels, making it impossible to create linear story progressions without manually adding choices to every scene.

## Current Implementation Analysis

### 1. DialogueSystem.CompleteDialogue() Behavior

**File**: `novel-core/Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`

**Current Logic** (lines 285-290):
```csharp
private void CompleteDialogue()
{
    Debug.Log("DialogueSystem: Dialogue complete");
    _isPlaying = false;
    OnDialogueComplete?.Invoke();
}
```

**Call Stack Analysis**:
- Called from: `AdvanceDialogue()` when dialogue exhausted and no choices exist
- Effect: Sets `_isPlaying = false` and fires `OnDialogueComplete` event
- **Gap**: No logic to check for or load a next scene

**Event Subscribers** (OnDialogueComplete):
- `DialogueBoxController.OnDialogueComplete()` - hides UI
- `ChoiceUIController.OnDialogueComplete()` - hides choice panel
- **None** trigger scene transitions

### 2. SceneManager.LoadScene() Integration

**File**: `novel-core/Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/SceneManager.cs`

**Current Callers**:
1. `GameStarter.StartGame()` - loads initial scene
2. `DialogueSystem.SelectChoice()` - loads scene from choice's targetScene

**Signature**:
```csharp
public async void LoadScene(SceneData sceneData)
```

**Implementation** (lines 69-138):
- Uses Addressables to load background/characters
- Plays background music with fade
- Applies scene transition (Fade/Cut/Slide)
- Fires events: `OnSceneLoadStarted`, `OnSceneTransitionComplete`, `OnSceneLoadComplete`

**Gap**: No concept of "navigation context" - cannot restore previous scene state or track history

### 3. SaveSystem Structure

**File**: `novel-core/Assets/Scripts/NovelCore/Runtime/Core/SaveSystem/SaveSystem.cs`

**Current Save Data**:
```csharp
// Inferred from usage - no direct SaveData class found in codebase
// Uses JsonUtility for serialization
```

**Extension Points**:
- Save/Load methods exist
- Uses JSON serialization via JsonUtility
- No navigation state currently saved

**Integration Point**: Need to add `SceneNavigationState` field to save data

### 4. Addressables Async Loading

**File**: `novel-core/Assets/Scripts/NovelCore/Runtime/Core/AssetManagement/AddressablesAssetManager.cs`

**Method**: `LoadAssetAsync<T>(AssetReference reference)`

**Behavior**:
- Returns `Task<T>` for async loading
- Can load multiple assets concurrently
- Memory managed by Addressables system

**Preloading Strategy**:
- Call `LoadAssetAsync()` for next scene's assets during current scene
- Assets remain in memory until explicitly released
- No blocking on main thread

**Memory Considerations**:
- Background: ~2-5MB per scene
- Characters: ~1-3MB per sprite
- Total preload budget: ~20MB safe for mobile
- Can preload 3-5 scenes without exceeding budget

## Scene Data Structure

**Current SceneData Fields**:
```csharp
- _sceneId: string
- _sceneName: string
- _backgroundImage: AssetReference
- _backgroundMusic: AssetReference
- _characters: List<CharacterPlacement>
- _dialogueLines: List<DialogueLineData>
- _choices: List<ChoiceData>
- _transitionType: TransitionType
- _transitionDuration: float
- _autoAdvance: bool
- _autoAdvanceDelay: float
```

**Missing**:
- `_nextScene`: AssetReference (for linear progression)
- `_transitionRules`: List<SceneTransitionRule> (for conditional logic)

## Circular Reference Detection

**Algorithm**: Depth-First Search (DFS) with visited set

**Implementation Strategy**:
```csharp
HashSet<string> visited = new();
HashSet<string> recursionStack = new();

bool HasCircularReference(SceneData scene, HashSet<string> visited, HashSet<string> stack)
{
    if (stack.Contains(scene.SceneId)) return true; // Circular!
    if (visited.Contains(scene.SceneId)) return false; // Already checked
    
    visited.Add(scene.SceneId);
    stack.Add(scene.SceneId);
    
    // Check nextScene
    if (scene.NextScene != null)
    {
        var next = LoadScene(scene.NextScene);
        if (HasCircularReference(next, visited, stack)) return true;
    }
    
    // Check choice targets
    foreach (var choice in scene.Choices)
    {
        foreach (var option in choice.Options)
        {
            if (option.targetScene != null)
            {
                var target = LoadScene(option.targetScene);
                if (HasCircularReference(target, visited, stack)) return true;
            }
        }
    }
    
    stack.Remove(scene.SceneId);
    return false;
}
```

**Performance**: O(V + E) where V = scenes, E = transitions  
**Runs**: Editor validation on SceneData save  
**Fallback**: Runtime max depth check (e.g., 100 scenes)

## Technical Decisions

### 1. Scene History Implementation

**Choice**: Stack-based with List<SceneHistoryEntry>  
**Rationale**: 
- Simple LIFO semantics for back/forward
- List allows indexed access for forward navigation
- Memory-efficient (store references, not full scene data)

**Alternative Considered**: Linked list  
**Rejected**: More complex, no performance benefit for our use case

### 2. Conditional Transition Syntax

**Choice**: Simple flag/variable checks only (v1)  
**Syntax**: `"flagName == true"`, `"variableNa me >= 5"`  
**Rationale**:
- Covers 90% of use cases
- Simple to parse and evaluate
- No external expression library needed

**Future**: Can add AND/OR logic in v2 if needed

### 3. Preloading Strategy

**Choice**: Preload all possible next scenes  
**Rationale**:
- Visual novel assets are small (~5MB per scene)
- Mobile memory budget allows 3-5 preloaded scenes
- Eliminates worst-case transition delay

**Alternative Considered**: Preload only most likely scene  
**Rejected**: Hard to predict player choices, not worth the complexity

### 4. Navigation History Snapshot

**What to Store**:
- Scene reference (AssetReference or sceneId)
- Dialogue line index
- Game state flags/variables

**What to Skip**:
- Character positions (can be recomputed from scene data)
- Animation state (reset on scene load)
- UI state (transient)

**Rationale**: Balance between state fidelity and memory usage

## Implementation Constraints

### From Constitution

1. **No .meta file modification**: All changes must be in `.cs` files only
2. **EditMode tests preferred**: Faster execution, no PlayMode overhead
3. **Call stack analysis mandatory**: Must analyze all callers before modifying methods
4. **Test-first approach**: Tests must fail before implementation

### Technical Constraints

1. **Unity 2022.3 LTS**: Cannot use C# 10+ features
2. **Addressables 1.21+**: Must use compatible async patterns
3. **VContainer 1.14+**: All services must use constructor injection
4. **IL2CPP**: Avoid reflection where possible (serialize with JsonUtility)

### Performance Constraints

1. **60 FPS**: Scene transition logic must complete in <16ms
2. **<1s transitions**: Total scene load time including assets
3. **512MB mobile RAM**: History capped at 50 entries (~5MB max)

## Next Steps

**Phase 1 (MVP)**:
1. Add `_nextScene` field to SceneData
2. Modify `DialogueSystem.CompleteDialogue()` to check nextScene
3. Update SceneData.Validate() for nextScene validation
4. Create editor UI for nextScene field

**Phase 2 (Navigation)**:
1. Implement SceneNavigationHistory stack
2. Add NavigateBack/NavigateForward to SceneManager
3. Integrate with SaveSystem

**Phase 3 (Conditional)**:
1. Create GameStateManager for flags/variables
2. Add SceneTransitionRule support
3. Implement condition evaluator

**Phase 4 (Optimization)**:
1. Add PreloadSceneAsync() to SceneManager
2. Trigger preload at scene start
3. Profile memory and performance

---

**Research Status**: ✅ Complete  
**Critical Findings**: 
- No automatic scene transition mechanism exists
- SaveSystem extensible for navigation state
- Addressables supports efficient preloading
- Circular reference detection straightforward with DFS

**Ready for**: Implementation Phase (User Story 1)
