# Spec Theory Template

Используйте этот шаблон для документирования теории о проблемах в спецификации в `.fix/theories/spec_theory.md`.

---

# Specification Theory

## Hypothesis

Баг вызван [выберите одно или несколько]:
- [ ] Противоречиями в спецификации
- [ ] Устаревшими требованиями
- [ ] Неоднозначными требованиями
- [ ] Отсутствием требований для edge case
- [ ] Несоответствием между spec.md и plan.md
- [ ] Нарушением constitution principles

**Summary**: [1-2 предложения о гипотезе]

## Evidence

### Spec Contradictions

[Если найдены противоречия в спецификации]

1. **Contradiction #1**:
   - **Location 1**: `.specify/specs/[feature]/spec.md`, section [X], line [Y]
   - **Statement 1**: "[точная цитата]"
   - **Location 2**: [where the contradiction exists]
   - **Statement 2**: "[точная цитата]"
   - **Conflict**: [описание конфликта - почему эти утверждения противоречат друг другу]
   - **Actual Behavior Matches**: [Statement 1 | Statement 2 | Neither]

[Повторить для каждого противоречия]

### Outdated Requirements

[Если спецификация устарела]

1. **Requirement #1**:
   - **Specified**: [when/where requirement was documented]
   - **Spec Statement**: "[цитата из спецификации]"
   - **Current Implementation**: [описание фактической реализации]
   - **Mismatch**: [описание расхождения]
   - **Last Updated**: [дата последнего обновления спецификации]
   - **Evidence**: [git history, recent changes]

[Повторить для каждого устаревшего требования]

### Ambiguous Requirements

[Если требования неоднозначны]

1. **Ambiguity #1**:
   - **Spec Location**: `.specify/specs/[feature]/spec.md`, section [X]
   - **Ambiguous Statement**: "[цитата]"
   - **Interpretation A**: [возможная интерпретация 1]
   - **Interpretation B**: [возможная интерпретация 2]
   - **Current Behavior**: [какая интерпретация реализована]
   - **Expected Behavior (Unclear)**: [почему неясно, что правильно]

[Повторить для каждой неоднозначности]

### Missing Requirements

[Если отсутствуют требования для edge cases]

1. **Missing Requirement #1**:
   - **Edge Case**: [описание edge case]
   - **Spec Coverage**: [что говорит спецификация, если вообще]
   - **Current Behavior**: [как система обрабатывает этот case]
   - **Impact**: [как отсутствие требования привело к багу]

[Повторить для каждого отсутствующего требования]

## Expected Behavior Mismatch

- **According to Spec**: [поведение, описанное в спецификации]
- **Actual Behavior**: [фактическое поведение системы]
- **Bug Behavior Aligns With**: 
  - [ ] Specification (код правильно реализует неправильную спецификацию)
  - [ ] Implementation (спецификация правильная, реализация некорректная)
  - [ ] Neither (и spec, и impl неверны)

## Constitution Alignment

[Проверка на нарушение принципов constitution]

### Principle Violations

1. **Principle**: [Principle VI - Modular Architecture & Testing]
   - **Violation**: [описание нарушения]
   - **Evidence**: [как это проявляется в баге]
   - **Relevance to Bug**: [как нарушение привело к багу]

[Повторить для каждого нарушенного принципа]

### Principle Compliance

- [ ] Principle I (Creator-First Design)
- [ ] Principle II (Cross-Platform Parity)
- [ ] Principle III (Asset Pipeline Integrity)
- [ ] Principle IV (Runtime Performance Guarantees)
- [ ] Principle V (Save System Reliability)
- [ ] Principle VI (Modular Architecture & Testing)
- [ ] Principle VII (AI Development Constraints)
- [ ] Principle VIII (Editor-Runtime Bridge)
- [ ] Principle IX (User Documentation Language)
- [ ] Principle X (File Organization)

## Cross-Reference Analysis

### Spec vs Plan

- **Spec Location**: `.specify/specs/[feature]/spec.md`
- **Plan Location**: `.specify/specs/[feature]/plan.md`
- **Discrepancy**: [описание расхождений между spec и plan]
- **Impact on Bug**: [как расхождение привело к багу]

### Spec vs Constitution

- **Relevant Constitution Section**: [section number and name]
- **Spec Compliance**: [соответствует ли спецификация constitution]
- **Conflict**: [если есть конфликт, описать]

## Confidence Score

**[0-100]%**

### Confidence Breakdown

| Factor                          | Score | Weight | Contribution |
|---------------------------------|-------|--------|--------------|
| Evidence Strength               | [0-10]| 30%    | [X]%         |
| Spec Contradiction Clarity      | [0-10]| 25%    | [X]%         |
| Outdated Requirement Likelihood | [0-10]| 20%    | [X]%         |
| Ambiguity Impact                | [0-10]| 15%    | [X]%         |
| Constitution Violation          | [0-10]| 10%    | [X]%         |
| **Total**                       |       |        | **[X]%**     |

### Reasoning

[Подробное объяснение оценки уверенности]

**Why High Confidence** (if >70%):
[аргументы за высокую уверенность]

**Why Low Confidence** (if <50%):
[аргументы за низкую уверенность]

**Uncertainties**:
[что ещё нужно проверить для повышения уверенности]

## Recommended Action

### If Spec Theory is Correct

- [ ] Update specification in `.specify/specs/[feature]/spec.md`
- [ ] Clarify ambiguous requirements
- [ ] Resolve contradictions between spec sections
- [ ] Align spec with constitution principles
- [ ] Update plan.md to match corrected spec
- [ ] Update implementation to match corrected spec
- [ ] Add missing requirements for edge cases

### Specific Changes Needed

1. **Change #1**:
   - **File**: [путь к файлу спецификации]
   - **Section**: [название секции]
   - **Change**: [конкретное изменение]
   - **Justification**: [почему нужно изменение]

[Повторить для каждого изменения]

## Supporting Evidence

### Related Spec Files Analyzed

- [ ] `.specify/specs/[feature]/spec.md`
- [ ] `.specify/specs/[feature]/plan.md`
- [ ] `.specify/specs/[feature]/data-model.md`
- [ ] `.specify/specs/[feature]/contracts/[contract].md`
- [ ] `.specify/memory/constitution.md`
- [ ] Other: [specify]

### External References

[Если есть внешние ссылки на документацию, issue tracker, user reports]

1. **Reference #1**: [description]
   - **URL/Location**: [link or path]
   - **Relevance**: [how it supports spec theory]

## Alternative Explanations

[Почему это НЕ проблема реализации]

1. **Implementation Appears Correct Because**:
   - [аргумент 1]
   - [аргумент 2]

2. **Code Review Findings**:
   - [что показал анализ кода]

## Next Steps

### If Spec Theory is Selected

1. [Шаг 1 - e.g., clarify requirement in spec.md]
2. [Шаг 2 - e.g., update plan.md to align]
3. [Шаг 3 - e.g., refactor implementation to match updated spec]

### Validation Criteria

- [ ] Updated spec resolves all contradictions
- [ ] All ambiguities clarified with concrete examples
- [ ] Constitution compliance verified
- [ ] Implementation updated to match new spec
- [ ] Tests updated to verify new requirements
