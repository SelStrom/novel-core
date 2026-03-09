# User Manual: Visual Novel Constructor

**For**: Content Creators (Writers, Artists, Game Designers)  
**Version**: 1.0  
**Date**: 2026-03-06  
**No Programming Required** ✨

---

## Welcome!

Welcome to the Visual Novel Constructor! This tool helps you create professional visual novels for Steam and mobile platforms **without writing any code**. If you can write a story and use basic computer applications, you can create a visual novel.

**What you'll be able to do**:
- Write branching stories with player choices
- Add character sprites with emotions
- Insert background images and music
- Publish to Windows, macOS, iOS, and Android

**What you don't need**:
- Programming experience
- Game development knowledge
- Technical skills

Let's get started! 🎮

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Creating Your First Scene](#creating-your-first-scene)
3. [Writing Dialogue](#writing-dialogue)
4. [Adding Characters](#adding-characters)
5. [Creating Story Branches](#creating-story-branches)
6. [Adding Audio](#adding-audio)
7. [Previewing Your Novel](#previewing-your-novel)
8. [Publishing Your Novel](#publishing-your-novel)
9. [Troubleshooting](#troubleshooting)
10. [Tips & Best Practices](#tips--best-practices)

---

## Getting Started

### Unity Project Location

Your Unity project is located in the `novel-core` folder. This is where all your game files, scripts, and assets are stored.

```
novel-core/                 # Your Unity project (repository root)
├── Assets/                 # Game assets and code
│   ├── Scripts/            # C# scripts
│   ├── Content/            # Your visual novel content
│   └── Resources/          # Game resources
├── Packages/               # Unity packages
└── ProjectSettings/        # Project configuration
```

**For Developers**: Ask your developer to set up the Unity project if not already done.

### Opening the Constructor

1. **Launch Unity** (the program your developer installed for you)
2. **Open your project** (it should open automatically)
3. **Wait for Unity to load** (you'll see a loading screen with a Unity logo)

### Understanding the Interface

When Unity opens, you'll see several windows:

```
┌─────────────────────────────────────────────────────────┐
│  Menu Bar (NovelCore menu is here)                      │
├──────────────┬──────────────────────┬───────────────────┤
│              │                      │                   │
│  Hierarchy   │   Scene View         │   Inspector       │
│  (List of    │   (What you see)     │   (Properties)    │
│  objects)    │                      │                   │
│              │                      │                   │
├──────────────┴──────────────────────┴───────────────────┤
│  Project (Your files and assets)                        │
└─────────────────────────────────────────────────────────┘
```

**Don't worry about most of these!** You'll mainly use the **NovelCore** menu.

### Creating Your First Project

1. Click **NovelCore → New Project** in the menu bar
2. Enter your project details:
   - **Project Name**: "My First Visual Novel"
   - **Author**: Your name
   - **Description**: Brief summary of your story
3. Click **Create**

Your project folder will appear in the **Content/Projects/** folder.

---

## Creating Your First Scene

A **scene** is a single moment or location in your story. Think of it like a page in a book or a panel in a comic.

### Step 1: Open Scene Editor

1. Click **NovelCore → Scene Editor** in the menu
2. A new window titled "Scene Editor" will open

### Step 2: Create a New Scene

1. Click **+ New Scene** button (top left)
2. Fill in the details:
   - **Scene Name**: "Opening Scene"
   - **Scene ID**: `scene_001` (must be unique)

### Step 3: Add a Background

Every scene needs a background image.

1. **Find your image**:
   - Go to **Project** window at the bottom
   - Navigate to **Content → Backgrounds**
   - Your developer should have imported images here

2. **Drag and drop**:
   - Click and hold on your background image
   - Drag it to the **Background** field in Scene Editor
   - Release the mouse button

**Tip**: If you don't see your images, ask your developer to import them. Supported formats: PNG, JPG

### Step 4: Add Background Music (Optional)

1. In **Project** window, go to **Content → Audio → Music**
2. Drag a music file to the **Background Music** field
3. Music will loop automatically during the scene

### Step 5: Save Your Scene

1. Click **Save Scene** button (bottom right)
2. Your scene is now saved!

---

## Writing Dialogue

Dialogue is the text that characters say in your visual novel.

### Adding Dialogue Lines

1. In **Scene Editor**, find the **Dialogue Lines** section
2. Click **+ Add Line** button
3. Fill in the dialogue:

```
┌─────────────────────────────────────────┐
│ Speaker: [Select character or Narrator] │
│ Text: "Hello! Welcome to my story."     │
│ Emotion: neutral                         │
└─────────────────────────────────────────┘
```

**Fields explained**:
- **Speaker**: Who is talking? (Choose from your characters or "Narrator")
- **Text**: What they say (type normally, like in a text editor)
- **Emotion**: Character's facial expression (happy, sad, angry, etc.)

### Adding Multiple Lines

Keep clicking **+ Add Line** to add more dialogue. They will play in order from top to bottom.

**Example conversation**:
```
Line 1: Alice (happy): "Hi! I'm Alice."
Line 2: Bob (neutral): "Nice to meet you, Alice."
Line 3: Alice (excited): "Want to explore the forest?"
```

### Editing Dialogue

- **Change text**: Click on the text field and type
- **Reorder lines**: Drag the ☰ handle on the left to move lines up/down
- **Delete line**: Click the 🗑️ trash icon on the right

### Narrator Text

For narration (no character speaking):
1. Set **Speaker** to "Narrator"
2. The text will appear centered without a character portrait

---

## Adding Characters

Characters are the people (or creatures!) in your visual novel.

### Step 1: Open Character Editor

1. Click **NovelCore → Character Editor** in the menu
2. A new window titled "Character Editor" will open

### Step 2: Create a New Character

1. Click **+ New Character** button
2. Fill in the details:
   - **Character Name**: "Alice"
   - **Character ID**: `alice` (short, no spaces)

### Step 3: Add Character Sprites

A sprite is the character's image. You need different sprites for different emotions.

1. **Add Neutral Emotion** (required):
   - Click **+ Add Emotion**
   - Emotion Name: `neutral`
   - Drag sprite image from **Content → Characters → Alice** folder
   - This is the default expression

2. **Add More Emotions**:
   - Click **+ Add Emotion** again
   - Emotion Name: `happy`
   - Drag the happy sprite image
   - Repeat for: `sad`, `angry`, `surprised`, `worried`, etc.

**Tip**: You need at least one emotion (`neutral`). More emotions = more expressive characters!

### Step 4: Position Character in Scene

1. Go back to **Scene Editor**
2. In the **Characters** section, click **+ Add Character**
3. Select your character (e.g., "Alice")
4. Set position:
   - **X Position**: 0.25 = left side, 0.5 = center, 0.75 = right side
   - **Y Position**: Usually 0.5 (middle)
5. Select **Initial Emotion** (what emotion they start with in this scene)

**Visual Guide**:
```
Screen Position:
┌──────────────────────────────────┐
│  0.0          0.5          1.0   │  ← X Position
│   ◀           ◀            ◀      │
│  Left       Center       Right   │
└──────────────────────────────────┘
```

### Step 5: Save Your Character

Click **Save Character** button in Character Editor.

---

## Creating Story Branches

Story branches let players make choices that affect the outcome.

### Understanding Branches

```
Scene A: "Do you want to explore the forest?"
   ├─ Choice 1: "Yes" → Scene B (Forest path)
   └─ Choice 2: "No"  → Scene C (Stay in town)
```

### Adding a Choice Point

1. In **Scene Editor**, scroll to **Choices** section
2. Click **+ Add Choice**
3. Fill in the choice:

```
┌─────────────────────────────────────────┐
│ Question: "What do you want to do?"     │
│                                          │
│ Option 1:                                │
│   Text: "Explore the forest"            │
│   Goes To: [Select scene]               │
│                                          │
│ Option 2:                                │
│   Text: "Stay in town"                  │
│   Goes To: [Select scene]               │
│                                          │
│ [+ Add Option]                           │
└─────────────────────────────────────────┘
```

**Fields explained**:
- **Question**: Optional text shown before the choices (e.g., "What will you do?")
- **Option Text**: The button text players click (e.g., "Go left")
- **Goes To**: Which scene to play next (select from dropdown)

### Creating Multiple Endings

1. Create several "ending" scenes (e.g., `ending_good`, `ending_bad`)
2. Add choices in earlier scenes that lead to different endings
3. Use the **Story Flow** window to visualize your branches

### Viewing Story Flow

1. Click **NovelCore → Story Flow** in the menu
2. You'll see a graph showing:
   - Boxes = Scenes
   - Arrows = Connections (dialogue flow or choices)

**Example**:
```
┌─────────┐
│ Scene 1 │───→ Dialogue
└────┬────┘
     │ Choice
     ├───→┌─────────┐
     │    │ Scene 2A│──→ Ending A
     │    └─────────┘
     │
     └───→┌─────────┐
          │ Scene 2B│──→ Ending B
          └─────────┘
```

**Tip**: If you see a red warning icon on a scene, it means a choice points to a scene that doesn't exist yet. Create that scene or fix the choice!

---

## Adding Audio

Audio makes your visual novel more immersive.

### Types of Audio

1. **Background Music**: Loops during a scene (e.g., peaceful town theme)
2. **Sound Effects**: Play once for events (e.g., door opening, footsteps)
3. **Voice Acting**: Character voices speaking dialogue (optional)

### Adding Background Music

1. In **Scene Editor**, find the **Background Music** field
2. Drag music file from **Content → Audio → Music**
3. Music will fade in when scene starts, fade out when scene ends

**Tip**: Use .mp3 or .ogg files for best compatibility

### Adding Sound Effects

1. In **Dialogue Lines** section, select a dialogue line
2. Find the **Sound Effect** field
3. Drag sound file from **Content → Audio → SFX**
4. Sound plays when that dialogue line appears

**Examples**:
- Door closing sound when dialogue says "She left the room."
- Thunder sound when dialogue says "Suddenly, lightning struck!"
- Phone ringing when dialogue says "The phone rang."

### Adding Voice Acting

If you have voice actors:
1. In **Dialogue Lines** section, select a line
2. Find the **Voice Clip** field
3. Drag voice audio file from **Content → Audio → Voice**
4. Voice plays when dialogue appears (text auto-advances when voice finishes)

### Audio Tips

- **Music**: Use calming music for peaceful scenes, tense music for dramatic scenes
- **SFX**: Don't overuse! Only add sounds for important events
- **Voice**: Make sure voice files match the text exactly (same language, same emotion)

---

## Previewing Your Novel

Always preview your work before publishing!

### Quick Preview

1. Click **NovelCore → Preview Scene** in the menu
2. Unity will enter "Play Mode" (you'll see colored tint)
3. Your scene will play like in the final game:
   - Background appears
   - Characters appear
   - Dialogue plays
   - Music plays
4. Click the screen or press **Space** to advance dialogue
5. If there are choices, click the choice buttons

### Exiting Preview

1. Click the **Play** button (▶) at the top of Unity to stop preview
2. You're back in editing mode

### Testing Story Branches

1. Preview a scene with choices
2. Make a choice (e.g., "Go left")
3. Verify it goes to the correct next scene
4. Stop preview and restart to test other choices

**Important**: Any choices you make in preview don't save! Each preview starts fresh.

---

## Publishing Your Novel

When your visual novel is complete, it's time to publish!

### Step 1: Open Build Window

1. Click **NovelCore → Build Pipeline** in the menu
2. A new window titled "Build Pipeline" will open

### Step 2: Configure Build Settings

Fill in your game information:
- **Product Name**: "My Amazing Visual Novel"
- **Company Name**: Your name or studio
- **Version**: 1.0.0 (increase for updates: 1.1.0, 1.2.0, etc.)

### Step 3: Select Platforms

Check which platforms you want to publish to:
- ☑ **Windows** (PC via Steam or itch.io)
- ☑ **macOS** (Mac via Steam or itch.io)
- ☑ **iOS** (iPhone/iPad via App Store)
- ☑ **Android** (Google Play Store)

**Note**: Building for iOS requires a Mac computer. Building for Android requires additional setup (ask your developer).

### Step 4: Build Your Game

1. Click **Build All Platforms** button (or **Build Windows** for just one)
2. Wait for the build to complete (this can take 5-15 minutes)
3. When finished, you'll see a success message

### Step 5: Find Your Build

Your built game will be in:
- **Windows**: `Builds/Windows/` folder (contains `.exe` file)
- **macOS**: `Builds/macOS/` folder (contains `.app` file)
- **iOS**: `Builds/iOS/` folder (Xcode project, needs developer)
- **Android**: `Builds/Android/` folder (`.apk` or `.aab` file)

### Step 6: Test Your Build

**Before publishing**:
1. Navigate to the build folder (e.g., `Builds/Windows/`)
2. Double-click the game executable to run it
3. Play through your entire visual novel
4. Check:
   - All scenes work
   - All choices work
   - All audio plays
   - All images show correctly
   - No error messages appear

### Publishing to Steam

1. Create a **Steam Partner** account (https://partner.steamgames.com)
2. Pay the $100 app submission fee (one-time)
3. Follow Steam's upload instructions (use `Builds/Windows/` and `Builds/macOS/` folders)
4. Submit for review

**Note**: Your developer can help with the technical Steam upload process.

### Publishing to Mobile

**iOS (App Store)**:
1. Need Apple Developer account ($99/year)
2. Need Mac computer with Xcode
3. Your developer must handle iOS build upload (requires code signing)

**Android (Google Play)**:
1. Need Google Play Developer account ($25 one-time)
2. Upload the `.aab` file from `Builds/Android/`
3. Fill in store listing (screenshots, description)
4. Submit for review

---

## Troubleshooting

### My background image doesn't show

**Fix**:
1. Check the image is in **Content → Backgrounds** folder
2. Right-click the image → **Addressables → Mark Addressable**
3. Try dragging it to the Background field again

### My character sprite doesn't appear

**Fix**:
1. Make sure you added at least the `neutral` emotion
2. Check the sprite is in **Content → Characters** folder
3. In Scene Editor, verify you added the character to the **Characters** list
4. Verify the **Initial Emotion** is set to an emotion that exists (e.g., `neutral`)

### Dialogue text doesn't show

**Fix**:
1. Check you added dialogue lines in **Scene Editor → Dialogue Lines**
2. Verify the **Text** field is not empty
3. Try previewing the scene again

### Choice buttons don't appear

**Fix**:
1. Check you added choices in **Scene Editor → Choices**
2. Verify each option has **Text** filled in
3. Verify each option has **Goes To** scene selected
4. Make sure the target scene exists

### Music doesn't play

**Fix**:
1. Check audio file is in **Content → Audio → Music** folder
2. Supported formats: .mp3, .ogg, .wav
3. Right-click the audio → **Addressables → Mark Addressable**
4. Try dragging it to the Background Music field again
5. Check your computer volume is not muted

### Build fails with error

**Fix**:
1. Click **NovelCore → Validate Project** (checks for errors)
2. Read the error messages (they explain what's wrong)
3. Common issues:
   - Missing asset: Add the missing image/audio file
   - Broken link: Fix the choice that points to non-existent scene
   - Invalid character: Check character has at least `neutral` emotion
4. If you can't fix it, ask your developer for help

### Preview mode is stuck

**Fix**:
1. Click the **Play** button (▶) at the top to stop preview
2. If Unity freezes, close and reopen Unity
3. Save your work frequently (File → Save Project)

---

## Tips & Best Practices

### Writing Tips

**✅ DO**:
- Keep dialogue short (2-3 sentences per line)
- Give each character a unique voice/personality
- Use emotions to match the dialogue mood
- Add narrator text for scene descriptions
- Test every story branch

**❌ DON'T**:
- Write walls of text (players will skip it)
- Forget to add choices (linear stories are boring)
- Use too many sound effects (gets annoying)
- Create dead-end branches (every path should reach an ending)

### Asset Organization

**Folder Structure**:
```
Content/
├── Backgrounds/
│   ├── town_day.png
│   ├── town_night.png
│   └── forest.png
├── Characters/
│   ├── Alice/
│   │   ├── alice_neutral.png
│   │   ├── alice_happy.png
│   │   └── alice_sad.png
│   └── Bob/
│       └── ... (Bob's sprites)
└── Audio/
    ├── Music/
    │   ├── town_theme.mp3
    │   └── battle_theme.mp3
    └── SFX/
        ├── door_close.wav
        └── footsteps.wav
```

**Naming Conventions**:
- Use lowercase with underscores: `forest_morning.png` ✅
- Avoid spaces: `Forest Morning.png` ❌
- Avoid special characters: `forest#morning!.png` ❌

### Performance Tips

**For Mobile**:
- Keep images under 2048×2048 pixels
- Use .jpg for backgrounds (smaller file size)
- Use .png for characters (transparency needed)
- Keep total project size under 500MB

**For Steam**:
- Use high-quality images (up to 4096×4096)
- Use .ogg for music (better quality than .mp3)
- No strict size limit, but avoid going over 5GB

### Saving Your Work

**Save often!**:
1. Press **Ctrl+S** (Windows) or **Cmd+S** (Mac) frequently
2. Click **File → Save Project** before closing Unity
3. Keep backups of your **Content/** folder (copy to external drive)

### Getting Help

**If you're stuck**:
1. Click **NovelCore → Help** → Opens this manual
2. Click **NovelCore → Validate Project** → Checks for errors
3. Ask your developer for technical help
4. Join the community forum (link in Help menu)

---

## Keyboard Shortcuts

**Scene Editor**:
- `Ctrl/Cmd + S`: Save scene
- `Ctrl/Cmd + N`: New scene
- `Ctrl/Cmd + D`: Duplicate selected item
- `Delete`: Remove selected item

**Preview Mode**:
- `Space` or `Click`: Advance dialogue
- `Ctrl/Cmd + Space`: Skip dialogue
- `Esc`: Show menu
- `Ctrl/Cmd + S`: Quick save
- `Ctrl/Cmd + L`: Load save

**Unity**:
- `Ctrl/Cmd + S`: Save project
- `Ctrl/Cmd + Shift + S`: Save all
- `Ctrl/Cmd + P`: Play/Stop preview

---

## Glossary

**Terms you'll see**:

- **Scene**: A single moment or location in your story
- **Dialogue Line**: One piece of text spoken by a character or narrator
- **Character Sprite**: The image of a character
- **Emotion**: A character's facial expression (happy, sad, etc.)
- **Choice/Branch**: A decision point where players pick an option
- **Background**: The image behind characters (location)
- **Asset**: Any file (image, audio) used in your game
- **Preview**: Testing your game inside Unity
- **Build**: Creating the final game file for players
- **Addressable**: A system that loads assets (you usually don't need to worry about this!)

---

## Appendix: Image Requirements

### Background Images

- **Format**: PNG or JPG
- **Size**: 1920×1080 (Full HD) or 1280×720 (HD)
- **Aspect Ratio**: 16:9 (widescreen)
- **File Size**: Keep under 5MB per image

### Character Sprites

- **Format**: PNG (with transparency)
- **Size**: 512×1024 to 1024×2048 pixels
- **Background**: Transparent (no background)
- **File Size**: Keep under 2MB per sprite

### Audio Files

**Music**:
- **Format**: .mp3 or .ogg
- **Bitrate**: 128-192 kbps
- **Length**: Any (will loop)
- **File Size**: Keep under 10MB per track

**Sound Effects**:
- **Format**: .wav or .ogg
- **Bitrate**: 96 kbps
- **Length**: Under 5 seconds
- **File Size**: Keep under 1MB per sound

---

## Appendix: Common Workflows

### Workflow 1: Adding a New Scene

1. **NovelCore → Scene Editor**
2. **+ New Scene**
3. Drag background image
4. Drag background music
5. Add characters (if any)
6. Add dialogue lines
7. Add choices (if branching)
8. **Save Scene**
9. **Preview** to test

**Time**: 10-20 minutes per scene

### Workflow 2: Creating a Character

1. **NovelCore → Character Editor**
2. **+ New Character**
3. Enter name and ID
4. Add `neutral` emotion sprite
5. Add other emotions (happy, sad, etc.)
6. **Save Character**

**Time**: 5-10 minutes per character

### Workflow 3: Updating Dialogue

1. **NovelCore → Scene Editor**
2. Select your scene
3. Find the dialogue line to edit
4. Click the text field and type changes
5. **Save Scene**
6. **Preview** to verify

**Time**: 1-2 minutes per change

### Workflow 4: Publishing an Update

1. Make your changes to scenes/characters
2. **Save All** (File → Save Project)
3. **Validate Project** (NovelCore → Validate Project)
4. Fix any errors
5. **NovelCore → Build Pipeline**
6. Increase version number (1.0.0 → 1.1.0)
7. **Build All Platforms**
8. Test all builds
9. Upload to Steam/App Store/Google Play

**Time**: 30-60 minutes (mostly build time)

---

## Need More Help?

**Resources**:
- **Video Tutorials**: [Link to tutorial videos] (if available)
- **Community Forum**: [Link to forum] (if available)
- **Sample Project**: Open `Content/Projects/SampleProject` to see an example
- **Developer Contact**: Ask your developer for technical support

**Remember**: You don't need to understand the technical parts. Focus on creating great stories! 📖✨

---

**Version History**:
- **1.0** (2026-03-06): Initial release

**Made with ❤️ using Novel Core Constructor**
