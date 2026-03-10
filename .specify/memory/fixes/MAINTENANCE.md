# Fix Archive Maintenance Guide

Это руководство по управлению архивом фиксов при масштабировании проекта.

## Структура Директорий

```
.specify/memory/fixes/
├── README.md              # Master index всех фиксов (auto-generated)
├── 2026-03/              # Активные фиксы за март 2026
│   ├── 10-sample-project-backgrounds/
│   │   ├── bug_context/
│   │   ├── theories/
│   │   └── patch/
│   └── 10-audio-di-null/
│       ├── bug_context/
│       ├── theories/
│       └── patch/
├── 2026-04/              # Активные фиксы за апрель 2026
│   └── ...
└── archived/             # Архивы старых фиксов (>90 дней)
    ├── 2026-Q1.tar.gz
    └── 2025-Q4.tar.gz
```

---

## Retention Policy

### Active Fixes (0-90 дней)

**Location**: `.specify/memory/fixes/YYYY-MM/DD-bug-name/`

**Naming Convention**: `DD-component-issue-description`

**Examples**:
- `10-sample-project-backgrounds`
- `10-audio-di-null`
- `15-scene-manager-transition-crash`

**Content**: Full documentation (bug context, theories, patch)

### Archived Fixes (90+ дней)

**Location**: `.specify/memory/fixes/archived/YYYY-QX.tar.gz`

**Format**: Quarterly archives (Q1, Q2, Q3, Q4)

**Content**: Compressed month directories

**Access**: Extract with `tar -xzf archived/2026-Q1.tar.gz`

### Purged Fixes (optional)

**Policy**: Архивы старше 1 года могут быть удалены

**Rationale**: Git commit history сохраняет информацию о фиксах через commit messages

---

## Maintenance Commands

### 1. After Each Fix (Automatic)

После выполнения `/speckit.fix` автоматически:
1. Fix artifacts сохраняются в `.specify/memory/fixes/YYYY-MM/DD-bug-name/`
2. README.md обновляется (добавляется новая запись)

**Manual update** (если нужно):
```bash
.specify/scripts/update-fix-index.sh
```

### 2. Monthly Archive (Run on 1st of each month)

Архивировать фиксы старше 90 дней:

```bash
.specify/scripts/archive-old-fixes.sh
```

**What it does**:
- Находит directories старше 90 дней
- Создаёт/обновляет quarterly archive (YYYY-QX.tar.gz)
- Удаляет исходные directories
- Выводит summary

### 3. Search Fixes

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

**By keyword in bug description**:
```bash
grep -r "DontDestroyOnLoad" .specify/memory/fixes/*/*/bug_context/bug.md
```

### 4. Extract Archived Fix

```bash
# Extract specific quarter
tar -xzf .specify/memory/fixes/archived/2026-Q1.tar.gz -C /tmp/

# View archived fix
ls /tmp/2026-03/10-audio-di-null/
```

---

## Scalability Strategy

### For 10-50 Fixes

**Current structure works**:
- Manual review of README.md
- No archiving needed

### For 50-200 Fixes

**Add automation**:
1. Run `update-fix-index.sh` after each fix
2. Monthly review of README.md statistics
3. Identify recurring issues (top components)

### For 200+ Fixes

**Full automation**:
1. Auto-archive on 1st of month (cron job or CI/CD)
2. Generate monthly reports (top issues, resolution time)
3. Database approach (SQLite для metadata, directories для full docs)

**Optional: SQLite index**:
```sql
CREATE TABLE fixes (
    id INTEGER PRIMARY KEY,
    date TEXT,
    component TEXT,
    summary TEXT,
    commit TEXT,
    root_cause TEXT,
    files_modified TEXT,
    confidence INTEGER
);
```

Query examples:
```sql
-- Top problematic components
SELECT component, COUNT(*) as count 
FROM fixes 
WHERE date >= '2026-03-01' 
GROUP BY component 
ORDER BY count DESC 
LIMIT 10;

-- Average confidence by component
SELECT component, AVG(confidence) as avg_confidence
FROM fixes
GROUP BY component
ORDER BY avg_confidence ASC;
```

---

## Best Practices

### 1. Consistent Naming

**Format**: `DD-component-issue-keyword`

**Good examples**:
- `10-scene-manager-background-null`
- `15-dialogue-system-auto-advance-crash`
- `20-vcontainer-monobehaviour-registration`

**Bad examples**:
- `fix1` (no context)
- `bug-today` (no component)
- `10-some-random-bug` (not specific)

### 2. Update README.md Immediately

After `/speckit.fix` completes, run:
```bash
.specify/scripts/update-fix-index.sh
git add .specify/memory/fixes/README.md
git commit --amend --no-edit  # Add to last commit
```

### 3. Quarterly Review

**1st of Q2, Q3, Q4, Q1**:
1. Run `archive-old-fixes.sh`
2. Review statistics в README.md
3. Identify patterns (recurring issues → refactoring needed)
4. Update Constitution if patterns found

### 4. Knowledge Transfer

**Monthly summary email/report**:
```markdown
## March 2026 Fixes Summary

- Total: 89 fixes
- Top component: SceneManager (23 fixes) → needs refactoring
- Average resolution time: 2 iterations
- Most common root cause: API misuse (34%), incomplete implementation (28%)

Recurring patterns:
- VContainer MonoBehaviour registration errors (5 instances)
- DontDestroyOnLoad missing (3 instances)

Recommendations:
- Add VContainer best practices to Constitution
- Create reusable components for common patterns
```

---

## Directory Size Management

### Current

```bash
# Check size
du -sh .specify/memory/fixes/
# ~2MB for 2 fixes
```

### Projected (1000 fixes)

```bash
# Without archiving: ~1GB
# With quarterly archiving: ~200MB
```

**Optimization**:
- Архивы сжимаются в ~10% от исходного размера
- Старые архивы можно хранить в S3/GitHub Releases
- `.fix/` временная директория не коммитится (в .gitignore)

---

## Integration with CI/CD (Future)

### Auto-archive on deployment

```yaml
# .github/workflows/archive-fixes.yml
name: Archive Old Fixes

on:
  schedule:
    - cron: '0 0 1 * *'  # 1st of each month

jobs:
  archive:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Archive old fixes
        run: .specify/scripts/archive-old-fixes.sh
      - name: Commit archives
        run: |
          git add .specify/memory/fixes/archived/
          git commit -m "chore: archive fixes older than 90 days"
          git push
```

---

## Emergency: "Too Many Fixes" Recovery

Если директория разрослась до неуправляемых размеров:

### Экстренная очистка

```bash
# 1. Создать полный backup
tar -czf fixes-backup-$(date +%Y%m%d).tar.gz .specify/memory/fixes/

# 2. Архивировать всё старше 30 дней
find .specify/memory/fixes -type d -name "[0-9][0-9][0-9][0-9]-[0-9][0-9]" -mtime +30 -exec \
    tar -czf .specify/memory/fixes/archived/{}.tar.gz {} \; -exec rm -rf {} \;

# 3. Пересоздать README.md
.specify/scripts/update-fix-index.sh

# 4. Commit cleanup
git add .specify/memory/fixes/
git commit -m "chore: archive old fixes (emergency cleanup)"
```

---

## Monitoring

### Weekly Check

```bash
# Count active fixes
ls .specify/memory/fixes/2026-* -R | grep "^[0-9][0-9]-" | wc -l

# Check directory size
du -sh .specify/memory/fixes/

# If >500MB → run archive-old-fixes.sh
```

### Alerts

**Trigger**: `du -sh .specify/memory/fixes/` > 500MB  
**Action**: Email dev team + auto-run archiving script

---

## Future Enhancements

1. **Web UI** для просмотра фиксов (static site generator)
2. **Search API** (Elasticsearch/Algolia для быстрого поиска)
3. **AI Pattern Detection** (анализ theories для выявления recurring patterns)
4. **Auto-refactoring Suggestions** (если >5 похожих багов → suggest refactoring)
