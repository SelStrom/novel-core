# March 2026 Fixes

**Total Fixes**: 2  
**Period**: 2026-03-10

---

## Summary

### [10-sample-project-backgrounds](./10-sample-project-backgrounds/)

**Commit**: a030566  
**Component**: SampleProjectGenerator + SceneManager  
**Severity**: High

**Root Cause**: 
- SampleProjectGenerator не создавал AssetReference для BackgroundImage
- SceneManager GameObject не помечались DontDestroyOnLoad

**Fix**: 
- Added BackgroundImage/CharacterData creation in SampleProjectGenerator
- Added DontDestroyOnLoad for background and character GameObject

---

### [10-audio-di-null](./10-audio-di-null/)

**Commit**: bc50296  
**Component**: VContainer/GameLifetimeScope  
**Severity**: Critical

**Root Cause**:
- UnityAudioService (MonoBehaviour) зарегистрирован через `builder.Register<>()` вместо `RegisterComponentOnNewGameObject<>()`

**Fix**:
- Changed to `RegisterComponentOnNewGameObject<UnityAudioService>().AsImplementedInterfaces()`

---

## Patterns Identified

### Recurring Issues

1. **MonoBehaviour DI Registration** (1 instance):
   - Pattern: Забывают использовать `RegisterComponentOnNewGameObject` для MonoBehaviour
   - Solution: Add to Constitution guidance

2. **GameObject Lifecycle** (1 instance):
   - Pattern: Singleton service создаёт GameObject без `DontDestroyOnLoad`
   - Solution: Add to Constitution guidance

### Recommendations

- ✅ Add VContainer MonoBehaviour registration best practices to Constitution
- ✅ Add DI Singleton GameObject lifecycle guidance to Constitution
- 🔄 Create code review checklist for VContainer registrations

---

## Constitution Updates Suggested

Based on fixes this month:

### Principle VI: Add VContainer Guidance

```markdown
### VContainer Registration Best Practices

MonoBehaviour services MUST be registered using RegisterComponentOnNewGameObject:

✅ CORRECT:
builder.RegisterComponentOnNewGameObject<UnityAudioService>(Lifetime.Singleton)
    .AsImplementedInterfaces();

❌ WRONG:
builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);

Rationale: VContainer cannot instantiate MonoBehaviour via constructor.
```

### Principle VI: Add GameObject Lifecycle Guidance

```markdown
### DI Singleton GameObject Lifecycle

Singleton services creating GameObject MUST:
- Call `UnityEngine.Object.DontDestroyOnLoad(gameObject)` after creation
- Ensure GameObject lifecycle matches service lifetime
- Document GameObject ownership in comments
```
