# Manual Unity Editor Steps Required

**Project**: Visual Novel Constructor  
**Date**: 2026-03-07  
**Status**: MVP Development - Game Entry Point Setup

## Required Manual Steps

These tasks require Unity Editor operations. Some package management tasks MAY be automated via AI when explicitly requested by the user (per Constitution Principle VII updates), but manual verification in Unity Editor is still recommended.

### Step 0: Setup Game Entry Point (GameStarter)

**REQUIRED FOR GAME TO START**: Before testing any visual novel content, you must set up the game entry point.

**Open**: Unity Editor → Assets/Scenes/SampleScene.unity

**Create GameStarter GameObject**:

1. **Create GameObject**:
   - In Hierarchy window: Right Click → Create Empty
   - Rename to "GameStarter"

2. **Add GameStarter Component**:
   - Select "GameStarter" GameObject
   - Inspector → Add Component
   - Search for "Game Starter"
   - Click to add component

3. **Configure Starting Scene**:
   - In GameStarter component Inspector:
   - **Starting Scene** field: Drag `Scene01_Introduction.asset` from Project window
     - Location: `Assets/Content/Projects/Sample/Scenes/Scene01_Introduction.asset`
   - **Auto Start**: ☑ Enabled (check the box)
   - **Start Delay**: 0.5 seconds (default)

4. **Verify GameLifetimeScope Exists**:
   - Check Hierarchy for "GameLifetimeScope" GameObject
   - If missing:
     - Right Click → Create Empty
     - Rename to "GameLifetimeScope"
     - Add Component → "Game Lifetime Scope"

**Verification**:
- Press Play ▶️
- Console shows: "GameStarter: Starting game with scene: ..."
- Scene loads automatically after 0.5 seconds
- Dialogue appears on screen

**Troubleshooting**:
- "Starting scene is not assigned": Drag Scene01_Introduction.asset to Starting Scene field
- "DialogueSystem not injected": Verify GameLifetimeScope GameObject exists
- Nothing happens on Play: Check Auto Start is enabled, check Console for errors

**Detailed Guide**: See GAME_STARTER_SETUP.md for comprehensive setup instructions

---

### Step 1: Install Required Packages

**Open**: Unity Editor → Window → Package Manager

**Install the following packages**:

1. **Addressables** (version 2.0 or higher)
   - Search for "Addressables"
   - Click "Install"
   - Wait for installation to complete

2. **Localization** (version 2.0 or higher)
   - Search for "Localization"
   - Click "Install"
   - Wait for installation to complete

3. **VContainer** (version 1.14 or higher)
   - Method 1: Add via Git URL
     - Click "+" button → "Add package from git URL"
     - Enter: `https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.14.0`
   - Method 2: Download from GitHub and import as local package

**Verification**:
- Open Package Manager
- Verify all packages show "Installed" status
- Console should show no errors

### Step 2: Configure Platform Settings

**Open**: Edit → Project Settings → Player

**Configure Scripting Backend**:

All platforms MUST use **IL2CPP** scripting backend for cross-platform parity (Constitution Principle II).

1. **Windows Platform**:
   - Select "PC, Mac & Linux Standalone" icon
   - Go to "Other Settings" section
   - Find "Scripting Backend"
   - Set to: **IL2CPP**

2. **macOS Platform**:
   - Select "PC, Mac & Linux Standalone" icon (if not already selected)
   - Target: macOS
   - Scripting Backend: **IL2CPP**

3. **iOS Platform**:
   - Select "iOS" icon
   - Go to "Other Settings" section
   - Scripting Backend: **IL2CPP** (mandatory for iOS)

4. **Android Platform**:
   - Select "Android" icon  
   - Go to "Other Settings" section
   - Scripting Backend: **IL2CPP**

**Verification**:
- Check each platform's scripting backend setting
- Ensure ALL platforms = IL2CPP

**Rationale**: IL2CPP on all platforms ensures:
- Cross-platform parity (Constitution Principle II)
- Better performance (native code compilation)
- Consistent behavior during development and testing
- Early detection of IL2CPP-specific issues

### Step 3: Verify Unity Version

**Check**: Help → About Unity

**Expected**: Unity 6.x.x LTS

**If different version**: 
- Project was created with Unity 6
- Using older/newer version may cause compatibility issues
- Recommend using Unity 6 LTS

### Next Steps After Manual Setup

Once these manual steps are complete:
1. Reopen Unity project to ensure packages loaded
2. Check Console for any errors
3. Return to AI implementation
4. Continue with **ITERATION 1: Dependency Injection Infrastructure**

---

## Step 4: Generate UI Prefabs (Optional - Can be done later)

**When**: Before testing ITERATION 4 (Dialogue Display), ITERATION 9 (Choice UI), or ITERATION 22 (Save System)

**Method**: Use AI-generated Editor scripts to create prefabs programmatically

**Open**: Unity Editor → Top Menu → **NovelCore → Generate UI Prefabs**

**Available generators**:

1. **Generate All Prefabs** (Recommended - generates all 3 at once)
   - Creates: DialogueBox.prefab, ChoiceButton.prefab, SaveLoadUI.prefab
   - Location: `Assets/Resources/NovelCore/UI/`

2. **Dialogue Box** (Individual generator)
   - Creates DialogueBox prefab for dialogue display
   - Required for: ITERATION 4 (T027)

3. **Choice Button** (Individual generator)
   - Creates ChoiceButton prefab for player choices
   - Required for: ITERATION 9 (T044)

4. **Save Load UI** (Individual generator)
   - Creates SaveLoadUI prefab for save/load screens
   - Required for: ITERATION 22 (T100)

**Verification**:
- Open Project window → Assets/Resources/NovelCore/UI/
- Verify prefabs exist: DialogueBox.prefab, ChoiceButton.prefab, SaveLoadUI.prefab
- Double-click prefab → Prefab editor opens with UI hierarchy visible

**Note**: These prefabs can be customized later in Unity Editor (colors, fonts, layout). The generators create functional defaults.

---

### Tasks Completion Status

- [X] T001: Unity project created
- [X] T002: Install packages
- [X] T003: URP configured
- [X] T004: Runtime asmdef created
- [X] T005: Editor asmdef created
- [X] T006: .editorconfig exists
- [X] T007: Folder structure created
- [X] T008: Configure scripting backend

## After Completing Manual Steps

All ITERATION 0 tasks are now complete! You can proceed to ITERATION 1 implementation.

**Optional**: Generate UI prefabs now using Step 4 above, or generate them later when needed for specific iterations.
