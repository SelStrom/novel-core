#!/bin/bash
# setup-fix.sh - Initialize .fix/ directory structure for bug fixing pipeline

set -euo pipefail

# Source common utilities
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/common.sh"

# Configuration
FIX_DIR="${REPO_ROOT}/.fix"
CONFIG_FILE="${REPO_ROOT}/.specify/config/fix.json"

# Parse arguments
BUG_ID=""
CLEAN=false
OUTPUT_JSON=false

print_usage() {
    cat <<EOF
Usage: $(basename "$0") [OPTIONS]

Initialize .fix/ directory structure for bug fixing pipeline.

OPTIONS:
    --bug-id ID         Bug identifier (issue number or descriptive name)
    --clean             Clean existing .fix/ directory before initialization
    --json              Output JSON with directory paths
    -h, --help          Show this help message

EXAMPLES:
    $(basename "$0") --bug-id "dialogue-text-missing"
    $(basename "$0") --bug-id "123" --clean
    $(basename "$0") --json

DIRECTORY STRUCTURE:
    .fix/
    ├── agents/              # Agent logs (temporary)
    ├── bug_context/         # Bug context (preserved)
    ├── theories/            # Theories (preserved)
    ├── tests/               # Tests (preserved)
    └── patch/               # Patch files (preserved)
EOF
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --bug-id)
            BUG_ID="$2"
            shift 2
            ;;
        --clean)
            CLEAN=true
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

# Main execution
main() {
    log_info "Initializing .fix/ directory structure..."

    # Clean if requested
    if [[ "${CLEAN}" == "true" && -d "${FIX_DIR}" ]]; then
        log_warn "Cleaning existing .fix/ directory..."
        rm -rf "${FIX_DIR}"
    fi

    # Create directory structure
    log_info "Creating directories..."
    mkdir -p "${FIX_DIR}/agents"
    mkdir -p "${FIX_DIR}/bug_context"
    mkdir -p "${FIX_DIR}/theories"
    mkdir -p "${FIX_DIR}/tests"
    mkdir -p "${FIX_DIR}/patch"

    # Create README in .fix/
    cat > "${FIX_DIR}/README.md" <<'EOF'
# Bug Fix Pipeline

This directory contains artifacts from the automated bug fixing pipeline.

## Structure

- `agents/` - Agent execution logs (temporary, not committed)
- `bug_context/` - Bug reproduction context (preserved)
- `theories/` - Spec and implementation theories (preserved)
- `tests/` - Diagnostic and break tests (preserved)
- `patch/` - Fix patches and summaries (preserved)

## Workflow

1. BugContextAgent → `bug_context/`
2. SpecTheoryAgent + ImplTheoryAgent → `theories/`
3. TheoryArbitratorAgent → `theories/selected_theory.md`
4. FixAgent → `patch/`
5. BreakAgent → `tests/break_tests.cs`
6. Commit → Archive to `.specify/memory/fixes/`

## Cleanup

After successful commit, contents are archived to:
`.specify/memory/fixes/[bug-id]-[YYYY-MM-DD]/`

Temporary logs in `agents/` are removed.
EOF

    # Create .gitignore for .fix/
    cat > "${FIX_DIR}/.gitignore" <<'EOF'
# Agent logs (temporary)
agents/

# Unity logs (temporary)
patch/unity_*.log
patch/test_results_iter*.xml

# Only preserve final artifacts
!bug_context/
!theories/
!tests/
!patch/fix_summary.md
!patch/patch.diff
!patch/iterations.log
EOF

    # Initialize bug metadata if bug-id provided
    if [[ -n "${BUG_ID}" ]]; then
        log_info "Initializing bug metadata for: ${BUG_ID}"
        
        cat > "${FIX_DIR}/bug_context/metadata.json" <<EOF
{
  "bug_id": "${BUG_ID}",
  "created_at": "$(date -u +"%Y-%m-%dT%H:%M:%SZ")",
  "status": "initialized",
  "pipeline_version": "1.0.0"
}
EOF
    fi

    # Verify .fix/ is in root .gitignore (except final artifacts)
    GITIGNORE="${REPO_ROOT}/.gitignore"
    if [[ -f "${GITIGNORE}" ]]; then
        if ! grep -q "^\.fix/" "${GITIGNORE}" 2>/dev/null; then
            log_info "Adding .fix/ to .gitignore..."
            cat >> "${GITIGNORE}" <<'EOF'

# Bug fixing pipeline (temporary artifacts)
.fix/agents/
.fix/patch/unity_*.log
.fix/patch/test_results_iter*.xml
EOF
        fi
    else
        log_warn ".gitignore not found in repo root"
    fi

    # Verify config exists
    if [[ ! -f "${CONFIG_FILE}" ]]; then
        log_warn "Fix configuration not found at ${CONFIG_FILE}"
        log_info "Using default configuration..."
        mkdir -p "$(dirname "${CONFIG_FILE}")"
        cat > "${CONFIG_FILE}" <<'EOF'
{
  "max_fix_iterations": 10,
  "max_patch_size_lines": 200,
  "break_tests": {
    "enabled": true,
    "generate_edge_cases": true,
    "generate_adversarial": true
  },
  "test_execution": {
    "editmode_first": true,
    "playmode_if_needed": true,
    "timeout_seconds": 300
  },
  "commit": {
    "auto_commit": false,
    "require_manual_approval": true
  }
}
EOF
    fi

    # Output results
    if [[ "${OUTPUT_JSON}" == "true" ]]; then
        cat <<EOF
{
  "fix_dir": "${FIX_DIR}",
  "bug_context_dir": "${FIX_DIR}/bug_context",
  "theories_dir": "${FIX_DIR}/theories",
  "tests_dir": "${FIX_DIR}/tests",
  "patch_dir": "${FIX_DIR}/patch",
  "agents_dir": "${FIX_DIR}/agents",
  "config_file": "${CONFIG_FILE}",
  "bug_id": "${BUG_ID}",
  "status": "initialized"
}
EOF
    else
        log_success ".fix/ directory structure initialized!"
        log_info "Location: ${FIX_DIR}"
        if [[ -n "${BUG_ID}" ]]; then
            log_info "Bug ID: ${BUG_ID}"
        fi
    fi
}

# Run main
main "$@"
