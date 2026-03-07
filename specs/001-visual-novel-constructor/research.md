# Research & Technical Decisions: Visual Novel Constructor

**Feature**: Visual Novel Constructor  
**Date**: 2026-03-06  
**Status**: Complete

## Overview

This document captures research findings and technical decisions for building a Unity-based visual novel constructor. All technology choices are finalized based on user requirements and industry best practices.

## Technology Stack Decisions

### Project Structure & Location

**Decision**: Unity project located at repository root (`./novel-core`)

**Rationale**:
- Unity expects `Assets/`, `Packages/`, and `ProjectSettings/` folders at project root
- Simplifies path references (no nested project folder)
- Standard Unity project structure
- `.specify/` and `specs/` folders coexist with Unity folders at root level

**Project Layout**:
```
novel-core/                 # Repository & Unity project root
├── .specify/               # Spec system files
├── specs/                  # Feature specifications
├── Assets/                 # Unity assets folder
│   ├── Scripts/            # All C# code (AI can modify)
│   ├── Content/            # User content (Addressables)
│   └── Resources/          # Runtime-loaded resources
├── Packages/               # Unity packages
├── ProjectSettings/        # Unity project settings
├── Library/                # Unity cache (gitignored)
├── Temp/                   # Unity temp files (gitignored)
└── Builds/                 # Build output (gitignored)
```

**Alternatives Considered**:
- Nested Unity project (e.g., `novel-core/UnityProject/`): Rejected - adds unnecessary path complexity
- Separate repositories: Rejected - harder to maintain docs + code synchronization

### Unity Version & Rendering

**Decision**: Unity 6 (LTS) with Universal Render Pipeline (URP) 2D

**Rationale**:
- **Unity 6 is the new LTS version** (released 2024, long-term support guaranteed)
- **Major improvements over 2022 LTS**:
  - 2x faster build times with improved IL2CPP compiler
  - Better URP performance (reduced draw calls, improved batching)
  - GPU Resident Drawer for lower CPU overhead
  - Improved memory management and garbage collection
  - Native support for Apple Silicon (better macOS performance)
  - Better Android performance with updated IL2CPP backend
- **URP 2D optimizations**:
  - 2D Renderer specifically designed for sprites and visual novels
  - Superior batching vs Unity 2022
  - Post-processing v3 with better mobile performance
  - 2D Lights with minimal overhead
- **C# 10 support**: Record types, global usings, file-scoped namespaces
- **Stable and production-ready**: Unity 6 is designated LTS with multi-year support

**Alternatives Considered**:
- Unity 2022.3 LTS: Previous LTS, but Unity 6 offers significant performance gains
- Built-in Pipeline: Deprecated, not recommended for new projects
- HDRP: Overkill for 2D, heavier than URP

**Unity 6 Benefits for Visual Novels**:
- Faster iteration (quicker builds during development)
- Better mobile performance out-of-the-box
- Improved Addressables streaming performance
- Better integration with modern C# features
- Long-term support and stability guarantees

### Asset Management System

**Decision**: Unity Addressables 1.21+

**Rationale**:
- Constitution Principle III mandates Addressables for asset pipeline integrity
- Enables asset streaming to meet <50MB mobile build size requirement
- Supports content updates without full app resubmission (DLC/patches)
- Provides persistent asset GUIDs preventing broken references
- Built-in support for remote asset catalogs (future mod/workshop support)

**Implementation Notes**:
- Assets/Content/ will be configured as Addressable groups
- Each visual novel project gets isolated Addressable catalogs
- Automatic asset reference validation via AssetReferenceT<T> in ScriptableObjects

### Dependency Injection

**Decision**: VContainer 1.14+ (user requirement)

**Rationale**:
- User-specified choice
- Lightweight DI container with Unity-specific optimizations
- Constructor injection aligns with constitution Principle VI (avoid singletons)
- Compile-time validation of dependencies (catches wiring errors early)
- Scene-scoped and project-scoped lifetime management
- Better performance than Zenject for mobile (IL2CPP friendly)

**Alternatives Considered**:
- Zenject: More features but heavier, slower IL2CPP compile times
- Manual singletons: Violates Principle VI

### Animation System

**Decision**: Hybrid approach - Unity Animator (simple) + Spine-Unity 4.1+ (complex)

**Rationale**:
- User requirement: Unity Animator for simple cases, Spine for complex
- Unity Animator: Built-in, good for basic fade/slide transitions, simple character poses
- Spine: Industry standard for 2D skeletal animation, advanced character expressions/movements
- Spine runtime is lightweight (<500KB) and mobile-optimized
- Clear separation: Spine for character animation, Animator for UI/scene transitions

**Implementation Notes**:
- ICharacterAnimator interface abstracts both backends
- SpineCharacterAnimator and UnityCharacterAnimator implementations
- Creator chooses animation system per character in editor

### Audio System

**Decision**: Custom AudioManager wrapper around Unity AudioSource

**Rationale**:
- User requirement: Wrapper for future extensibility
- Unity AudioSource sufficient for MVP (music looping, SFX playback, volume control)
- Wrapper isolates implementation from game logic
- Future swap to FMOD/Wwise possible without refactoring game code
- Interface: IAudioService with methods Play(), Stop(), SetVolume(), etc.

**Implementation Notes**:
```csharp
public interface IAudioService {
    void PlayMusic(AudioClip clip, float volume = 1.0f, bool loop = true);
    void PlaySFX(AudioClip clip, float volume = 1.0f);
    void StopMusic();
    void SetMasterVolume(float volume);
}
```

### Localization

**Decision**: Unity Localization Package 1.4+ (user requirement)

**Rationale**:
- User-specified: Standard Unity module
- Text-only localization per user requirement (no audio/texture variants)
- Built-in support for CSV/Excel import workflows
- Automatic string table generation from dialogue data
- Runtime locale switching without game restart

**Implementation Notes**:
- All dialogue text stored in Localization tables
- String table keys: `SCENE_{sceneId}_LINE_{lineId}`
- Editor tool auto-generates tables from DialogueData ScriptableObjects
- Support for UTF-8, RTL languages per constitution

### Platform SDKs

**Decision**: Steamworks.NET 20.2+ for Steam, native iOS/Android APIs

**Rationale**:
- Steamworks.NET: Community-maintained C# wrapper for Steamworks SDK
- Handles achievements, cloud saves, Steam overlay compatibility
- Platform-specific code isolated behind IPlatformService interface (Principle II, VI)

**Implementation Notes**:
- IPlatformService: Abstracts cloud saves, achievements, input
- SteamPlatformService: Steamworks.NET implementation
- iOSPlatformService: iCloud + Game Center integration
- AndroidPlatformService: Google Play Games Services integration

### Scripting Backend

**Decision**: IL2CPP for all platforms (Windows, macOS, iOS, Android)

**Rationale**:
- **Cross-Platform Parity** (Constitution Principle II): Using IL2CPP on all platforms ensures identical behavior and eliminates platform-specific bugs during development
- **iOS Requirement**: IL2CPP mandatory (Apple App Store policy, no JIT allowed)
- **Better Performance**: AOT compilation provides native code performance on all platforms
- **Early Issue Detection**: IL2CPP-specific issues discovered during Windows development, not at final testing stage
- **Smaller Binary Size**: IL2CPP produces smaller binaries compared to Mono
- **Code Protection**: IL2CPP output is C++ code, harder to decompile than Mono bytecode
- **Unity 6 IL2CPP improvements**:
  - 2x faster build times vs Unity 2022
  - Better code generation (10-15% performance improvement)
  - Improved debugging support

**Platform-Specific Rationale**:
- Windows (Steam): IL2CPP ensures development environment matches production behavior on all other platforms
- macOS (Steam): IL2CPP for consistency with mobile, smaller build size
- iOS: IL2CPP mandatory (Apple requirement)
- Android: IL2CPP for performance parity with iOS

**Trade-offs**:
- Slower build times acceptable for cross-platform reliability (Unity 6 IL2CPP builds 2x faster than Unity 2022)
- Limited reflection usage (acceptable - we use interfaces and ScriptableObjects)
- Single backend simplifies testing and reduces platform-specific edge cases

**Testing Strategy**:
- Development: Use IL2CPP on all platforms for consistent behavior
- CI/CD: Build with IL2CPP for all platforms (unified backend)
- Pre-release: Full testing on IL2CPP builds for all platforms

### C# 10 Language Features

**Decision**: Use C# 10 features available in Unity 6

**Key Features for Visual Novels**:

1. **Record Types** (immutable data):
   ```csharp
   public record DialogueLineData(string LineId, string TextKey, string Emotion);
   ```

2. **Global Usings** (reduce boilerplate):
   ```csharp
   // GlobalUsings.cs
   global using UnityEngine;
   global using System.Collections.Generic;
   global using VContainer;
   ```

3. **File-Scoped Namespaces** (cleaner code):
   ```csharp
   namespace NovelCore.Runtime.DialogueSystem; // No braces needed
   
   public class DialogueSystem { }
   ```

4. **Improved Pattern Matching**:
   ```csharp
   if (sceneData is { IsValid: true, DialogueLines.Count: > 0 })
   {
       // Scene is valid and has dialogue
   }
   ```

**Rationale**:
- Record types perfect for immutable ScriptableObject data
- File-scoped namespaces reduce indentation (cleaner code)
- Global usings reduce repetitive imports across hundreds of files
- Unity 6 fully supports C# 10 (previous versions only C# 9)

**Not Used** (avoiding complexity):
- Async streams (not needed for visual novel workflow)
- Interpolated string handlers (minimal benefit)

### Testing Framework

**Decision**: Unity Test Framework (UTF) with NUnit 3.5

**Rationale**:
- Built into Unity, no additional dependencies
- Supports both PlayMode (runtime) and EditMode (editor) tests
- Constitution Principle VI requires >80% coverage
- Integrates with Unity Cloud Build and CI/CD pipelines

**Test Strategy**:
- PlayMode tests: DialogueSystem, SaveSystem, SceneManagement (run in actual Unity player)
- EditMode tests: AssetValidators, EditorWindows, BuildPipeline (fast unit tests)
- Platform-specific tests: Run PlayMode tests on Windows, macOS, iOS (CI/CD)

## Architecture Patterns

### Modular Design (Principle VI)

**Decision**: Separate assembly definitions per subsystem

**Assemblies**:
1. `NovelCore.Runtime.Core` - Core systems (Dialogue, Save, Scene)
2. `NovelCore.Runtime.UI` - UI components
3. `NovelCore.Runtime.Platform` - Platform abstractions
4. `NovelCore.Runtime.Animation` - Animation systems
5. `NovelCore.Editor.Tools` - Editor windows and tools
6. `NovelCore.Tests.Runtime` - PlayMode tests
7. `NovelCore.Tests.Editor` - EditMode tests

**Benefits**:
- Faster iteration (Unity only recompiles changed assemblies)
- Enforces dependency direction (UI depends on Core, not vice versa)
- Easier testing (mock interfaces across assembly boundaries)
- Clear ownership (each assembly has specific responsibility)

### Data-Driven Design

**Decision**: ScriptableObjects for all content definitions

**Rationale**:
- Unity-native, no custom serialization required
- Asset pipeline handles GUID references automatically
- Inspector-friendly for non-programmers
- Addressable-compatible
- Version control friendly (YAML format, readable diffs)

**Key ScriptableObjects**:
- `SceneData`: Background, characters, dialogue lines, transitions
- `CharacterData`: Name, sprites, emotion variants, animation settings
- `DialogueLineData`: Speaker, text localization key, audio, emotion
- `ChoiceData`: Option text, destination scene, condition requirements
- `ProjectConfig`: Visual novel metadata, build settings, platform configs

### Save System Architecture

**Decision**: Versioned JSON serialization with upgrade handlers

**Save Format**:
```json
{
  "version": "1.0.0",
  "timestamp": "2026-03-06T12:00:00Z",
  "currentScene": "scene_001",
  "choices": ["choice_scene001_opt1", "choice_scene003_opt2"],
  "variables": { "affection_alice": 50, "route": "romance" },
  "playTime": 3600
}
```

**Rationale**:
- JSON human-readable for debugging
- Versioning enables backward compatibility (Principle V)
- JsonUtility lightweight (no external dependencies)
- Cloud sync friendly (small file size, diff-able)

**Upgrade Strategy**:
```csharp
public interface ISaveUpgrader {
    string FromVersion { get; }
    string ToVersion { get; }
    string Upgrade(string oldJson);
}
```

## Performance Optimization Strategies

### Memory Management

**Targets**: 512MB mobile, 1GB desktop (Principle IV)

**Strategies**:
1. **Addressables Streaming**: Load assets on-demand, unload after scene transitions
2. **Texture Compression**: ASTC (mobile), DXT (desktop), automatic per-platform
3. **Audio Compression**: Vorbis for music (streaming), ADPCM for SFX (decompress to RAM)
4. **Object Pooling**: Dialogue UI elements, character sprites (avoid GC spikes)
5. **Async Loading**: Load next scene assets during dialogue (hide latency)

### Rendering Performance

**Target**: 60 FPS on iPhone 12 / Intel HD 620 (Principle IV)

**Strategies**:
1. **URP 2D Optimizations**:
   - 2D Renderer profile with sprite batching
   - Single Camera setup (no camera stacking overhead)
   - Post-processing disabled by default (enable only for transitions)
   - 2D Light disabled by default (optional atmospheric effects)
2. **Sprite Atlasing**: Batch character sprites + UI into atlases (reduce draw calls)
3. **Layer Culling**: Only render visible UI layers
4. **Overdraw Minimization**: Z-ordering backgrounds behind characters, transparent pixel discard
5. **Shader Simplicity**: Unlit shaders for sprites (no lighting calculations)
6. **Resolution Scaling**: Dynamic resolution on low-end devices (maintain 60 FPS)

**URP Benefits**:
- Better sprite batching than Built-in pipeline
- SRP Batcher for consistent draw call performance
- Faster culling and rendering on mobile GPUs
- **Unity 6 Improvements**:
  - GPU Resident Drawer (reduces CPU overhead by 50%+)
  - Improved batching algorithms in URP
  - Post-processing v3 with better mobile performance
  - 2x faster IL2CPP builds (less waiting during development)

### Build Size Optimization

**Target**: <50MB initial download (Principle - Platform Requirements)

**Strategies**:
1. **Addressables Remote Catalog**: Core app <50MB, download content on first launch
2. **On-Demand Resources** (iOS): Stream assets from App Store servers
3. **App Bundles** (Android): Google Play generates optimized APKs per device
4. **Compression**: LZ4 for fast decompression, LZMA for maximum compression

## Build Pipeline Architecture

### Multi-Platform Build Strategy

**Platforms**: Windows x64, macOS (Universal), iOS, Android

**Automation**:
1. **Unity Cloud Build** or **GitHub Actions** for CI/CD
2. **Build script**: `BuildPipeline.BuildPlayer()` with platform-specific settings
3. **Post-processing**: Code signing (macOS/iOS), APK signing (Android), Steam depot upload

**Build Configurations**:
- Development: Debugging symbols, profiler enabled, uncompressed assets
- Release: IL2CPP, strip symbols, compress assets, enable optimizations

**Steam Integration**:
- Steamworks SDK integrated via Steamworks.NET
- Build script uploads depots via `steamcmd` automation
- Platform-specific depots: Windows x64, macOS Universal, Linux (future)

**Mobile Submission**:
- iOS: Xcode project generation → Archive → App Store Connect upload
- Android: Gradle build → AAB (Android App Bundle) → Google Play Console upload

## Risk Mitigation

### Technical Risks

**Risk**: Built-in pipeline performance issues on mobile  
**Mitigation**: Early profiling on iPhone 12, fallback to URP if needed (requires constitution amendment)

**Risk**: IL2CPP compile times slow iteration  
**Mitigation**: Unity 6 IL2CPP builds are 2x faster than Unity 2022, incremental builds enabled, development on all platforms uses same backend for consistency

**Risk**: Addressables learning curve for creators  
**Mitigation**: Asset import auto-configures Addressables groups, transparent to users

**Risk**: Cross-platform save sync conflicts  
**Mitigation**: Timestamp-based conflict resolution, keep both saves, let user choose

### Platform-Specific Risks

**iOS App Store Rejection**:
- **Risk**: 4.2 Minimum Functionality (template apps prohibited)
- **Mitigation**: Each published visual novel is unique content, not a template

**Android Fragmentation**:
- **Risk**: Performance variance across devices
- **Mitigation**: Dynamic quality settings, target API 21+ (95%+ device coverage)

**Steam Workshop**:
- **Risk**: User-generated content moderation
- **Mitigation**: Phase 2 feature, implement reporting/review system

## Open Questions (Future Phases)

1. **Analytics**: Which SDK? (Unity Analytics, Google Analytics, custom?)
   - Decision: Phase 2, opt-in per GDPR requirement

2. **Mod Support**: Steam Workshop vs. custom system?
   - Decision: Phase 3, depends on creator community interest

3. **Multiplayer/Social**: Leaderboards, achievements?
   - Decision: Phase 2, Steam achievements first, then mobile

4. **Advanced Scripting**: Lua/Python for power users?
   - Decision: Not MVP, evaluate after launch based on demand

## References

- Unity Addressables Documentation: https://docs.unity3d.com/Packages/com.unity.addressables@1.21/
- VContainer GitHub: https://github.com/hadashiA/VContainer
- Spine-Unity Documentation: http://esotericsoftware.com/spine-unity
- Steamworks.NET: https://steamworks.github.io/
- Unity Performance Best Practices: https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity.html

## Conclusion

All technology choices finalized. Architecture satisfies all constitution principles. Ready to proceed to Phase 1 (data model and contracts design).
