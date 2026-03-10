#!/bin/bash
# Check if Constitution needs updating based on fix patterns
# Usage: .specify/scripts/check-constitution-updates.sh <fix-directory>
# Example: .specify/scripts/check-constitution-updates.sh .specify/memory/fixes/2026-03/10-audio-di-null

set -e

FIX_DIR="${1:-.fix}"
FIXES_ARCHIVE=".specify/memory/fixes"

if [ ! -d "$FIX_DIR" ]; then
    echo "❌ Error: Fix directory not found: $FIX_DIR"
    exit 1
fi

echo "📋 Checking if Constitution update needed..."
echo ""

# Extract root cause category from impl_theory.md
IMPL_THEORY="$FIX_DIR/theories/impl_theory.md"

if [ ! -f "$IMPL_THEORY" ]; then
    echo "⚠️  Warning: impl_theory.md not found, skipping analysis"
    exit 0
fi

# Extract category
CATEGORY=$(grep -A1 "### Category" "$IMPL_THEORY" | grep "^\- \[x\]" | sed 's/- \[x\] //' | sed 's/ (.*//')

if [ -z "$CATEGORY" ]; then
    echo "ℹ️  No clear category identified, manual review recommended"
    exit 0
fi

echo "🔍 Root Cause Category: $CATEGORY"
echo ""

# Search for similar fixes in archive
SIMILAR_FIXES=$(grep -r "\[x\] $CATEGORY" "$FIXES_ARCHIVE" 2>/dev/null | grep "impl_theory.md" | wc -l | tr -d ' ')

echo "📊 Statistics:"
echo "  Similar fixes found: $SIMILAR_FIXES"
echo ""

# Check thresholds
if [ "$SIMILAR_FIXES" -ge 3 ]; then
    echo "⚠️  CONSTITUTION UPDATE RECOMMENDED"
    echo ""
    echo "Reason: $SIMILAR_FIXES instances of '$CATEGORY' found"
    echo ""
    echo "Recommended actions:"
    echo "  1. Review similar fixes:"
    echo "     grep -r \"\\[x\\] $CATEGORY\" $FIXES_ARCHIVE"
    echo ""
    echo "  2. Identify common pattern"
    echo ""
    echo "  3. Add preventive guidance to Constitution:"
    echo "     - Update Principle VI with best practices"
    echo "     - Add code review checklist item"
    echo "     - Consider adding linter rule or .editorconfig"
    echo ""
    echo "  4. Run: /speckit.constitution"
    echo ""
    exit 2
elif [ "$SIMILAR_FIXES" -ge 2 ]; then
    echo "ℹ️  Pattern emerging: $SIMILAR_FIXES instances found"
    echo "   Monitor for 3rd occurrence → then update Constitution"
    echo ""
else
    echo "✅ No pattern detected (only $SIMILAR_FIXES instance)"
    echo "   Constitution update not needed"
    echo ""
fi

# Check archiving threshold
TOTAL_FIXES=$(find "$FIXES_ARCHIVE" -type d -name "[0-9][0-9]-*" 2>/dev/null | wc -l | tr -d ' ')
ARCHIVE_SIZE=$(du -sm "$FIXES_ARCHIVE" 2>/dev/null | cut -f1)

echo "📦 Archive Status:"
echo "  Total active fixes: $TOTAL_FIXES"
echo "  Directory size: ${ARCHIVE_SIZE}MB"
echo ""

if [ "$TOTAL_FIXES" -ge 100 ]; then
    echo "⚠️  ARCHIVING RECOMMENDED"
    echo "   Run: .specify/scripts/archive-old-fixes.sh"
    echo ""
elif [ "$ARCHIVE_SIZE" -ge 500 ]; then
    echo "⚠️  ARCHIVING RECOMMENDED (size threshold)"
    echo "   Run: .specify/scripts/archive-old-fixes.sh"
    echo ""
else
    echo "✅ Archiving not needed yet"
    echo ""
fi

exit 0
