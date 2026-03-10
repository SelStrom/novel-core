# Selected Theory

## Decision

**Selected**: **IMPL_THEORY**

## Reasoning

### Theory Comparison

| Aspect                     | Spec Theory      | Impl Theory      |
|----------------------------|------------------|------------------|
| Confidence Score           | 15%              | 95%              |
| Evidence Quality           | LOW              | HIGH             |
| Failing Test Alignment     | N/A              | STRONG           |
| Constitution Alignment     | ALIGNED (implicit)| ALIGNED (fix required) |
| Root Cause Clarity         | Ambiguous        | Explicit         |
| Fix Complexity             | High (update spec + impl) | Low (2-3 lines per method) |
| Reference Implementation   | None             | SampleProjectGenerator exists |

### Decision Rationale

1. **Evidence Strength**:
   - **Impl Theory**: Прямое доказательство — код использует `AssetDatabase.CreateAsset()` вместо `AssetDatabase.AddObjectToAsset()`
   - **Spec Theory**: Только косвенные доказательства — Constitution подразумевает sub-assets, но не требует явно

2. **Test Alignment**:
   - **Impl Theory**: Failing test явно демонстрирует API misuse
   - **Spec Theory**: Нет failing test для spec ambiguity

3. **Root Cause Coverage**:
   - **Impl Theory**: 100% coverage — баг объясняется одной строкой кода
   - **Spec Theory**: Недостаточно — даже если добавить explicit requirement в spec, все равно нужно исправлять код

4. **Fix Strategy**:
   - **Impl Theory**: Trivial fix (замена 1 API call на другой)
   - **Spec Theory**: Требует обновление Constitution + код (избыточно)

5. **Reference Implementation**:
   - **Impl Theory**: SampleProjectGenerator уже показывает правильный подход
   - **Spec Theory**: Если spec была ambiguous, SampleProjectGenerator не существовал бы с правильной реализацией

### Rejected Theory

**Rejected**: **SPEC_THEORY**

**Reason for Rejection**:

Хотя Constitution не содержит **явного** требования "MUST use sub-assets for DialogueLineData", это **подразумевается** из:

- **Principle I (Creator-First Design)**: "Creators expect 'build once' without per-asset debugging"
- **Principle III (Asset Pipeline Integrity)**: "No Broken References MUST be detected at import-time"
- **Principle VI (Modular Architecture)**: "Scene as atomic unit of work"

Однако, **отсутствие явного требования — это не баг спецификации**, а просто недостаточная детализация. Фактически:

1. **SampleProjectGenerator следует implicit spec** — это доказывает, что spec была понятна разработчику SampleProjectGenerator
2. **SceneEditorWindow отклонился от паттерна** — это implementation error, а не spec ambiguity
3. **Constitution достаточна** — Principles I, III, VI ясно указывают на sub-assets как правильный подход

**Conclusion**: Это **Implementation Bug**, не Spec Issue.

## Confidence in Decision

**98%**

### Justification

1. **Root cause trivial**: Одна строка кода (`CreateAsset` vs `AddObjectToAsset`)
2. **Fix trivial**: 2-3 строки изменений
3. **Reference impl exists**: SampleProjectGenerator доказывает, что sub-assets — правильный подход
4. **Evidence direct**: Файл `DialogueLine 1.asset` существует (git status)
5. **Reproducible 100%**: Каждый вызов `CreateNewDialogueLine()` создает standalone asset

**2% uncertainty** на случай, если:
- Есть скрытая причина использовать standalone assets (не обнаружена в анализе)
- Unity API ограничения для sub-assets (маловероятно, SampleProjectGenerator работает)

Но эти сценарии крайне маловероятны.

## Recommended Fix Strategy

### If Impl Theory Selected (✅ CURRENT)

#### Step 1: Fix Code

**File**: `SceneEditorWindow.cs`

**Method 1**: `CreateNewDialogueLine()` (Lines 570-588)

```diff
private void CreateNewDialogueLine(SerializedObject serializedObject, SerializedProperty dialogueProperty)
{
-   string path = AssetDatabase.GetAssetPath(_currentScene);
-   string directory = System.IO.Path.GetDirectoryName(path);
-
    var newLine = CreateInstance<DialogueLineData>();
-   string linePath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/DialogueLine.asset");
+   newLine.name = $"DialogueLine_{dialogueProperty.arraySize + 1}";

-   AssetDatabase.CreateAsset(newLine, linePath);
+   AssetDatabase.AddObjectToAsset(newLine, _currentScene);
+   EditorUtility.SetDirty(_currentScene);
    AssetDatabase.SaveAssets();

    dialogueProperty.arraySize++;
    var newElement = dialogueProperty.GetArrayElementAtIndex(dialogueProperty.arraySize - 1);
    newElement.objectReferenceValue = newLine;

    serializedObject.ApplyModifiedProperties();

    _selectedDialogueIndex = dialogueProperty.arraySize - 1;
}
```

**Method 2**: `CreateNewChoice()` (Lines 590-608)

```diff
private void CreateNewChoice(SerializedObject serializedObject, SerializedProperty choicesProperty)
{
-   string path = AssetDatabase.GetAssetPath(_currentScene);
-   string directory = System.IO.Path.GetDirectoryName(path);
-
    var newChoice = CreateInstance<ChoiceData>();
-   string choicePath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/Choice.asset");
+   newChoice.name = $"Choice_{choicesProperty.arraySize + 1}";

-   AssetDatabase.CreateAsset(newChoice, choicePath);
+   AssetDatabase.AddObjectToAsset(newChoice, _currentScene);
+   EditorUtility.SetDirty(_currentScene);
    AssetDatabase.SaveAssets();

    choicesProperty.arraySize++;
    var newElement = choicesProperty.GetArrayElementAtIndex(choicesProperty.arraySize - 1);
    newElement.objectReferenceValue = newChoice;

    serializedObject.ApplyModifiedProperties();

    _selectedChoiceIndex = choicesProperty.arraySize - 1;
}
```

#### Step 2: Add Regression Tests

**File**: `SceneEditorWindowSubAssetReproductionTests.cs` (already created)

Tests should verify:
1. ✅ DialogueLine created as sub-asset (not standalone)
2. ✅ Choice created as sub-asset (not standalone)
3. ✅ No orphaned asset files created

#### Step 3: Manual Cleanup Guidance

**For users with existing standalone assets**:

Create a migration utility (optional):

```csharp
[MenuItem("Tools/NovelCore/Migrate Standalone Assets to Sub-Assets")]
public static void MigrateStandaloneAssets()
{
    // Find all SceneData with external DialogueLine/Choice references
    // Convert them to sub-assets
    // Delete original standalone files
}
```

Or provide manual instructions in fix summary.

#### Step 4: Optional Constitution Update

**Add explicit guidance to Principle VI**:

```markdown
### Asset Structure Best Practices

Scene components (DialogueLineData, ChoiceData, etc.) MUST be created as sub-assets of SceneData:

- **Use**: `AssetDatabase.AddObjectToAsset(component, sceneData)`
- **NOT**: `AssetDatabase.CreateAsset(component, path)`
- **Rationale**: Ensures scene atomicity, referential integrity, and creator-friendly workflow
- **Example**: See `SampleProjectGenerator.CreateScene()` for reference implementation
```

This prevents future deviations from the pattern.

### If Spec Theory Selected (❌ REJECTED)

- [ ] Update `.specify/specs/[feature]/spec.md`
- [ ] Clarify ambiguous requirements
- [ ] Update constitution if needed
- [ ] Update implementation to match corrected spec

**Reason for rejection**: Spec is sufficient; implementation diverged.

## Next Steps

### FixAgent Tasks

1. **Apply code changes** to `SceneEditorWindow.cs`:
   - Update `CreateNewDialogueLine()` (Lines 570-588)
   - Update `CreateNewChoice()` (Lines 590-608)

2. **Verify compilation**: Unity batch mode compile check

3. **Run tests**:
   - Run `SceneEditorWindowSubAssetReproductionTests.cs`
   - Verify all tests pass (currently expect 2 failures → 0 failures after fix)

4. **Manual verification**:
   - Open Scene Editor Window in Unity
   - Select SceneData
   - Click "+ Add Dialogue Line"
   - Verify: No `DialogueLine N.asset` file created in Project Browser
   - Verify: DialogueLine appears as sub-asset when expanding SceneData in Project Browser

5. **Break tests** (BreakAgent):
   - Test edge cases: null SceneData, max dialogue lines, concurrent creation
   - Test regression: Existing standalone references still work

### BreakAgent Considerations

**Edge cases to test**:
1. Creating DialogueLine when SceneData has 100+ existing lines
2. Creating DialogueLine when SceneData path is invalid (should not happen in normal flow)
3. Deleting SceneData with sub-assets (should delete all sub-assets)
4. Duplicating SceneData (should duplicate sub-assets automatically)
5. Backward compatibility: Existing SceneData with external references still loads correctly

### Commit Message Template

```
fix(editor): create DialogueLineData and ChoiceData as sub-assets

Root Cause:
SceneEditorWindow used AssetDatabase.CreateAsset() instead of
AssetDatabase.AddObjectToAsset(), creating standalone asset files
instead of embedding them as sub-assets in SceneData.

Changes:
- SceneEditorWindow.cs: CreateNewDialogueLine() - use AddObjectToAsset
- SceneEditorWindow.cs: CreateNewChoice() - use AddObjectToAsset

Tests:
- Added SceneEditorWindowSubAssetReproductionTests.cs
- Verified sub-asset creation in Unity Editor
- All tests passing (EditMode: 3/3)

Constitution Compliance:
- Principle I (Creator-First Design): Clean Project Browser
- Principle III (Asset Pipeline Integrity): Referential integrity maintained
- Principle VI (Modular Architecture): Scene as atomic unit
- Code Style: Allman braces, underscore prefix enforced

Resolves: DialogueLine/Choice standalone asset creation issue
```

## Summary

**IMPL_THEORY selected** with 98% confidence. Fix is trivial (2-3 lines per method), low risk, and aligns with existing reference implementation (SampleProjectGenerator). No spec changes required.
