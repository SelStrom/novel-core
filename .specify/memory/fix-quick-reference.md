# Spec-Kit Fix Quick Reference

Краткая справка по команде `/speckit.fix`.

## Быстрый старт

```bash
/speckit.fix "Описание бага"
```

## Pipeline Overview

```
BugContextAgent → SpecTheoryAgent + ImplTheoryAgent → TheoryArbitratorAgent → FixAgent → BreakAgent → Commit
```

## Agents Cheat Sheet

| Agent                    | Цель                                      | Output                         |
|--------------------------|-------------------------------------------|--------------------------------|
| BugContextAgent          | Создать failing test                      | `bug_context/failing_test.cs`  |
| SpecTheoryAgent          | Проверить спецификацию                    | `theories/spec_theory.md`      |
| ImplTheoryAgent          | Проверить реализацию                      | `theories/impl_theory.md`      |
| TheoryArbitratorAgent    | Выбрать теорию                            | `theories/selected_theory.md`  |
| FixAgent (max 10 iter)   | Сгенерировать патч                        | `patch/patch.diff`             |
| BreakAgent               | Попытаться сломать фикс                   | `tests/break_tests.cs`         |

## Директории

```
.fix/
├── bug_context/    # Контекст бага + failing test
├── theories/       # Spec + impl theories + selected
├── tests/          # Diagnostic + break tests
├── patch/          # Patch.diff + fix_summary.md
└── agents/         # Логи агентов (temporary)
```

## Conditions для Commit

| Условие                    | Статус |
|----------------------------|--------|
| Build succeeds             | ✅     |
| Failing test passes        | ✅     |
| Break tests pass           | ✅     |
| No regressions             | ✅     |
| Constitution compliance    | ✅     |

## Config (.specify/config/fix.json)

```json
{
  "max_fix_iterations": 10,
  "max_patch_size_lines": 200,
  "break_tests": { "enabled": true },
  "commit": { "auto_commit": false }
}
```

## Failure Modes

| Mode                      | Recovery                                    |
|---------------------------|---------------------------------------------|
| Cannot reproduce          | Request more info from user                 |
| Conflicting theories      | Generate diagnostic tests                   |
| Iterations exceeded       | Return best patch + manual review           |
| Critical regressions      | Revert patch, revise theory                 |

## Bash Scripts

```bash
# Инициализация .fix/
.specify/scripts/bash/setup-fix.sh --bug-id "dialogue-bug"

# Архивация после fix
.specify/scripts/bash/archive-fix.sh --bug-id "dialogue-bug"
```

## Templates

| Template                        | Location                                      |
|---------------------------------|-----------------------------------------------|
| Bug Context                     | `.specify/templates/fix-bug-context-template.md` |
| Spec Theory                     | `.specify/templates/fix-spec-theory-template.md` |
| Impl Theory                     | `.specify/templates/fix-impl-theory-template.md` |
| Selected Theory                 | `.specify/templates/fix-selected-theory-template.md` |
| Fix Summary                     | `.specify/templates/fix-summary-template.md` |

## Examples

### Простой баг
```bash
/speckit.fix Dialogue text не отображается после загрузки сохранения
```

### Spec issue
```bash
/speckit.fix Scene transitions не соответствуют UX-спецификации
```

### С issue ID
```bash
/speckit.fix #123
```

## Handoffs

```bash
# После fix → создать задачи
/speckit.tasks --from-fix .fix/tests/break_tests.cs

# Обновить constitution (если SPEC_THEORY)
/speckit.constitution --review-principle VI

# Обновить spec (если SPEC_THEORY)
/speckit.specify --update-from-fix .fix/theories/spec_theory.md
```

## Commit Format

```
fix: resolve [краткое описание]

Root Cause: [1-2 предложения]
Changes: [список изменений]
Tests: [добавленные тесты]
Closes: #[issue-id]
```

## Архивация

После commit:
```
.specify/memory/fixes/[bug-id]-[YYYY-MM-DD]/
├── bug_context/
├── theories/
├── tests/
└── patch/
```

## Troubleshooting

| Проблема                          | Решение                                    |
|-----------------------------------|--------------------------------------------|
| FixAgent stuck on same error      | Check `iterations.log`, switch theory      |
| Break tests false positives       | Disable adversarial in config              |
| Unity compilation timeout         | Increase `timeout_seconds` in config       |
| Cannot reproduce flaky bug        | Request logs, exact steps from user        |

## Constitution Compliance

- ✅ Principle VI: Tests written, EditMode-first
- ✅ Principle VII: Only .cs files modified
- ✅ Code Style: Allman braces, underscore prefix

## Monitoring

Track в `.specify/memory/fix-metrics.json`:
- Success rate
- Average iterations
- Theory distribution
- Failure modes

---

**Полное руководство**: `.specify/memory/fix-workflow-guide.md`
