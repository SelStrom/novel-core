# Specification Theory

## Hypothesis

Баг **НЕ вызван** некорректной спецификацией. Спецификация (Constitution) явно требует referential integrity для assets, но не предоставляет детальных требований к управлению sub-assets в Editor Tools.

## Evidence

### Constitution Analysis

**Principle III - Asset Pipeline Integrity** (Lines 53-63):
- **Requirement**: "No Broken References: Missing asset references MUST be detected at import-time and edit-time with clear warnings"
- **Requirement**: "Dependency Tracking: Moving/renaming assets MUST automatically update all references across scenes, prefabs, and scripts"

**Analysis**: 
- Constitution требует **integrity of asset references**, но НЕ специфицирует:
  - Как должны управляться sub-assets (embedded assets via `AssetDatabase.AddObjectToAsset()`)
  - Что происходит при удалении ссылок на sub-assets из parent asset
  - Требование автоматического удаления orphaned sub-assets

**Principle VI - Modular Architecture & Testing** (Lines 166-175):
- **Sub-Asset Naming Convention**: `dialog_line_001`, `choice_001` (zero-padded, snake_case)
- **Analysis**: Constitution описывает *naming*, но не описывает *lifecycle management* sub-assets

### Spec Coverage Gaps

1. **Lifecycle Management**: Нет явного требования "при удалении ссылки на sub-asset из parent, сам sub-asset ДОЛЖЕН быть удален"
2. **Editor Tool Behavior**: Нет спецификации поведения SceneEditorWindow при удалении DialogueLine/Choice
3. **Sub-Asset Ownership**: Нет явного описания "owner-owned relationship" между SceneData и его sub-assets

### Expected Behavior Mismatch

- **According to Constitution**: Asset references должны поддерживать integrity (не должно быть broken references)
- **Current Bug Behavior**: Удаление ссылки на sub-asset создает "orphaned" sub-asset, который больше не используется SceneData, но всё ещё существует в asset database
- **Mismatch Analysis**: 
  - Bug **нарушает дух** Principle III (referential integrity), но Constitution не описывает **явно** поведение для sub-assets
  - Orphaned sub-assets технически НЕ являются "broken references" (они корректно существуют в AssetDatabase), но являются "unused assets" ("мусором")

### Ambiguous Requirements

**Ambiguity**: Constitution требует "Dependency Tracking: Moving/renaming assets MUST automatically update all references", но не описывает обратную операцию — "Deleting reference MUST clean up orphaned sub-assets"

**Spec Location**: `.specify/memory/constitution.md`, Principle III (Asset Pipeline Integrity)

**Current Behavior**: Deletion оставляет orphaned sub-assets

**Expected Behavior (from spirit of Constitution)**: Deletion должна удалять orphaned sub-assets для поддержания asset pipeline cleanliness

### Constitution Alignment

- **Principle Violated**: Principle III (Asset Pipeline Integrity) — **косвенно нарушается**
  - Orphaned sub-assets нарушают "clean asset pipeline" (мусорные assets загрязняют project)
  - Constitution требует integrity, но orphaned assets — это integrity issue (unused, unreferenced assets)

- **Principle NOT Violated**: 
  - Принцип III не ЯВНО запрещает orphaned sub-assets
  - Нет explicit requirement "clean up orphaned sub-assets on deletion"

## Confidence Score

**35%**

### Reasoning

- **Low confidence** в том, что это spec issue, потому что:
  1. Constitution НЕ содержит **явных противоречий** в описании поведения удаления sub-assets
  2. Constitution описывает referential integrity в общем виде, но НЕ специфицирует детали lifecycle management для sub-assets
  3. Баг касается **реализации editor tool**, который не полностью описан в спецификации (SceneEditorWindow behavior не имеет dedicated spec.md)
  
- **Moderate confidence** в том, что Constitution *неполная*:
  1. Нет явного требования "при удалении ссылки удалить orphaned sub-asset"
  2. Lifecycle management для sub-assets не специфицирован

- **Evidence pointing to IMPL issue**:
  1. Предыдущий fix (10-scene-editor-sub-assets) исправил **создание** sub-assets
  2. Текущий баг — **удаление** sub-assets (симметричная проблема)
  3. SceneEditorWindow просто не реализовал proper cleanup logic

## Recommended Action

- [ ] ~~Update specification~~ (NOT primary action)
- [x] **Clarify Constitution**: Add explicit requirement to Principle III:
  - "Sub-Asset Lifecycle: When a reference to a sub-asset is removed from parent asset, the orphaned sub-asset MUST be deleted from AssetDatabase to maintain clean asset pipeline"
- [x] **Update implementation**: SceneEditorWindow должен вызывать `AssetDatabase.RemoveObjectFromAsset()` перед `DeleteArrayElementAtIndex()`

## Conclusion

**Primary Root Cause: IMPLEMENTATION ISSUE**

Баг вызван **недостаточной реализацией** в SceneEditorWindow, а НЕ некорректной спецификацией. Constitution корректна в своих текущих требованиях, но может быть **расширена** для явного описания lifecycle management sub-assets.

**Recommended Path**: 
1. **Fix implementation first** (SceneEditorWindow.cs)
2. **Update Constitution** (optional enhancement): Добавить explicit sub-asset lifecycle requirement в Principle III для предотвращения подобных багов в будущем
