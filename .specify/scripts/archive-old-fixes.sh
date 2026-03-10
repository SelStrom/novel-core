#!/bin/bash
# Archive old fixes (>90 days) to reduce directory clutter
# Usage: .specify/scripts/archive-old-fixes.sh

set -e

FIXES_DIR=".specify/memory/fixes"
ARCHIVED_DIR="$FIXES_DIR/archived"
CURRENT_DATE=$(date +%s)
RETENTION_DAYS=90
RETENTION_SECONDS=$((RETENTION_DAYS * 24 * 60 * 60))

echo "🗄️  Archiving fixes older than $RETENTION_DAYS days..."

# Create archived directory if not exists
mkdir -p "$ARCHIVED_DIR"

# Find all month directories (YYYY-MM format)
for month_dir in "$FIXES_DIR"/[0-9][0-9][0-9][0-9]-[0-9][0-9]; do
    if [ ! -d "$month_dir" ]; then
        continue
    fi
    
    # Get month directory modification time
    month_name=$(basename "$month_dir")
    
    # Check if month directory is older than retention period
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        dir_mtime=$(stat -f %m "$month_dir")
    else
        # Linux
        dir_mtime=$(stat -c %Y "$month_dir")
    fi
    
    age_seconds=$((CURRENT_DATE - dir_mtime))
    
    if [ $age_seconds -gt $RETENTION_SECONDS ]; then
        echo "  📦 Archiving: $month_name (age: $((age_seconds / 86400)) days)"
        
        # Extract year and quarter from month
        year=$(echo "$month_name" | cut -d'-' -f1)
        month=$(echo "$month_name" | cut -d'-' -f2)
        quarter=$(( (10#$month - 1) / 3 + 1 ))
        
        archive_name="$year-Q$quarter.tar.gz"
        
        # Create or append to quarterly archive
        if [ -f "$ARCHIVED_DIR/$archive_name" ]; then
            # Append to existing archive
            tar -czf "$ARCHIVED_DIR/${archive_name}.tmp" -C "$FIXES_DIR" "$month_name"
            tar -xzf "$ARCHIVED_DIR/$archive_name" -C /tmp/
            tar -xzf "$ARCHIVED_DIR/${archive_name}.tmp" -C /tmp/
            tar -czf "$ARCHIVED_DIR/$archive_name" -C /tmp/ .
            rm "$ARCHIVED_DIR/${archive_name}.tmp"
            rm -rf /tmp/$(basename "$month_dir")
        else
            # Create new archive
            tar -czf "$ARCHIVED_DIR/$archive_name" -C "$FIXES_DIR" "$month_name"
        fi
        
        # Remove original directory
        rm -rf "$month_dir"
        echo "    ✅ Archived to: archived/$archive_name"
    else
        echo "  ⏭️  Skipping: $month_name (age: $((age_seconds / 86400)) days < $RETENTION_DAYS days)"
    fi
done

echo "✅ Archiving complete!"
echo ""
echo "Archived fixes location: $ARCHIVED_DIR"
ls -lh "$ARCHIVED_DIR" 2>/dev/null || echo "  (no archives yet)"
