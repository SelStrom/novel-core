# Data Model: Visual Novel Constructor

**Feature**: Visual Novel Constructor  
**Date**: 2026-03-06  
**Status**: Design Complete

## Overview

This document defines the data structures for the visual novel constructor, derived from the functional requirements in [spec.md](./spec.md). All entities are implemented as ScriptableObjects for Unity integration and Addressables compatibility.

## Core Entities

### 1. ProjectConfig

Represents a complete visual novel project with metadata and global settings.

**Fields**:
- `projectId`: string (unique GUID)
- `projectName`: string (display name)
- `author`: string (creator name)
- `description`: string (project summary)
- `version`: string (semantic version, e.g., "1.0.0")
- `createdDate`: DateTime
- `lastModifiedDate`: DateTime
- `startingScene`: AssetReference<SceneData> (entry point)
- `targetPlatforms`: PlatformFlags enum (Windows, macOS, iOS, Android)
- `defaultLocale`: string (ISO code, e.g., "en-US")
- `supportedLocales`: List<string> (enabled localizations)

**Relationships**:
- Has many `SceneData` (all scenes in project)
- Has many `CharacterData` (all characters in project)
- References `BuildConfig` for platform-specific settings

**Validation Rules**:
- `projectId` must be unique across all projects
- `startingScene` must reference a valid SceneData asset
- At least one `targetPlatform` must be selected
- `projectName` required, max 100 characters

**State Transitions**:
- Draft → In Progress → Testing → Published

---

### 2. SceneData

A single location/moment in the story with visual and narrative content.

**Fields**:
- `sceneId`: string (unique within project)
- `sceneName`: string (display name)
- `backgroundImage`: AssetReference<Sprite> (fullscreen background)
- `backgroundMusic`: AssetReference<AudioClip> (looping music)
- `characters`: List<CharacterPlacement> (characters in scene)
- `dialogueLines`: List<DialogueLineData> (ordered conversation)
- `choices`: List<ChoiceData> (branching points)
- `sceneTransition`: TransitionType enum (Fade, Slide, Cut, Custom)
- `transitionDuration`: float (seconds)
- `nextScene`: AssetReference<SceneData> (linear progression target, optional)
- `transitionRules`: List<SceneTransitionRule> (conditional transitions, evaluated before nextScene)
- `autoAdvance`: bool (skip wait for input)
- `autoAdvanceDelay`: float (seconds between lines if autoAdvance)

**Relationships**:
- Belongs to `ProjectConfig`
- Has many `DialogueLineData` (scene content)
- Has many `ChoiceData` (branching points)
- References `CharacterData` via `CharacterPlacement`

**Validation Rules**:
- `sceneId` unique within project
- `backgroundImage` must resolve to valid Addressable
- If `autoAdvance` true, `autoAdvanceDelay` must be > 0
- Must have at least one `DialogueLineData` or one `ChoiceData`
- Circular scene references detected and warned
- If both `choices` and `nextScene` defined, choices take priority (nextScene ignored)
- If both `transitionRules` and `choices` defined, choices take priority
- Transition rules evaluated in priority order before `nextScene`

**State Transitions**:
- Created → Editing → Preview → Finalized

---

### 3. CharacterData

A story character with visual sprites and metadata.

**Fields**:
- `characterId`: string (unique within project)
- `characterName`: string (display name)
- `description`: string (bio/notes for creator)
- `defaultEmotion`: string (emotion key, e.g., "neutral")
- `emotions`: Dictionary<string, CharacterEmotion> (emotion → sprite mapping)
- `animationType`: AnimationType enum (Unity, Spine, Static)
- `spineDataAsset`: AssetReference<SkeletonDataAsset> (if Spine)
- `defaultScale`: Vector2 (character size multiplier)
- `voiceAudioSet`: AssetReference<VoiceSet> (optional voice clips)

**Relationships**:
- Belongs to `ProjectConfig`
- Used by `CharacterPlacement` in `SceneData`
- Referenced by `DialogueLineData` (speaker)

**Validation Rules**:
- `characterId` unique within project
- `emotions` dictionary must contain `defaultEmotion` key
- Each `CharacterEmotion` must have valid sprite or Spine skin
- If `animationType` is Spine, `spineDataAsset` required

**Nested Type: CharacterEmotion**:
```csharp
[Serializable]
public struct CharacterEmotion {
    public string emotionName;           // "happy", "sad", "angry"
    public AssetReference<Sprite> sprite; // Static image (if Unity Animator)
    public string spineSkin;              // Skin name (if Spine)
    public string spineAnimation;         // Animation name (if Spine)
}
```

**Nested Type: CharacterPlacement**:
```csharp
[Serializable]
public struct CharacterPlacement {
    public AssetReference<CharacterData> character;
    public Vector2 position;              // Normalized screen coordinates (0-1)
    public string initialEmotion;         // Starting emotion for scene
    public int sortingOrder;              // Z-order (higher = front)
}
```

---

### 4. DialogueLineData

A single piece of spoken/narrated text with metadata.

**Fields**:
- `lineId`: string (unique within scene)
- `speaker`: AssetReference<CharacterData> (null for narrator)
- `textKey`: string (Unity Localization table key)
- `emotion`: string (speaker's emotion during line)
- `voiceClip`: AssetReference<AudioClip> (optional voice acting)
- `soundEffect`: AssetReference<AudioClip> (SFX played with line)
- `displayDuration`: float (auto-advance time, -1 = wait for input)
- `characterAction`: CharacterAction enum (None, Enter, Exit, Move)
- `actionParameters`: string (JSON for action details)

**Relationships**:
- Belongs to `SceneData`
- References `CharacterData` (speaker)
- References Unity Localization table via `textKey`

**Validation Rules**:
- `lineId` unique within scene
- If `speaker` is null, displayed as narrator text (center screen)
- `emotion` must exist in `speaker.emotions` dictionary
- `textKey` must resolve to valid localization entry
- If `characterAction` != None, `actionParameters` required

**CharacterAction Examples**:
- `Enter`: `{"side": "left", "transition": "slideIn"}`
- `Exit`: `{"side": "right", "transition": "fadeOut"}`
- `Move`: `{"toPosition": {"x": 0.5, "y": 0.5}, "duration": 1.0}`

---

### 5. ChoiceData

A decision point offering 2-6 player options.

**Fields**:
- `choiceId`: string (unique within scene)
- `promptTextKey`: string (localization key for question, optional)
- `options`: List<ChoiceOption> (2-6 choices)
- `timerSeconds`: float (0 = no timer, >0 = countdown)
- `defaultOptionIndex`: int (selected if timer expires)

**Relationships**:
- Belongs to `SceneData`
- Each `ChoiceOption` links to target `SceneData`

**Validation Rules**:
- `choiceId` unique within scene
- `options.Count` must be between 2 and 6
- Each `ChoiceOption.textKey` must resolve to localization
- Each `ChoiceOption.targetScene` must reference valid `SceneData`
- If `timerSeconds` > 0, `defaultOptionIndex` must be valid index

**Nested Type: ChoiceOption**:
```csharp
[Serializable]
public struct ChoiceOption {
    public string optionId;                         // Unique within choice
    public string textKey;                          // Localization key
    public AssetReference<SceneData> targetScene;   // Destination scene
    public List<ChoiceCondition> conditions;        // Conditions for availability
    public bool isAvailable;                        // Runtime availability (computed)
    public Sprite icon;                             // Optional icon
}
```

**Nested Type: ChoiceCondition**:
```csharp
[Serializable]
public struct ChoiceCondition {
    public ConditionType type;                      // RequireChoice, RequireVariable, RequireFlag
    public string requiredChoiceId;                 // Previous choice that must be selected (if type == RequireChoice)
    public int minimumOccurrences;                  // How many times choice must be made (default: 1)
    public string variableName;                     // Variable name to check (if type == RequireVariable)
    public ComparisonOperator comparison;           // GreaterThan, LessThan, Equal, NotEqual
    public int comparisonValue;                     // Value to compare against
}
```

**Enums**:
```csharp
public enum ConditionType {
    RequireChoice,      // Must have selected specific previous choice
    RequireVariable,    // Check game variable (e.g., affection >= 50)
    RequireFlag         // Check boolean flag (e.g., hasKey == true)
}

public enum ComparisonOperator {
    GreaterThan,        // >
    GreaterOrEqual,     // >=
    LessThan,           // <
    LessOrEqual,        // <=
    Equal,              // ==
    NotEqual            // !=
}
```

**Conditional Branching Evaluation**:

1. **At Choice Display Time**:
   - For each `ChoiceOption`, evaluate all `conditions`
   - If ALL conditions pass, set `isAvailable = true`
   - If ANY condition fails, set `isAvailable = false` (choice grayed out or hidden)

2. **Condition Evaluation Logic**:
   ```csharp
   bool EvaluateCondition(ChoiceCondition condition, SaveData saveData) {
       switch (condition.type) {
           case RequireChoice:
               int count = saveData.choiceHistory.Count(c => c == condition.requiredChoiceId);
               return count >= condition.minimumOccurrences;
           
           case RequireVariable:
               if (!saveData.variables.TryGetValue(condition.variableName, out var value)) 
                   return false;
               return CompareValues((int)value, condition.comparison, condition.comparisonValue);
           
           case RequireFlag:
               if (!saveData.variables.TryGetValue(condition.variableName, out var flag)) 
                   return false;
               return (bool)flag == (condition.comparisonValue == 1);
       }
   }
   ```

3. **Multiple Conditions**:
   - All conditions must pass (AND logic)
   - For OR logic, duplicate choice with different conditions

**Example Use Cases**:

- **Romance Route Requirement**:
  ```
  Condition: RequireChoice
  RequiredChoiceId: "choice_02_flirt_with_alice"
  MinimumOccurrences: 3
  → Choice only available if player flirted with Alice 3+ times
  ```

- **Affection Threshold**:
  ```
  Condition: RequireVariable
  VariableName: "alice_affection"
  Comparison: GreaterOrEqual
  ComparisonValue: 75
  → Choice only available if Alice's affection ≥ 75
  ```

- **Key Item Check**:
  ```
  Condition: RequireFlag
  VariableName: "hasKey"
  Comparison: Equal
  ComparisonValue: 1 (true)
  → Choice only available if player has the key
  ```

---

### 6. SaveData

Player progress snapshot with versioning.

**Fields**:
- `version`: string (save format version, e.g., "1.0.0")
- `saveId`: string (unique GUID)
- `slotIndex`: int (1-10 for manual saves, 0 for auto-save)
- `timestamp`: DateTime (save creation time)
- `projectId`: string (which visual novel)
- `currentSceneId`: string (active scene)
- `choiceHistory`: List<string> (ordered list of choiceId_optionId)
- `variables`: Dictionary<string, object> (custom state, e.g., affection points)
- `playTimeSeconds`: int (total gameplay time)
- `screenshotThumbnail`: string (base64 PNG, for save slot UI)

**Relationships**:
- Belongs to `ProjectConfig` (via `projectId`)
- References current `SceneData` (via `currentSceneId`)

**Validation Rules**:
- `version` must match current SaveData schema version
- `projectId` must exist and match loaded project
- `currentSceneId` must reference valid scene in project
- `slotIndex` 0 = auto-save (system managed), 1-10 = manual saves

**Serialization**:
- Serialized to JSON via `JsonUtility.ToJson()`
- Saved to platform-specific location:
  - Windows: `%AppData%/NovelCore/Saves/{projectId}/{saveId}.json`
  - macOS: `~/Library/Application Support/NovelCore/Saves/{projectId}/{saveId}.json`
  - iOS: `Application.persistentDataPath/Saves/{projectId}/{saveId}.json`
  - Android: `Application.persistentDataPath/Saves/{projectId}/{saveId}.json`

**Cloud Sync**:
- Steam: Saved to Steam Cloud via `SteamRemoteStorage` API
- iOS: Synced to iCloud via `NSUbiquitousKeyValueStore`
- Android: Synced to Google Play Saved Games via `PlayGamesPlatform`

**Migration**:
```csharp
public interface ISaveUpgrader {
    string FromVersion { get; }      // "1.0.0"
    string ToVersion { get; }        // "1.1.0"
    SaveData Upgrade(SaveData old);  // Transform old → new
}
```

---

### 7. BuildConfig

Platform-specific build settings.

**Fields**:
- `targetPlatform`: PlatformType enum (Windows, macOS, iOS, Android)
- `buildPath`: string (output directory)
- `companyName`: string (metadata)
- `productName`: string (app name)
- `version`: string (semantic version)
- `bundleIdentifier`: string (reverse DNS, e.g., "com.author.novelname")
- `scriptingBackend`: ScriptingImplementation (IL2CPP for all platforms)
- `compressionMethod`: Compression (LZ4 for dev, LZMA for release)
- `strippingLevel`: ManagedStrippingLevel (Low/Medium/High)
- `steamAppId`: uint (Steam app ID, if targeting Steam)
- `mobileProvisionProfile`: string (iOS signing, if targeting iOS)
- `keystorePath`: string (Android signing, if targeting Android)

**Relationships**:
- Belongs to `ProjectConfig`
- Referenced by BuildPipeline automation

**Validation Rules**:
- `targetPlatform` must be enabled in `ProjectConfig.targetPlatforms`
- `bundleIdentifier` must match platform requirements (reverse DNS)
- If `targetPlatform` is iOS, `mobileProvisionProfile` required
- If `targetPlatform` is Android, `keystorePath` required
- `steamAppId` required only if targeting Steam (Windows/macOS)

---

### 8. StoryFlowGraph

Visual representation of VN Scene connections and branching structure for the Story Flow window.

**Fields**:
- `nodes`: List<SceneNode> (all VN Scenes in project as graph nodes)
- `edges`: List<ChoiceEdge> (connections between scenes via choices)
- `layout`: LayoutAlgorithm enum (Hierarchical, ForceDirected, Manual)
- `rootSceneId`: string (starting scene, root of graph)

**Relationships**:
- Built from `ProjectConfig` by analyzing all `SceneData` and `ChoiceData` assets
- Read-only visualization (editing happens in Scene Editor, not graph)

**Validation Rules**:
- Graph must be connected (all scenes reachable from root)
- Warn if scenes have no path to ending (dead-end branches)
- Detect circular loops and warn if no exit path exists

**Nested Type: SceneNode**:
```csharp
[Serializable]
public struct SceneNode {
    public string sceneId;                    // SceneData.sceneId
    public string sceneName;                  // Display name
    public Vector2 position;                  // Graph coordinates (x, y)
    public bool isStartingScene;              // Root node indicator
    public bool isEnding;                     // Terminal node (no outgoing edges)
    public int incomingEdgeCount;             // How many choices lead here
    public int outgoingEdgeCount;             // How many choices from this scene
}
```

**Nested Type: ChoiceEdge**:
```csharp
[Serializable]
public struct ChoiceEdge {
    public string sourceSceneId;              // Origin scene
    public string targetSceneId;              // Destination scene
    public string choiceText;                 // Option text (localization key)
    public string choiceId;                   // ChoiceData.choiceId
    public Color edgeColor;                   // Visual indicator for editor
    public bool isConditional;                // Has requiredChoices conditions
}
```

**Layout Algorithms**:

- **Hierarchical** (default for <50 scenes):
  - Starting scene at top
  - Scenes arranged in levels by distance from root
  - Minimizes edge crossings
  - Best for linear stories with occasional branches

- **Force-Directed** (for 50-200 scenes):
  - Physics simulation (nodes repel, edges attract)
  - Auto-arranges complex graphs
  - Better for highly branched stories
  - Performance: 60 FPS up to 200 nodes

- **Manual** (user-positioned):
  - Preserves creator-defined positions
  - Saved in SceneNode.position
  - Fallback if automated layouts unclear

**Graph Rendering**:
- Unity GraphView API for node/edge visualization
- Zoom/pan controls (0.1x to 2.0x zoom range)
- Minimap for navigation (200+ scene projects)
- Click node → opens Scene Editor for that scene
- Right-click node → context menu (Delete, Duplicate, Set as Starting Scene)

**Performance Optimization**:
- Virtualization for >100 scenes (only render visible nodes)
- Edge batching (single draw call per edge type)
- LOD: Simplify node rendering at low zoom levels

---

## Supporting Data Types

### SceneTransitionRule

Conditional transition rule for scene navigation, evaluated in priority order.

**Fields**:
- `priority`: int (lower number = higher priority, e.g., 0 evaluated before 10)
- `conditionExpression`: string (condition to evaluate, e.g., "hasKey == true", "affinity >= 50")
- `targetScene`: AssetReference<SceneData> (scene to load if condition is met)
- `description`: string (optional human-readable description for debugging)

**Relationships**:
- Belongs to `SceneData` (as part of `transitionRules` list)
- References target `SceneData`

**Validation Rules**:
- `conditionExpression` must not be empty
- `targetScene` must reference valid SceneData asset
- Priority should be unique within scene (warning if duplicates)

**Evaluation**:
- Rules are sorted by priority (ascending) and evaluated in order
- First rule where condition evaluates to `true` determines target scene
- If no rules match, falls back to `SceneData.nextScene` (if set)
- Requires `IGameStateManager` for condition evaluation

**Condition Syntax**:
```
Simple expressions:
- "flagName == true"
- "flagName == false"
- "variableName >= value"
- "variableName <= value"
- "variableName == value"
```

---

### AssetMetadata

Tracks asset usage across project (for reference validation).

**Fields**:
- `assetGuid`: string (Unity GUID)
- `assetPath`: string (e.g., "Assets/Content/Backgrounds/forest.png")
- `assetType`: AssetType enum (Sprite, AudioClip, SkeletonDataAsset)
- `usageCount`: int (how many SceneData/DialogueLineData reference this)
- `addressableKey`: string (Addressables address)
- `fileSize`: long (bytes, for build size estimation)

**Validation**:
- Editor tool scans project and builds `AssetMetadata` cache
- Warns if `usageCount` = 0 (unused asset)
- Errors if referenced but `assetPath` invalid (missing asset)

---

## Entity Relationships Diagram

```
ProjectConfig
├── [1] StartingScene → SceneData (entry point)
├── [1..N] SceneData (all scenes in project)
│   ├── [0..N] CharacterPlacement → CharacterData
│   ├── [1..N] DialogueLineData → CharacterData (speaker)
│   ├── [0..N] ChoiceData
│   │   └── [2..6] ChoiceOption → SceneData (target)
│   ├── [0..1] NextScene → SceneData (linear progression)
│   └── [0..N] SceneTransitionRule → SceneData (conditional transitions)
├── [0..N] CharacterData
│   └── [1..N] CharacterEmotion (list)
├── [1..N] BuildConfig per platform
└── [0..N] SaveData (runtime, not in project asset)
    ├── projectId → ProjectConfig (which visual novel)
    └── currentSceneId → SceneData (active scene)
```

---

## Data Flow Examples

### Scene Playback Flow

1. Load `SceneData` from Addressables
2. Load `backgroundImage` and `backgroundMusic` assets
3. Instantiate `characters` from `CharacterPlacement` list
4. Iterate `dialogueLines`:
   - Resolve `textKey` from Localization table
   - Apply `emotion` to speaker's character sprite/animation
   - Play `voiceClip` and `soundEffect` if present
   - Wait for player input or `displayDuration`
5. If `choices` exist, display choice UI
6. Player selects choice → load `targetScene` and repeat

### Save/Load Flow

**Save**:
1. Capture current `SceneData.sceneId`
2. Serialize `choiceHistory` (all choices made so far)
3. Capture `variables` dictionary (custom state)
4. Record `playTimeSeconds` and `timestamp`
5. Generate `screenshotThumbnail` (render current frame to PNG)
6. Serialize to JSON, write to `Application.persistentDataPath`
7. If cloud sync enabled, upload to platform cloud storage

**Load**:
1. Deserialize JSON from `SaveData` file
2. Check `version`, run upgrade if needed (ISaveUpgrader chain)
3. Load `ProjectConfig` matching `saveData.projectId`
4. Load `SceneData` matching `saveData.currentSceneId`
5. Restore `variables` to runtime game state
6. Resume playback from loaded scene

---

## Schema Versioning

### Version 1.0.0 (Initial)

Current schema as documented above.

### Future Version Example (1.1.0)

If we add "character affection system":

**Changes**:
- Add `CharacterData.affinityThreshold: int`
- Add `SaveData.characterAffinity: Dictionary<string, int>`

**Upgrade**:
```csharp
public class SaveUpgrader_1_0_to_1_1 : ISaveUpgrader {
    public string FromVersion => "1.0.0";
    public string ToVersion => "1.1.0";
    
    public SaveData Upgrade(SaveData old) {
        old.version = "1.1.0";
        old.variables["characterAffinity"] = new Dictionary<string, int>();
        return old;
    }
}
```

---

## Addressables Configuration

### Asset Groups

- **Content_Backgrounds**: All background sprites
- **Content_Characters**: All character sprites/Spine assets
- **Content_Audio_Music**: Music tracks
- **Content_Audio_SFX**: Sound effects
- **Content_Localization**: Localization tables
- **Runtime_Prefabs**: UI prefabs, dialogue box templates
- **Runtime_Defaults**: Fallback assets (missing texture, error sounds)

### Loading Strategy

- **Synchronous**: UI prefabs, defaults (loaded at app start)
- **Asynchronous**: Scene backgrounds, characters (loaded per scene)
- **Streaming**: Music (streamed, not fully loaded into RAM)
- **Preloading**: Next scene assets loaded during dialogue (hide latency)

---

## Conclusion

All entities defined with clear validation rules and relationships. Data model supports all functional requirements from spec.md while maintaining constitution principles (asset integrity, modularity, performance). Ready to proceed to contracts definition (Phase 1 continued).
