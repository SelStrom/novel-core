# Bug Fix Archive

Этот директорий содержит архивы всех багов, исправленных с помощью `/speckit.fix` команды.

## Структура

Каждый исправленный баг архивируется в отдельную директорию:

```
fixes/
├── [bug-id]-[YYYY-MM-DD]/
│   ├── bug_context/
│   │   ├── bug.md                      # Описание бага
│   │   ├── failing_test.cs             # Failing reproduction test
│   │   ├── relevant_files.md           # Релевантные файлы
│   │   └── reproduction_log.txt        # Лог воспроизведения
│   ├── theories/
│   │   ├── spec_theory.md              # Теория о проблемах в spec
│   │   ├── impl_theory.md              # Теория о проблемах в impl
│   │   └── selected_theory.md          # Выбранная теория
│   ├── tests/
│   │   ├── diagnostic_tests.cs         # Диагностические тесты (если есть)
│   │   ├── break_tests.cs              # Adversarial/edge case тесты
│   │   └── break_results.md            # Результаты break tests
│   ├── patch/
│   │   ├── patch.diff                  # Финальный патч
│   │   ├── fix_summary.md              # Резюме исправления
│   │   └── iterations.log              # Лог итераций FixAgent
│   ├── metadata.json                   # Метаданные архива
│   └── README.md                       # Описание конкретного фикса
└── README.md (this file)
```

## Назначение

### Knowledge Base

Архив служит базой знаний для:

1. **Анализа паттернов багов**: Какие типы багов встречаются чаще
2. **Обучения**: Как были решены аналогичные проблемы
3. **Regression Prevention**: История исправлений для предотвращения повторов
4. **Onboarding**: Новые разработчики могут изучить типичные баги и подходы к решению

### Audit Trail

Полная документация процесса исправления:

- Root cause analysis (spec vs impl theory)
- Decision rationale (почему выбрана конкретная теория)
- Fix approach (как был исправлен баг)
- Test coverage (какие тесты были добавлены)

### Metrics & Analytics

Данные для анализа эффективности fix pipeline:

```bash
# Подсчёт успешных фиксов по типу теории
grep -r "Selected: IMPL_THEORY" fixes/*/theories/selected_theory.md | wc -l

# Средняя длительность (iterations)
grep "Total Iterations:" fixes/*/patch/fix_summary.md | awk '{sum+=$3; n++} END {print sum/n}'

# Частота failure modes
ls -d fixes/*/ | while read dir; do
    if grep -q "Manual Review Required" "$dir/bug_context/bug.md"; then
        echo "$(basename $dir): Cannot Reproduce"
    fi
done
```

## Использование

### Поиск похожего бага

```bash
# Поиск по ключевым словам
grep -r "DialogueSystem" fixes/*/bug_context/bug.md

# Поиск по типу теории
grep -r "IMPL_THEORY" fixes/*/theories/selected_theory.md

# Поиск по root cause
grep -r "off-by-one" fixes/*/patch/fix_summary.md
```

### Извлечение патчей

```bash
# Применить патч из архива
cd /path/to/project
patch -p1 < .specify/memory/fixes/dialogue-bug-2026-03-09/patch/patch.diff
```

### Анализ regression patterns

```bash
# Баги, которые потребовали revision теории
grep -r "iterations_exceeded" fixes/*/patch/iterations.log
```

## Retention Policy

- **Permanent**: Все архивы сохраняются постоянно
- **Cleanup**: Временные логи (unity_*.log) удаляются после 30 дней (если не указано `--keep-logs`)
- **Compression**: Архивы старше 1 года могут быть сжаты (.tar.gz)

## Archive Process

Автоматическая архивация выполняется после успешного commit:

```bash
.specify/scripts/bash/archive-fix.sh --bug-id "dialogue-bug"
```

Ручная архивация (если auto-commit отключен):

```bash
# 1. Скопировать .fix/ в fixes/
cp -r .fix/ .specify/memory/fixes/my-bug-2026-03-09/

# 2. Очистить .fix/
rm -rf .fix/bug_context/* .fix/theories/* .fix/tests/* .fix/patch/*
```

## Metadata Format

Каждый архив содержит `metadata.json`:

```json
{
  "bug_id": "dialogue-text-missing",
  "archived_at": "2026-03-09T14:30:00Z",
  "archive_name": "dialogue-text-missing-2026-03-09",
  "kept_logs": false,
  "source_dir": "/path/to/.fix",
  "theory_selected": "IMPL_THEORY",
  "iterations": 3,
  "tests_added": 6,
  "files_modified": 2,
  "fix_confidence": 95
}
```

## Integration с CI/CD

### Automated Analysis

CI pipeline может анализировать архивы для metrics:

```yaml
# .github/workflows/fix-metrics.yml
on:
  push:
    paths:
      - '.specify/memory/fixes/**'

jobs:
  analyze:
    runs-on: ubuntu-latest
    steps:
      - name: Calculate success rate
        run: |
          total=$(ls -d .specify/memory/fixes/*/ | wc -l)
          successful=$(grep -r "status: archived" .specify/memory/fixes/*/metadata.json | wc -l)
          echo "Success Rate: $((successful * 100 / total))%"
```

### Changelog Generation

Автоматическая генерация changelog из fix summaries:

```bash
# Generate changelog for version
for dir in .specify/memory/fixes/*/; do
    echo "- $(grep "Bug Title:" "$dir/patch/fix_summary.md" | cut -d: -f2-)"
done > CHANGELOG.md
```

## Best Practices

1. **Descriptive Bug IDs**: Используйте понятные имена (`dialogue-text-missing`, не просто `bug-123`)
2. **Keep Summaries Updated**: fix_summary.md должен быть полным и точным
3. **Link Issues**: Включайте ссылки на issue tracker в bug.md
4. **Document Learnings**: Добавляйте уроки в fix_summary.md
5. **Review Periodically**: Раз в квартал анализируйте архивы для выявления паттернов

## Statistics (Example)

```
Total Fixes: 25
├─ IMPL_THEORY: 18 (72%)
├─ SPEC_THEORY: 4 (16%)
└─ HYBRID: 3 (12%)

Success Rate: 88% (22/25)
├─ Manual Review Required: 2
└─ Iterations Exceeded: 1

Average Iterations: 2.4
Average Tests Added: 4.8
Average Confidence: 87%

Most Common Root Causes:
1. Null handling (8 bugs)
2. Off-by-one errors (5 bugs)
3. State management (4 bugs)
4. Spec ambiguity (3 bugs)
5. Edge cases (5 bugs)
```

## См. также

- **Fix Workflow Guide**: `.specify/memory/fix-workflow-guide.md`
- **Quick Reference**: `.specify/memory/fix-quick-reference.md`
- **Command Documentation**: `.cursor/commands/speckit.fix.md`
- **Templates**: `.specify/templates/fix-*-template.md`
