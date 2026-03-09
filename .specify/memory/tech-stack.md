# Novel Core - Technical Stack

**Last Updated**: 2026-03-09

## Core Technologies

### Unity Engine
- **Version**: Unity 6 (Long Term Support)
- **Language**: C# 9.0+
- **Scripting Backend**: IL2CPP (all platforms)
- **Rendering Pipeline**: Universal Render Pipeline (URP) 2D

### Dependencies

#### Unity Packages
- **Unity Addressables**: 2.0+ (asset management, streaming)
- **VContainer**: 1.14+ (dependency injection)
- **Spine-Unity**: 4.2+ (advanced character animations)
- **Steamworks.NET**: 20.2+ (Steam integration)
- **Unity Localization**: 2.0+ (multi-language support)
- **Unity Input System**: Latest (input abstraction)
- **TextMesh Pro**: Built-in (text rendering)
- **Unity Test Framework**: Built-in (testing)

### Target Platforms

#### Desktop
- **Windows**: x64 (Steam)
- **macOS**: Intel + Apple Silicon (Steam)

#### Mobile
- **iOS**: 15+ (App Store)
- **Android**: API 24+ / Nougat (Google Play)

### Performance Targets

#### Desktop
- **FPS**: 60 FPS on Intel HD 620 / AMD Vega 8
- **RAM**: ≤1GB
- **Scene Transition**: <1 second

#### Mobile
- **FPS**: 60 FPS on iPhone 13 / Samsung Galaxy S21
- **RAM**: ≤512MB
- **Scene Transition**: <1 second
- **Battery**: <10% drain per hour

### Storage & Serialization

- **Save Files**: JSON (via `JsonUtility`)
- **Asset Catalogs**: Unity Addressables
- **Settings**: PlayerPrefs (lightweight settings only)
- **Cloud Sync**: Steam Cloud / iCloud / Google Play

### Testing Stack

- **Framework**: Unity Test Framework (UTF) + NUnit
- **Strategy**: EditMode-first (unit tests), PlayMode (integration tests)
- **Coverage Target**: >80% (post-MVP v0.4.0+)
- **CI/CD**: Automated test execution on pre-commit and pipeline

### Entry Point Architecture

- **GameStarter Component**: Initializes VContainer, loads starting scene, starts DialogueSystem/SceneManager
- **Play Modes**: 
  - Full Start: Play ▶️ button runs complete game initialization
  - Scene Preview: Scene Editor preview button for isolated testing

### Build Configuration

- **Build Size**: <50MB initial download (mobile)
- **Asset Optimization**: Per-platform texture/audio compression
- **Code Stripping**: IL2CPP with managed stripping level

## Compliance Requirements

- **App Store**: Apple App Store Review Guidelines 4.2 compliance
- **Google Play**: Google Play Content Policies compliance
- **GDPR**: Opt-in analytics with privacy policy
- **Accessibility**: VoiceOver/TalkBack support, font scaling
- **Localization**: UTF-8, right-to-left language support
