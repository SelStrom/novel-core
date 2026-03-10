#!/bin/bash
# Update fix index (README.md) after new fix is added
# Usage: .specify/scripts/update-fix-index.sh

set -e

FIXES_DIR=".specify/memory/fixes"
INDEX_FILE="$FIXES_DIR/README.md"

echo "📊 Updating fix index..."

# Count total fixes
TOTAL_FIXES=$(find "$FIXES_DIR" -type d -name "[0-9][0-9]-*" 2>/dev/null | wc -l | tr -d ' ')

# Count fixes this month
CURRENT_MONTH=$(date +%Y-%m)
MONTH_FIXES=$(find "$FIXES_DIR/$CURRENT_MONTH" -type d -name "[0-9][0-9]-*" 2>/dev/null | wc -l | tr -d ' ')

# Get current date
CURRENT_DATE=$(date +%Y-%m-%d)

echo "  Total fixes: $TOTAL_FIXES"
echo "  This month: $MONTH_FIXES"

# Generate recent fixes table (last 50)
RECENT_FIXES=""
fix_id=1

# Use simple iteration instead of associative arrays
for month_dir in $(ls -r "$FIXES_DIR" 2>/dev/null | grep -E "^[0-9]{4}-[0-9]{2}$"); do
    if [ ! -d "$FIXES_DIR/$month_dir" ]; then
        continue
    fi
    
    for fix_dir in $(ls -r "$FIXES_DIR/$month_dir" 2>/dev/null | grep -E "^[0-9]{2}-"); do
        fix_path="$FIXES_DIR/$month_dir/$fix_dir"
        
        if [ ! -d "$fix_path" ]; then
            continue
        fi
        
        # Extract info from bug.md and fix_summary.md
        summary_file="$fix_path/patch/fix_summary.md"
        
        if [ -f "$summary_file" ]; then
            # Extract commit from fix_summary.md
            commit=$(grep -m1 "Commit" "$summary_file" | sed 's/.*Commit.*: *//' | sed 's/`//g' | sed 's/\*//g' | tr -d ' ' | head -c 7 || echo "N/A")
            
            # Extract root cause summary (first non-empty line after "## Root Cause")
            root_cause=$(awk '/^## Root Cause/{getline; while(getline && NF==0); if(NF>0) print; exit}' "$summary_file" | sed 's/^[[:space:]]*//' | cut -c1-50 || echo "N/A")
        else
            commit="N/A"
            root_cause="See documentation"
        fi
        
        # Extract component from directory name
        component=$(echo "$fix_dir" | sed 's/^[0-9][0-9]-//' | sed 's/-/ /g' | awk '{for(i=1;i<=NF;i++) $i=toupper(substr($i,1,1)) tolower(substr($i,2))}1' | cut -c1-25)
        
        # Format date (YYYY-MM-DD from directory name)
        fix_date="${month_dir}-${fix_dir%%-*}"
        
        # Pad fix_id to 3 digits
        fix_id_padded=$(printf "%03d" $fix_id)
        
        # Add to recent fixes
        RECENT_FIXES+="| $fix_date | $fix_id_padded | $component | $root_cause | $commit | ✅ Fixed |\n"
        
        fix_id=$((fix_id + 1))
        
        # Limit to 50 recent fixes
        if [ $fix_id -gt 50 ]; then
            break 2
        fi
    done
done

echo "  Recent fixes table generated"

# Update README.md
cat > "$INDEX_FILE" << EOF
# Fix Index

**Last Updated**: $CURRENT_DATE  
**Total Fixes**: $TOTAL_FIXES

---

## Statistics

### By Month

| Month | Fixes Count |
|-------|-------------|
| $CURRENT_MONTH | $MONTH_FIXES |

---

## Recent Fixes (Last 50)

| Date | ID | Component | Summary | Commit | Status |
|------|----|-----------|---------|--------|--------|
$(echo -e "$RECENT_FIXES")

---

## Fix Documentation Structure

Each fix directory contains:
\`\`\`
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
\`\`\`

---

## Retention Policy

- **Active** (0-90 дней): Full documentation в \`YYYY-MM/DD-bug-name/\`
- **Archived** (90+ дней): Compressed в \`archived/YYYY-QX.tar.gz\`
- **Purged** (optional): Архивы старше 1 года могут быть удалены

---

## Maintenance Commands

### Add New Fix

\`\`\`bash
# Автоматически создаётся через /speckit.fix
# Структура: .specify/memory/fixes/YYYY-MM/DD-component-issue/
\`\`\`

### Archive Old Fixes (>90 days)

\`\`\`bash
.specify/scripts/archive-old-fixes.sh
\`\`\`

### Update This Index

\`\`\`bash
.specify/scripts/update-fix-index.sh
\`\`\`

### Search Fixes

\`\`\`bash
# По компоненту
grep -r "SceneManager" .specify/memory/fixes/*/*/bug_context/relevant_files.md

# По дате
ls .specify/memory/fixes/2026-03/

# По commit
grep -r "bc50296" .specify/memory/fixes/
\`\`\`

---

## See Also

- [MAINTENANCE.md](./MAINTENANCE.md) - Detailed maintenance guide
- [.specify/scripts/archive-old-fixes.sh](../scripts/archive-old-fixes.sh) - Archiving automation
- [.specify/scripts/update-fix-index.sh](../scripts/update-fix-index.sh) - Index update automation
EOF

echo "✅ Fix index updated!"
echo "  File: $INDEX_FILE"
echo ""
echo "Summary:"
echo "  Total Fixes: $TOTAL_FIXES"
echo "  This Month ($CURRENT_MONTH): $MONTH_FIXES"
