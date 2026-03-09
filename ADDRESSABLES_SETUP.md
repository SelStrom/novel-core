# Addressables Configuration Required

**Task**: T009 - Setup Addressables groups

## Manual Steps Required in Unity Editor

Once Addressables package is installed (T002):

### Step 1: Initialize Addressables

1. Open Unity Editor
2. Go to **Window → Asset Management → Addressables → Groups**
3. If prompted "Addressables not initialized", click **Create Addressables Settings**

### Step 2: Create Addressable Groups

Create the following groups for content organization:

#### Group: Content_Backgrounds
- **Path**: `Assets/Content/Backgrounds/`
- **Build Path**: LocalBuildPath
- **Load Path**: LocalLoadPath
- **Purpose**: Background images for scenes

#### Group: Content_Characters
- **Path**: `Assets/Content/Characters/`
- **Build Path**: LocalBuildPath
- **Load Path**: LocalLoadPath
- **Purpose**: Character sprites and emotion variants

#### Group: Content_Audio_Music
- **Path**: `Assets/Content/Audio/Music/`
- **Build Path**: LocalBuildPath
- **Load Path**: LocalLoadPath
- **Compression**: Vorbis (for music files)
- **Purpose**: Background music tracks

#### Group: Content_Audio_SFX
- **Path**: `Assets/Content/Audio/SFX/`
- **Build Path**: LocalBuildPath
- **Load Path**: LocalLoadPath
- **Purpose**: Sound effects

#### Group: Content_Localization
- **Path**: `Assets/Content/Localization/`
- **Build Path**: LocalBuildPath
- **Load Path**: LocalLoadPath
- **Purpose**: Localization string tables

#### Group: Runtime_Prefabs
- **Path**: `Assets/Resources/NovelCore/UI/`
- **Build Path**: LocalBuildPath
- **Load Path**: LocalLoadPath
- **Purpose**: UI prefabs (dialogue box, choice buttons)

#### Group: Runtime_Defaults
- **Path**: `Assets/Resources/NovelCore/DefaultAssets/`
- **Build Path**: LocalBuildPath
- **Load Path**: LocalLoadPath
- **Purpose**: Fallback assets (missing texture placeholder, etc.)

### Step 3: Configure Group Settings

For each group:
1. Right-click group → **Inspect Group Settings**
2. Set **Bundle Mode**: Pack Together
3. Set **Compression**: LZ4 (fast loading) for most assets
4. Set **Compression**: Vorbis for music (better compression)

### Step 4: Mark Assets as Addressable

Example workflow:
1. Select an asset in Project window (e.g., a background image)
2. In Inspector, check **Addressable** checkbox
3. Set **Address**: Use asset name or custom key (e.g., "bg_forest")
4. Assign to appropriate group (e.g., Content_Backgrounds)

### Verification

- Open **Window → Asset Management → Addressables → Groups**
- Verify all groups are created
- Verify settings show proper build/load paths
- Build Addressables content: **Build → New Build → Default Build Script**

### Expected Folder Structure After Setup

```
Assets/
├── AddressableAssetsData/
│   ├── AddressableAssetSettings.asset
│   ├── AssetGroups/
│   │   ├── Content_Backgrounds.asset
│   │   ├── Content_Characters.asset
│   │   ├── Content_Audio_Music.asset
│   │   ├── Content_Audio_SFX.asset
│   │   ├── Content_Localization.asset
│   │   ├── Runtime_Prefabs.asset
│   │   └── Runtime_Defaults.asset
│   └── DataBuilders/
├── StreamingAssets/
│   └── aa/  (generated after first build)
```

## Task Completion

After completing these steps, mark T009 as complete in tasks.md:
- Change `- [ ] T009` to `- [X] T009`
