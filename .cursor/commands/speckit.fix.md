---
description: Automated bug fixing pipeline using multiple specialized agents for deterministic debugging and patching.
handoffs:
  - label: Create Regression Tests
    agent: speckit.tasks
    prompt: Create test tasks for the regression tests from .fix/tests/
  - label: Review Constitution
    agent: speckit.constitution
    prompt: Verify fix aligns with constitution principles
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Outline

Команда `/speckit.fix` запускает структурированный процесс отладки и исправления багов с использованием специализированных агентов. Процесс детерминированный и ограничен итерациями.

## Execution Graph

```
BugContextAgent
      ↓
┌──────────────────────┐
│ SpecTheoryAgent      │ (параллельно)
│ ImplTheoryAgent      │ (параллельно)
└──────────────────────┘
      ↓
TheoryArbitratorAgent
      ↓
FixAgent (loop max=10)
      ↓
BreakAgent
      ↓
CommitStage
```

## Prerequisites

1. **Инициализация структуры**:
   - Создать директорию `.fix/` в корне проекта (если не существует)
   - Создать поддиректории:
     - `.fix/bug_context/`
     - `.fix/theories/`
     - `.fix/tests/`
     - `.fix/patch/`
     - `.fix/agents/` (для логов агентов)

2. **Проверка окружения**:
   - Убедиться, что `.fix/` добавлена в `.gitignore` (кроме финальных патчей)
   - Проверить доступность Unity для запуска тестов (если баг связан с Unity)
   - Загрузить constitution.md для проверки соответствия

3. **Парсинг ввода**:
   - Если передан issue ID → загрузить описание из issue tracker
   - Если передано текстовое описание → использовать напрямую
   - Извлечь:
     - Описание бага
     - Ожидаемое поведение
     - Фактическое поведение
     - Шаги воспроизведения (если есть)

## Agent 1: BugContextAgent

**Цель**: Создать детерминированное воспроизведение бага.

### Responsibilities

1. **Анализ описания**:
   - Парсинг описания бага из пользовательского ввода
   - Идентификация затронутых компонентов
   - Определение ожидаемого vs фактического поведения

2. **Локализация кода**:
   - Поиск релевантных файлов через SemanticSearch
   - Анализ call stack (если доступен)
   - Извлечение связанных классов/методов

3. **Создание failing test**:
   - Написать unit/integration тест, воспроизводящий баг
   - Тест ДОЛЖЕН падать до применения исправления
   - Использовать Unity Test Framework (EditMode или PlayMode)
   - Тест должен соответствовать constitution (Principle VI - Testing)

4. **Валидация воспроизведения**:
   - Запустить failing test
   - Подтвердить, что тест падает ожидаемым образом
   - Записать лог воспроизведения

### Outputs

**`.fix/bug_context/bug.md`**:
```markdown
# Bug Description

[Детальное описание бага]

## Expected Behavior

[Что должно происходить]

## Actual Behavior

[Что происходит на самом деле]

## Steps to Reproduce

1. [Шаг 1]
2. [Шаг 2]
3. [Шаг 3]

## Environment

- Unity Version: [версия]
- Platform: [Windows/macOS/iOS/Android]
- NovelCore Version: [версия]
```

**`.fix/bug_context/relevant_files.md`**:
```markdown
# Relevant Files

## Primary Files (directly affected)

- `Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs`
  - Location: [namespace, class, method]
  - Reason: [почему релевантен]

## Secondary Files (dependencies)

- `Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs`
  - Reason: [почему релевантен]
```

**`.fix/bug_context/failing_test.cs`**:
```csharp
using NUnit.Framework;
using NovelCore.Runtime.[Component];

namespace NovelCore.Tests.Runtime.[Component]
{
    [TestFixture]
    public class [BugName]ReproductionTests
    {
        [Test]
        public void [Method]_[Scenario]_[ShouldFail]()
        {
            // Arrange
            // ... setup test context

            // Act
            // ... reproduce bug

            // Assert
            Assert.That(actual, Is.EqualTo(expected), "Bug reproduced: [description]");
        }
    }
}
```

**`.fix/bug_context/reproduction_log.txt`**:
```
[Timestamp] Running failing test: [TestName]
[Timestamp] Test setup: [context]
[Timestamp] Test execution: [details]
[Timestamp] Test FAILED: [error message]
[Timestamp] Expected: [value]
[Timestamp] Actual: [value]
```

### Constraints

- **Тест ДОЛЖЕН падать**: Если тест проходит, агент пересматривает воспроизведение
- **Детерминированность**: Тест должен падать каждый раз (не flaky)
- **Изоляция**: Тест не должен зависеть от внешних ресурсов
- **Соответствие constitution**: Тест следует Principle VI (EditMode-first strategy)

### Error Handling

Если воспроизведение не удается:
1. Запросить дополнительную информацию у пользователя
2. Повторить анализ с расширенным контекстом
3. Если после 3 попыток не удалось → вернуть MANUAL_REVIEW

## Agent 2: SpecTheoryAgent

**Цель**: Определить, является ли баг результатом некорректной спецификации.

### Inputs

- `.fix/bug_context/*` (все файлы контекста)
- `.specify/memory/constitution.md`
- `.specify/specs/*/spec.md` (релевантные спецификации)
- `.specify/specs/*/plan.md` (планы реализации)
- `.specify/memory/testing-strategy.md`

### Analysis Steps

1. **Сравнение с спецификацией**:
   - Загрузить релевантные спецификации
   - Сравнить фактическое поведение с описанным в spec.md
   - Идентифицировать расхождения

2. **Поиск противоречий**:
   - Проверить на противоречия между разными spec.md
   - Проверить на противоречия между spec.md и constitution.md
   - Проверить на противоречия между spec.md и plan.md

3. **Анализ устаревших требований**:
   - Проверить даты последнего обновления спецификаций
   - Идентифицировать устаревшие требования
   - Сравнить с актуальным кодом

4. **Анализ неоднозначностей**:
   - Найти неоднозначные формулировки в spec.md
   - Идентифицировать edge cases, не описанные в спецификации
   - Проверить отсутствие важных деталей реализации

### Output

**`.fix/theories/spec_theory.md`**:
```markdown
# Specification Theory

## Hypothesis

Баг вызван [некорректной/устаревшей/неоднозначной] спецификацией.

## Evidence

### Spec Contradictions

1. **Contradiction in spec.md**:
   - Location: `.specify/specs/[feature]/spec.md`, section [X]
   - Statement 1: "[цитата]"
   - Statement 2: "[цитата]"
   - Conflict: [описание конфликта]

### Outdated Requirements

1. **Requirement**: [описание]
   - Specified: [дата/версия]
   - Current implementation: [описание]
   - Mismatch: [описание]

### Ambiguous Requirements

1. **Ambiguity**: [описание]
   - Spec location: [путь]
   - Current behavior: [описание]
   - Expected behavior (unclear): [описание]

## Expected Behavior Mismatch

- **According to spec**: [поведение из спецификации]
- **Actual behavior**: [фактическое поведение]
- **Bug behavior aligns with**: [spec | implementation]

## Constitution Alignment

- **Principle violated**: [номер и название принципа]
- **Violation details**: [описание]

## Confidence Score

**[0-100]%**

### Reasoning

[Подробное объяснение оценки уверенности]

## Recommended Action

- [ ] Update specification
- [ ] Clarify ambiguous requirements
- [ ] Resolve contradictions
- [ ] Update implementation to match spec
```

### Confidence Scoring

- **90-100%**: Явное противоречие в спецификации, четкая документация
- **70-89%**: Устаревшие требования, неоднозначности с вероятным решением
- **50-69%**: Возможное расхождение между spec и реализацией
- **0-49%**: Слабые доказательства проблем в спецификации

## Agent 3: ImplTheoryAgent

**Цель**: Определить, является ли баг результатом некорректной реализации.

### Inputs

- `.fix/bug_context/*`
- Исходный код (релевантные файлы)
- Git history (релевантные коммиты)
- Test logs

### Analysis Steps

1. **Инспекция code paths**:
   - Анализ call graph для затронутых методов
   - Трассировка выполнения от точки входа до точки failure
   - Идентификация подозрительных участков кода

2. **Анализ edge cases**:
   - Проверка граничных условий (null, empty, max values)
   - Анализ обработки ошибок
   - Проверка race conditions (если async)
   - Проверка thread safety (если многопоточность)

3. **Git history анализ**:
   - Поиск недавних изменений в релевантных файлах
   - Анализ коммитов, которые могли ввести баг
   - Проверка связанных PR и их ревью

4. **Логические несоответствия**:
   - Поиск логических ошибок (off-by-one, неправильные операторы)
   - Проверка порядка операций
   - Анализ инвариантов и их нарушений

### Output

**`.fix/theories/impl_theory.md`**:
```markdown
# Implementation Theory

## Hypothesis

Баг вызван [логической ошибкой/edge case/race condition/недавним изменением] в реализации.

## Location

### Primary Location

- **File**: `Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs`
- **Namespace**: `NovelCore.Runtime.[Component]`
- **Class**: `[ClassName]`
- **Method**: `[MethodName]()`
- **Line Range**: [start]-[end]

### Secondary Locations (if applicable)

- **File**: [путь]
- **Reason**: [связь с primary location]

## Root Cause Hypothesis

### Category

- [ ] Logical error (off-by-one, wrong operator, incorrect condition)
- [ ] Edge case not handled (null, empty collection, boundary value)
- [ ] Race condition (async/threading issue)
- [ ] State management error (incorrect state transitions)
- [ ] Memory/resource leak
- [ ] API misuse (incorrect usage of Unity/library APIs)
- [ ] Other: [описание]

### Detailed Explanation

[Подробное объяснение, как баг возникает]

```csharp
// Suspected problematic code
[код из файла, который предположительно содержит баг]
```

**Issue**: [описание проблемы в коде]

### Call Stack Analysis

```
[Entry Point]
  ↓
[Intermediate Method 1]
  ↓
[Intermediate Method 2]
  ↓
[Bug Location]
```

**Analysis**: [анализ call stack]

## Git History Insights

### Recent Changes

1. **Commit**: [hash] - [date]
   - **Author**: [автор]
   - **Message**: [сообщение]
   - **Changes**: [релевантные изменения]
   - **Suspicion Level**: [LOW/MEDIUM/HIGH]
   - **Reason**: [почему подозрительный]

## Edge Cases Analysis

1. **Edge Case**: [описание]
   - **Current handling**: [как обрабатывается]
   - **Problem**: [почему это баг]

## Confidence Score

**[0-100]%**

### Reasoning

[Подробное объяснение оценки уверенности]

## Recommended Fix Approach

[Краткое описание подхода к исправлению]

```csharp
// Proposed fix (conceptual)
[концептуальный код исправления]
```
```

### Confidence Scoring

- **90-100%**: Явная логическая ошибка, воспроизводимая в дебаггере
- **70-89%**: Подозрительный код с недавними изменениями
- **50-69%**: Вероятная проблема с edge cases
- **0-49%**: Слабые доказательства проблем в реализации

## Agent 4: TheoryArbitratorAgent

**Цель**: Выбрать наиболее согласованную теорию происхождения бага.

### Inputs

- `.fix/theories/spec_theory.md`
- `.fix/theories/impl_theory.md`
- `.fix/bug_context/failing_test.cs`

### Responsibilities

1. **Сравнение гипотез**:
   - Сравнить confidence scores обеих теорий
   - Оценить качество доказательств
   - Проверить согласованность с failing test

2. **Оценка противоречий**:
   - Выявить противоречия между теориями
   - Определить, могут ли обе теории быть верны одновременно
   - Приоритизировать теорию с более сильными доказательствами

3. **Валидация против failing test**:
   - Проверить, какая теория лучше объясняет поведение failing test
   - Убедиться, что выбранная теория покрывает все симптомы бага

4. **Генерация диагностических тестов** (опционально):
   - Если теории противоречат друг другу → создать тесты для разрешения
   - Тесты должны дать разные результаты в зависимости от верной теории

### Output

**`.fix/theories/selected_theory.md`**:
```markdown
# Selected Theory

## Decision

**Selected**: [SPEC_THEORY | IMPL_THEORY]

## Reasoning

### Theory Comparison

| Aspect                | Spec Theory      | Impl Theory      |
|-----------------------|------------------|------------------|
| Confidence Score      | [X]%             | [Y]%             |
| Evidence Quality      | [HIGH/MED/LOW]   | [HIGH/MED/LOW]   |
| Failing Test Alignment| [STRONG/WEAK]    | [STRONG/WEAK]    |
| Constitution Alignment| [ALIGNED/VIOLATION] | [ALIGNED/VIOLATION] |

### Decision Rationale

[Подробное объяснение, почему выбрана данная теория]

1. **Evidence Strength**: [анализ]
2. **Test Alignment**: [анализ]
3. **Root Cause Coverage**: [анализ]

### Rejected Theory

**Rejected**: [SPEC_THEORY | IMPL_THEORY]

**Reason for Rejection**:
[Почему отклонена альтернативная теория]

## Confidence in Decision

**[0-100]%**

### Justification

[Обоснование уверенности в решении]

## Recommended Fix Strategy

[Стратегия исправления на основе выбранной теории]

### If Spec Theory Selected

- [ ] Update `.specify/specs/[feature]/spec.md`
- [ ] Clarify ambiguous requirements
- [ ] Update constitution if needed
- [ ] Update implementation to match corrected spec

### If Impl Theory Selected

- [ ] Fix code at identified location
- [ ] Add edge case handling
- [ ] Refactor if architectural issue
- [ ] Add regression tests

## Next Steps

1. [Шаг 1]
2. [Шаг 2]
3. [Шаг 3]
```

**`.fix/tests/diagnostic_tests.cs`** (если сгенерированы):
```csharp
using NUnit.Framework;
using NovelCore.Runtime.[Component];

namespace NovelCore.Tests.Runtime.[Component]
{
    [TestFixture]
    public class [BugName]DiagnosticTests
    {
        [Test]
        public void [Scenario]_If_SpecTheory_Correct_Should_[Behavior]()
        {
            // Arrange
            // ... setup

            // Act
            // ... execute scenario

            // Assert
            // If spec theory correct, this should pass/fail
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void [Scenario]_If_ImplTheory_Correct_Should_[Behavior]()
        {
            // Arrange
            // ... setup

            // Act
            // ... execute scenario

            // Assert
            // If impl theory correct, this should pass/fail
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
```

### Decision Rules

1. **High Confidence Theory (>80%)**: Выбрать теорию с highest confidence
2. **Close Confidence (<20% difference)**: Генерировать диагностические тесты
3. **Both Low Confidence (<50%)**: Запросить дополнительную информацию
4. **Contradictory Evidence**: Рассмотреть гибридную теорию (баг и в spec, и в impl)

## Agent 5: FixAgent

**Цель**: Сгенерировать рабочий патч на основе выбранной теории.

### Inputs

- `.fix/theories/selected_theory.md`
- `.fix/bug_context/failing_test.cs`
- `.fix/tests/diagnostic_tests.cs` (если есть)

### Algorithm

```
MAX_ITERATIONS = 10
iteration = 1

while iteration <= MAX_ITERATIONS:
    1. Generate patch based on selected theory
    2. Apply patch (in-memory or on disk)
    3. Run build (Unity compilation check)
    
    if build fails:
        log error
        revise patch
        continue
    
    4. Run failing_test.cs
    5. Run diagnostic_tests.cs (if exist)
    6. Run related tests (find via SemanticSearch)
    
    if all tests pass:
        break  # Success!
    
    if iteration == MAX_ITERATIONS:
        send feedback to TheoryArbitratorAgent
        request revised theory
        break
    
    iteration++
```

### Implementation Steps

1. **Анализ fix strategy** (из selected_theory.md)
2. **Генерация патча**:
   - Если SPEC_THEORY → обновить spec.md и затем код
   - Если IMPL_THEORY → исправить код напрямую
3. **Применение патча** (используя StrReplace tool)
4. **Компиляция** (Unity batch mode check):
   ```bash
   /Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
     -quit -batchmode -nographics \
     -projectPath "$(pwd)/novel-core" \
     -logFile "$(pwd)/.fix/patch/unity_compile_iter[N].log" 2>&1
   ```
5. **Запуск тестов** (Unity Test Runner):
   ```bash
   /Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
     -batchmode -nographics \
     -projectPath "[project_path]" \
     -runTests -testPlatform EditMode \
     -testResults ".fix/patch/test_results_iter[N].xml" \
     -logFile ".fix/patch/unity_tests_iter[N].log"
   ```

### Constraints

- **Patch Size Limit**: Максимум 200 строк изменений (configurable)
- **Minimal Modification**: Предпочтение минимальным изменениям
- **Constitution Compliance**: Патч должен соответствовать constitution
- **Code Style**: Патч должен соответствовать Code Style Standards (Principle VI)

### Output

**`.fix/patch/patch.diff`**:
```diff
diff --git a/Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs b/Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs
index abc123..def456 100644
--- a/Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs
+++ b/Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs
@@ -10,7 +10,7 @@ namespace NovelCore.Runtime.[Component]
     public void [Method]()
     {
-        // Old code
+        // Fixed code
     }
```

**`.fix/patch/fix_summary.md`**:
```markdown
# Fix Summary

## Root Cause

[Краткое описание root cause]

## Patch Explanation

### Changes Made

1. **File**: `Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs`
   - **Line**: [номер строки]
   - **Change**: [описание изменения]
   - **Reason**: [почему это исправляет баг]

### Files Modified

- `[путь к файлу 1]` ([X] lines changed)
- `[путь к файлу 2]` ([Y] lines changed)

## Test Results

### Iteration [N]

- **Build Status**: ✅ SUCCESS
- **Failing Test**: ✅ PASSED
- **Diagnostic Tests**: ✅ PASSED (if applicable)
- **Related Tests**: ✅ PASSED

### Test Execution Log

```
[Timestamp] Running failing_test.cs...
[Timestamp] ✅ [TestName] PASSED
[Timestamp] Running diagnostic_tests.cs...
[Timestamp] ✅ [TestName] PASSED
[Timestamp] Running related tests...
[Timestamp] ✅ [TestName] PASSED
```

## Constitution Compliance

- **Principle VI (Testing)**: ✅ Tests written and passing
- **Code Style Standards**: ✅ Allman braces, underscore fields
- **AI Development Constraints**: ✅ Only .cs files modified

## Confidence

**[0-100]%** that this fix resolves the bug without introducing regressions.
```

### Iteration Tracking

**`.fix/patch/iterations.log`**:
```
Iteration 1: Build FAILED - CS0103 error at line 42
Iteration 2: Build SUCCESS, Tests FAILED - NullReferenceException
Iteration 3: Build SUCCESS, Tests SUCCESS - Fix complete
```

## Agent 6: BreakAgent

**Цель**: Попытаться сломать исправление с помощью adversarial и edge case тестов.

### Inputs

- `.fix/patch/patch.diff`
- Existing test suite
- Code coverage reports (if available)

### Responsibilities

1. **Генерация adversarial тестов**:
   - Создать тесты, специально пытающиеся сломать исправление
   - Протестировать граничные условия
   - Проверить edge cases, не покрытые failing_test

2. **Генерация edge case тестов**:
   - Null inputs
   - Empty collections
   - Maximum/minimum values
   - Concurrent access (если применимо)
   - Platform-specific scenarios

3. **Запуск полного test suite**:
   - EditMode tests
   - PlayMode tests (если применимо)
   - Integration tests

### Implementation

1. **Генерация break tests**:
   ```csharp
   [TestFixture]
   public class [BugName]BreakTests
   {
       [Test]
       public void [Method]_WithNullInput_ShouldNotThrow()
       {
           // Try to break the fix with null
       }

       [Test]
       public void [Method]_WithEmptyCollection_ShouldHandleGracefully()
       {
           // Try to break with empty input
       }

       [Test]
       public void [Method]_WithMaxValue_ShouldNotOverflow()
       {
           // Try to break with boundary values
       }
   }
   ```

2. **Запуск тестов**:
   ```bash
   # EditMode tests
   Unity -batchmode -runTests -testPlatform EditMode \
     -testResults ".fix/tests/break_results_editmode.xml"
   
   # PlayMode tests (if needed)
   Unity -batchmode -runTests -testPlatform PlayMode \
     -testResults ".fix/tests/break_results_playmode.xml"
   ```

### Output

**`.fix/tests/break_tests.cs`**:
```csharp
using NUnit.Framework;
using NovelCore.Runtime.[Component];

namespace NovelCore.Tests.Runtime.[Component]
{
    /// <summary>
    /// Adversarial tests attempting to break the fix for [BugName].
    /// </summary>
    [TestFixture]
    public class [BugName]BreakTests
    {
        // Generated adversarial tests
    }
}
```

**`.fix/tests/break_results.md`**:
```markdown
# Break Test Results

## Summary

- **Total Break Tests**: [N]
- **Passed**: [X]
- **Failed**: [Y]
- **Skipped**: [Z]

## Adversarial Tests

### ✅ Passed Tests

1. **[TestName]**
   - **Scenario**: [описание]
   - **Result**: PASSED

### ❌ Failed Tests

1. **[TestName]**
   - **Scenario**: [описание]
   - **Error**: [сообщение об ошибке]
   - **Action Required**: RETURN TO FixAgent

## Edge Case Tests

[аналогично]

## Full Test Suite Results

### EditMode Tests

- **Total**: [N]
- **Passed**: [X]
- **Failed**: [Y]

### PlayMode Tests

- **Total**: [N]
- **Passed**: [X]
- **Failed**: [Y]

## Regression Analysis

- **New Failures**: [список тестов, которые раньше проходили]
- **Root Cause**: [анализ, если есть регрессии]

## Recommendation

- [ ] ✅ APPROVE - All tests passed, no regressions
- [ ] ⚠️ RETURN TO FixAgent - Break tests found issues
- [ ] ❌ ESCALATE - Critical regressions detected
```

### Decision Flow

```
if any break test fails:
    return patch to FixAgent with failure details
else if any existing test now fails:
    return patch to FixAgent with regression details
else:
    approve patch for commit
```

## Commit Stage

**Условия для коммита**: Коммит разрешен ТОЛЬКО если:

1. ✅ Build succeeds (Unity compilation passes)
2. ✅ Failing test now passes
3. ✅ Break tests pass
4. ✅ No compilation errors
5. ✅ No test regressions (все ранее проходящие тесты всё ещё проходят)
6. ✅ Constitution compliance verified

### Commit Format

```bash
git add [modified files]
git add .fix/bug_context/failing_test.cs
git add .fix/tests/break_tests.cs

git commit -m "$(cat <<'EOF'
fix: resolve [краткое описание бага]

Root Cause:
[1-2 предложения о root cause]

Changes:
- [файл 1]: [изменение]
- [файл 2]: [изменение]

Tests:
- Added failing reproduction test
- Added [N] break tests for edge cases
- All tests passing (EditMode: [X]/[X], PlayMode: [Y]/[Y])

Closes: [issue ID if applicable]

Constitution Compliance:
- Principle VI (Testing): Tests added and passing
- Code Style: Allman braces, underscore prefix enforced
EOF
)"
```

### Post-Commit Actions

1. **Archive fix documentation**:
   - Сохранить `.fix/*` в `.specify/memory/fixes/YYYY-MM/DD-bug-name/`
   - Naming format: `DD-component-issue-keyword` (e.g., `10-scene-manager-background-null`)
   - Update `.specify/memory/fixes/README.md` index
   - Run `.specify/scripts/update-fix-index.sh` для автоматического обновления

2. **Check archiving period** (if needed):
   - Count active fixes: `find .specify/memory/fixes -type d -name "[0-9][0-9]-*" | wc -l`
   - If >100 fixes → run `.specify/scripts/archive-old-fixes.sh`
   - If directory size >500MB → run archiving immediately

3. **Constitution review** (MANDATORY after each fix):
   - Review `.fix/theories/impl_theory.md` and `.fix/theories/spec_theory.md`
   - Check if fix revealed:
     - **API misuse pattern** → add best practices to Constitution
     - **Recurring bug category** → add validation requirement to Constitution
     - **Missing guidance** → add explicit rules to Constitution
   - If 3+ similar fixes found → MUST update Constitution with preventive guidance
   - Example patterns requiring Constitution update:
     - Multiple VContainer registration errors → add VContainer best practices
     - Multiple DontDestroyOnLoad missing → add GameObject lifecycle rules
     - Multiple async/await issues → add async patterns guidance
     - Multiple null checks missing → add defensive programming requirements

4. **Plan review** (if SPEC_THEORY selected):
   - Check if spec.md needs updates (contradictions, outdated requirements)
   - Update plan.md if implementation approach changed
   - Propagate changes to related specs

5. **Monthly pattern analysis** (1st of each month):
   - Review `.specify/memory/fixes/YYYY-MM/README.md`
   - Identify recurring patterns (>3 instances)
   - Update Constitution with preventive guidance
   - Consider refactoring if component has >10 fixes

6. **Update documentation** (если нужно):
   - Обновить `.specify/memory/testing-strategy.md` (если добавлен новый паттерн тестирования)
   - Обновить spec.md (если был выбран SPEC_THEORY)

3. **Report to user**:
   ```markdown
   ## ✅ Fix Complete

   **Bug**: [описание]
   **Root Cause**: [краткое описание]
   **Files Modified**: [N] files
   **Tests Added**: [M] tests
   **Commit**: [hash]

   ### Summary
   [краткое резюме исправления]

   ### Test Results
   - ✅ Reproduction test now passes
   - ✅ [N] break tests added and passing
   - ✅ No regressions detected

   ### Next Steps
   - Manually test the fix in Unity Editor (if needed)
   - Close issue [issue ID]
   - Monitor for related bugs
   ```

4. **Constitution Review Check** (MANDATORY):
   - Analyze fix patterns from `.fix/theories/impl_theory.md`
   - Check if similar fixes exist: `grep -r "[root cause category]" .specify/memory/fixes/`
   - If 3+ similar fixes → recommend Constitution update
   - Output:
   ```markdown
   ## 📋 Constitution Review

   **Pattern Detected**: [API misuse / Missing validation / etc.]
   **Occurrences**: [N] similar fixes found
   **Recommendation**: 
   - [ ] Add [specific guidance] to Constitution Principle [X]
   - [ ] Update [spec.md / plan.md] with clarifications
   
   **Auto-check for archiving**:
   - Total active fixes: [N]
   - Action: [None / Run archive-old-fixes.sh]
   ```

## Directory Structure

После выполнения команды `.fix/` директория будет выглядеть так:

```
.fix/
├── agents/                          # Логи агентов (временные)
│   ├── bug_context_agent.log
│   ├── spec_theory_agent.log
│   ├── impl_theory_agent.log
│   ├── theory_arbitrator_agent.log
│   ├── fix_agent.log
│   └── break_agent.log
│
├── bug_context/                     # Контекст бага (сохраняется)
│   ├── bug.md
│   ├── relevant_files.md
│   ├── failing_test.cs
│   └── reproduction_log.txt
│
├── theories/                        # Теории (сохраняются)
│   ├── spec_theory.md
│   ├── impl_theory.md
│   └── selected_theory.md
│
├── tests/                           # Тесты (сохраняются)
│   ├── diagnostic_tests.cs          # (если сгенерированы)
│   ├── break_tests.cs
│   ├── break_results.md
│   ├── break_results_editmode.xml
│   └── break_results_playmode.xml
│
└── patch/                           # Патч (сохраняется)
    ├── patch.diff
    ├── fix_summary.md
    ├── iterations.log
    ├── unity_compile_iter1.log
    ├── unity_compile_iter2.log
    ├── test_results_iter1.xml
    ├── test_results_iter2.xml
    ├── unity_tests_iter1.log
    └── unity_tests_iter2.log
```

После коммита перемещается в:
```
.specify/memory/fixes/
└── YYYY-MM/                    # Month directory
    └── DD-bug-name/            # Fix directory
        ├── bug_context/
        ├── theories/
        ├── tests/
        └── patch/
```

**Example**:
```
.specify/memory/fixes/2026-03/10-audio-di-null/
```

## Execution Constraints

### Max Fix Iterations
- **Default**: 10
- **Configurable**: через `.specify/config/fix.json`

### Max Patch Size
- **Default**: 200 lines
- **Configurable**: через `.specify/config/fix.json`

### Mandatory Checks
1. Build success (Unity compilation)
2. Test success (failing test passes)
3. Deterministic reproduction (тест падает стабильно до фикса)

## Failure Conditions

Pipeline останавливается, если:

1. **Bug cannot be reproduced**:
   - После 3 попыток BugContextAgent не может создать failing test
   - **Action**: Вернуть для ручной отладки с запросом дополнительной информации

2. **Conflicting theories cannot be resolved**:
   - TheoryArbitratorAgent не может выбрать между SPEC и IMPL теориями
   - Diagnostic tests дают противоречивые результаты
   - **Action**: Запросить помощь у пользователя

3. **Fix iterations exceed limit**:
   - FixAgent исчерпал MAX_ITERATIONS без успешного патча
   - **Action**: Вернуть пользователю лучший патч + лог попыток для ручной доработки

4. **Critical regressions detected**:
   - BreakAgent обнаружил, что фикс сломал critical functionality
   - **Action**: Откатить патч, вернуть для пересмотра теории

В этих случаях:
```markdown
## ⚠️ Manual Review Required

**Reason**: [причина остановки]

**Collected Evidence**:
- Bug context: `.fix/bug_context/`
- Theories: `.fix/theories/`
- Attempted fixes: `.fix/patch/iterations.log`

**Recommended Next Steps**:
[конкретные рекомендации]

**Request**:
[что нужно от пользователя для продолжения]
```

## Expected Benefits

1. **Детерминированная отладка**: Воспроизводимый failing test обеспечивает надежность
2. **Test-driven bug fixing**: Тест создается до исправления
3. **Spec-driven validation**: Проверка соответствия спецификациям
4. **Reduced hallucinated fixes**: Теории основаны на доказательствах, не догадках
5. **Automatic regression protection**: Break tests + full test suite предотвращают регрессии
6. **Knowledge preservation**: Весь процесс документируется в `.specify/memory/fixes/`

## Integration with Spec-Kit

### Handoffs

1. **From /speckit.fix to /speckit.tasks**:
   - После успешного fix создать задачи для regression tests
   - Передать `.fix/tests/break_tests.cs` для интеграции в test suite

2. **From /speckit.fix to /speckit.constitution**:
   - Если SPEC_THEORY выбрана → проверить constitution alignment
   - Обновить constitution если найдены новые принципы

3. **From /speckit.fix to /speckit.specify**:
   - Если SPEC_THEORY выбрана → обновить spec.md
   - Создать новую спецификацию для недокументированного поведения

## Configuration

**`.specify/config/fix.json`** (опционально):
```json
{
  "max_fix_iterations": 10,
  "max_patch_size_lines": 200,
  "break_tests": {
    "enabled": true,
    "generate_edge_cases": true,
    "generate_adversarial": true
  },
  "test_execution": {
    "editmode_first": true,
    "playmode_if_needed": true,
    "timeout_seconds": 300
  },
  "commit": {
    "auto_commit": false,
    "require_manual_approval": true
  }
}
```

## Usage Examples

### Example 1: Simple Bug Fix

```bash
/speckit.fix Dialogue text не отображается после загрузки сохранения
```

**Pipeline**:
1. BugContextAgent → создает failing test `DialogueSystem_LoadSave_TextNotDisplayed()`
2. SpecTheoryAgent → анализирует spec, confidence 30%
3. ImplTheoryAgent → находит пропущенную инициализацию в `LoadDialogueState()`, confidence 85%
4. TheoryArbitratorAgent → выбирает IMPL_THEORY
5. FixAgent → добавляет `InitializeTextRenderer()` после `LoadDialogueState()`, тест проходит за iteration 1
6. BreakAgent → генерирует 5 edge case tests, все проходят
7. Commit → фикс закоммичен с regression tests

### Example 2: Spec Issue

```bash
/speckit.fix Scene transitions не соответствуют UX-спецификации
```

**Pipeline**:
1. BugContextAgent → создает failing test `SceneManager_Transition_Duration_ShouldMatch_Spec()`
2. SpecTheoryAgent → находит противоречие: spec.md говорит "1 second", plan.md говорит "500ms", confidence 90%
3. ImplTheoryAgent → код реализует 500ms правильно, confidence 40%
4. TheoryArbitratorAgent → выбирает SPEC_THEORY
5. FixAgent → обновляет spec.md (унификация требований), затем код для соответствия
6. BreakAgent → тесты проходят
7. Commit → обновлены spec.md и SceneManager.cs

### Example 3: Cannot Reproduce

```bash
/speckit.fix Редкий краш при выборе диалога
```

**Pipeline**:
1. BugContextAgent → пытается создать failing test, но краш не воспроизводится стабильно
2. Попытка 2 → добавляет thread stress test, всё ещё нестабильно
3. Попытка 3 → не удалось воспроизвести

**Output**:
```markdown
## ⚠️ Manual Review Required

**Reason**: Cannot create deterministic reproduction

**Attempted Approaches**:
1. Direct scenario test → non-deterministic
2. Thread stress test → flaky results
3. Memory pressure test → no crash

**Request**:
Пожалуйста, предоставьте:
- Unity Editor logs (Editor.log)
- Crash dump (if available)
- Точные шаги воспроизведения (environment, input sequence)
- Частота краша (1 из N попыток)
```

## Notes

- Используйте абсолютные пути для всех файлов
- Все тесты должны соответствовать Principle VI (EditMode-first strategy)
- Логи Unity компиляции и тестов сохранять в `.fix/patch/`
- После успешного fix сохранить документацию в `.specify/memory/fixes/`
- Если пользователь явно указывает root cause, BugContextAgent может пропустить theory agents
