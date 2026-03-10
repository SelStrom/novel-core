# Fix Archive Quick Start

Быстрый reference для работы с архивом фиксов.

## Структура (Гибридный Подход)

```
.specify/memory/fixes/
├── README.md          ← Master index (search here first!)
├── MAINTENANCE.md     ← Detailed guide
├── QUICKSTART.md      ← This file
├── 2026-03/           ← Active fixes (March 2026)
│   ├── 10-sample-project-backgrounds/
│   ├── 10-audio-di-null/
│   └── README.md      ← Monthly summary
├── 2026-04/           ← Active fixes (April 2026)
│   └── ...
└── archived/          ← Quarterly archives (>90 days)
    └── 2026-Q1.tar.gz
```

---

## Common Tasks

### 🔍 Search for a Fix

**By component**:
```bash
grep -r "SceneManager" .specify/memory/fixes/*/*/bug_context/relevant_files.md
```

**By date**:
```bash
ls .specify/memory/fixes/2026-03/
```

**By commit hash**:
```bash
grep -r "bc50296" .specify/memory/fixes/
```

**By keyword**:
```bash
grep -ri "DontDestroyOnLoad" .specify/memory/fixes/
```

**Check master index**:
```bash
cat .specify/memory/fixes/README.md
```

---

### 📝 After Fixing a Bug

1. `/speckit.fix` автоматически создаст fix в `.specify/memory/fixes/YYYY-MM/DD-bug-name/`
2. Update index:
   ```bash
   .specify/scripts/update-fix-index.sh
   ```
3. Review monthly README:
   ```bash
   cat .specify/memory/fixes/2026-03/README.md
   ```

---

### 🗄️ Archive Old Fixes (Quarterly)

**Run on 1st of month** (or when fixes >100):

```bash
.specify/scripts/archive-old-fixes.sh
```

**What it does**:
- Archives fixes older than 90 days
- Creates/updates `archived/YYYY-QX.tar.gz`
- Removes archived directories
- Updates statistics

---

### 📊 View Statistics

**Quick stats**:
```bash
# Total fixes
find .specify/memory/fixes -type d -name "[0-9][0-9]-*" | wc -l

# This month
ls .specify/memory/fixes/$(date +%Y-%m)/ | wc -l

# Archived quarters
ls -lh .specify/memory/fixes/archived/
```

**Detailed stats** (in README.md):
```bash
cat .specify/memory/fixes/README.md | grep -A 20 "## Statistics"
```

---

### 📦 Extract Archived Fix

**Extract specific quarter**:
```bash
tar -xzf .specify/memory/fixes/archived/2026-Q1.tar.gz -C /tmp/
ls /tmp/2026-03/10-audio-di-null/
```

**View archived fix**:
```bash
tar -tzf .specify/memory/fixes/archived/2026-Q1.tar.gz | grep "10-audio"
```

---

## Retention Timeline

```
Day 0-90:  Active in .specify/memory/fixes/YYYY-MM/DD-bug-name/
Day 90+:   Archived in .specify/memory/fixes/archived/YYYY-QX.tar.gz
Year 1+:   (Optional) Delete archive, rely on git history
```

---

## Naming Convention

**Format**: `DD-component-issue-keyword`

**Examples**:
- ✅ `10-scene-manager-background-null`
- ✅ `15-dialogue-system-crash`
- ✅ `20-vcontainer-di-registration`
- ❌ `fix1` (no context)
- ❌ `today-bug` (no component)

---

## Quick Commands

| Task | Command |
|------|---------|
| Search by component | `grep -r "ComponentName" .specify/memory/fixes/*/*/bug_context/` |
| List this month | `ls .specify/memory/fixes/$(date +%Y-%m)/` |
| Update index | `.specify/scripts/update-fix-index.sh` |
| Archive old | `.specify/scripts/archive-old-fixes.sh` |
| View stats | `cat .specify/memory/fixes/README.md` |
| Monthly summary | `cat .specify/memory/fixes/2026-03/README.md` |

---

## Scaling Guide

| Fixes Count | Action |
|-------------|--------|
| 1-50 | Use as-is, manual review |
| 50-200 | Run update-fix-index.sh after each fix |
| 200-500 | Monthly archiving, pattern analysis |
| 500+ | Quarterly archiving, consider SQLite index |

---

## See Also

- [README.md](./README.md) — Master index of all fixes
- [MAINTENANCE.md](./MAINTENANCE.md) — Detailed maintenance guide
- [.specify/scripts/](../scripts/) — Automation scripts
