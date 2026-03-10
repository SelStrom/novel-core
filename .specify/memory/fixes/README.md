# Fix Index

**Last Updated**: 2026-03-10  
**Total Fixes**: 2

---

## Statistics

### By Month

| Month | Fixes Count |
|-------|-------------|
| 2026-03 | 2 |

---

## Recent Fixes (Last 50)

| Date | ID | Component | Summary | Commit | Status |
|------|----|-----------|---------|--------|--------|
| 2026-03-10 | 001 | Sample Project Background | `UnityAudioService` — это `MonoBehaviour`, но был  |  | ✅ Fixed |
| 2026-03-10 | 002 | Audio Di Null | `UnityAudioService` — это `MonoBehaviour`, но был  |  | ✅ Fixed |

---

## Fix Documentation Structure

Each fix directory contains:
```
YYYY-MM/DD-bug-name/
├── bug_context/
│   ├── bug.md                  # Описание бага
│   ├── relevant_files.md       # Затронутые файлы
│   └── failing_test.cs         # Воспроизводящий тест (если создан)
├── theories/
│   ├── impl_theory.md          # Implementation theory
│   ├── spec_theory.md          # Specification theory
│   └── selected_theory.md      # Выбранная теория
└── patch/
    └── fix_summary.md          # Итоговый summary фикса
```

---

## Retention Policy

- **Active** (0-90 дней): Full documentation в `YYYY-MM/DD-bug-name/`
- **Archived** (90+ дней): Compressed в `archived/YYYY-QX.tar.gz`
- **Purged** (optional): Архивы старше 1 года могут быть удалены

---

## Maintenance Commands

### Add New Fix

```bash
# Автоматически создаётся через /speckit.fix
# Структура: .specify/memory/fixes/YYYY-MM/DD-component-issue/
```

### Archive Old Fixes (>90 days)

```bash
.specify/scripts/archive-old-fixes.sh
```

### Update This Index

```bash
.specify/scripts/update-fix-index.sh
```

### Search Fixes

```bash
# По компоненту
grep -r "SceneManager" .specify/memory/fixes/*/*/bug_context/relevant_files.md

# По дате
ls .specify/memory/fixes/2026-03/

# По commit
grep -r "bc50296" .specify/memory/fixes/
```

---

## See Also

- [MAINTENANCE.md](./MAINTENANCE.md) - Detailed maintenance guide
- [.specify/scripts/archive-old-fixes.sh](../scripts/archive-old-fixes.sh) - Archiving automation
- [.specify/scripts/update-fix-index.sh](../scripts/update-fix-index.sh) - Index update automation
