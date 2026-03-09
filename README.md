# Novel Core - Visual Novel Constructor

Unity-based visual novel constructor for creating professional visual novels without programming.

## 🎯 Project Overview

**Novel Core** is a Unity 6-based framework for building cross-platform visual novels. It provides:
- Visual scene editor for non-programmers
- Dialogue system with branching narratives
- Save/load system with cloud sync
- Cross-platform builds (Windows, macOS, iOS, Android)
- Modular, testable architecture

**Current Status**: MVP Development (v0.3.0)

## 📂 Documentation Structure

This project uses **spec-kit** methodology for organized documentation:

### Core Documentation (`.specify/memory/`)
- **[Constitution](./specify/memory/constitution.md)** - Project principles & non-negotiable rules
- **[Tech Stack](./specify/memory/tech-stack.md)** - Unity version, dependencies, platforms
- **[Testing Strategy](./specify/memory/testing-strategy.md)** - Test organization & workflow
- **[Project Structure](./specify/memory/project-structure.md)** - Directory layout & boundaries

### Feature Specifications (`specs/`)
- **[001-visual-novel-constructor](./specs/001-visual-novel-constructor/)** - Core feature
  - `plan.md` - Implementation plan
  - `spec.md` - Requirements specification
  - `tasks.md` - Task breakdown
  - `quickstart.md` - Developer setup guide
  - `user-manual.md` - User guide (non-programmers)
  - `data-model.md` - Entity definitions
  - `contracts/` - System interfaces

## Project Structure

```
novel-core/                     # Unity project root
├── .specify/                   # Spec-kit: Core documentation
│   ├── memory/                 # Centralized project knowledge
│   │   ├── constitution.md     # Project principles (SINGLE SOURCE OF TRUTH)
│   │   ├── tech-stack.md       # Technical stack
│   │   ├── testing-strategy.md # Testing guidelines
│   │   └── project-structure.md # Directory structure
│   └── templates/              # Document templates
├── specs/                      # Feature specifications
│   └── 001-visual-novel-constructor/
├── Assets/                     # Unity assets
│   ├── Scripts/NovelCore/      # All C# code (AI-modifiable)
│   ├── Content/                # User content (Addressables source)
│   └── Resources/              # Runtime-loaded assets
├── Packages/                   # Unity packages
├── ProjectSettings/            # Unity settings (AI-restricted)
└── README.md                   # This file
```

**See**: [Project Structure](./specify/memory/project-structure.md) for detailed directory breakdown

## Quick Start

### 🎮 For Content Creators (Non-Programmers)

**Try the demo in 3 minutes**:

1. Open Unity with `novel-core` project
2. Menu: **NovelCore → Generate Sample Project**
3. Press **Play** ▶️ in Unity
4. Click to advance dialogue, make choices!

**📖 Full Guide**: [User Manual (Russian)](./specs/001-visual-novel-constructor/user-manual-ru.md)

### 👨‍💻 For Developers

**Setup**:
1. Install Unity 6 (LTS)
2. Clone repo: `git clone <url> && cd novel-core`
3. Open `novel-core` folder in Unity Hub
4. Install dependencies (Addressables, VContainer, Localization)
5. Generate sample project to verify setup

**📖 Developer Guide**: [Quick Start](./specs/001-visual-novel-constructor/quickstart.md)

## Documentation

### Essential Docs (Start Here)

- **[Constitution](./specify/memory/constitution.md)** - Project principles & rules
- **[Quick Start Guide](./specs/001-visual-novel-constructor/quickstart.md)** - Developer setup
- **[User Manual (RU)](./specs/001-visual-novel-constructor/user-manual-ru.md)** - For creators
- **[Testing Strategy](./specify/memory/testing-strategy.md)** - How to run tests

### Detailed Specifications

- **[Tech Stack](./specify/memory/tech-stack.md)** - Unity version, dependencies, platforms
- **[Specification](./specs/001-visual-novel-constructor/spec.md)** - Feature requirements
- **[Implementation Plan](./specs/001-visual-novel-constructor/plan.md)** - Architecture
- **[Tasks](./specs/001-visual-novel-constructor/tasks.md)** - Implementation roadmap
- **[Data Model](./specs/001-visual-novel-constructor/data-model.md)** - Entity definitions
- **[Contracts](./specs/001-visual-novel-constructor/contracts/)** - System interfaces

## Technology Stack

**See**: [Tech Stack](./specify/memory/tech-stack.md) for detailed version requirements

- **Engine**: Unity 6 (LTS)
- **Language**: C# 9.0+
- **Rendering**: Universal Render Pipeline (URP) 2D
- **Asset Management**: Unity Addressables 2.0+
- **Dependency Injection**: VContainer 1.14+
- **Localization**: Unity Localization 2.0+
- **Platforms**: Windows, macOS, iOS, Android (IL2CPP)

## Code Style

**Strict C# standards enforced** (see [Constitution - Code Style](./specify/memory/constitution.md#code-style-standards)):

- ✅ Allman braces (opening brace on new line)
- ✅ Mandatory braces (even for single-line statements)
- ✅ `var` in loops
- ✅ Underscore prefix for private/protected fields (`_fieldName`)

## Constitution Principles

**See**: [Constitution](./specify/memory/constitution.md) for full details

**Core Principles**:
1. Creator-First Design - No programming required
2. Cross-Platform Parity - Identical behavior everywhere
3. Asset Pipeline Integrity - No broken references
4. Runtime Performance - 60 FPS target
5. Save System Reliability - Auto-save, cloud sync
6. Modular Architecture - Testable, independent modules
7. AI Development Constraints - Script-only modifications
8. Editor-Runtime Bridge - Preview integration
9. User Documentation Language - Russian for end-users

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
