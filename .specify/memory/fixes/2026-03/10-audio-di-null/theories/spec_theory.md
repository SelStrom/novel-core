# Specification Theory

## Hypothesis

Баг **НЕ вызван** некорректной спецификацией. Это чистый баг реализации (API misuse).

## Evidence

### Constitution Requirements

**Principle VI (Modular Architecture & Testing)** требует:
- "Dependency Injection: Systems MUST use constructor injection or ScriptableObject configuration, avoiding singletons"
- `UnityAudioService` корректно использует DI, но **регистрация неправильная**

### Spec Requirements Analysis

Constitution не содержит явных требований о том, **как именно** регистрировать MonoBehaviour в VContainer, но это **базовое знание VContainer API**, которое должно применяться.

### Current Implementation vs Requirements

| Requirement (Constitution) | Current Implementation | Status |
|---------------------------|------------------------|--------|
| DI через VContainer (Principle VI) | ✅ UnityAudioService имплементирует IAudioService | COMPLIANT |
| Сервисы инжектируются (Principle VI) | ❌ Регистрация неправильная → null | VIOLATION |
| Audio system functional (Principle I) | ❌ Не работает из-за null | VIOLATION |

### Spec Contradictions

**Противоречий НЕТ**. Constitution ясно требует DI, но не уточняет детали регистрации MonoBehaviour.

### Outdated Requirements

**Устаревших требований НЕТ**. Constitution актуальна.

### Ambiguous Requirements

**Неоднозначность**: Constitution не содержит явного guidance о регистрации MonoBehaviour через `RegisterComponentOnNewGameObject`.

Однако, это **не проблема спецификации**, а **документация VContainer** (external library) явно требует использовать `RegisterComponentOnNewGameObject` для MonoBehaviour.

## Expected Behavior Mismatch

**According to Constitution**:
- Audio system должна быть доступна через DI
- Сервисы должны корректно инжектироваться

**Actual behavior**:
- `IAudioService` инжектируется как `null`
- При попытке использования → `NullReferenceException`

**Bug behavior aligns with**: **Implementation bug** (API misuse)

## Constitution Alignment

### Principle Violated: Principle VI (Modular Architecture & Testing)

**Violation details**:
- DI используется, но **неправильно**
- VContainer API misuse → сервис не инжектируется

## Confidence Score

**5%**

### Reasoning

1. **Спецификация ясна**: Constitution требует DI
2. **Нет противоречий**: Requirements не противоречат друг другу
3. **Нет устаревших требований**: Constitution актуальна
4. **Неоднозначность несущественна**: VContainer documentation (external) явно определяет, как регистрировать MonoBehaviour
5. **Баг — это API misuse**: Разработчик использовал неправильный метод VContainer API

## Recommended Action

- [ ] ~~Update specification~~
- [ ] ~~Clarify ambiguous requirements~~
- [ ] ~~Resolve contradictions~~
- [x] **Update implementation** (использовать правильный VContainer API)
- [ ] Optionally: Add guidance to Constitution о регистрации MonoBehaviour в VContainer

## Notes

Хотя баг **не вызван спецификацией**, можно добавить в Constitution явное guidance:

**Potential Constitution amendment**:

```markdown
### VContainer MonoBehaviour Registration

When registering MonoBehaviour services in VContainer:
- MUST use `RegisterComponentOnNewGameObject<TComponent>()` for MonoBehaviour classes
- MUST use `.AsImplementedInterfaces()` to register all implemented interfaces
- MUST NOT use `Register<TInterface, TImplementation>()` for MonoBehaviour (will fail)

Example:
```csharp
// ✅ CORRECT (MonoBehaviour)
builder.RegisterComponentOnNewGameObject<UnityAudioService>(Lifetime.Singleton)
    .AsImplementedInterfaces();

// ❌ WRONG (MonoBehaviour)
builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);
```
```

Но это **не меняет вердикт** — баг вызван API misuse, а не отсутствием спецификации.
