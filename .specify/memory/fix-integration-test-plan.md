# Fix Pipeline Integration Test Plan

**Version**: 1.0.0  
**Purpose**: Проверка корректности работы `/speckit.fix` pipeline end-to-end

## Test Categories

### 1. Happy Path Tests

Проверка стандартного workflow без ошибок.

---

#### Test 1.1: Simple Implementation Bug Fix

**Scenario**: Простая логическая ошибка в коде, легко воспроизводимая.

**Setup**:
```csharp
// Intentionally introduce bug in DialogueSystem.cs
public string GetCurrentText()
{
    return _currentLine.text;  // BUG: не проверяет null
}
```

**Input**:
```bash
/speckit.fix DialogueSystem.GetCurrentText() бросает NullReferenceException когда _currentLine = null
```

**Expected Pipeline Flow**:

1. **BugContextAgent**:
   - ✅ Creates failing test: `DialogueSystem_GetCurrentText_WhenLineIsNull_ShouldReturnEmpty()`
   - ✅ Test fails with NullReferenceException

2. **SpecTheoryAgent**:
   - ✅ Reviews spec.md, finds no contradictions
   - ✅ Confidence: ~30%

3. **ImplTheoryAgent**:
   - ✅ Identifies missing null check
   - ✅ Confidence: ~85%

4. **TheoryArbitratorAgent**:
   - ✅ Selects IMPL_THEORY (85% > 30%)

5. **FixAgent** (iteration 1):
   ```csharp
   public string GetCurrentText()
   {
       if (_currentLine == null)
       {
           return string.Empty;
       }
       return _currentLine.text;
   }
   ```
   - ✅ Build succeeds
   - ✅ Failing test passes

6. **BreakAgent**:
   - ✅ Generates 3 break tests (null, empty, boundary)
   - ✅ All tests pass

7. **Commit**:
   - ✅ `fix: resolve DialogueSystem null check missing`

**Expected Outputs**:
- `.fix/bug_context/failing_test.cs` exists
- `.fix/theories/selected_theory.md` contains "Selected: IMPL_THEORY"
- `.fix/patch/fix_summary.md` shows "Iterations: 1"
- Git log shows commit with proper format
- `.specify/memory/fixes/dialoguesystem-null-[date]/` archive created

**Success Criteria**:
- ✅ Fix applied in ≤3 iterations
- ✅ All tests pass
- ✅ No regressions
- ✅ Archive created

---

#### Test 1.2: Specification Contradiction Fix

**Scenario**: Противоречие между spec.md и plan.md.

**Setup**:
```markdown
# spec.md (section 3.2)
Scene transitions MUST be 1 second.

# plan.md (section 4.1)
Transition duration: 500ms (performance optimization)
```

**Input**:
```bash
/speckit.fix Scene transitions не соответствуют UX spec (длительность)
```

**Expected Pipeline Flow**:

1. **BugContextAgent**:
   - ✅ Creates failing test comparing actual vs spec duration

2. **SpecTheoryAgent**:
   - ✅ Identifies contradiction between spec.md and plan.md
   - ✅ Confidence: ~90%

3. **ImplTheoryAgent**:
   - ✅ Code correctly implements plan.md (500ms)
   - ✅ Confidence: ~40%

4. **TheoryArbitratorAgent**:
   - ✅ Selects SPEC_THEORY (90% > 40%)

5. **FixAgent**:
   - ✅ Updates spec.md OR plan.md (clarifies requirement)
   - ✅ Updates code to match unified spec

6. **BreakAgent**:
   - ✅ Verifies consistent behavior

7. **Commit**:
   - ✅ Updates spec + plan + code

**Success Criteria**:
- ✅ Spec and plan aligned
- ✅ Code matches unified spec
- ✅ All tests pass

---

### 2. Edge Case Tests

Проверка обработки нестандартных ситуаций.

---

#### Test 2.1: Cannot Reproduce Bug

**Scenario**: Баг нестабильно воспроизводится (flaky).

**Setup**: Intermittent race condition

**Input**:
```bash
/speckit.fix Редкий краш в ChoiceHandler при concurrent доступе
```

**Expected Pipeline Flow**:

1. **BugContextAgent** (attempt 1):
   - ⚠️ Creates test, but it passes (cannot reproduce)

2. **BugContextAgent** (attempt 2):
   - ⚠️ Adds threading stress, test is flaky

3. **BugContextAgent** (attempt 3):
   - ❌ Still cannot reproduce deterministically

4. **Pipeline Output**:
   ```markdown
   ⚠️ Manual Review Required
   Reason: Cannot create deterministic reproduction
   Request: Provide logs, exact steps, crash frequency
   ```

**Success Criteria**:
- ✅ Pipeline correctly identifies non-reproducible bug
- ✅ Requests additional info from user
- ✅ Does NOT apply speculative fix
- ✅ Preserves attempted tests in `.fix/bug_context/attempted_tests.cs`

---

#### Test 2.2: Conflicting Theories (Close Confidence)

**Scenario**: Spec и impl теории имеют близкие confidence scores.

**Setup**: Ambiguous spec + suspicious code

**Input**:
```bash
/speckit.fix SaveSystem сохраняет dialogue state некорректно
```

**Expected Pipeline Flow**:

1. **SpecTheoryAgent**:
   - ✅ Finds ambiguous spec language
   - ✅ Confidence: 60%

2. **ImplTheoryAgent**:
   - ✅ Finds suspicious serialization logic
   - ✅ Confidence: 55%

3. **TheoryArbitratorAgent**:
   - ⚠️ Difference: 5% (< 20% threshold)
   - ✅ Generates diagnostic tests to resolve conflict

4. **Diagnostic Tests Run**:
   - ✅ `If_SpecTheory_Correct_Should_SaveWithFormat()`
   - ✅ `If_ImplTheory_Correct_Should_SerializeWithOrder()`

5. **Results**:
   - Based on diagnostic test results → selects winning theory

**Success Criteria**:
- ✅ Diagnostic tests generated
- ✅ Diagnostic tests resolve conflict
- ✅ Correct theory selected based on evidence

---

#### Test 2.3: Fix Iterations Exceed Limit

**Scenario**: FixAgent не может найти working patch за 10 iterations.

**Setup**: Сложный баг, требующий архитектурных изменений

**Input**:
```bash
/speckit.fix State desync между DialogueSystem и SaveSystem
```

**Expected Pipeline Flow**:

1-4. **Theory Selection**: Selects IMPL_THEORY

5. **FixAgent**:
   - Iteration 1: Build fails (missing dependency)
   - Iteration 2: Build succeeds, tests fail (state mismatch)
   - ...
   - Iteration 10: Still failing

6. **Pipeline Output**:
   ```markdown
   ⚠️ Max Iterations Exceeded
   Iterations: 10/10
   Best Attempt: Iteration 7 (build passed, 2 tests failed)
   
   Recommended: Manual review + theory revision
   ```

**Success Criteria**:
- ✅ Pipeline stops at iteration 10 (does NOT infinite loop)
- ✅ Returns best attempt + logs
- ✅ Suggests manual review
- ✅ Preserves `.fix/patch/iterations.log` for debugging

---

### 3. Constitution Compliance Tests

Проверка соответствия исправлений constitution principles.

---

#### Test 3.1: Code Style Violations

**Scenario**: FixAgent генерирует патч с нарушениями code style.

**Setup**: Allow FixAgent to generate non-compliant code

**Expected**:
- ✅ ReadLints detects violations
- ✅ FixAgent auto-corrects:
  - Missing braces → adds braces
  - Wrong namespace format → converts to traditional
  - Missing underscore → adds `_` prefix

**Success Criteria**:
- ✅ Final patch complies with Code Style Standards
- ✅ Allman braces enforced
- ✅ Mandatory braces (zero tolerance)
- ✅ Underscore prefix for fields

---

#### Test 3.2: Test Coverage Validation

**Scenario**: Fix должен включать regression tests.

**Expected**:
- ✅ BugContextAgent creates failing_test.cs
- ✅ BreakAgent creates break_tests.cs
- ✅ Commit includes both test files
- ✅ Test files follow Principle VI (EditMode-first)

**Success Criteria**:
- ✅ Minimum 1 failing test (reproduction)
- ✅ Minimum 3 break tests (edge cases)
- ✅ All tests in proper location (`Assets/Scripts/NovelCore/Tests/`)

---

### 4. Integration Tests

Проверка интеграции с другими Spec-Kit командами.

---

#### Test 4.1: Fix → Tasks Handoff

**After successful fix**:
```bash
/speckit.tasks --from-fix .fix/tests/break_tests.cs
```

**Expected**:
- ✅ Tasks created for integrating break tests into CI
- ✅ Tasks reference `.fix/tests/break_tests.cs`

---

#### Test 4.2: Fix → Constitution Handoff

**If SPEC_THEORY selected with constitution violation**:
```bash
/speckit.constitution --review-principle VI
```

**Expected**:
- ✅ Constitution review triggered
- ✅ Principle VI updated if needed

---

#### Test 4.3: Fix → Specify Handoff

**If SPEC_THEORY selected and spec updated**:
```bash
/speckit.specify --update-from-fix .fix/theories/spec_theory.md
```

**Expected**:
- ✅ Spec.md synchronized with fix
- ✅ Plan.md updated if needed

---

### 5. Bash Scripts Tests

---

#### Test 5.1: setup-fix.sh

```bash
.specify/scripts/bash/setup-fix.sh --bug-id "test-bug" --json
```

**Expected Output**:
```json
{
  "fix_dir": "/path/.fix",
  "bug_context_dir": "/path/.fix/bug_context",
  "theories_dir": "/path/.fix/theories",
  "tests_dir": "/path/.fix/tests",
  "patch_dir": "/path/.fix/patch",
  "agents_dir": "/path/.fix/agents",
  "config_file": "/path/.specify/config/fix.json",
  "bug_id": "test-bug",
  "status": "initialized"
}
```

**Validation**:
- ✅ All directories created
- ✅ .gitignore updated
- ✅ metadata.json created with bug_id

---

#### Test 5.2: archive-fix.sh

```bash
.specify/scripts/bash/archive-fix.sh --bug-id "test-bug" --json
```

**Expected**:
- ✅ Archive created at `.specify/memory/fixes/test-bug-[date]/`
- ✅ All artifacts copied
- ✅ .fix/ cleaned up
- ✅ metadata.json generated

---

### 6. Configuration Tests

---

#### Test 6.1: Custom Config

**Setup**:
```json
{
  "max_fix_iterations": 3,
  "max_patch_size_lines": 50,
  "break_tests": { "enabled": false }
}
```

**Expected**:
- ✅ FixAgent stops at iteration 3
- ✅ Patch rejected if > 50 lines
- ✅ BreakAgent skipped (break_tests disabled)

---

### 7. Regression Tests

Проверка на отсутствие регрессий в fix pipeline.

---

#### Test 7.1: Critical Regression Detection

**Scenario**: Fix ломает существующую функциональность.

**Setup**: Патч изменяет поведение, используемое другими модулями

**Expected**:
- ✅ BreakAgent detects regression in full test suite
- ✅ Pipeline output:
   ```markdown
   ❌ Critical Regression Detected
   Fix broke 3 existing tests
   Action: Reverting patch, requesting theory revision
   ```
- ✅ Patch NOT committed
- ✅ User notified of regression

---

## Test Execution Plan

### Phase 1: Happy Path (Priority: P0)
- [ ] Test 1.1: Simple implementation bug fix
- [ ] Test 1.2: Specification contradiction fix

### Phase 2: Edge Cases (Priority: P1)
- [ ] Test 2.1: Cannot reproduce bug
- [ ] Test 2.2: Conflicting theories
- [ ] Test 2.3: Fix iterations exceed limit

### Phase 3: Constitution (Priority: P1)
- [ ] Test 3.1: Code style violations
- [ ] Test 3.2: Test coverage validation

### Phase 4: Integration (Priority: P2)
- [ ] Test 4.1: Fix → Tasks handoff
- [ ] Test 4.2: Fix → Constitution handoff
- [ ] Test 4.3: Fix → Specify handoff

### Phase 5: Infrastructure (Priority: P2)
- [ ] Test 5.1: setup-fix.sh
- [ ] Test 5.2: archive-fix.sh
- [ ] Test 6.1: Custom config

### Phase 6: Regression (Priority: P0)
- [ ] Test 7.1: Critical regression detection

## Test Metrics

Track успешность тестов:

```json
{
  "test_run_date": "2026-03-09",
  "total_tests": 13,
  "passed": 12,
  "failed": 1,
  "skipped": 0,
  "pass_rate": "92%",
  "failed_tests": [
    {
      "test_id": "2.3",
      "name": "Fix iterations exceed limit",
      "reason": "Pipeline did not stop at iteration 10",
      "severity": "HIGH"
    }
  ]
}
```

## Success Criteria (Overall)

Pipeline считается готовым к production если:

- ✅ P0 tests: 100% pass rate
- ✅ P1 tests: ≥90% pass rate
- ✅ P2 tests: ≥80% pass rate
- ✅ No critical severity failures
- ✅ All constitution compliance tests pass
- ✅ All regression detection tests pass

## Continuous Testing

Эти тесты должны выполняться:

1. **Pre-release**: Перед каждым релизом Spec-Kit
2. **Monthly**: Ежемесячная regression проверка
3. **On-demand**: При изменении fix pipeline logic

## Test Artifacts

Все тесты создают артефакты в:
```
.specify/tests/fix-pipeline/
├── test-1.1-simple-bug/
│   ├── input.md
│   ├── expected_output.md
│   ├── actual_output.md
│   └── result.json
├── test-1.2-spec-contradiction/
│   └── ...
└── test-results-summary.json
```

---

**Maintained by**: Spec-Kit team  
**Next review**: 2026-04-09
