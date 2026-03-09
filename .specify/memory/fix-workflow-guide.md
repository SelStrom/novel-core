# Spec-Kit Fix Workflow Guide

**Version**: 1.0.0  
**Last Updated**: 2026-03-09

## Обзор

Команда `/speckit.fix` предоставляет автоматизированный pipeline для исправления багов с использованием специализированных агентов. Процесс детерминированный, воспроизводимый и соответствует Principle VI (Modular Architecture & Testing) из constitution.

## Когда использовать /speckit.fix

### ✅ Подходит для

- **Воспроизводимые баги**: Баг можно стабильно воспроизвести
- **Ясные симптомы**: Понятно, что работает неправильно
- **Локализованный scope**: Баг затрагивает конкретный компонент/модуль
- **Есть failing test (или можно создать)**: Можно написать тест, который падает до исправления
- **Логические ошибки**: Off-by-one, неправильные условия, edge cases
- **Проблемы спецификации**: Противоречия в spec.md, неоднозначности

### ❌ НЕ подходит для

- **Flaky bugs**: Баг проявляется нестабильно (race conditions с плохой воспроизводимостью)
- **Системные проблемы**: Проблемы с окружением, Unity Editor, hardware
- **Требования новых фич**: Это не баг, а отсутствие функциональности
- **Performance tuning**: Оптимизация производительности требует профилирования, не fix pipeline
- **Масштабные рефакторинги**: Архитектурные изменения лучше планировать через /speckit.plan

## Workflow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│ Пользователь: /speckit.fix "Описание бага"                  │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ Setup: Инициализация .fix/ структуры                        │
│ - setup-fix.sh --bug-id [auto-generated]                    │
│ - Создание bug_context/, theories/, tests/, patch/         │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ AGENT 1: BugContextAgent                                    │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ 1. Парсинг описания бага                                │ │
│ │ 2. Локализация кода (SemanticSearch)                    │ │
│ │ 3. Создание failing_test.cs                            │ │
│ │ 4. Валидация воспроизведения (тест падает?)            │ │
│ └─────────────────────────────────────────────────────────┘ │
│ Output: .fix/bug_context/{bug.md, failing_test.cs, ...}    │
└─────────────────────────────────────────────────────────────┘
                             ↓
        ┌────────────────────┴────────────────────┐
        ↓                                         ↓
┌──────────────────────┐              ┌──────────────────────┐
│ AGENT 2:             │              │ AGENT 3:             │
│ SpecTheoryAgent      │ (параллельно)│ ImplTheoryAgent      │
│ ┌──────────────────┐ │              │ ┌──────────────────┐ │
│ │ Анализ spec.md   │ │              │ │ Анализ кода      │ │
│ │ Противоречия     │ │              │ │ Call stack       │ │
│ │ Неоднозначности  │ │              │ │ Edge cases       │ │
│ │ Constitution     │ │              │ │ Git history      │ │
│ └──────────────────┘ │              │ └──────────────────┘ │
│ Output: spec_theory  │              │ Output: impl_theory  │
│ (confidence: X%)     │              │ (confidence: Y%)     │
└──────────────────────┘              └──────────────────────┘
        │                                         │
        └────────────────────┬────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ AGENT 4: TheoryArbitratorAgent                              │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ 1. Сравнение confidence scores                          │ │
│ │ 2. Оценка качества доказательств                        │ │
│ │ 3. Проверка согласованности с failing test             │ │
│ │ 4. Генерация диагностических тестов (если нужно)       │ │
│ └─────────────────────────────────────────────────────────┘ │
│ Output: .fix/theories/selected_theory.md                    │
│ Decision: [SPEC_THEORY | IMPL_THEORY | HYBRID]             │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ AGENT 5: FixAgent (iterative, max 10 iterations)            │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ Loop:                                                   │ │
│ │   1. Generate patch (based on selected theory)         │ │
│ │   2. Apply patch (StrReplace)                          │ │
│ │   3. Run Unity compilation (batch mode)                │ │
│ │   4. Run failing_test.cs                               │ │
│ │   5. Run diagnostic_tests.cs (if exist)                │ │
│ │   6. If all pass → break                               │ │
│ │   7. If iteration == 10 → escalate                     │ │
│ └─────────────────────────────────────────────────────────┘ │
│ Output: .fix/patch/{patch.diff, fix_summary.md, ...}       │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ AGENT 6: BreakAgent                                         │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ 1. Генерация adversarial tests (null, boundary, etc.)  │ │
│ │ 2. Генерация edge case tests                           │ │
│ │ 3. Запуск full test suite (EditMode + PlayMode)        │ │
│ │ 4. Regression analysis                                  │ │
│ └─────────────────────────────────────────────────────────┘ │
│ Output: .fix/tests/{break_tests.cs, break_results.md}      │
│ Decision: [APPROVE | RETURN_TO_FIX_AGENT | ESCALATE]       │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ Commit Stage (только если все проверки пройдены)            │
│ ✅ Build succeeds                                           │
│ ✅ Failing test passes                                      │
│ ✅ Break tests pass                                         │
│ ✅ No regressions                                           │
│ ✅ Constitution compliance                                  │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ 1. git add [modified files]                             │ │
│ │ 2. git add .fix/bug_context/failing_test.cs → tests/   │ │
│ │ 3. git add .fix/tests/break_tests.cs → tests/          │ │
│ │ 4. git commit -m "fix: [description]"                   │ │
│ │ 5. Archive to .specify/memory/fixes/[bug-id]-[date]/   │ │
│ │ 6. Cleanup .fix/ directory                              │ │
│ └─────────────────────────────────────────────────────────┘ │
│ Output: Коммит + архив + отчёт пользователю                │
└─────────────────────────────────────────────────────────────┘
                             ↓
                    ✅ Fix Complete!
```

## Примеры использования

### Пример 1: Простой логический баг

**Ситуация**: Dialogue text не отображается после загрузки сохранения.

```bash
/speckit.fix Dialogue text не отображается после загрузки сохранения
```

**Pipeline**:

1. **BugContextAgent** → создаёт failing test:
   ```csharp
   [Test]
   public void DialogueSystem_LoadSave_TextNotDisplayed()
   {
       // Arrange
       var dialogueSystem = new DialogueSystem();
       dialogueSystem.StartDialogue("Hello");
       var saveData = dialogueSystem.Save();
       
       // Act
       var newDialogueSystem = new DialogueSystem();
       newDialogueSystem.Load(saveData);
       
       // Assert
       Assert.That(newDialogueSystem.CurrentText, Is.EqualTo("Hello"));
       // FAILS: CurrentText is null
   }
   ```

2. **SpecTheoryAgent** → анализирует spec.md, confidence 30% (spec не противоречива)

3. **ImplTheoryAgent** → находит пропущенную инициализацию в `LoadDialogueState()`:
   ```csharp
   public void Load(SaveData data)
   {
       _currentLine = data.currentLine;
       // BUG: забыли вызвать InitializeTextRenderer()
   }
   ```
   Confidence: 85%

4. **TheoryArbitratorAgent** → выбирает IMPL_THEORY (85% > 30%)

5. **FixAgent** → iteration 1:
   ```csharp
   public void Load(SaveData data)
   {
       _currentLine = data.currentLine;
       InitializeTextRenderer(); // FIX
   }
   ```
   Тест проходит! ✅

6. **BreakAgent** → генерирует 5 edge case tests:
   - `Load_WithNullData_ShouldThrow()`
   - `Load_WithEmptyDialogue_ShouldNotCrash()`
   - `Load_MultipleTimes_ShouldResetState()`
   - Все проходят ✅

7. **Commit** → `fix: resolve dialogue text missing after load`

**Итог**: Баг исправлен за 1 iteration, 6 тестов добавлены, 0 regressions.

---

### Пример 2: Проблема спецификации

**Ситуация**: Scene transitions не соответствуют UX-спецификации.

```bash
/speckit.fix Scene transitions не соответствуют UX-спецификации (длительность)
```

**Pipeline**:

1. **BugContextAgent** → создаёт failing test:
   ```csharp
   [Test]
   public void SceneManager_Transition_Duration_ShouldMatch_Spec()
   {
       // Act
       var duration = SceneManager.TransitionDuration;
       
       // Assert (from spec.md)
       Assert.That(duration, Is.EqualTo(1.0f), "Spec requires 1 second transition");
       // FAILS: duration is 0.5f
   }
   ```

2. **SpecTheoryAgent** → находит противоречие:
   - `spec.md` (section 3.2): "Scene transitions MUST be 1 second"
   - `plan.md` (section 4.1): "Transition duration: 500ms"
   - Confidence: 90%

3. **ImplTheoryAgent** → код реализует 0.5s правильно (согласно plan.md):
   ```csharp
   public float TransitionDuration => 0.5f; // Matches plan.md
   ```
   Confidence: 40%

4. **TheoryArbitratorAgent** → выбирает SPEC_THEORY (90% > 40%)

5. **FixAgent** → iteration 1:
   - **Шаг 1**: Обновляет `spec.md`:
     ```markdown
     ## Scene Transitions
     
     - Duration: **1 second** (aligned with UX research)
     ```
   - **Шаг 2**: Обновляет `plan.md`:
     ```markdown
     ### Technical Decisions
     
     - Transition duration: **1.0s** (per spec.md section 3.2)
     ```
   - **Шаг 3**: Обновляет код:
     ```csharp
     public float TransitionDuration => 1.0f; // Aligned with spec
     ```
   Тест проходит! ✅

6. **BreakAgent** → проверяет, что изменение не сломало другие сцены ✅

7. **Commit** → `fix: align scene transition duration with UX spec`

**Итог**: Противоречие в спецификации разрешено, spec + plan + impl синхронизированы.

---

### Пример 3: Не удалось воспроизвести

**Ситуация**: Редкий краш при выборе диалога.

```bash
/speckit.fix Редкий краш при выборе диалога (NullReferenceException в ChoiceHandler)
```

**Pipeline**:

1. **BugContextAgent** → попытка 1:
   ```csharp
   [Test]
   public void ChoiceHandler_SelectChoice_ShouldNotCrash()
   {
       // Arrange
       var handler = new ChoiceHandler();
       handler.AddChoice("Choice 1");
       
       // Act
       handler.SelectChoice(0);
       
       // Assert
       Assert.DoesNotThrow(() => handler.SelectChoice(0));
   }
   ```
   Тест проходит (краш не воспроизводится) ❌

2. **BugContextAgent** → попытка 2 (thread stress test):
   ```csharp
   [Test]
   public void ChoiceHandler_SelectChoice_ConcurrentAccess_ShouldNotCrash()
   {
       // Параллельный доступ
   }
   ```
   Тест flaky (иногда падает, иногда нет) ❌

3. **BugContextAgent** → попытка 3 (memory pressure):
   Всё ещё не удалось стабильно воспроизвести ❌

**Output**:
```markdown
## ⚠️ Manual Review Required

**Reason**: Cannot create deterministic reproduction

**Attempted Approaches**:
1. Direct scenario test → non-deterministic
2. Thread stress test → flaky results
3. Memory pressure test → no crash

**Collected Evidence**:
- Bug description: .fix/bug_context/bug.md
- Related files: .fix/bug_context/relevant_files.md
- Attempted tests: .fix/bug_context/attempted_tests.cs

**Request**:
Пожалуйста, предоставьте:
- Unity Editor logs (Editor.log)
- Crash dump (if available)
- Точные шаги воспроизведения (environment, input sequence)
- Частота краша (1 из N попыток)
- Условия проявления (конкретные сцены, состояния)

**Recommended Next Steps**:
1. Добавить extensive logging в ChoiceHandler
2. Проверить race conditions в асинхронных операциях
3. Использовать Unity Profiler для анализа memory allocations
4. Проверить thread safety (если multi-threaded)
```

**Итог**: Pipeline корректно определил, что баг не воспроизводится детерминированно, и запросил дополнительную информацию.

---

## Конфигурация

### .specify/config/fix.json

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
  },
  "theory": {
    "min_confidence_threshold": 50,
    "require_diagnostic_tests_if_close": true,
    "confidence_difference_threshold": 20
  }
}
```

### Настраиваемые параметры

| Параметр                        | Значение по умолчанию | Описание                                          |
|---------------------------------|-----------------------|---------------------------------------------------|
| `max_fix_iterations`            | 10                    | Максимальное количество попыток FixAgent         |
| `max_patch_size_lines`          | 200                   | Максимальный размер патча (строки)               |
| `break_tests.enabled`           | true                  | Генерировать break tests                         |
| `break_tests.generate_edge_cases` | true                | Генерировать edge case tests                     |
| `break_tests.generate_adversarial` | true             | Генерировать adversarial tests                   |
| `test_execution.editmode_first` | true                  | Запускать EditMode tests перед PlayMode         |
| `test_execution.timeout_seconds`| 300                   | Таймаут для test execution (5 минут)             |
| `commit.auto_commit`            | false                 | Автоматически коммитить (требует одобрения)      |
| `theory.min_confidence_threshold` | 50                  | Минимальная confidence для выбора теории         |
| `theory.confidence_difference_threshold` | 20         | Разница confidence для diagnostic tests          |

## Failure Modes и Recovery

### Failure Mode 1: Bug Cannot Be Reproduced

**Симптомы**: BugContextAgent не может создать failing test после 3 попыток.

**Recovery**:
1. Pipeline останавливается
2. Запрашивается дополнительная информация у пользователя
3. Опции:
   - Предоставить больше деталей (logs, crash dumps)
   - Предоставить точные шаги воспроизведения
   - Использовать manual debugging вместо automated pipeline

**Пример output**:
```markdown
⚠️ Manual Review Required

Reason: Cannot create deterministic reproduction
Request: Provide Unity Editor logs, exact steps, frequency
```

---

### Failure Mode 2: Conflicting Theories

**Симптомы**: SpecTheoryAgent и ImplTheoryAgent имеют близкие confidence scores (разница < 20%).

**Recovery**:
1. TheoryArbitratorAgent генерирует diagnostic tests
2. Diagnostic tests выполняются для разрешения конфликта
3. Если diagnostic tests inconclusive → запрос пользователю

**Пример output**:
```markdown
⚠️ Diagnostic Tests Required

Spec Theory: 60% confidence
Impl Theory: 55% confidence
Difference: 5% (< 20% threshold)

Generated Diagnostic Tests:
- .fix/tests/diagnostic_tests.cs

Running tests to resolve conflict...
```

---

### Failure Mode 3: Fix Iterations Exceeded

**Симптомы**: FixAgent исчерпал 10 iterations без успешного патча.

**Recovery**:
1. Pipeline останавливается
2. Возвращается лучший патч + лог попыток
3. Опции:
   - Ручная доработка патча
   - Пересмотр теории (возможно, выбрана неправильная)
   - Запрос помощи у senior developer

**Пример output**:
```markdown
⚠️ Max Iterations Exceeded

Iterations: 10/10
Best Attempt: Iteration 7 (build passed, 2 tests failed)

Logs:
- .fix/patch/iterations.log

Recommended Action:
1. Review iteration 7 patch
2. Manually fix remaining test failures
3. OR request theory revision
```

---

### Failure Mode 4: Critical Regressions

**Симптомы**: BreakAgent обнаружил, что фикс сломал critical functionality.

**Recovery**:
1. Патч откатывается
2. Pipeline возвращается к TheoryArbitratorAgent
3. Теория пересматривается
4. Опции:
   - Выбрать альтернативную теорию
   - Уточнить scope фикса (минимизировать изменения)

**Пример output**:
```markdown
❌ Critical Regression Detected

Fix broke 3 existing tests:
- DialogueSystem_Save_ShouldPersistState (PASS → FAIL)
- DialogueSystem_Load_ShouldRestoreState (PASS → FAIL)
- SceneManager_Transition_ShouldNotCrash (PASS → FAIL)

Action: Reverting patch, requesting theory revision
```

---

## Best Practices

### 1. Предоставляйте детальное описание бага

**✅ Хорошо**:
```
/speckit.fix Dialogue text не отображается после загрузки сохранения.
Ожидаемое поведение: текущая реплика должна отобразиться.
Фактическое: пустой экран, CurrentText = null.
Шаги: 1) Начать диалог 2) Сохранить 3) Загрузить → текст пропал.
```

**❌ Плохо**:
```
/speckit.fix Диалог не работает
```

### 2. Используйте issue ID если доступен

```bash
/speckit.fix #123
```

Pipeline автоматически загрузит описание из issue tracker.

### 3. Укажите компонент если известен

```bash
/speckit.fix [DialogueSystem] Text missing after load
```

Помогает BugContextAgent быстрее локализовать код.

### 4. Проверяйте .fix/ директорию после каждого агента

```bash
# После BugContextAgent
cat .fix/bug_context/bug.md

# После TheoryArbitratorAgent
cat .fix/theories/selected_theory.md

# После FixAgent
cat .fix/patch/fix_summary.md
```

### 5. Используйте configuration для вашего workflow

Для быстрых фиксов:
```json
{
  "max_fix_iterations": 5,
  "break_tests": {
    "enabled": false
  }
}
```

Для critical фиксов:
```json
{
  "max_fix_iterations": 20,
  "break_tests": {
    "enabled": true,
    "generate_edge_cases": true,
    "generate_adversarial": true
  }
}
```

---

## Integration с другими Spec-Kit командами

### /speckit.fix → /speckit.tasks

После успешного fix:

```bash
/speckit.tasks --from-fix .fix/tests/break_tests.cs
```

Создаёт задачи для интеграции regression tests в test suite.

---

### /speckit.fix → /speckit.constitution

Если SPEC_THEORY выбрана и найдено нарушение constitution:

```bash
/speckit.constitution --review-principle VI
```

Проверяет и обновляет constitution если нужно.

---

### /speckit.fix → /speckit.specify

Если SPEC_THEORY выбрана и spec.md обновлена:

```bash
/speckit.specify --update-from-fix .fix/theories/spec_theory.md
```

Синхронизирует спецификацию с исправлением.

---

## Monitoring и Metrics

### Успешность fix pipeline

Track в `.specify/memory/fix-metrics.json`:

```json
{
  "total_fixes": 25,
  "successful_fixes": 22,
  "manual_reviews": 3,
  "average_iterations": 2.4,
  "theory_distribution": {
    "impl_theory": 18,
    "spec_theory": 4,
    "hybrid": 3
  },
  "failure_modes": {
    "cannot_reproduce": 2,
    "iterations_exceeded": 1,
    "conflicting_theories": 0
  }
}
```

### Анализ эффективности

- **Success Rate**: 22/25 = 88%
- **Average Iterations**: 2.4 (хорошо, < 5)
- **Theory Accuracy**: Если impl_theory выбрана → исправление успешно в 90% случаев

---

## Troubleshooting

### Q: FixAgent застрял на одной и той же ошибке

**A**: Проверьте iterations.log:
```bash
cat .fix/patch/iterations.log
```

Если ошибка повторяется → возможно, выбрана неправильная теория.

**Recovery**:
1. Вручную проверьте selected_theory.md
2. Если теория неверна → переключитесь на rejected theory
3. Запустите заново с явным указанием:
   ```bash
   /speckit.fix "Описание бага" --force-theory impl
   ```

---

### Q: Break tests генерируют слишком много false positives

**A**: Настройте break test generation:
```json
{
  "break_tests": {
    "enabled": true,
    "generate_edge_cases": true,
    "generate_adversarial": false  // Отключить adversarial
  }
}
```

---

### Q: Unity compilation timeout

**A**: Увеличьте timeout:
```json
{
  "test_execution": {
    "timeout_seconds": 600  // 10 минут
  }
}
```

---

## Заключение

Команда `/speckit.fix` обеспечивает:

✅ **Детерминированную отладку** через failing tests  
✅ **Test-driven bug fixing** (тесты до исправления)  
✅ **Spec-driven validation** (проверка соответствия спецификациям)  
✅ **Автоматическую regression protection** (break tests + full suite)  
✅ **Knowledge preservation** (вся документация в `.specify/memory/fixes/`)  

Используйте эту команду для систематического исправления багов с гарантией качества и минимизацией регрессий.
