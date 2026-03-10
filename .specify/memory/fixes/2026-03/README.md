# Fixes Archive - March 2026

## 10-scene-editor-sub-assets

**Date**: 2026-03-10  
**Component**: SceneEditorWindow  
**Category**: API Misuse  
**Severity**: Medium  

**Summary**: SceneEditorWindow создавал DialogueLineData и ChoiceData как standalone assets вместо sub-assets, нарушая паттерн SampleProjectGenerator и Constitution Principles I, III, VI.

**Root Cause**: Использование `AssetDatabase.CreateAsset()` вместо `AssetDatabase.AddObjectToAsset()`

**Fix**: Замена API call в `CreateNewDialogueLine()` и `CreateNewChoice()`

**Tests Added**: 12 tests (3 reproduction + 9 break tests)

**Confidence**: 95%

**Impact**: Low (Editor-only, backward compatible)

---

## Pattern Analysis

### Recurring Patterns

**Current Month**: 1 fix

**API Misuse Category**: 
- Unity AssetDatabase API misuse (10-scene-editor-sub-assets)

### Constitution Review

**Potential Enhancement**: Add explicit guidance to Constitution Principle VI:

```markdown
**Asset Structure Best Practices**:
- Scene components (DialogueLineData, ChoiceData) MUST use AddObjectToAsset
- Rationale: Ensures scene atomicity and referential integrity
```

This would prevent similar API misuse in future.

---

_This file is auto-updated monthly with fix patterns and Constitution recommendations._
