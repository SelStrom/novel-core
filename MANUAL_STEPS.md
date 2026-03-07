# Manual Unity Editor Steps Required

**Project**: Visual Novel Constructor  
**Date**: 2026-03-07  
**Status**: Iteration 0 - Project Bootstrap

## Required Manual Steps

These tasks cannot be automated via AI due to Constitution Principle VII (AI Development Constraints). They must be completed in Unity Editor.

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

1. **Windows Platform**:
   - Select "PC, Mac & Linux Standalone" icon
   - Go to "Other Settings" section
   - Find "Scripting Backend"
   - Set to: **Mono**

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
- Ensure Windows = Mono, others = IL2CPP

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

### Tasks Completion Status

- [X] T001: Unity project created
- [ ] **T002: Install packages** ← **DO THIS NOW**
- [X] T003: URP configured
- [X] T004: Runtime asmdef created
- [X] T005: Editor asmdef created
- [X] T006: .editorconfig exists
- [X] T007: Folder structure created
- [ ] **T008: Configure scripting backend** ← **DO THIS NOW**

## After Completing Manual Steps

Mark tasks as complete in `specs/001-visual-novel-constructor/tasks.md`:
- Change `- [ ] T002` to `- [X] T002`
- Change `- [ ] T008` to `- [X] T008`

Then proceed to ITERATION 1 implementation.
