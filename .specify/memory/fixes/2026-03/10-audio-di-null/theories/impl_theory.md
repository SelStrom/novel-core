# Implementation Theory

## Hypothesis

Баг вызван **API misuse**: `UnityAudioService` — это `MonoBehaviour`, но регистрируется в VContainer через `builder.Register<>()` вместо `builder.RegisterComponentOnNewGameObject<>()`. VContainer не может создать MonoBehaviour через обычный constructor, поэтому возвращает `null`.

## Location

### Primary Location

- **File**: `novel-core/Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs`
- **Namespace**: `NovelCore.Runtime.Core`
- **Class**: `GameLifetimeScope`
- **Method**: `Configure()`
- **Line**: 33

### Secondary Location

- **File**: `novel-core/Assets/Scripts/NovelCore/Runtime/Core/AudioSystem/UnityAudioService.cs`
- **Line**: 13
- **Reason**: Класс определён как MonoBehaviour, что требует специальной регистрации

## Root Cause Hypothesis

### Category

- [x] API misuse (incorrect usage of VContainer registration API)
- [ ] Logical error
- [ ] Edge case not handled
- [ ] Race condition
- [ ] State management error
- [ ] Memory/resource leak

### Detailed Explanation

#### Проблема: Неправильная регистрация MonoBehaviour в VContainer

```csharp
// GameLifetimeScope.cs, строка 33
// ❌ INCORRECT: Trying to register MonoBehaviour with builder.Register
builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);

// UnityAudioService.cs, строка 13
public class UnityAudioService : MonoBehaviour, IAudioService
//                               ^^^^^^^^^^^^^^
//                               MonoBehaviour cannot be instantiated via constructor!
```

**Issue**: 

VContainer имеет **два разных метода** для регистрации:

1. **`builder.Register<TInterface, TImplementation>()`**:
   - Используется для **обычных C# классов** (POCO, non-MonoBehaviour)
   - Создаёт экземпляр через **constructor** (`new TImplementation(...)`)
   - Примеры: `AddressablesAssetManager`, `DialogueSystem`, `SceneManager`

2. **`builder.RegisterComponentOnNewGameObject<TComponent>()`**:
   - Используется для **MonoBehaviour классов**
   - Создаёт **GameObject** и добавляет компонент через `AddComponent<>()`
   - Примеры: `UnityInputService` (строка 36-37 в GameLifetimeScope)

**Текущий код использует неправильный метод**:

```csharp
// ❌ WRONG (line 33)
builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);
// Попытка: new UnityAudioService(...) 
// Unity: ОШИБКА - MonoBehaviour не может быть создан через constructor!
// Результат: null
```

**Правильный код** (как для UnityInputService):

```csharp
// ✅ CORRECT (line 36-37, для UnityInputService)
builder.RegisterComponentOnNewGameObject<UnityInputService>(Lifetime.Singleton)
    .AsImplementedInterfaces();
// 1. Создаёт GameObject "UnityInputService"
// 2. Добавляет компонент AddComponent<UnityInputService>()
// 3. Регистрирует все интерфейсы (IInputService)
// Результат: валидный MonoBehaviour экземпляр
```

### Call Stack Analysis

```
Unity Start
  ↓
GameLifetimeScope.Configure()
  ↓
builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton)
  ↓
VContainer tries: new UnityAudioService(...)
  ↓
❌ Unity Error: MonoBehaviour constructor cannot be called directly
  ↓
VContainer returns: null
  ↓
SceneManager constructor
  ↓
SceneManager(IAssetManager assetManager, IAudioService audioService, ...)
  ↓
audioService = null  // ← Bug appears here!
  ↓
_audioService = audioService ?? throw new ArgumentNullException(...)
  ↓
✅ Exception thrown (if null check exists)
OR
❌ NullReferenceException later when calling _audioService.PlayMusic()
```

**Analysis**: VContainer не может создать MonoBehaviour через constructor → возвращает `null` → зависимые сервисы получают `null` → NullReferenceException при использовании.

## Git History Insights

### Recent Changes

Анализ показывает, что:
- `UnityAudioService` был создан как MonoBehaviour (использует `AudioSource`, `Coroutine`)
- Регистрация в VContainer скопирована с паттерна для non-MonoBehaviour сервисов
- `UnityInputService` был зарегистрирован **правильно** через `RegisterComponentOnNewGameObject`
- Но для `UnityAudioService` разработчик **забыл** использовать тот же паттерн

**Suspicion Level**: **HIGH**

**Reason**: Это классическая ошибка при работе с VContainer — copy-paste регистрации без учёта различий между POCO и MonoBehaviour.

## Edge Cases Analysis

Этот баг **НЕ edge case** — он проявляется **всегда**:
- Не зависит от конфигурации
- Не зависит от runtime условий
- Воспроизводится **100%** времени при попытке инжекта `IAudioService`

## Confidence Score

**99%**

### Reasoning

1. ✅ **Явная ошибка API usage**: MonoBehaviour зарегистрирован через неправильный метод
2. ✅ **VContainer documentation**: [Official docs](https://vcontainer.hadashikick.jp/registering/register-monobehaviour) явно требуют `RegisterComponentOnNewGameObject` для MonoBehaviour
3. ✅ **Working example exists**: `UnityInputService` зарегистрирован правильно в том же файле
4. ✅ **Direct causation**: Неправильная регистрация → VContainer возвращает null → NullReferenceException
5. ✅ **User report aligns**: "во время инжекта зависимостей этот объект null"

Уверенность 99% (а не 100%) только потому, что теоретически может быть кастомная конфигурация VContainer, которая позволяет создавать MonoBehaviour через constructor (хотя это противоречит Unity API).

## Recommended Fix Approach

### Шаг 1: Изменить регистрацию UnityAudioService

В `GameLifetimeScope.cs`, строка 33:

```csharp
// BEFORE (WRONG)
builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);

// AFTER (CORRECT)
builder.RegisterComponentOnNewGameObject<UnityAudioService>(Lifetime.Singleton)
    .AsImplementedInterfaces();
```

**Explanation**:
- `RegisterComponentOnNewGameObject<UnityAudioService>()` создаёт GameObject и добавляет компонент
- `.AsImplementedInterfaces()` регистрирует все интерфейсы (`IAudioService`)
- Паттерн идентичен `UnityInputService` (строки 36-37)

### Шаг 2: (Опционально) Добавить DontDestroyOnLoad

Если `UnityAudioService` создаётся через VContainer, **VContainer автоматически** помечает корневой GameObject как `DontDestroyOnLoad` для Singleton lifetime.

Но если нужно явно:

```csharp
// In UnityAudioService.Awake()
private void Awake()
{
    // Ensure audio service persists across scene loads
    DontDestroyOnLoad(gameObject);
    
    // ... existing audio source creation code ...
}
```

### Validation Checklist

После применения фикса:
- [ ] Build успешно компилируется
- [ ] Failing test проходит (IAudioService != null)
- [ ] SceneManager инжектируется с валидным audioService
- [ ] DialogueSystem инжектируется с валидным audioService
- [ ] Вручную протестировать: музыка воспроизводится при загрузке сцены
- [ ] Проверить Unity Console: "UnityAudioService: Initialized" появляется при запуске
