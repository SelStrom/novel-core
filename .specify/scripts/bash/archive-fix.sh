#!/bin/bash
# archive-fix.sh - Archive .fix/ artifacts to .specify/memory/fixes/ after successful commit

set -euo pipefail

# Source common utilities
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/common.sh"

# Configuration
FIX_DIR="${REPO_ROOT}/.fix"
MEMORY_DIR="${REPO_ROOT}/.specify/memory/fixes"

# Parse arguments
BUG_ID=""
KEEP_LOGS=false
OUTPUT_JSON=false

print_usage() {
    cat <<EOF
Usage: $(basename "$0") [OPTIONS]

Archive .fix/ artifacts to .specify/memory/fixes/ after successful bug fix.

OPTIONS:
    --bug-id ID         Bug identifier (required)
    --keep-logs         Keep temporary logs in archive
    --json              Output JSON with archive paths
    -h, --help          Show this help message

EXAMPLES:
    $(basename "$0") --bug-id "dialogue-text-missing"
    $(basename "$0") --bug-id "123" --keep-logs

ARCHIVE STRUCTURE:
    .specify/memory/fixes/[bug-id]-[YYYY-MM-DD]/
    ├── bug_context/
    ├── theories/
    ├── tests/
    └── patch/
EOF
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --bug-id)
            BUG_ID="$2"
            shift 2
            ;;
        --keep-logs)
            KEEP_LOGS=true
            shift
            ;;
        --json)
            OUTPUT_JSON=true
            shift
            ;;
        -h|--help)
            print_usage
            exit 0
            ;;
        *)
            echo "Unknown option: $1" >&2
            print_usage >&2
            exit 1
            ;;
    esac
done

# Validation
if [[ -z "${BUG_ID}" ]]; then
    log_error "Bug ID is required (--bug-id)"
    print_usage >&2
    exit 1
fi

if [[ ! -d "${FIX_DIR}" ]]; then
    log_error ".fix/ directory not found: ${FIX_DIR}"
    exit 1
fi

# Main execution
main() {
    local timestamp
    timestamp=$(date +"%Y-%m-%d")
    local archive_name="${BUG_ID}-${timestamp}"
    local archive_dir="${MEMORY_DIR}/${archive_name}"

    log_info "Archiving fix artifacts for bug: ${BUG_ID}"

    # Create archive directory
    mkdir -p "${archive_dir}"

    # Copy preserved directories
    log_info "Copying bug_context..."
    if [[ -d "${FIX_DIR}/bug_context" ]]; then
        cp -r "${FIX_DIR}/bug_context" "${archive_dir}/"
    else
        log_warn "bug_context/ directory not found"
    fi

    log_info "Copying theories..."
    if [[ -d "${FIX_DIR}/theories" ]]; then
        cp -r "${FIX_DIR}/theories" "${archive_dir}/"
    else
        log_warn "theories/ directory not found"
    fi

    log_info "Copying tests..."
    if [[ -d "${FIX_DIR}/tests" ]]; then
        cp -r "${FIX_DIR}/tests" "${archive_dir}/"
    else
        log_warn "tests/ directory not found"
    fi

    log_info "Copying patch..."
    if [[ -d "${FIX_DIR}/patch" ]]; then
        # Copy patch directory
        mkdir -p "${archive_dir}/patch"
        
        # Always copy these
        [[ -f "${FIX_DIR}/patch/fix_summary.md" ]] && \
            cp "${FIX_DIR}/patch/fix_summary.md" "${archive_dir}/patch/"
        [[ -f "${FIX_DIR}/patch/patch.diff" ]] && \
            cp "${FIX_DIR}/patch/patch.diff" "${archive_dir}/patch/"
        [[ -f "${FIX_DIR}/patch/iterations.log" ]] && \
            cp "${FIX_DIR}/patch/iterations.log" "${archive_dir}/patch/"
        
        # Optionally copy logs
        if [[ "${KEEP_LOGS}" == "true" ]]; then
            log_info "Preserving temporary logs..."
            cp "${FIX_DIR}"/patch/unity_*.log "${archive_dir}/patch/" 2>/dev/null || true
            cp "${FIX_DIR}"/patch/test_results_*.xml "${archive_dir}/patch/" 2>/dev/null || true
        fi
    else
        log_warn "patch/ directory not found"
    fi

    # Create archive metadata
    cat > "${archive_dir}/metadata.json" <<EOF
{
  "bug_id": "${BUG_ID}",
  "archived_at": "$(date -u +"%Y-%m-%dT%H:%M:%SZ")",
  "archive_name": "${archive_name}",
  "kept_logs": ${KEEP_LOGS},
  "source_dir": "${FIX_DIR}"
}
EOF

    # Create README in archive
    cat > "${archive_dir}/README.md" <<EOF
# Bug Fix Archive: ${BUG_ID}

**Archived**: ${timestamp}

## Contents

- \`bug_context/\` - Bug reproduction context
- \`theories/\` - Spec and implementation theories
- \`tests/\` - Diagnostic and break tests
- \`patch/\` - Fix patches and summaries

## Fix Summary

See \`patch/fix_summary.md\` for detailed fix information.

## Theory Selected

See \`theories/selected_theory.md\` for theory selection rationale.
EOF

    # Clean up .fix/ directory (but keep structure for next fix)
    log_info "Cleaning up .fix/ directory..."
    rm -rf "${FIX_DIR}/bug_context"/*
    rm -rf "${FIX_DIR}/theories"/*
    rm -rf "${FIX_DIR}/tests"/*
    rm -rf "${FIX_DIR}/patch"/*
    rm -rf "${FIX_DIR}/agents"/*

    # Output results
    if [[ "${OUTPUT_JSON}" == "true" ]]; then
        cat <<EOF
{
  "bug_id": "${BUG_ID}",
  "archive_dir": "${archive_dir}",
  "archive_name": "${archive_name}",
  "timestamp": "${timestamp}",
  "kept_logs": ${KEEP_LOGS},
  "status": "archived"
}
EOF
    else
        log_success "Fix artifacts archived successfully!"
        log_info "Archive location: ${archive_dir}"
        log_info "Bug ID: ${BUG_ID}"
        log_info "Timestamp: ${timestamp}"
    fi
}

# Run main
main "$@"
