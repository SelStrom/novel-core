# Spec-Kit Fix - Quick Start Guide

**Version**: 1.0.0  
**Status**: ✅ Ready to Use

## Что это?

`/speckit.fix` - автоматизированная система исправления багов с использованием 6 специализированных агентов. Система создаёт воспроизводимые тесты, анализирует root cause, генерирует патчи и предотвращает регрессии.

## Быстрый старт (30 секунд)

### 1. Запустите команду

```bash
/speckit.fix "Описание бага"
```

**Пример**:
```bash
/speckit.fix Dialogue text не отображается после загрузки сохранения
```

### 2. Дождитесь завершения pipeline

Система автоматически:
- ✅ Создаст failing test для воспроизведения
- ✅ Проанализирует спецификацию и код
- ✅ Выберет root cause (spec vs impl)
- ✅ Сгенерирует и применит патч
- ✅ Создаст regression tests
- ✅ Проверит отсутствие регрессий

### 3. Проверьте результаты

```bash
# Прочитайте резюме
cat .fix/patch/fix_summary.md

# Проверьте патч
cat .fix/patch/patch.diff

# Проверьте тесты
cat .fix/tests/break_tests.cs
```

### 4. Закоммитьте (если одобрено)

Pipeline подготовит commit message. Просто одобрите:

```bash
# Commit message уже создан в .fix/patch/fix_summary.md
# Просто выполните commit
```

## Pipeline в 6 шагов

```
1. BugContextAgent → создаёт failing test
2. SpecTheoryAgent + ImplTheoryAgent → анализируют root cause (параллельно)
3. TheoryArbitratorAgent → выбирает теорию (spec vs impl)
4. FixAgent → генерирует патч (max 10 iterations)
5. BreakAgent → пытается сломать fix
6. Commit → если все проверки пройдены
```

## Примеры

### Пример 1: Логический баг

```bash
/speckit.fix DialogueSystem.GetCurrentText() бросает NullReferenceException
```

**Результат** (за ~30 секунд):
- Failing test создан
- Root cause: пропущенная null check
- Патч применён за 1 iteration
- 5 regression tests добавлены
- 0 регрессий найдено
- ✅ Ready to commit

---

### Пример 2: Проблема в спецификации

```bash
/speckit.fix Scene transitions не соответствуют UX spec (должно быть 1s, а 500ms)
```

**Результат**:
- Failing test создан
- Root cause: противоречие между spec.md и plan.md
- Spec обновлена, план синхронизирован, код исправлен
- ✅ Ready to commit

---

### Пример 3: Не удалось воспроизвести

```bash
/speckit.fix Редкий краш в ChoiceHandler (flaky bug)
```

**Результат**:
```markdown
⚠️ Manual Review Required

Reason: Cannot create deterministic reproduction
Request: Provide logs, exact steps, crash frequency
```

Pipeline корректно определил невоспроизводимый баг.

---

## Директории

После выполнения команды:

```
.fix/
├── bug_context/         # Описание бага + failing test
├── theories/            # Анализ root cause (spec vs impl)
├── tests/               # Regression tests
└── patch/               # Патч + резюме
```

После commit:

```
.specify/memory/fixes/[bug-id]-[date]/
├── bug_context/
├── theories/
├── tests/
└── patch/
```

## Что проверяется перед commit

| Проверка                | Описание                              |
|-------------------------|---------------------------------------|
| ✅ Build succeeds       | Unity компиляция успешна              |
| ✅ Failing test passes  | Тест воспроизведения теперь проходит  |
| ✅ Break tests pass     | Adversarial tests не сломали fix      |
| ✅ No regressions       | Все существующие тесты проходят       |
| ✅ Constitution compliance | Соответствие Principles VI, VII     |
| ✅ Code style           | Allman braces, underscore prefix      |

## Конфигурация (опционально)

Настройте `.specify/config/fix.json`:

```json
{
  "max_fix_iterations": 10,
  "max_patch_size_lines": 200,
  "break_tests": {
    "enabled": true,
    "generate_edge_cases": true
  }
}
```

## Troubleshooting

### Q: FixAgent застрял на ошибке

```bash
# Проверьте логи
cat .fix/patch/iterations.log
```

**Решение**: Возможно, неправильная теория выбрана. Проверьте `.fix/theories/selected_theory.md`.

---

### Q: Тесты падают после fix

```bash
# Проверьте результаты break tests
cat .fix/tests/break_results.md
```

**Решение**: BreakAgent обнаружил regression. Pipeline автоматически откатит патч и запросит revision теории.

---

### Q: Unity compilation timeout

**Решение**: Увеличьте timeout в config:
```json
{
  "test_execution": {
    "timeout_seconds": 600
  }
}
```

---

## Интеграция с другими командами

### После fix → создать задачи

```bash
/speckit.tasks --from-fix .fix/tests/break_tests.cs
```

### Если spec обновлена → синхронизировать

```bash
/speckit.specify --update-from-fix .fix/theories/spec_theory.md
```

### Проверить constitution

```bash
/speckit.constitution --review-principle VI
```

---

## Полная документация

- **Подробное руководство**: `.specify/memory/fix-workflow-guide.md`
- **Краткая справка**: `.specify/memory/fix-quick-reference.md`
- **Тесты**: `.specify/memory/fix-integration-test-plan.md`
- **Команда**: `.cursor/commands/speckit.fix.md`

---

## Bash утилиты

### Инициализация .fix/ вручную

```bash
.specify/scripts/bash/setup-fix.sh --bug-id "my-bug"
```

### Архивация после fix

```bash
.specify/scripts/bash/archive-fix.sh --bug-id "my-bug"
```

---

## Метрики (после первых 10 fixes)

Track успешность в `.specify/memory/fix-metrics.json`:

```json
{
  "total_fixes": 10,
  "successful_fixes": 9,
  "success_rate": "90%",
  "average_iterations": 2.3,
  "average_tests_added": 5.1
}
```

---

## Ограничения

1. **Unity-specific**: Работает только с Unity проектами
2. **C#-specific**: Анализ кода заточен под C#
3. **Требует воспроизводимость**: Flaky bugs требуют manual debugging
4. **Manual approval**: Auto-commit отключен для safety

---

## Поддержка

**Вопросы?** Проверьте:
1. `.specify/memory/fix-workflow-guide.md` (детальное руководство)
2. `.specify/memory/fix-quick-reference.md` (быстрая справка)
3. Примеры в этом файле

**Баги в самом fix pipeline?** Создайте issue с тегом `[spec-kit-fix]`.

---

## Начните сейчас!

```bash
# Попробуйте на простом баге
/speckit.fix "Ваше описание бага"

# Дождитесь завершения pipeline
# Проверьте .fix/patch/fix_summary.md
# Одобрите commit

# Готово! 🎉
```

---

**Создано**: 2026-03-09  
**Версия**: 1.0.0  
**Статус**: ✅ Production Ready
