# Selected Theory

## Decision

**Selected**: **IMPL_THEORY** (Implementation Theory)

## Reasoning

### Theory Comparison

| Aspect                | Spec Theory      | Impl Theory      |
|-----------------------|------------------|------------------|
| Confidence Score      | 5%               | 99%              |
| Evidence Quality      | LOW              | HIGH             |
| Failing Test Alignment| WEAK             | STRONG           |
| Constitution Alignment| VIOLATION (Principle VI) | VIOLATION (Principle VI) |

### Decision Rationale

#### 1. Evidence Strength

**Impl Theory (HIGH)**:
- ✅ Явная ошибка API usage: MonoBehaviour зарегистрирован через неправильный метод
- ✅ VContainer documentation явно требует `RegisterComponentOnNewGameObject` для MonoBehaviour
- ✅ Working example существует: `UnityInputService` зарегистрирован правильно
- ✅ Детерминированность: баг воспроизводится 100% времени
- ✅ Direct causation: Неправильная регистрация → null → NullReferenceException

**Spec Theory (LOW)**:
- ❌ Constitution не содержит специфических требований к VContainer API usage
- ❌ Это external library documentation issue, не Constitution issue
- ❌ Неоднозначность несущественна (VContainer docs ясны)

#### 2. Test Alignment

**Failing Test будет проверять**:
```csharp
[Test]
public void VContainer_UnityAudioService_ShouldInjectSuccessfully()
{
    // Arrange
    var lifetimeScope = CreateTestLifetimeScope();
    
    // Act
    var audioService = lifetimeScope.Container.Resolve<IAudioService>();
    
    // Assert
    Assert.That(audioService, Is.Not.Null, "IAudioService should be injected");  // ❌ FAILS without fix
    Assert.That(audioService, Is.TypeOf<UnityAudioService>(), "Should be UnityAudioService instance");
}
```

**Impl Theory объясняет failure**:
- `builder.Register<IAudioService, UnityAudioService>()` не может создать MonoBehaviour
- VContainer возвращает `null`
- Тест падает на Assert

**Spec Theory НЕ объясняет failure**:
- Constitution правильна
- VContainer API documented externally
- Проблема — в коде, а не в спецификации

#### 3. Root Cause Coverage

**Impl Theory** покрывает **ВСЕ** симптомы:
1. ❌ IAudioService = null → неправильная регистрация
2. ❌ NullReferenceException → попытка вызвать методы на null
3. ✅ UnityInputService работает → зарегистрирован правильно
4. ❌ UnityAudioService не работает → зарегистрирован неправильно

**Spec Theory** не объясняет root cause.

### Rejected Theory

**Rejected**: **SPEC_THEORY** (Specification Theory)

**Reason for Rejection**:

1. **Низкая уверенность (5%)**
2. **External API documentation**: VContainer docs явно определяют, как регистрировать MonoBehaviour
3. **Working example exists**: `UnityInputService` показывает правильный паттерн
4. **Не объясняет root cause**: Spec Theory указывает на violation Principle VI, но не объясняет **почему** код не работает

## Confidence in Decision

**99%**

### Justification

1. **Impl Theory confidence (99%)** >> **Spec Theory confidence (5%)**
2. **Явная ошибка API usage**: неправильный метод VContainer registration
3. **VContainer documentation**: [Official docs](https://vcontainer.hadashikick.jp/registering/register-monobehaviour) подтверждают необходимость `RegisterComponentOnNewGameObject`
4. **Working example**: `UnityInputService` зарегистрирован правильно в том же файле
5. **Direct causation**: Неправильная регистрация → null → bug

## Recommended Fix Strategy

### Summary

Изменить регистрацию `UnityAudioService` с `builder.Register<>()` на `builder.RegisterComponentOnNewGameObject<>()`.

### Detailed Steps

#### Шаг 1: Изменить регистрацию в GameLifetimeScope

В `GameLifetimeScope.cs`, строка 33:

```csharp
// BEFORE (WRONG)
builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);

// AFTER (CORRECT)
builder.RegisterComponentOnNewGameObject<UnityAudioService>(Lifetime.Singleton)
    .AsImplementedInterfaces();
```

**Explanation**:
- `RegisterComponentOnNewGameObject<UnityAudioService>()` создаст GameObject с компонентом
- `.AsImplementedInterfaces()` зарегистрирует `IAudioService` интерфейс
- Идентично регистрации `UnityInputService` (строки 36-37)

#### Шаг 2: (Опционально) Добавить DontDestroyOnLoad

VContainer автоматически помечает root GameObject как `DontDestroyOnLoad` для Singleton, но для ясности можно добавить в `UnityAudioService.Awake()`:

```csharp
private void Awake()
{
    // Ensure audio service persists across scene loads
    DontDestroyOnLoad(gameObject);
    
    // ... existing code ...
}
```

## Next Steps

1. ✅ **Impl Theory выбрана** → применить fix
2. ⚙️ **Implement patch** → изменить GameLifetimeScope.cs
3. 🧪 **Compile** → проверить компиляцию
4. 🧪 **Manual test** → запустить игру и проверить Unity Console logs

## Validation Checklist

- [ ] Build успешно компилируется
- [ ] Unity Console показывает "UnityAudioService: Initialized"
- [ ] SceneManager корректно инжектируется с IAudioService
- [ ] DialogueSystem корректно инжектируется с IAudioService
- [ ] Музыка воспроизводится при загрузке сцены (если BackgroundMusic задана)
- [ ] No NullReferenceException при вызове аудио методов
