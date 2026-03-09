# Implementation Theory Template

Используйте этот шаблон для документирования теории о проблемах в реализации в `.fix/theories/impl_theory.md`.

---

# Implementation Theory

## Hypothesis

Баг вызван [выберите одно или несколько]:
- [ ] Логической ошибкой (off-by-one, неправильный оператор, некорректное условие)
- [ ] Необработанным edge case (null, empty collection, boundary value)
- [ ] Race condition (async/threading issue)
- [ ] Ошибкой управления состоянием (некорректные state transitions)
- [ ] Утечкой памяти/ресурсов
- [ ] Неправильным использованием API (Unity/library APIs)
- [ ] Нарушением инвариантов
- [ ] Other: [specify]

**Summary**: [1-2 предложения о гипотезе]

## Location

### Primary Location

- **File**: `Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs`
- **Namespace**: `NovelCore.Runtime.[Component]`
- **Class**: `[ClassName]`
- **Method**: `[MethodName]([parameters])`
- **Line Range**: [start]-[end]
- **Code Block**:
```csharp
[код из файла, который предположительно содержит баг]
```

### Secondary Locations (if applicable)

1. **Location #1**:
   - **File**: [путь]
   - **Class**: [имя класса]
   - **Method**: [имя метода]
   - **Reason**: [связь с primary location - e.g., caller, dependency, shared state]

[Повторить для каждой secondary location]

## Root Cause Hypothesis

### Category

- [X] [Selected category from hypothesis above]

### Detailed Explanation

[Подробное объяснение, как баг возникает]

**Problem Statement**:
[Точное описание проблемы в коде]

**How Bug Manifests**:
1. [Шаг 1 - что происходит в коде]
2. [Шаг 2 - как это приводит к неправильному поведению]
3. [Шаг 3 - как это проявляется для пользователя]

### Problematic Code

```csharp
// Current (buggy) code
[код, содержащий баг с комментариями]

// Issue: [детальное объяснение проблемы]
```

**Specific Issue**:
- **Line [N]**: [проблема в конкретной строке]
- **Variable**: `[variableName]` - [проблема с переменной]
- **Condition**: `[condition]` - [почему условие некорректное]

### Expected Behavior

```csharp
// Expected (corrected) code
[как код должен выглядеть, концептуально]

// Fix: [объяснение исправления]
```

## Call Stack Analysis

### Call Graph

```
[Entry Point Method]
  ↓ [file, line]
[Caller Method]
  ↓ [file, line]
[Intermediate Method 1]
  ↓ [file, line]
[Intermediate Method 2]
  ↓ [file, line]
[Bug Location Method] ← BUG HERE
```

### Call Stack Details

1. **Entry Point**: `[FullMethodSignature]`
   - **File**: [путь]
   - **Line**: [номер]
   - **State**: [входное состояние]

2. **Caller**: `[FullMethodSignature]`
   - **File**: [путь]
   - **Line**: [номер]
   - **State Changes**: [как состояние изменяется]

[Повторить для каждого вызова в stack]

### Analysis

**Call Stack Insights**:
[Анализ call stack - откуда пришли некорректные данные, где состояние сломалось, etc.]

**State Propagation**:
[Как ошибочное состояние распространяется по call stack]

**Caller Assumptions**:
[Какие предположения делают вызывающие методы, которые нарушаются]

## Git History Insights

### Recent Changes Analysis

1. **Commit #1**:
   - **Hash**: [commit hash]
   - **Date**: [дата]
   - **Author**: [автор]
   - **Message**: [сообщение коммита]
   - **Relevant Changes**: 
     ```diff
     [diff релевантных изменений]
     ```
   - **Suspicion Level**: [LOW | MEDIUM | HIGH]
   - **Reason**: [почему подозрительный - e.g., изменил граничное условие, ввёл новую логику]

[Повторить для каждого подозрительного коммита]

### Git Blame Analysis

**Lines [X-Y] (buggy code)**:
- **Last Modified**: [дата] by [автор]
- **Commit**: [hash]
- **Original Intent**: [из commit message, что хотели сделать]
- **Actual Result**: [что получилось на самом деле]

## Edge Cases Analysis

### Identified Edge Cases

1. **Edge Case #1**: [описание - e.g., null input]
   - **Current Handling**: [как код обрабатывает этот case]
   - **Problem**: [почему это некорректно]
   - **Test Coverage**: [есть ли тест для этого case - YES/NO]
   - **Relation to Bug**: [как это связано с багом]

[Повторить для каждого edge case]

### Boundary Conditions

| Boundary          | Current Handling      | Issue                | Fix Needed         |
|-------------------|-----------------------|----------------------|--------------------|
| Null input        | [NullRefException/OK] | [проблема]           | [исправление]      |
| Empty collection  | [OK/Crash]            | [проблема]           | [исправление]      |
| Max value         | [OK/Overflow]         | [проблема]           | [исправление]      |
| Min value         | [OK/Underflow]        | [проблема]           | [исправление]      |

## Threading/Async Analysis (if applicable)

### Concurrency Issues

- **Race Condition**: [описание race condition, если применимо]
- **Shared State**: [какое состояние разделяется между потоками]
- **Synchronization**: [какая синхронизация используется или отсутствует]
- **Timing Dependency**: [зависит ли баг от timing]

### Async/Await Issues

- **Async Method**: `[method signature]`
- **Deadlock Risk**: [YES/NO] - [объяснение]
- **Task Cancellation**: [обрабатывается корректно - YES/NO]
- **Exception Handling**: [как обрабатываются async exceptions]

## Performance/Resource Issues (if applicable)

### Memory Leak

- **Resource**: [что утекает - objects, memory, handles]
- **Allocation Point**: [где аллоцируется]
- **Expected Deallocation**: [где должно освобождаться]
- **Actual Problem**: [почему не освобождается]

### Performance Regression

- **Expected Performance**: [O(n), <100ms, etc.]
- **Actual Performance**: [O(n²), 500ms, etc.]
- **Cause**: [почему медленнее]

## API Misuse Analysis

### Unity API Misuse

1. **API**: `[UnityEngine.Something.Method()]`
   - **Current Usage**: 
     ```csharp
     [как используется сейчас]
     ```
   - **Correct Usage**: 
     ```csharp
     [как должно использоваться]
     ```
   - **Documentation Reference**: [ссылка на Unity docs]

### Library API Misuse

[аналогично для сторонних библиотек]

## Invariant Violations

### Class Invariants

1. **Invariant**: [описание инварианта - e.g., "_count should always equal _items.Count"]
   - **Where Violated**: [метод/строка]
   - **How Violated**: [как нарушается]
   - **Consequence**: [последствия нарушения]

### Preconditions/Postconditions

1. **Method**: `[MethodName]`
   - **Precondition**: [что должно быть true перед вызовом]
   - **Actual State**: [что на самом деле]
   - **Violation**: [описание нарушения]

## Confidence Score

**[0-100]%**

### Confidence Breakdown

| Factor                        | Score | Weight | Contribution |
|-------------------------------|-------|--------|--------------|
| Evidence in Code              | [0-10]| 35%    | [X]%         |
| Git History Suspicion         | [0-10]| 20%    | [X]%         |
| Edge Case Coverage            | [0-10]| 20%    | [X]%         |
| Reproducibility in Debugger   | [0-10]| 15%    | [X]%         |
| Spec Compliance               | [0-10]| 10%    | [X]%         |
| **Total**                     |       |        | **[X]%**     |

### Reasoning

[Подробное объяснение оценки уверенности]

**Why High Confidence** (if >70%):
[аргументы за высокую уверенность]

**Why Low Confidence** (if <50%):
[аргументы за низкую уверенность]

**Debugger Evidence**:
[что показал debugger, если использовался]

## Recommended Fix Approach

### Strategy

[Краткое описание стратегии исправления]

### Conceptual Fix

```csharp
// Proposed fix (conceptual)
[концептуальный код исправления с комментариями]

// Explanation:
// - [объяснение, почему это исправляет баг]
// - [объяснение, почему это не сломает другую функциональность]
```

### Fix Impact Analysis

- **Files to Modify**: [список файлов]
- **Methods to Change**: [список методов]
- **Breaking Changes**: [YES/NO] - [объяснение]
- **Refactoring Needed**: [YES/NO] - [объяснение]
- **Test Updates Needed**: [YES/NO] - [какие тесты]

### Alternative Approaches

1. **Alternative #1**: [описание альтернативного подхода]
   - **Pros**: [преимущества]
   - **Cons**: [недостатки]
   - **Preferred**: [YES/NO] - [почему]

[Повторить для каждой альтернативы]

## Related Code Patterns

### Similar Patterns in Codebase

[Есть ли похожие участки кода, которые могут иметь аналогичный баг]

1. **Similar Pattern #1**:
   - **File**: [путь]
   - **Method**: [имя метода]
   - **Similarity**: [чем похож]
   - **Has Same Bug**: [YES/NO/UNKNOWN]
   - **Action**: [нужно ли проверить/исправить]

## Constitution Compliance

### Code Style Violations

- [ ] Missing braces (Principle VI - mandatory braces)
- [ ] Incorrect namespace format (traditional vs file-scoped)
- [ ] Field naming (underscore prefix)
- [ ] Other: [specify]

### Testing Violations

- [ ] Missing unit tests (Principle VI - >80% coverage)
- [ ] Missing integration tests
- [ ] EditMode vs PlayMode incorrect choice
- [ ] Tests not following naming convention

## Supporting Evidence

### Files Analyzed

- [ ] Primary buggy file
- [ ] Caller files (call stack)
- [ ] Dependency files
- [ ] Related test files
- [ ] Git history for relevant files

### Tools Used

- [ ] Unity Debugger
- [ ] Git blame
- [ ] Static code analysis
- [ ] Profiler (for performance issues)
- [ ] Memory profiler (for memory leaks)

### External References

1. **Unity Documentation**: [link to relevant Unity API docs]
2. **Related Issues**: [links to similar issues if any]

## Alternative Explanations

[Почему это НЕ проблема спецификации]

1. **Spec Appears Correct Because**:
   - [аргумент 1]
   - [аргумент 2]

2. **Spec Review Findings**:
   - [что показал анализ спецификации]

## Next Steps

### If Impl Theory is Selected

1. [Шаг 1 - e.g., fix logical error at line X]
2. [Шаг 2 - e.g., add edge case handling]
3. [Шаг 3 - e.g., add regression tests]

### Validation Criteria

- [ ] Logical error fixed
- [ ] All edge cases handled
- [ ] Failing test now passes
- [ ] No regressions in existing tests
- [ ] Code style compliance verified
- [ ] Similar patterns in codebase checked
