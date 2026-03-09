# Feature Specification: Visual Novel Constructor

**Feature Branch**: `001-visual-novel-constructor`  
**Created**: 2026-03-06  
**Status**: Draft  
**Input**: User description: "Приложение для быстрого создания визуальной новеллы. Приложение должно: 1. иметь весь core-функционал и механики популярных визуальных новелл. 2. иметь функционал для создания сцен, диалогов, настройки персонажей и добавления графики. 3. На основе доступного функционала не-программист должен мочь собрать визуальную новеллу. 4. Должен быть настроен build pipline для публикации на целевые платформы и площадки."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create Basic Visual Novel Scene (Priority: P1)

A content creator (writer/artist with no programming experience) opens the constructor and creates a simple visual novel scene with character dialogue and background image. They can preview the scene immediately and see it working like a published visual novel.

**Why this priority**: This is the absolute minimum viable product. If a user cannot create a single scene with dialogue, the tool has no value. This proves the core "no-code visual novel creation" promise.

**Independent Test**: Can be fully tested by creating a new project, adding one background, one character sprite, writing 3 lines of dialogue, and playing the scene in preview mode. Delivers a working mini visual novel.

**Acceptance Scenarios**:

1. **Given** the constructor is open with a new project, **When** the user adds a background image and creates dialogue text, **Then** the preview shows the background with dialogue text displayed in a dialogue box
2. **Given** a scene with dialogue exists, **When** the user clicks through dialogue in preview mode, **Then** each dialogue line advances to the next, mimicking published visual novel behavior
3. **Given** a scene is being edited, **When** the user adds a character sprite with position, **Then** the preview immediately shows the character at the specified position on top of the background

---

### User Story 2 - Create Branching Narrative (Priority: P2)

A content creator designs a story with player choices that lead to different outcomes. They create choice points where the story branches, and each path leads to different scenes and endings.

**Why this priority**: Branching narratives are the defining feature of visual novels. Without choices, it's just a linear slideshow. This enables true interactive storytelling.

**Independent Test**: Create 2 scenes, add a choice in scene 1 with 2 options, link each option to different dialogue/outcomes. Test by playing through and verifying both paths work independently.

**Acceptance Scenarios**:

1. **Given** a scene with dialogue, **When** the user adds a choice point with 2+ options, **Then** preview mode shows the choice buttons to the player
2. **Given** a choice point exists, **When** the user links each choice to different story branches, **Then** selecting a choice in preview navigates to the correct branch
3. **Given** multiple story branches exist, **When** the user tests all paths, **Then** each path maintains its own story progression and can reach different endings
4. **Given** branching paths are created, **When** the user views the story structure, **Then** a visual flow diagram shows all branches and connections

---

### User Story 3 - Manage Characters and Emotions (Priority: P3)

A content creator defines characters with multiple emotional states (happy, sad, angry, surprised) and switches between character expressions during dialogue to match the story mood.

**Why this priority**: Emotional expression is critical for storytelling quality. This separates a professional visual novel from a basic text display.

**Independent Test**: Create a character with 3 emotion sprites, create dialogue that switches emotions mid-conversation, preview and verify emotion changes happen at correct moments.

**Acceptance Scenarios**:

1. **Given** the character editor, **When** the user creates a character and uploads multiple emotion sprites, **Then** all emotion variants are available for use in scenes
2. **Given** a scene with character dialogue, **When** the user assigns different emotions to different dialogue lines, **Then** preview shows the character expression changing as dialogue progresses
3. **Given** multiple characters in a scene, **When** the user switches between speakers, **Then** preview highlights the active speaker and displays their emotion correctly

---

### User Story 4 - Add Audio and Visual Effects (Priority: P4)

A content creator enhances their visual novel with background music, sound effects (door closing, footsteps), and visual transitions (fade in/out, character entrance animations).

**Why this priority**: Audio and effects elevate production quality but aren't essential for storytelling. The story can work without them, but they improve player immersion.

**Independent Test**: Add background music to a scene, add a sound effect to a dialogue line, add a fade transition between scenes. Preview and verify audio plays and transitions work.

**Acceptance Scenarios**:

1. **Given** a scene, **When** the user adds background music, **Then** preview plays the music looping during the scene
2. **Given** a dialogue line, **When** the user attaches a sound effect, **Then** preview plays the sound when that line appears
3. **Given** multiple scenes, **When** the user adds transitions (fade, slide), **Then** preview shows smooth transitions between scenes matching the selected effect
4. **Given** a character enters a scene, **When** the user applies an entrance animation, **Then** preview shows the character appearing with the animation (slide in, fade in)

---

### User Story 5 - Build and Publish Visual Novel (Priority: P5)

A content creator completes their visual novel and uses the build pipeline to package it for Windows, macOS (Steam) and mobile platforms (iOS, Android). They can test builds on each platform before publishing.

**Why this priority**: Publishing is the ultimate goal, but only matters if the content creation features work. Users need a working visual novel before they need to publish it.

**Independent Test**: Create a minimal visual novel (2 scenes), trigger build process for each target platform, verify builds launch and play correctly on target devices.

**Acceptance Scenarios**:

1. **Given** a completed visual novel project, **When** the user initiates build for Windows/macOS, **Then** the system generates a standalone executable that runs on the target platform
2. **Given** a completed project, **When** the user initiates mobile build (iOS/Android), **Then** the system generates an app package compatible with App Store/Google Play submission requirements
3. **Given** a build is generated, **When** the user tests it on the target platform, **Then** all scenes, dialogue, choices, and assets work identically to the preview mode
4. **Given** multiple platform builds are needed, **When** the user triggers batch build, **Then** the system builds for all platforms sequentially and reports success/failure for each

---

### Edge Cases

- What happens when the user creates a choice that leads to a non-existent scene? System should warn about broken links and prevent build until resolved.
- How does the system handle very large background images (8K resolution)? System should auto-compress/resize to target platform limits with quality warning.
- What happens when the user creates circular story loops (scene A → B → A)? System should detect loops and warn if there's no exit path to an ending.
- How does the system handle missing audio files referenced in scenes? System should show missing asset warnings and provide placeholder audio during preview.
- What happens when the user tries to build with no scenes? System should block build and show error message requiring at least one scene.
- How does the system handle Unicode text (Japanese, Russian, emoji) in dialogue? System must support UTF-8 and render all characters correctly on all platforms.
- What happens when a mobile build exceeds app store size limits? System should warn before build and suggest asset optimization strategies.

## Requirements *(mandatory)*

### Functional Requirements

**Core Visual Novel Mechanics**

- **FR-001**: System MUST support displaying background images fullscreen with dialogue text overlay
- **FR-002**: System MUST support character sprites with layering (background behind characters, UI on top)
- **FR-003**: System MUST support advancing dialogue via click/tap with visual indication of more content
- **FR-004**: System MUST support dialogue history (backlog) allowing players to review previous text
- **FR-005**: System MUST support text skip/auto-advance modes for experienced players
- **FR-006**: System MUST support save/load functionality with multiple save slots
- **FR-007**: System MUST support auto-save at choice points and scene transitions
- **FR-008**: System MUST support player choice points with 2-6 options per choice
- **FR-009**: System MUST track player choices and enable conditional branching based on previous decisions
- **FR-010**: System MUST support multiple endings based on player choices

**Content Creation Interface**

- **FR-011**: System MUST provide visual scene editor accessible without coding
- **FR-012**: System MUST support drag-and-drop asset import (images, audio)
- **FR-013**: System MUST provide character configuration interface for managing sprites and emotions
- **FR-014**: System MUST provide dialogue editor with support for speaker names, emotion assignment, and audio triggers
- **FR-015**: System MUST provide story flow visualization showing scene connections and branches
- **FR-016**: System MUST support undo/redo for all editing operations
- **FR-017**: System MUST validate project for errors (missing assets, broken links) before build
- **FR-018**: System MUST provide real-time preview mode matching final published visual novel behavior

**Asset Management**

- **FR-019**: System MUST support common image formats (PNG, JPG) for backgrounds and character sprites
- **FR-020**: System MUST support common audio formats (MP3, OGG, WAV) for music and sound effects
- **FR-021**: System MUST detect and warn about missing asset references
- **FR-022**: System MUST support asset replacement without breaking scene references
- **FR-023**: System MUST organize assets by category (backgrounds, characters, audio) in asset browser

**Build Pipeline**

- **FR-024**: System MUST build standalone executables for Windows (x64)
- **FR-025**: System MUST build standalone executables for macOS (Intel and Apple Silicon)
- **FR-026**: System MUST build mobile packages for iOS (App Store compatible)
- **FR-027**: System MUST build mobile packages for Android (APK and AAB for Google Play)
- **FR-028**: System MUST integrate Steam SDK features (achievements, cloud saves) for Steam builds
- **FR-029**: System MUST optimize assets per platform (texture compression, resolution scaling)
- **FR-030**: System MUST report build errors with actionable guidance for resolution

### Key Entities

- **Project**: Represents a complete visual novel with metadata (title, author, description), asset collections, and scene graph
- **Scene**: A single location/moment in the story containing background, characters, dialogue lines, and choices. Links to other scenes via transitions or choice outcomes
- **Character**: A story character with metadata (name, description) and sprite collections. Each character has multiple emotion states (neutral, happy, sad, angry, etc.)
- **Dialogue Line**: A single piece of text with speaker attribution, emotion state, audio triggers, and timing. Multiple lines form conversations
- **Choice Point**: A decision moment with 2-6 options. Each option links to a different story branch or scene
- **Asset**: Any imported resource (image, audio) with metadata (filename, type, usage count). Tracked for reference integrity
- **Build Configuration**: Platform-specific settings (resolution, compression, SDK integrations) used during build process

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Non-programmers can create a 3-scene visual novel with branching choices in under 30 minutes on first use
- **SC-002**: Preview mode renders scenes identically to final published builds on all platforms (visual parity within 5%)
- **SC-003**: Build pipeline generates working executables/packages for all 4 target platforms (Windows, macOS, iOS, Android) in under 10 minutes for a 100-scene project
- **SC-004**: System prevents 95%+ of asset reference errors through real-time validation and warnings
- **SC-005**: 90% of users can publish their first visual novel to Steam or mobile stores without external technical support
- **SC-006**: Load times for 100-scene visual novels are under 3 seconds on target hardware (iPhone 12, midrange Android, Intel HD 620)
- **SC-007**: Visual novels created with the constructor achieve 60 FPS during dialogue and transitions on all target platforms
- **SC-008**: Asset import workflow (drag-and-drop to usable in scene) completes in under 5 seconds per asset
- **SC-009**: Story flow visualization renders comprehensibly for projects with up to 200 scenes and 50 choice points
- **SC-010**: Generated builds comply with platform submission requirements (App Store guidelines, Steam policies) on first attempt for 80%+ of projects

### Assumptions

- Users have basic computer literacy (file management, understanding of visual novel genre)
- Target hardware meets minimum requirements: 4GB RAM, dedicated or integrated GPU supporting DirectX 11/Metal/OpenGL ES 3.0
- Users have legal rights to all assets (images, audio) they import
- Visual novel content complies with platform content policies (no prohibited content requiring age gates beyond standard ratings)
- Users are creating narrative-focused visual novels (not action games or complex mechanics requiring custom programming)
- Default dialogue text rendering supports Latin, Cyrillic, and CJK (Chinese/Japanese/Korean) character sets
- Build pipeline assumes users have installed platform SDKs (Xcode for iOS, Android SDK) where required by platform policies
