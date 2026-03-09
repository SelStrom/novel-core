# Data Model: Scene Transition Mechanics

**Date**: 2026-03-07  
**Status**: Complete  
**Purpose**: Define data structures and their relationships for scene transition system

## Core Data Structures

### SceneData (Extended)

**File**: `Runtime/Data/Scenes/SceneData.cs`

**New Fields**:
```csharp
[SerializeField]
[Tooltip("Next scene to load after dialogue completes (for linear progression). Ignored if choices are present.")]
private AssetReference _nextScene;
```

**Properties**:
```csharp
public AssetReference NextScene => _nextScene;
```

**Purpose**: Enable linear scene progression by specifying the next scene to load when dialogue completes without choices.

**Validation Rules**:
- nextScene can be null (scene ends without progression)
- If both choices and nextScene are defined, choices take priority
- nextScene reference should be valid if set (checked via RuntimeKeyIsValid())

---

### SceneHistoryEntry (Future - US3)

**File**: `Runtime/Data/Scenes/SceneHistoryEntry.cs` (to be created)

```csharp
[Serializable]
public class SceneHistoryEntry
{
    public string sceneId;
    public int dialogueLineIndex;
    public Dictionary<string, object> gameStateSnapshot;
}
```

**Purpose**: Store scene state for navigation history (back/forward).

---

### SceneNavigationState (Future - US3)

**File**: `Runtime/Data/Scenes/SceneNavigationState.cs` (to be created)

```csharp
[Serializable]
public class SceneNavigationState
{
    public List<SceneHistoryEntry> history;
    public int currentIndex;
    public int maxHistorySize = 50;
}
```

**Purpose**: Serializable state for save/load of scene navigation history.

---

### SceneTransitionRule (Future - US4)

**File**: `Runtime/Data/Scenes/SceneTransitionRule.cs` (to be created)

```csharp
[Serializable]
public class SceneTransitionRule
{
    public int priority;
    public string conditionExpression;
    public AssetReference targetScene;
}
```

**Purpose**: Define conditional scene transitions based on game state.

**Fields**:
- `priority`: Order of evaluation (higher = checked first)
- `conditionExpression`: Simple condition like "flagName == true"
- `targetScene`: Scene to load if condition matches

---

### SceneTransitionContext (Future - US5)

**File**: `Runtime/Core/SceneManagement/SceneTransitionContext.cs` (to be created)

```csharp
public class SceneTransitionContext
{
    public SceneData SourceScene { get; set; }
    public SceneData TargetScene { get; set; }
    public TransitionType TransitionType { get; set; }
    public List<Object> PreloadedAssets { get; set; }
    public bool IsComplete { get; set; }
}
```

**Purpose**: Track ongoing scene transition state for preloading and lifecycle management.

---

## Relationships

### Scene → Next Scene (Linear Progression)

```
SceneData
    ├── NextScene: AssetReference → SceneData (nullable)
    ├── Choices: List<ChoiceData>
    └── TransitionRules: List<SceneTransitionRule> (future)
```

**Logic**:
1. If dialogue exhausted AND Choices.Count > 0 → Show choices (existing)
2. Else if TransitionRules.Count > 0 → Evaluate rules (future US4)
3. Else if NextScene != null → Load NextScene (US1 - implemented)
4. Else → CompleteDialogue() with no transition

### Scene Graph Structure

```
Scene01_Introduction
    ↓ nextScene
Scene02_ChoicePoint
    ↓ choice.Option[0].targetScene
Scene03a_PathA
    ↓ nextScene
Scene04_Continuation
    ↓ conditional rule: if hasFlag("ending_unlocked")
Scene05a_GoodEnding
    OR
Scene05b_NormalEnding
```

### Navigation History Structure

```
SceneNavigationHistory
    └── Stack<SceneHistoryEntry>
        ├── Entry: Scene01 (dialogueIndex: 0)
        ├── Entry: Scene02 (dialogueIndex: 2)
        └── Entry: Scene03a (dialogueIndex: 1) ← Current
```

**Operations**:
- `Push()`: Add new scene when navigating forward
- `Pop()`: Return to previous scene (back button)
- `Clear()`: Reset history (new game)

---

## State Machine: Scene Transition

```
[Playing Dialogue]
    │
    ├─ Advance → Next line exists? → Continue playing
    │
    └─ Advance → No more lines?
           │
           ├─ Has Choices? → [Waiting for Choice]
           │                     │
           │                     └─ Choice selected → Load targetScene
           │
           ├─ Has Transition Rules? (future) → Evaluate → Load matched scene
           │
           ├─ Has NextScene? → Load NextScene
           │
           └─ None → [Dialogue Complete]
```

---

## Memory Considerations

### Estimated Memory Usage

**Per Scene**:
- SceneData: ~1KB (metadata only)
- DialogueLines: ~500B per line (text + metadata)
- Choices: ~300B per choice

**History Entry**:
- SceneHistoryEntry: ~2KB (scene ref + snapshot)
- Max 50 entries: ~100KB total (negligible)

**Preloaded Assets** (future US5):
- Background: ~2-5MB
- Characters: ~1-3MB per sprite
- Budget: ~20MB safe for 3-5 scenes

**Total for 100-scene story**:
- Scene metadata: ~100KB
- Current scene assets: ~10MB
- Preloaded assets: ~20MB
- History: ~100KB
- **Total: ~30MB** (well within budget)

---

## Serialization Strategy

### JSON via JsonUtility

**SceneNavigationState**:
```json
{
  "history": [
    {
      "sceneId": "scene_intro_001",
      "dialogueLineIndex": 0,
      "gameStateSnapshot": {"flag_metCharacter": true}
    }
  ],
  "currentIndex": 0,
  "maxHistorySize": 50
}
```

**Advantages**:
- Unity's built-in serializer
- No external dependencies
- Fast serialization
- IL2CPP compatible

**Limitations**:
- No polymorphism support
- Dictionary serialization requires custom converter

---

## Validation Rules

### Editor Time

- nextScene reference must be valid (if set)
- No circular references (Scene A → Scene B → Scene A)
- Choice targetScene must be valid
- Transition rule priority must be unique per scene

### Runtime

- Max scene transition depth: 100 (prevent infinite loops)
- History size limit: 50 entries (prevent memory growth)
- Asset preload limit: 5 scenes max (prevent memory overflow)

---

## Future Extensions (Post-MVP)

### Conditional Transitions (US4)

```csharp
SceneData.TransitionRules = [
    { priority: 1, condition: "hasFlag('ending_unlocked')", target: SceneEnding },
    { priority: 2, condition: "chapterNumber >= 3", target: SceneChapter3 }
]
```

### Scene Preloading (US5)

```csharp
// Trigger preload at scene start
OnSceneStart() {
    if (currentScene.NextScene != null) {
        sceneManager.PreloadSceneAsync(currentScene.NextScene);
    }
}
```

### Navigation History (US3)

```csharp
// Track history on every scene transition
OnSceneLoad(newScene) {
    history.Push(new SceneHistoryEntry {
        sceneId = currentScene.SceneId,
        dialogueIndex = currentLineIndex,
        gameState = gameStateManager.CreateSnapshot()
    });
}
```

---

**Data Model Status**: ✅ Phase 1 Complete (Linear Progression)  
**Next Phase**: US2 (Choice Validation), US3 (Navigation History)
