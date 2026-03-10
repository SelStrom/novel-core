# Selected Theory

## Decision

**Selected**: **IMPL_THEORY** (Implementation Theory)

## Reasoning

| Aspect                | Spec Theory | Impl Theory    |
|-----------------------|-------------|----------------|
| Confidence Score      | 5%          | **98%**        |
| Evidence Quality      | N/A         | **TRIVIAL**    |
| Root Cause Coverage   | Not applicable | **COMPLETE** |

### Decision Rationale

1. **Trivial Implementation Bug**: Early `return` before `EndHorizontal()` — objectively verified by code inspection
2. **100% Reproducible**: Every "×" button click triggers GUI error
3. **Unity Framework Constraint**: Not a spec issue — Unity IMGUI requires Begin/End balance
4. **Regression from Previous Fix**: Commit 7ef9131 introduced `return` (copied from original buggy code)

### Rejected Theory

**Rejected**: SPEC_THEORY

**Reason**: Not applicable — this is Unity API usage error, not architectural/spec issue.

## Confidence in Decision

**100%**

Unity GUI balance requirement is non-negotiable framework constraint. Баг очевидный.

## Recommended Fix Strategy

### Implementation Fix

**Change**: Replace `return;` with `break;` in 3 deletion handlers

**Locations**:
1. **DrawDialogue()** - Line 319: `return;` → `break;`
2. **DrawChoices()** - Line 424: `return;` → `break;`
3. **DrawCharacters()** - Line 231: `return;` → `break;`

**Total**: 3 lines changed (1-word replacement each)

### Rationale

`break` exits loop (preventing index shifting issues) but allows method to continue → `EndHorizontal()` gets called → GUI balance maintained ✅

## Next Steps

1. ✅ Apply fix (3 one-word changes)
2. ✅ Compile Unity project
3. ✅ Manual test: Click "×" button → verify NO GUI error
4. ✅ Commit fix
