# Selected Theory Template

Используйте этот шаблон для документирования выбранной теории в `.fix/theories/selected_theory.md`.

---

# Selected Theory

## Decision

**Selected**: [SPEC_THEORY | IMPL_THEORY | HYBRID]

## Executive Summary

[1-2 параграфа объясняющих решение для быстрого понимания]

## Reasoning

### Theory Comparison

| Aspect                      | Spec Theory        | Impl Theory        | Winner           |
|-----------------------------|--------------------|--------------------|------------------|
| **Confidence Score**        | [X]%               | [Y]%               | [SPEC/IMPL]      |
| **Evidence Quality**        | [HIGH/MEDIUM/LOW]  | [HIGH/MEDIUM/LOW]  | [SPEC/IMPL]      |
| **Failing Test Alignment**  | [STRONG/WEAK]      | [STRONG/WEAK]      | [SPEC/IMPL]      |
| **Constitution Alignment**  | [ALIGNED/VIOLATION]| [ALIGNED/VIOLATION]| [SPEC/IMPL]      |
| **Git History Support**     | [STRONG/WEAK/N/A]  | [STRONG/WEAK/N/A]  | [SPEC/IMPL]      |
| **Edge Case Coverage**      | [STRONG/WEAK/N/A]  | [STRONG/WEAK/N/A]  | [SPEC/IMPL]      |
| **Code Inspection**         | [STRONG/WEAK/N/A]  | [STRONG/WEAK/N/A]  | [SPEC/IMPL]      |
| **Spec Review**             | [STRONG/WEAK/N/A]  | [STRONG/WEAK/N/A]  | [SPEC/IMPL]      |

### Confidence Differential

- **Spec Theory Confidence**: [X]%
- **Impl Theory Confidence**: [Y]%
- **Difference**: [|X - Y|]%
- **Threshold for Diagnostic Tests**: 20%

**Analysis**:
[Если разница < 20%, нужны ли диагностические тесты?]

## Decision Rationale

### Primary Reasons for Selection

1. **Reason #1**: [e.g., "Impl theory has 85% confidence vs 30% spec confidence"]
   - **Evidence**: [конкретные доказательства]
   - **Weight**: [HIGH/MEDIUM/LOW]

2. **Reason #2**: [e.g., "Git history shows recent change introduced bug"]
   - **Evidence**: [конкретные доказательства]
   - **Weight**: [HIGH/MEDIUM/LOW]

3. **Reason #3**: [e.g., "Failing test directly maps to logical error in code"]
   - **Evidence**: [конкретные доказательства]
   - **Weight**: [HIGH/MEDIUM/LOW]

### Evidence Strength Analysis

**Spec Theory Evidence**:
- [Сильная сторона 1]
- [Сильная сторона 2]
- [Слабая сторона 1]
- [Слабая сторона 2]

**Impl Theory Evidence**:
- [Сильная сторона 1]
- [Сильная сторона 2]
- [Слабая сторона 1]
- [Слабая сторона 2]

**Winner**: [SPEC/IMPL] because [объяснение]

### Test Alignment Analysis

**How Spec Theory Explains Failing Test**:
[Объяснение, как spec theory объясняет поведение failing test]

**How Impl Theory Explains Failing Test**:
[Объяснение, как impl theory объясняет поведение failing test]

**Better Alignment**: [SPEC/IMPL] because [объяснение]

### Root Cause Coverage

**Spec Theory Coverage**:
- Covers symptoms: [полностью/частично/слабо]
- Explains root cause: [полностью/частично/слабо]
- Predicts fix impact: [полностью/частично/слабо]

**Impl Theory Coverage**:
- Covers symptoms: [полностью/частично/слабо]
- Explains root cause: [полностью/частично/слабо]
- Predicts fix impact: [полностью/частично/слабо]

**Better Coverage**: [SPEC/IMPL] because [объяснение]

## Rejected Theory

**Rejected**: [SPEC_THEORY | IMPL_THEORY]

### Reason for Rejection

[Подробное объяснение, почему отклонена альтернативная теория]

**Main Reasons**:
1. [Причина 1 - e.g., "Low confidence (30%) compared to selected theory (85%)"]
2. [Причина 2 - e.g., "Evidence contradicts failing test behavior"]
3. [Причина 3 - e.g., "Spec review shows no contradictions"]

### Counterarguments Considered

[Какие аргументы в пользу отклонённой теории были рассмотрены]

1. **Argument #1**: [аргумент в пользу rejected theory]
   - **Counter**: [почему аргумент не убедителен]

2. **Argument #2**: [аргумент в пользу rejected theory]
   - **Counter**: [почему аргумент не убедителен]

## Confidence in Decision

**[0-100]%**

### Confidence Factors

| Factor                              | Score | Weight | Contribution |
|-------------------------------------|-------|--------|--------------|
| Confidence Differential             | [0-10]| 30%    | [X]%         |
| Evidence Quality Difference         | [0-10]| 25%    | [X]%         |
| Test Alignment Clarity              | [0-10]| 20%    | [X]%         |
| Alternative Theory Weakness         | [0-10]| 15%    | [X]%         |
| Diagnostic Test Results (if run)    | [0-10]| 10%    | [X]%         |
| **Total Decision Confidence**       |       |        | **[X]%**     |

### Justification

[Обоснование уверенности в решении]

**High Confidence Indicators** (if >80%):
- [индикатор 1]
- [индикатор 2]

**Low Confidence Indicators** (if <60%):
- [индикатор 1]
- [индикатор 2]

**Uncertainty Factors**:
- [фактор неопределённости 1]
- [фактор неопределённости 2]

## Diagnostic Tests

### Diagnostic Tests Generated

- [ ] YES - Diagnostic tests created in `.fix/tests/diagnostic_tests.cs`
- [ ] NO - Confidence differential > 20%, diagnostic tests not needed

**If YES**:

#### Test Results

1. **Test #1**: `[TestName]_If_SpecTheory_Correct_Should_[Behavior]()`
   - **Expected (if Spec Theory)**: [PASS/FAIL]
   - **Expected (if Impl Theory)**: [FAIL/PASS]
   - **Actual Result**: [PASS/FAIL]
   - **Supports Theory**: [SPEC/IMPL]

2. **Test #2**: `[TestName]_If_ImplTheory_Correct_Should_[Behavior]()`
   - **Expected (if Spec Theory)**: [PASS/FAIL]
   - **Expected (if Impl Theory)**: [FAIL/PASS]
   - **Actual Result**: [PASS/FAIL]
   - **Supports Theory**: [SPEC/IMPL]

#### Diagnostic Test Conclusion

**Tests Support**: [SPEC_THEORY | IMPL_THEORY | INCONCLUSIVE]

[Анализ результатов диагностических тестов]

## Recommended Fix Strategy

### If Spec Theory Selected

**Strategy**: Update specification, then align implementation

**Steps**:
1. **Update Spec**: Modify `.specify/specs/[feature]/spec.md`
   - Section: [секция для обновления]
   - Change: [конкретное изменение]
   - Justification: [обоснование]

2. **Update Plan**: Modify `.specify/specs/[feature]/plan.md`
   - Align with updated spec
   - Update technical decisions if needed

3. **Update Constitution** (if needed):
   - [ ] Principle violation found, constitution needs amendment
   - [ ] No constitution changes needed

4. **Update Implementation**:
   - Files: [список файлов]
   - Changes: [описание изменений для соответствия spec]

5. **Update Tests**:
   - Update existing tests to match new spec
   - Add new tests for clarified requirements

### If Impl Theory Selected

**Strategy**: Fix code implementation directly

**Steps**:
1. **Fix Code**: Modify implementation files
   - Primary file: [путь]
   - Method: [имя метода]
   - Line range: [start-end]
   - Change: [описание изменения]

2. **Add Edge Case Handling** (if needed):
   - Edge case: [описание]
   - Handling: [как обработать]

3. **Refactor** (if needed):
   - [ ] Architectural issue requires refactoring
   - Refactoring scope: [описание]

4. **Add Regression Tests**:
   - Failing test (already exists)
   - Edge case tests
   - Break tests

### If Hybrid Theory

**Strategy**: Fix both spec and implementation

**Steps**:
[Комбинация шагов из обеих стратегий выше]

## Fix Approach Details

### Primary Fix Location

**If Impl Theory**:
- **File**: `Assets/Scripts/NovelCore/Runtime/[Component]/[File].cs`
- **Class**: `[ClassName]`
- **Method**: `[MethodName]`
- **Lines**: [start]-[end]

**If Spec Theory**:
- **File**: `.specify/specs/[feature]/spec.md`
- **Section**: [section name]
- **Paragraph**: [paragraph identifier]

### Conceptual Fix

**If Impl Theory**:
```csharp
// Current (buggy) code
[buggy code]

// Proposed fix
[fixed code]

// Explanation: [why this fixes the bug]
```

**If Spec Theory**:
```markdown
## Current (incorrect) spec

[current spec text]

## Proposed (corrected) spec

[corrected spec text]

## Explanation

[why the spec change fixes the bug]
```

### Expected Fix Impact

- **Patch Size**: Estimated [X] lines changed
- **Files Modified**: [N] files
- **Breaking Changes**: [YES/NO]
- **Test Updates Needed**: [YES/NO]
- **Documentation Updates**: [YES/NO]

## Next Steps

### Immediate Actions

1. **[Action 1]**: [e.g., "Generate patch using FixAgent"]
2. **[Action 2]**: [e.g., "Run failing test to verify fix"]
3. **[Action 3]**: [e.g., "Run break tests"]

### FixAgent Instructions

**Theory to Follow**: [SPEC_THEORY | IMPL_THEORY]

**Fix Constraints**:
- Max patch size: [200] lines
- Max iterations: [10]
- Must pass: failing_test.cs, diagnostic_tests.cs (if any)

**Fix Priority**:
1. [Priority 1 - e.g., "Fix logical error at line X"]
2. [Priority 2 - e.g., "Add null check"]
3. [Priority 3 - e.g., "Refactor for clarity"]

### Success Criteria

- [ ] Build succeeds
- [ ] Failing test passes
- [ ] Diagnostic tests pass (if applicable)
- [ ] Related tests pass
- [ ] No regressions in existing tests
- [ ] Code style compliance
- [ ] Constitution compliance

## Potential Risks

### Risk #1: [Description]
- **Likelihood**: [LOW/MEDIUM/HIGH]
- **Impact**: [LOW/MEDIUM/HIGH]
- **Mitigation**: [how to mitigate]

### Risk #2: [Description]
- **Likelihood**: [LOW/MEDIUM/HIGH]
- **Impact**: [LOW/MEDIUM/HIGH]
- **Mitigation**: [how to mitigate]

## Fallback Plan

**If selected theory proves wrong during fix**:
1. [Fallback action 1 - e.g., "Switch to rejected theory"]
2. [Fallback action 2 - e.g., "Request manual review"]
3. [Fallback action 3 - e.g., "Generate more diagnostic tests"]

## Review Checklist

- [ ] Both theories analyzed thoroughly
- [ ] Confidence scores calculated objectively
- [ ] Evidence compared fairly
- [ ] Failing test alignment verified
- [ ] Constitution compliance checked
- [ ] Fix strategy is concrete and actionable
- [ ] Success criteria clearly defined
- [ ] Risks identified and mitigated
- [ ] Fallback plan exists
