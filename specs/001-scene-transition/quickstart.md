# Quickstart Guide: Scene Transition Mechanics

**Audience**: Content Creators (Writers, Game Designers)  
**Prerequisites**: Unity Editor, NovelCore project setup  
**Time**: 5-10 minutes

---

## 🎯 What You'll Learn

- How to create linear story progressions
- How to set up branching narratives with choices
- How to test scene transitions in editor

---

## Linear Scene Progression (Simple Stories)

### Step 1: Create Your First Scene

1. In Unity Project window, navigate to `Assets/Content/Projects/YourProject/Scenes/`
2. Right-click → Create → NovelCore → Scene Data
3. Name it `Scene01_Introduction`
4. Fill in:
   - **Scene ID**: `scene_intro_001` (auto-generated if empty)
   - **Scene Name**: `Introduction`
   - **Dialogue Lines**: Add your dialogue (click + button)

### Step 2: Create Your Second Scene

1. Create another Scene Data: `Scene02_Continue`
2. Fill in scene information
3. Add dialogue lines

### Step 3: Link Scenes Together

1. Select `Scene01_Introduction` in Project window
2. In Inspector, find **Scene Transition** section
3. Locate **Next Scene** field
4. Drag `Scene02_Continue` into the Next Scene field (or click circle to select)

✅ **Done!** When dialogue in Scene01 finishes, Scene02 will automatically load.

---

## Testing Your Linear Story

### Method 1: Scene Editor Preview (Quick Test)

1. Open Scene Editor window: `Window → NovelCore → Scene Editor`
2. Drag `Scene01_Introduction` into the Scene Editor
3. Click **Preview Scene** button
4. Click through dialogue
5. Verify Scene02 loads automatically

### Method 2: Play Mode (Full Test)

1. Open `SampleScene` Unity scene
2. Find `GameStarter` GameObject
3. In Inspector, set **Starting Scene** to `Scene01_Introduction`
4. Press Play ▶️
5. Game starts from Scene01, progresses to Scene02

---

## Branching Narratives (Stories with Choices)

### Step 1: Create Choice Point

1. Create `Scene02_ChoicePoint` with dialogue
2. In Inspector, find **Narrative Content** → **Choices**
3. Click + to add a ChoiceData asset
4. Fill in:
   - **Choice ID**: `choice_main_001`
   - **Prompt Text**: "What do you want to do?"

### Step 2: Add Choice Options

1. Expand the ChoiceData
2. Add Options (click + button twice for 2 options)
3. For **Option 1**:
   - **Option ID**: `option_go_outside`
   - **Fallback Text**: "Go outside"
   - **Target Scene**: Drag `Scene03a_Outside`
4. For **Option 2**:
   - **Option ID**: `option_stay_home`
   - **Fallback Text**: "Stay home"
   - **Target Scene**: Drag `Scene03b_Home`

### Step 3: Create Outcome Scenes

1. Create `Scene03a_Outside` with outcome dialogue
2. Create `Scene03b_Home` with outcome dialogue
3. Optionally link them to Scene04 via **Next Scene** to rejoin paths

---

## Understanding Priority Rules

### ⚠️ Important: Choices Always Win!

If a scene has **both choices and nextScene**:
- ✅ **Choices** take priority (player makes decision)
- ❌ **Next Scene** is ignored

**Example**:
```
Scene02_ChoicePoint
  ├── Choices: [Go Outside, Stay Home]  ← These are used
  └── Next Scene: Scene04              ← This is ignored!
```

The Inspector will show a **warning** if both are defined.

---

## Scene Flow Patterns

### Pattern 1: Pure Linear

```
Scene01 → Scene02 → Scene03 → Scene04 → Ending
   ↓         ↓         ↓         ↓         ↓
nextScene nextScene nextScene nextScene (none)
```

**Use For**: Kinetic novels, intro sequences, epilogues

---

### Pattern 2: Branching with Rejoin

```
Scene01 → Scene02_Choice
             ├─[Option A]→ Scene03a_PathA ┐
             └─[Option B]→ Scene03b_PathB ┘
                              ↓
                         Scene04_Rejoin → Ending
```

**Use For**: Short-term decision consequences, chapter structure

---

### Pattern 3: Diverging Endings

```
Scene01 → Scene02 → Scene03_Choice
                       ├─[Option A]→ Scene04a → EndingA
                       └─[Option B]→ Scene04b → EndingB
```

**Use For**: Multiple endings, significant story branches

---

## Debugging & Validation

### Inspector Warnings

The Inspector shows helpful messages:

**✓ Green**: "Linear progression enabled"
- Scene will auto-advance to nextScene
- Everything configured correctly

**⚠️ Yellow**: "Both choices and nextScene defined"
- Choices will be shown (nextScene ignored)
- Consider removing nextScene if not needed

**ℹ️ Blue**: "No nextScene or choices defined"
- Dialogue will end here
- Add nextScene or choices if you want progression

### Validate Scene Button

1. Select any SceneData in Project window
2. Scroll to bottom of Inspector
3. Click **Validate Scene** button
4. Check Console for validation results

---

## Common Issues & Solutions

### Issue: Scene doesn't transition

**Symptoms**: Dialogue ends, nothing happens

**Solutions**:
1. Check if **Next Scene** field is actually set (not empty)
2. Verify the referenced scene exists in Project
3. Check Console for error messages
4. Ensure scene has dialogue (empty scenes can't complete)

---

### Issue: Wrong scene loads

**Symptoms**: Different scene loads than expected

**Solutions**:
1. Check if scene has **Choices** defined (they override nextScene)
2. Verify the AssetReference points to correct scene
3. Check Scene ID matches in both scenes
4. Look for typos in scene names

---

### Issue: Circular references

**Symptoms**: Scenes loop infinitely (Scene A → Scene B → Scene A)

**Solutions**:
1. Remove circular nextScene references
2. Add an ending scene with no nextScene
3. Use Choices to create controlled loops if needed
4. Validation will warn about circular references

---

---

## Scene Navigation History (Back/Forward)

### Enable Player Navigation

The system automatically tracks scene history. Players can navigate back to previous scenes!

**Implementation**: Add navigation buttons to your UI

```csharp
// Example: In your custom UI script
public void OnBackButtonClick()
{
    if (_sceneManager.CanNavigateBack())
    {
        _sceneManager.NavigateBack();
    }
}

public void OnForwardButtonClick()
{
    if (_sceneManager.CanNavigateForward())
    {
        _sceneManager.NavigateForward();
    }
}
```

### How It Works

- **Automatic**: History tracked automatically on scene transitions
- **Memory**: Stores last 50 scenes visited
- **Save/Load**: History persists in save files
- **State Restoration**: Returns to exact dialogue line when navigating back

---

## Conditional Scene Transitions (Advanced)

### What Are Conditional Transitions?

Load different scenes based on game state (flags, choices made earlier).

**Example Use Cases**:
- "If player has key, show secret room; otherwise show locked door"
- "If romance level >= 5, show date scene; otherwise show friend scene"
- "If player chose [Path A] earlier, show consequence scene"

### Step 1: Set Game State Flags

In your dialogue scripts or custom code:

```csharp
// Set a flag (true/false)
_gameStateManager.SetFlag("hasKey", true);
_gameStateManager.SetFlag("metCharacter", true);

// Set a variable (integer)
_gameStateManager.SetVariable("romanceLevel", 5);
_gameStateManager.SetVariable("chapter", 2);
```

### Step 2: Add Transition Rules to Scene

1. Select your SceneData in Inspector
2. Find **Scene Transition** → **Transition Rules**
3. Add new rule (click + button)
4. Configure:
   - **Priority**: `0` (lower number = checked first)
   - **Condition Expression**: `"hasKey == true"`
   - **Target Scene**: Drag target scene (e.g., `SecretRoom`)

### Step 3: Add Fallback with Next Scene

1. In same scene, set **Next Scene** to default path (e.g., `LockedDoor`)
2. This loads if NO rules match

### Condition Expression Syntax

**Boolean flags:**
```
hasKey == true
metCharacter == false
completedTutorial != false
```

**Integer variables:**
```
chapter >= 2
health > 50
romanceLevel <= 3
score == 100
```

### Priority Order Example

```
Scene with multiple rules:
  Rule 1 [Priority: 0]: "hasKey == true" → SecretRoom
  Rule 2 [Priority: 1]: "chapter >= 2" → Chapter2Path
  Rule 3 [Priority: 2]: "score > 100" → BonusScene
  Next Scene: DefaultPath

Evaluation:
1. Check hasKey → If true, load SecretRoom (STOP)
2. Check chapter → If ≥2, load Chapter2Path (STOP)
3. Check score → If >100, load BonusScene (STOP)
4. No matches → Load DefaultPath
```

### Complete Priority Chain

```
When scene completes:
1. Has Choices? → Show choices, wait for selection
2. No choices? → Evaluate Transition Rules (by priority)
3. No matching rules? → Load Next Scene (if set)
4. No next scene? → End dialogue (show menu/credits)
```

---

## Testing Checklist

Before publishing your story:

- [ ] Test each scene individually in Scene Editor
- [ ] Play through entire story from start to finish in Play Mode
- [ ] Test all choice branches
- [ ] Test navigation back/forward through scenes
- [ ] Verify conditional transitions with different game states
- [ ] Verify all scenes have dialogue or choices
- [ ] Check Console for validation warnings
- [ ] Test scene transitions are smooth (<1 second)
- [ ] Verify no circular references exist
- [ ] Test save/load with navigation history
- [ ] Test conditional rules with various flag combinations

---

## Advanced: Scene Graph Visualization

For complex stories, visualize your scene flow:

```
Scene01_Introduction
    │
    ├─ nextScene ─> Scene02_Journey
    │                   │
    │                   ├─ choice[Go Left] ─> Scene03a_Forest
    │                   │                         │
    │                   │                         └─ nextScene ─> Scene05_Ending
    │                   │
    │                   └─ choice[Go Right] ─> Scene03b_Cave
    │                                              │
    │                                              └─ nextScene ─> Scene05_Ending
```

**Tool**: Use `Window → NovelCore → Scene Flow Visualizer` (future feature)

---

## Next Steps

1. **Create your first 3-scene linear story** to practice
2. **Add a choice with 2 options** for branching
3. **Test both paths** in Play Mode
4. **Experiment** with complex branching structures

**Need Help?**
- Check Console for error messages (always helpful!)
- Review scene validation warnings in Inspector
- Test scenes individually before combining

---

## Pattern Library: Common Scene Structures

### Hub-and-Spoke Model

```
MainHub (with choices)
  ├─[Explore Town] → TownScene → [Return] → MainHub
  ├─[Visit Shop] → ShopScene → [Return] → MainHub
  └─[Continue Story] → NextChapter
```

### State-Based Branching

```
Scene_MeetCharacter
  └─ Sets flag: metCharacter = true

Scene_Later (conditional)
  ├─ Rule[P0]: "metCharacter == true" → FriendlyDialogue
  └─ Next Scene (fallback): StrangerDialogue
```

### Chapter System

```
Chapter1_Start (sets: chapter = 1)
  → Chapter1_Middle
  → Chapter1_End

Chapter2_Start (sets: chapter = 2)
  └─ Rule[P0]: "chapter >= 2" → UnlockSpecialPath
  └─ Next Scene (fallback): NormalPath
```

---

## Performance Tips

### Best Practices

1. **Keep scene files small**: <2MB per scene asset
2. **Limit history**: System automatically caps at 50 scenes
3. **Test on target hardware**: Verify transitions <1 second
4. **Use validation**: Click Validate Scene before building

### Memory Management

- **Navigation History**: Max 50 scenes (oldest removed automatically)
- **Game State**: Unlimited flags/variables (stored in save file)
- **Asset References**: Use Addressables (automatic memory management)

---

**Quickstart Status**: ✅ Complete (All Features Covered)  
**Features**: Linear Progression, Choice Branching, Navigation History, Conditional Transitions  
**Documentation**: research.md, data-model.md, this file, contracts/  
**Next**: Build your first interactive story! 🎮
