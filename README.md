# Novel Core - Visual Novel Constructor

Unity-based visual novel constructor for creating professional visual novels without programming.

## Project Structure

```
novel-core/                     # Unity project root (this folder)
├── Assets/                     # Unity assets
│   ├── Scripts/NovelCore/      # All C# code
│   │   ├── Runtime/            # Runtime systems
│   │   ├── Editor/             # Editor tools
│   │   └── Tests/              # Unit tests
│   ├── Content/                # User content (your visual novels)
│   │   ├── Backgrounds/        # Background images
│   │   ├── Characters/         # Character sprites
│   │   ├── Audio/              # Music and SFX
│   │   └── Projects/           # Visual novel projects
│   ├── Resources/              # Runtime-loaded resources
│   └── AddressableAssets/      # Addressables configuration
├── Packages/                   # Unity packages (dependencies)
├── ProjectSettings/            # Unity project settings
├── .specify/                   # Specification system
├── specs/                      # Feature specifications & docs
│   └── 001-visual-novel-constructor/
│       ├── spec.md             # Feature specification
│       ├── plan.md             # Implementation plan
│       ├── quickstart.md       # Developer guide
│       └── user-manual.md      # User guide (non-programmers)
└── README.md                   # This file
```

## Quick Start

### For Content Creators (Non-Programmers)

Read the user manual: `specs/001-visual-novel-constructor/user-manual.md`

**Quick Access**:
1. Open Unity
2. Open `novel-core` folder as Unity project
3. Go to menu: **NovelCore → Scene Editor**
4. Start creating your visual novel!

### For Developers

Read the developer guide: `specs/001-visual-novel-constructor/quickstart.md`

**Setup**:
1. Install Unity 2022.3 LTS
2. Open `novel-core` folder in Unity
3. Install dependencies via Package Manager
4. Follow quickstart guide for implementation workflow

## Documentation

- **User Manual** (`specs/.../user-manual.md`): For content creators (writers, artists)
- **Quick Start** (`specs/.../quickstart.md`): For developers (technical setup)
- **Specification** (`specs/.../spec.md`): Feature requirements
- **Implementation Plan** (`specs/.../plan.md`): Technical architecture
- **Data Model** (`specs/.../data-model.md`): Entity definitions
- **Contracts** (`specs/.../contracts/`): System interfaces
- **Constitution** (`.specify/memory/constitution.md`): Project principles

## Technology Stack

- **Engine**: Unity 6 (LTS)
- **Language**: C# 10.0+
- **Rendering**: Universal Render Pipeline (URP) 2D
- **Asset Management**: Unity Addressables 2.0+
- **Dependency Injection**: VContainer 1.14+
- **Animation**: Unity Animator + Spine-Unity 4.2+
- **Localization**: Unity Localization 2.0+
- **Platform SDKs**: Steamworks.NET 20.2+ (Steam), iOS/Android native
- **Scripting Backend**: IL2CPP (all platforms)

## Code Style

This project enforces strict C# coding standards (see `.specify/memory/constitution.md` - Code Style Standards):

**Key Rules**:
- ✅ **Allman Style Braces**: Opening brace always on new line
- ✅ **Mandatory Braces**: Always use braces for if/else/for/while, even single-line
- ✅ **Var in Loops**: Use `var` for type declaration in foreach/for loops
- ✅ **Underscore Prefix**: Private and protected fields must use `_fieldName` format

**Automatic Formatting**:
- `.editorconfig` file configures IDE formatting rules
- Visual Studio / Rider will auto-format on save
- Code reviews enforce these standards

## Target Platforms

- Windows x64 (Steam)
- macOS Intel + Apple Silicon (Steam)
- iOS 15+ (App Store)
- Android API 24+ / Android 7.0+ (Google Play)

## Constitution Principles

This project follows strict design principles (see `.specify/memory/constitution.md`):

1. **Creator-First Design**: No programming required for content creation
2. **Cross-Platform Parity**: Identical behavior on all platforms
3. **Asset Pipeline Integrity**: No broken references, Addressables mandatory
4. **Runtime Performance**: 60 FPS on target hardware
5. **Save System Reliability**: Auto-save, cloud sync, no data loss
6. **Modular Architecture**: Independent, testable modules
7. **AI Development Constraints**: AI only modifies `Assets/Scripts/` (exception: package management when user-specified)
8. **User Documentation Language**: Russian as primary language for end-user documentation

## Branch Structure

- `main`: Stable releases
- `001-visual-novel-constructor`: Active development (current)

## Git Workflow

```bash
# Clone repository
git clone https://github.com/yourusername/novel-core.git
cd novel-core

# Switch to development branch
git checkout 001-visual-novel-constructor

# Open in Unity
# Unity Hub → Add → Select 'novel-core' folder
```

## Build Output

Builds are generated in `Builds/` folder (not tracked in git):

```
Builds/
├── Windows/            # Windows executable
├── macOS/              # macOS app bundle
├── iOS/                # Xcode project
└── Android/            # APK/AAB files
```

## License

[Add your license here]

## Contributors

[Add contributors here]

## Support

- **Issues**: [GitHub Issues link]
- **Documentation**: See `specs/001-visual-novel-constructor/`
- **Community**: [Forum/Discord link]

---

**Made with ❤️ for visual novel creators**
