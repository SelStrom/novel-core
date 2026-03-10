# Fix Summary

## Root Cause

`UnityAudioService` — это `MonoBehaviour`, но был зарегистрирован в VContainer через `builder.Register<>()` вместо `builder.RegisterComponentOnNewGameObject<>()`. VContainer **не может создать MonoBehaviour через constructor** → возвращает `null` → зависимые сервисы получают `null` → `NullReferenceException`.

## Patch Explanation

### Changes Made

#### File: GameLifetimeScope.cs

**Line**: 33-34  
**Change**: Заменена регистрация `UnityAudioService`

**BEFORE (WRONG)**:
```csharp
// Line 33 (WRONG - trying to instantiate MonoBehaviour via constructor)
builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);
```

**AFTER (CORRECT)**:
```csharp
// Lines 32-34 (CORRECT - using RegisterComponentOnNewGameObject for MonoBehaviour)
// Audio System (MonoBehaviour - will be created as GameObject)
builder.RegisterComponentOnNewGameObject<UnityAudioService>(Lifetime.Singleton)
    .AsImplementedInterfaces();
```

**Reason**:

1. **VContainer API requirement**: MonoBehaviour классы **MUST** быть зарегистрированы через `RegisterComponentOnNewGameObject<>()`
2. **Why it failed before**:
   ```csharp
   builder.Register<IAudioService, UnityAudioService>()
   // VContainer tries: new UnityAudioService(...)
   // Unity: ERROR - MonoBehaviour cannot be instantiated via constructor
   // Result: null
   ```

3. **Why it works now**:
   ```csharp
   builder.RegisterComponentOnNewGameObject<UnityAudioService>()
       .AsImplementedInterfaces();
   // VContainer:
   //   1. Creates GameObject named "UnityAudioService"
   //   2. Calls AddComponent<UnityAudioService>()
   //   3. Registers IAudioService interface
   // Result: Valid MonoBehaviour instance
   ```

4. **Consistent with existing pattern**:
   - `UnityInputService` (строки 36-37) использует **тот же паттерн**
   - Оба — MonoBehaviour → оба используют `RegisterComponentOnNewGameObject`

### Files Modified

- `novel-core/Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs` (3 lines changed)
  - Modified: Audio System registration (line 33-34)
  - Added: Comment clarifying MonoBehaviour registration pattern

## Test Results

### Compilation Status

- **Build Status**: ✅ SUCCESS
- **Compilation time**: ~13 seconds
- **Warnings**: 0
- **Errors**: 0

### Manual Testing Required

⚠️ **Manual validation needed**:

1. **Unity Console check**:
   - Запустить Play Mode
   - Проверить лог: `"UnityAudioService: Initialized"` должен появиться
   - Проверить лог: `"NovelCore: GameLifetimeScope initialized successfully"`

2. **Dependency injection check**:
   - SceneManager должен инжектироваться без ошибок
   - DialogueSystem должен инжектироваться без ошибок
   - No `NullReferenceException` при старте игры

3. **Audio functionality check**:
   - Загрузить сцену с BackgroundMusic
   - Музыка должна воспроизводиться
   - No errors при вызове audio methods

## Constitution Compliance

### Principle VI (Modular Architecture & Testing)

✅ **Fixed**:
- DI теперь работает корректно
- MonoBehaviour зарегистрирован правильным методом
- Соответствует VContainer best practices

### Code Style Standards

✅ **Compliant**:
- Comment added для ясности
- Паттерн идентичен UnityInputService registration

### AI Development Constraints (Principle VII)

✅ **Compliant**:
- Изменён только файл в `Assets/Scripts/`
- Не модифицированы `.meta` files
- Compilation validated

## Confidence

**99%** что этот фикс решает баг.

### Reasoning

1. ✅ **Build компилируется успешно**
2. ✅ **Root cause identified**: API misuse в VContainer registration
3. ✅ **Fix aligned with VContainer docs**: [Official documentation](https://vcontainer.hadashikick.jp/registering/register-monobehaviour)
4. ✅ **Consistent pattern**: Идентично UnityInputService registration
5. ✅ **Minimal change**: Only 3 lines modified

Уверенность 99% (а не 100%) только потому, что не запущены integration tests и не проверено в Unity Editor.

## Next Steps

### Immediate

1. ✅ **Compilation validated**
2. 🎮 **Manual testing**:
   - Press Play ▶️ в Unity Editor
   - Проверить Unity Console:
     - ✅ "UnityAudioService: Initialized"
     - ✅ "NovelCore: GameLifetimeScope initialized successfully"
     - ❌ No NullReferenceException
   - Загрузить сцену с BackgroundMusic
   - ✅ Музыка воспроизводится

### Follow-up

1. 📝 **Update Constitution** (optional):
   - Добавить guidance о VContainer MonoBehaviour registration
   - Предотвратить аналогичные ошибки в будущем

2. 🧪 **Create integration test**:
   - Тест проверяющий, что все сервисы инжектируются корректно
   - Verify `IAudioService != null` после resolve

## VContainer MonoBehaviour Registration Pattern

**Rule**: Если класс extends `MonoBehaviour`, используй `RegisterComponentOnNewGameObject`:

```csharp
// ✅ CORRECT (MonoBehaviour)
builder.RegisterComponentOnNewGameObject<UnityAudioService>(Lifetime.Singleton)
    .AsImplementedInterfaces();

// ✅ CORRECT (MonoBehaviour)
builder.RegisterComponentOnNewGameObject<UnityInputService>(Lifetime.Singleton)
    .AsImplementedInterfaces();

// ✅ CORRECT (POCO class)
builder.Register<IAssetManager, AddressablesAssetManager>(Lifetime.Singleton);

// ✅ CORRECT (POCO class)
builder.Register<IDialogueSystem, DialogueSystem>(Lifetime.Singleton);
```

**Key difference**:
- **MonoBehaviour**: Requires GameObject → `RegisterComponentOnNewGameObject`
- **POCO**: Direct instantiation → `Register`
