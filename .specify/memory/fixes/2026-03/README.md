# Fixes Archive - March 2026

## Index

### 10-audio-di-null (2026-03-10)
**Bug**: AudioService DI registration returned null
**Root Cause**: VContainer MonoBehaviour registration pattern incorrect
**Fix**: Use RegisterComponentOnNewGameObject + DontDestroyOnLoad
**Pattern**: VContainer MonoBehaviour lifecycle management
**Impact**: Constitution updated with Principle VI VContainer best practices

### 10-sample-project-backgrounds (2026-03-10)
**Bug**: Sample project missing background assets
**Root Cause**: Addressables catalog missing SampleBackgrounds group
**Fix**: Added background assets to catalog, updated sample scenes
**Pattern**: Asset pipeline integrity validation
**Impact**: None (isolated project setup issue)

### 10-dialogue-invalid-assetref (2026-03-10)
**Bug**: InvalidKeyException on last dialogue with empty NextScene
**Root Cause**: Missing AssetReference.RuntimeKeyIsValid() validation
**Fix**: Added RuntimeKeyIsValid() check in DialogueSystem and AddressablesAssetManager
**Pattern**: AssetReference defensive programming
**Impact**: Recommended Constitution update - add AssetReference validation guidelines

## Patterns Detected

### AssetReference Validation (Occurrences: 1)
**Pattern**: Missing RuntimeKeyIsValid() check before LoadAssetAsync
**Recommendation**: Add to Constitution Principle VI (Defensive Programming):
```
- AssetReference validation: MUST check RuntimeKeyIsValid() before calling Addressables API
- Two-level validation: Validate at business logic layer AND asset management layer
- Clear warnings: Log descriptive warnings (not errors) for invalid references
```

**Rationale**: 
- Unity AssetReference can be non-null with invalid RuntimeKey (empty GUID)
- Simple != null check is insufficient
- InvalidKeyException breaks user experience with technical error messages

**Action**: Update Constitution if this pattern recurs (currently only 1 instance)

## Monthly Statistics

- **Total fixes**: 3
- **VContainer issues**: 1 (Constitution updated)
- **Asset pipeline issues**: 1
- **Validation issues**: 1
- **Average fix time**: ~2 hours (estimated)

## Constitution Review Status

- [x] 10-audio-di-null: Constitution updated (Principle VI VContainer guidance)
- [x] 10-sample-project-backgrounds: No update needed (isolated issue)
- [ ] 10-dialogue-invalid-assetref: Pending pattern review (1 occurrence, threshold is 3)

**Next Review**: 2026-04-01 (monthly pattern analysis)
