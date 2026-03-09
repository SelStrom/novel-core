# Fix Summary Template

Используйте этот шаблон для документирования финального исправления в `.fix/patch/fix_summary.md`.

---

# Fix Summary

## Bug Information

- **Bug ID**: [issue ID or internal tracking number]
- **Bug Title**: [краткое название бага]
- **Reported**: [дата]
- **Severity**: [Critical/High/Medium/Low]
- **Fixed**: [дата]

## Root Cause

### Category

- [ ] Logical error (off-by-one, wrong operator, incorrect condition)
- [ ] Edge case not handled (null, empty collection, boundary value)
- [ ] Race condition (async/threading issue)
- [ ] State management error (incorrect state transitions)
- [ ] Memory/resource leak
- [ ] API misuse (incorrect usage of Unity/library APIs)
- [ ] Specification issue (incorrect/ambiguous requirements)
- [ ] Other: [specify]

### Detailed Explanation

[2-3 параграфа подробного объяснения root cause]

**What Went Wrong**:
[Точное описание, что было неправильно]

**Why It Happened**:
[Объяснение, почему баг возник - недосмотр, неверное предположение, edge case, etc.]

**How It Manifested**:
[Как баг проявлялся для пользователя]

## Patch Explanation

### Summary

[1-2 предложения краткого резюме патча]

### Changes Made

#### File #1: `[Full path to file]`

**Location**: Lines [X]-[Y]

**Before**:
```csharp
[код до исправления]
```

**After**:
```csharp
[код после исправления]
```

**Change Description**:
[Описание изменения]

**Reason**:
[Почему это изменение исправляет баг]

[Повторить для каждого изменённого файла]

### Files Modified

| File                                          | Lines Changed | Type        |
|-----------------------------------------------|---------------|-------------|
| `Assets/Scripts/NovelCore/Runtime/.../X.cs`   | [+X -Y]       | [Fix/Test]  |
| `Assets/Scripts/NovelCore/Tests/.../Test.cs`  | [+X -Y]       | [Test]      |

**Total Files Modified**: [N]
**Total Lines Changed**: [+X -Y]

### Patch Scope

- **Scope**: [MINIMAL | MODERATE | EXTENSIVE]
- **Breaking Changes**: [YES/NO]
- **Refactoring**: [YES/NO]
- **Architecture Impact**: [NONE | LOW | MEDIUM | HIGH]

## Fix Iterations

### Iteration Log

| Iteration | Build | Tests | Issue                          | Resolution                    |
|-----------|-------|-------|--------------------------------|-------------------------------|
| 1         | ❌    | N/A   | CS0103 error at line 42        | Added missing using statement |
| 2         | ✅    | ❌    | NullReferenceException         | Added null check              |
| 3         | ✅    | ✅    | N/A                            | ✅ Fix complete               |

**Total Iterations**: [N]
**Iterations Within Limit**: [YES/NO] (limit: 10)

### Iteration Details

#### Iteration 1
- **Build Status**: [SUCCESS/FAILED]
- **Build Error**: [error message if failed]
- **Resolution**: [what was done to fix build]

#### Iteration 2
- **Build Status**: [SUCCESS/FAILED]
- **Test Status**: [PASSED/FAILED]
- **Test Failures**: [list of failed tests]
- **Resolution**: [what was done to fix tests]

[Повторить для каждой итерации]

#### Final Iteration [N]
- **Build Status**: ✅ SUCCESS
- **Failing Test**: ✅ PASSED
- **Diagnostic Tests**: ✅ PASSED (if applicable)
- **Related Tests**: ✅ PASSED
- **Break Tests**: ✅ PASSED

## Test Results

### Test Execution Summary

| Test Category         | Total | Passed | Failed | Skipped |
|-----------------------|-------|--------|--------|---------|
| Failing Test          | 1     | 1      | 0      | 0       |
| Diagnostic Tests      | [N]   | [X]    | 0      | 0       |
| Related Tests         | [N]   | [X]    | 0      | 0       |
| Break Tests           | [N]   | [X]    | 0      | 0       |
| **Total**             | [N]   | [X]    | **0**  | 0       |

### Test Platform Results

#### EditMode Tests

- **Total**: [N]
- **Passed**: [X]
- **Failed**: 0
- **Duration**: [X]s
- **Results File**: `.fix/patch/test_results_editmode.xml`

#### PlayMode Tests

- **Total**: [N]
- **Passed**: [X]
- **Failed**: 0
- **Duration**: [X]s
- **Results File**: `.fix/patch/test_results_playmode.xml`

### Detailed Test Results

#### Failing Reproduction Test

**Test**: `[Namespace].[TestClass].[TestMethod]()`
- **Status**: ✅ PASSED
- **Before Fix**: ❌ FAILED (Expected: [X], Actual: [Y])
- **After Fix**: ✅ PASSED
- **Execution Time**: [X]ms

#### Diagnostic Tests (if applicable)

1. **Test**: `[TestName]_If_SpecTheory_Correct_Should_[Behavior]()`
   - **Status**: [PASS/FAIL/SKIPPED]
   - **Execution Time**: [X]ms

2. **Test**: `[TestName]_If_ImplTheory_Correct_Should_[Behavior]()`
   - **Status**: [PASS/FAIL/SKIPPED]
   - **Execution Time**: [X]ms

#### Break Tests

1. **Test**: `[TestName]_WithNullInput_ShouldNotThrow()`
   - **Status**: ✅ PASSED
   - **Execution Time**: [X]ms

2. **Test**: `[TestName]_WithEmptyCollection_ShouldHandleGracefully()`
   - **Status**: ✅ PASSED
   - **Execution Time**: [X]ms

[Список всех break tests]

### Regression Analysis

**Regression Tests Run**: [N]
**New Failures**: 0
**Previously Failing Tests Now Passing**: [N] (if any)

**Conclusion**: ✅ No regressions detected

## Constitution Compliance

### Principle Compliance Checklist

- [X] **Principle I (Creator-First Design)**: [compliance notes]
- [X] **Principle II (Cross-Platform Parity)**: [compliance notes]
- [X] **Principle III (Asset Pipeline Integrity)**: [compliance notes]
- [X] **Principle IV (Runtime Performance Guarantees)**: [compliance notes]
- [X] **Principle V (Save System Reliability)**: [compliance notes]
- [X] **Principle VI (Modular Architecture & Testing)**: [compliance notes]
  - [X] Tests written and passing
  - [X] EditMode-first strategy followed
  - [X] >80% code coverage (post-MVP)
  - [X] Call stack analysis performed
- [X] **Principle VII (AI Development Constraints)**: [compliance notes]
  - [X] Only .cs files modified (no .meta, .prefab, .unity files)
  - [X] No direct asset generation
- [X] **Principle VIII (Editor-Runtime Bridge)**: [compliance notes]
- [X] **Principle IX (User Documentation Language)**: [compliance notes]
- [X] **Principle X (File Organization)**: [compliance notes]

### Code Style Compliance

- [X] **Allman Braces**: Opening braces on new lines
- [X] **Mandatory Braces**: All control structures use braces (zero tolerance)
- [X] **Traditional Namespaces**: Block-scoped namespaces (not file-scoped)
- [X] **Field Naming**: Private/protected fields use underscore prefix (`_fieldName`)
- [X] **Var in Loops**: `var` used in foreach/for loops
- [X] **Code Organization**: Fields → Properties → Constructors → Methods

### Violations (if any)

[Если есть нарушения constitution или code style, перечислить и обосновать]

## Fix Confidence

**Overall Confidence**: [0-100]% that this fix resolves the bug without introducing regressions

### Confidence Factors

| Factor                              | Score | Weight | Contribution |
|-------------------------------------|-------|--------|--------------|
| Root Cause Understanding            | [0-10]| 30%    | [X]%         |
| Test Coverage (all tests pass)      | [0-10]| 25%    | [X]%         |
| No Regressions                      | [0-10]| 20%    | [X]%         |
| Code Review (self-review)           | [0-10]| 15%    | [X]%         |
| Break Tests (adversarial)           | [0-10]| 10%    | [X]%         |
| **Total**                           |       |        | **[X]%**     |

### Confidence Justification

**High Confidence Indicators** (if >80%):
- [индикатор 1 - e.g., "Clear root cause identified and fixed"]
- [индикатор 2 - e.g., "All tests including break tests pass"]
- [индикатор 3 - e.g., "No regressions in 50+ existing tests"]

**Moderate Confidence Indicators** (if 60-80%):
- [индикатор 1]
- [индикатор 2]

**Low Confidence Indicators** (if <60%):
- [индикатор 1]
- [индикатор 2]

### Potential Risks

1. **Risk #1**: [описание риска]
   - **Likelihood**: [LOW/MEDIUM/HIGH]
   - **Impact**: [LOW/MEDIUM/HIGH]
   - **Mitigation**: [как снизить риск]

## Related Changes

### Documentation Updates

- [ ] Spec updated (`.specify/specs/[feature]/spec.md`)
- [ ] Plan updated (`.specify/specs/[feature]/plan.md`)
- [ ] Constitution amended (if needed)
- [ ] Testing strategy updated
- [ ] User documentation updated (if user-facing change)

### Follow-up Actions

- [ ] Similar patterns in codebase checked for same bug
- [ ] Related issues closed (if applicable)
- [ ] Knowledge base updated (`.specify/memory/fixes/`)
- [ ] Team notified of fix

### Similar Bugs Prevention

**Lessons Learned**:
[Что можно сделать, чтобы предотвратить похожие баги в будущем]

1. [Урок 1 - e.g., "Add null checks for all external inputs"]
2. [Урок 2 - e.g., "Add integration tests for save/load workflows"]
3. [Урок 3 - e.g., "Clarify spec requirements for edge cases"]

**Preventive Actions**:
- [ ] Add linter rule (if applicable)
- [ ] Update code review checklist
- [ ] Add to common pitfalls documentation
- [ ] Create reusable test pattern

## Commit Information

### Commit Message

```
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
```

### Files to Commit

**Implementation Files**:
- [ ] `Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs`
- [ ] [other modified files]

**Test Files**:
- [ ] `.fix/bug_context/failing_test.cs` (move to `Assets/Scripts/NovelCore/Tests/Runtime/`)
- [ ] `.fix/tests/break_tests.cs` (move to `Assets/Scripts/NovelCore/Tests/Runtime/`)
- [ ] `.fix/tests/diagnostic_tests.cs` (if exists, move to tests)

**Documentation Files**:
- [ ] `.specify/specs/[feature]/spec.md` (if updated)
- [ ] `.specify/specs/[feature]/plan.md` (if updated)

### Archive to Memory

**Archive Location**: `.specify/memory/fixes/[bug-id]-[YYYY-MM-DD]/`

**Files to Archive**:
- `.fix/bug_context/` → archive
- `.fix/theories/` → archive
- `.fix/tests/break_results.md` → archive
- `.fix/patch/fix_summary.md` → archive
- `.fix/patch/patch.diff` → archive
- `.fix/patch/iterations.log` → archive

## Validation Checklist

### Pre-Commit Validation

- [X] Build succeeds (Unity compilation passes)
- [X] Failing test now passes
- [X] Break tests pass
- [X] No compilation errors
- [X] No test regressions
- [X] Constitution compliance verified
- [X] Code style compliance verified

### Post-Commit Actions

- [ ] Manual testing in Unity Editor (if needed)
- [ ] Close issue [issue ID]
- [ ] Monitor for related bugs
- [ ] Update release notes (if user-facing)

## Sign-Off

**Fixed By**: [AI Agent or Developer Name]
**Reviewed By**: [Reviewer Name if applicable]
**Date**: [YYYY-MM-DD]

**Approval**: [APPROVED | NEEDS_REVISION]

---

## Appendix

### Log Files

- Unity Compilation Logs: `.fix/patch/unity_compile_iter[N].log`
- Unity Test Logs: `.fix/patch/unity_tests_iter[N].log`
- Test Results XML: `.fix/patch/test_results_iter[N].xml`

### Patch Diff

See: `.fix/patch/patch.diff`

### Theory Documents

- Spec Theory: `.fix/theories/spec_theory.md`
- Impl Theory: `.fix/theories/impl_theory.md`
- Selected Theory: `.fix/theories/selected_theory.md`
