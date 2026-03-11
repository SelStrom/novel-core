---
description: Multi-agent bug fixing pipeline. Orchestrates custom subagents for deterministic debugging and patching.
handoffs:
  - label: Create Regression Tests
    agent: speckit.tasks
    prompt: Create test tasks for the regression tests from .fix/tests/
  - label: Review Constitution
    agent: speckit.constitution
    prompt: Verify fix aligns with constitution principles
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Overview

You are an **orchestrator**. Your job is to coordinate six custom subagents defined in `.claude/agents/` to fix a bug through a structured pipeline. You do NOT do the analysis or patching yourself — you delegate to subagents, validate their outputs, and manage the flow.

### Subagents (defined in `.claude/agents/`)

| Subagent | File | Role |
|----------|------|------|
| bug-context | `bug-context.md` | Reproduce the bug with a failing test |
| spec-theory | `spec-theory.md` | Analyze specs for contradictions (readonly) |
| impl-theory | `impl-theory.md` | Analyze code for logic errors (readonly) |
| theory-arbitrator | `theory-arbitrator.md` | Select the best theory |
| fix-patcher | `fix-patcher.md` | Generate minimal patch |
| break-tester | `break-tester.md` | Try to break the fix |

### Execution graph

```
You (Orchestrator)
  │
  ├─ Stage 1: Setup .fix/ directory
  │
  ├─ Stage 2: /bug-context  ← foreground, wait for result
  │     └─ validate: failing_test.cs exists?
  │
  ├─ Stage 3: /spec-theory + /impl-theory  ← PARALLEL (two subagents at once)
  │     └─ validate: both theory files exist?
  │
  ├─ Stage 4: /theory-arbitrator  ← foreground, wait for result
  │     └─ validate: selected_theory.md exists?
  │
  ├─ Stage 5: Fix loop (max 10 iterations)
  │     ├─ /fix-patcher  ← foreground
  │     ├─ You: validate build + run tests
  │     └─ If fail → resume fix-patcher with feedback OR restart
  │
  ├─ Stage 6: /break-tester  ← foreground
  │     └─ validate: recommendation == APPROVE?
  │
  └─ Stage 7: Commit (you do this directly)
```

## Stage 1: Prerequisites

**You do this directly** (no subagent needed).

1. Create directory structure:
   ```bash
   mkdir -p .fix/{bug_context,theories,tests,patch}
   ```

2. Ensure `.fix/` is in `.gitignore`:
   ```bash
   grep -q '^\.fix/' .gitignore || echo '.fix/' >> .gitignore
   ```

## Stage 2: Bug Reproduction

Invoke the bug-context subagent:

```
/bug-context Analyze and reproduce this bug: $ARGUMENTS
```

**Wait for completion**, then validate:

1. Read `.fix/bug_context/failing_test.cs` — must exist
2. Read `.fix/bug_context/bug.md` — must have Expected/Actual sections
3. If `.fix/bug_context/MANUAL_REVIEW_REQUIRED.md` exists → **STOP**, show contents to user, request more information

Only proceed to Stage 3 if validation passes.

## Stage 3: Theory Generation (PARALLEL)

Launch **both** subagents simultaneously:

```
/spec-theory Analyze specs for this bug. Context in .fix/bug_context/
```

```
/impl-theory Analyze implementation for this bug. Context in .fix/bug_context/
```

These run in **parallel** — both are readonly so they cannot conflict.

**Wait for both to complete**, then validate:

1. Read `.fix/theories/spec_theory.md` — extract confidence score
2. Read `.fix/theories/impl_theory.md` — extract confidence score
3. If either file is missing → retry that subagent once

## Stage 4: Theory Arbitration

Invoke the theory-arbitrator subagent:

```
/theory-arbitrator Compare theories and select fix strategy. Spec confidence: [X]%, Impl confidence: [Y]%
```

**Wait for completion**, then validate:

1. Read `.fix/theories/selected_theory.md` — must contain Decision and Fix Strategy
2. If decision is MANUAL_REVIEW_REQUIRED (both theories < 50%) → **STOP**, present both theories to user, ask which is more likely
3. If diagnostic tests were generated → optionally run them to refine decision

Extract the selected theory type (SPEC_THEORY / IMPL_THEORY) and fix strategy for Stage 5.

## Stage 5: Fix Iteration Loop

**You manage this loop.** Max 10 iterations.

```
iteration = 1

while iteration <= max_iterations:

    1. Invoke fix-patcher:
       /fix-patcher Generate fix (iteration [N]). Strategy from .fix/theories/selected_theory.md
       
       If iteration > 1, include feedback:
       /fix-patcher Iteration [N]. Previous attempt failed: [build errors / test failures]. 
       Fix strategy: .fix/theories/selected_theory.md

    2. Wait for completion

    3. Validate Unity build:
       Run: Unity -quit -batchmode -nographics -projectPath "$(pwd)" \
            -logFile ".fix/patch/unity_compile_iter${iteration}.log"
       
       If build fails → log to .fix/patch/iterations.log, increment, continue

    4. Run tests:
       Run: Unity -batchmode -nographics -projectPath "$(pwd)" \
            -runTests -testPlatform EditMode \
            -testResults ".fix/patch/test_results_iter${iteration}.xml" \
            -logFile ".fix/patch/unity_tests_iter${iteration}.log"
       
       Check: does failing_test.cs now PASS?
       Check: do all existing tests still pass?
       
       If all pass → break (success!)
       If fail → log to .fix/patch/iterations.log, increment, continue

    iteration++
```

If loop exhausts without success → **STOP**, show `.fix/patch/iterations.log` to user with best attempt details.

## Stage 6: Break Testing

Invoke the break-tester subagent:

```
/break-tester Try to break the fix. Patch details in .fix/patch/fix_explanation.md
```

**Wait for completion**, then read `.fix/tests/break_results.md`:

| Recommendation | Action |
|---------------|--------|
| APPROVE | Proceed to Stage 7 |
| RETURN_TO_FIX | Go back to Stage 5 with break test failures as feedback |
| ESCALATE | **STOP**, present regressions to user |

## Stage 7: Commit

**You do this directly** (no subagent needed). Only if ALL conditions met:

- Build succeeds
- Failing test now passes
- Break tests pass
- No regressions

### Commit process

1. Stage modified files:
   ```bash
   git add [modified source files]
   git add .fix/bug_context/failing_test.cs
   git add .fix/tests/break_tests.cs
   ```

2. Commit:
   ```bash
   git commit -m "$(cat <<'EOF'
   fix: [brief bug description]

   Root Cause: [from selected_theory.md]
   Theory: [SPEC_THEORY|IMPL_THEORY] ([X]% confidence)

   Changes:
   - [file]: [change]

   Tests:
   - Added reproduction test
   - Added [N] break tests
   - All passing (EditMode: X/X)
   EOF
   )"
   ```

3. Archive fix documentation:
   ```bash
   FIX_DIR=".specify/memory/fixes/$(date +%Y-%m)/$(date +%d)-[component]-[keyword]"
   mkdir -p "$FIX_DIR"
   cp -r .fix/* "$FIX_DIR/"
   ```

## Error Handling

### Bug cannot be reproduced
Show `MANUAL_REVIEW_REQUIRED.md` to user. Request: Editor.log, crash dump, exact steps, frequency.

### Theories both low confidence
Show both theory files. Ask user: "Which theory seems more likely? 1) Spec issue 2) Code issue 3) Neither"

### Fix iterations exhausted
Show `iterations.log`. Ask: "Revise theory? Proceed with best attempt? Manual debug?"

### Critical regressions
Do NOT commit. Show break test results. Ask: "Revise fix? Relax constraints? Manual investigation?"

## Report to User

After successful commit:

```markdown
## Fix Complete

**Bug**: [description]
**Root Cause**: [from selected theory]
**Theory**: [SPEC|IMPL] ([X]% confidence)
**Iterations**: [N]/10
**Files Modified**: [count]
**Tests Added**: [count]

### Next Steps
- Manually test in Unity Editor (recommended)
- Monitor for related bugs
```

## Defaults

- Max fix iterations: 10
- Max patch size: 200 lines
- Min theory confidence for auto-proceed: 50%
- Diagnostic tests required if confidence difference < 20%
