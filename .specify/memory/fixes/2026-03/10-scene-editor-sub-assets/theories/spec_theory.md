# Specification Theory

## Hypothesis

Баг **НЕ вызван** некорректной спецификацией. Спецификация подразумевает правильное поведение (sub-assets), но реализация в SceneEditorWindow отклонилась от паттерна.

## Evidence

### Constitution Analysis

**Principle I (Creator-First Design)**:
```
"Content creators (writers, artists, game designers) are the primary users."
```

**Interpretation**: Контент-криейторы ожидают, что сцена = один файл с всем контентом внутри. Это подразумевает sub-assets, не standalone assets.

**Principle VI (Modular Architecture & Testing)**:
```
"Scene as atomic unit of work"
```

**Interpretation**: Сцена должна быть атомарной единицей, что подразумевает sub-assets для всех компонентов сцены (dialogue lines, choices).

### Spec Contradictions

**None found**. Спецификация не содержит явных противоречий относительно asset structure.

### Outdated Requirements

**None found**. Constitution актуальна (v1.16.0, Last Amended: 2026-03-10).

### Ambiguous Requirements

**Potential ambiguity**: Constitution не содержит **явного** требования о том, что DialogueLineData и ChoiceData должны быть sub-assets. Однако это подразумевается из:

1. **Principle I**: "Asset management MUST maintain referential integrity throughout the project lifecycle"
   - Sub-assets обеспечивают referential integrity (невозможно удалить DialogueLine без удаления SceneData)

2. **Principle VI**: "Scene as atomic unit of work"
   - Атомарность требует, чтобы сцена была самодостаточной

## Expected Behavior Mismatch

- **According to Constitution (implicit)**: DialogueLineData и ChoiceData должны быть sub-assets SceneData
- **Actual behavior**: SceneEditorWindow создает standalone assets
- **Bug behavior aligns with**: Implementation error, NOT spec error

## Constitution Alignment

### Violations by Current Implementation

**Principle I (Creator-First Design)** - VIOLATED:
- ❌ "Immediate Feedback": Множественные asset-файлы усложняют навигацию
- ❌ "Asset Import": Broken references при копировании сцен

**Principle III (Asset Pipeline Integrity)** - VIOLATED:
- ❌ "No Broken References": Риск orphaned DialogueLines при удалении SceneData
- ❌ "Dependency Tracking": Копирование SceneData не копирует DialogueLines автоматически

**Principle VI (Modular Architecture)** - VIOLATED:
- ❌ "Scene as atomic unit": DialogueLines существуют вне SceneData

### Correct Implementation Alignment

**SampleProjectGenerator approach** (sub-assets) **fully aligns** with Constitution:
- ✅ Principle I: Чистый Project Browser, no manual cleanup
- ✅ Principle III: Referential integrity maintained
- ✅ Principle VI: Scene as atomic unit

## Confidence Score

**15%**

### Reasoning

1. **Constitution is clear** (implicit): Принципы I, III, VI подразумевают sub-assets
2. **No spec contradictions**: Нет противоречий между документами
3. **Implementation reference exists**: SampleProjectGenerator показывает правильный паттерн
4. **This is implementation divergence**, not spec ambiguity

**Low confidence** потому что:
- Constitution не содержит **явного** требования "MUST use sub-assets for DialogueLineData"
- Можно аргументировать, что spec недостаточно детальна

Однако, учитывая Principle VI ("Scene as atomic unit"), интерпретация очевидна: sub-assets — правильный подход.

## Recommended Action

- [ ] ~~Update specification~~ (не требуется)
- [ ] ~~Clarify ambiguous requirements~~ (требования достаточно ясны)
- [ ] ~~Resolve contradictions~~ (противоречий нет)
- [x] **Update implementation to match implicit spec** (SceneEditorWindow должен использовать sub-assets)

### Optional Constitution Enhancement

Хотя текущая Constitution достаточна, можно добавить **explicit guidance** в Principle VI:

```markdown
**Asset Structure**:
- Scene components (DialogueLineData, ChoiceData, etc.) MUST be created as sub-assets of SceneData
- Use `AssetDatabase.AddObjectToAsset()`, not `AssetDatabase.CreateAsset()`
- Rationale: Ensures scene atomicity and referential integrity
```

Это предотвратит будущие отклонения от паттерна.
