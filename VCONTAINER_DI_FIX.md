# VContainer DI Issue - Решение

## Ошибка

```
VContainerException: Failed to resolve NovelCore.Runtime.Core.DialogueSystem.DialogueSystem : 
Failed to resolve NovelCore.Runtime.Core.SceneManagement.SceneManager : 
Failed to resolve NovelCore.Runtime.Core.SceneManagement.SceneNavigationHistory : 
No such registration of type: System.Int32
```

## Корневая причина

### Проблема 1: Параметры конструктора с значениями по умолчанию

**Файл:** `SceneNavigationHistory.cs`

```csharp
// ❌ НЕПРАВИЛЬНО - VContainer не поддерживает default параметры
public SceneNavigationHistory(int maxHistorySize = DEFAULT_MAX_HISTORY_SIZE)
{
    _state = new SceneNavigationState
    {
        maxHistorySize = maxHistorySize
    };
}
```

**Проблема:**
- VContainer видит конструктор с параметром `int maxHistorySize`
- VContainer пытается инжектировать значение для этого параметра
- Нет регистрации `System.Int32` в DI контейнере
- **Exception:** "No such registration of type: System.Int32"

**Почему не работают default параметры:**
VContainer использует рефлексию и смотрит только на сигнатуру конструктора, игнорируя default значения. Он не может отличить:
- `public Foo(int x = 5)` - параметр с default значением
- `public Foo(int x)` - обязательный параметр

Для VContainer оба выглядят как `public Foo(int x)` и требуют инжекта.

### Проблема 2: Необязательные параметры (nullable)

**Файл:** `SceneManager.cs`

```csharp
// ❌ НЕПРАВИЛЬНО - VContainer не поддерживает optional параметры
public SceneManager(
    IAssetManager assetManager, 
    IAudioService audioService,
    ICharacterAnimatorFactory animatorFactory,
    ISceneNavigationHistory navigationHistory = null)
{
    // ...
}
```

**Проблема:**
- VContainer не понимает `= null` как "необязательный параметр"
- Пытается инжектировать `ISceneNavigationHistory`
- Если регистрация есть - инжектирует (что мы и хотим)
- Но семантически код предполагал, что это опционально

## Решение

### 1. Добавить конструктор без параметров

**Файл:** `SceneNavigationHistory.cs`

```csharp
// ✅ ПРАВИЛЬНО - Два конструктора
/// <summary>
/// Creates a new navigation history with default max size.
/// This parameterless constructor is required for VContainer DI.
/// </summary>
public SceneNavigationHistory() : this(DEFAULT_MAX_HISTORY_SIZE)
{
}

/// <summary>
/// Creates a new navigation history with custom max size.
/// </summary>
public SceneNavigationHistory(int maxHistorySize)
{
    _state = new SceneNavigationState
    {
        maxHistorySize = maxHistorySize
    };
}
```

**Как это работает:**
1. VContainer находит конструктор без параметров
2. Вызывает `new SceneNavigationHistory()`
3. Конструктор без параметров вызывает конструктор с параметром
4. Передаёт константу `DEFAULT_MAX_HISTORY_SIZE = 50`
5. Успешное создание экземпляра

**Преимущества:**
- ✅ VContainer может создать экземпляр
- ✅ Сохранена возможность задать custom size в тестах
- ✅ Default значение инкапсулировано в классе

### 2. Убрать default null параметр

**Файл:** `SceneManager.cs`

```csharp
// ✅ ПРАВИЛЬНО - Обязательный параметр
public SceneManager(
    IAssetManager assetManager, 
    IAudioService audioService,
    ICharacterAnimatorFactory animatorFactory,
    ISceneNavigationHistory navigationHistory) // Без = null
{
    _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
    _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
    _animatorFactory = animatorFactory ?? throw new ArgumentNullException(nameof(animatorFactory));
    _navigationHistory = navigationHistory; // VContainer всегда инжектирует
    _transitionFactory = new SceneTransitionFactory();

    InitializeSceneHierarchy();
}
```

**Обоснование:**
- `ISceneNavigationHistory` зарегистрирован в `GameLifetimeScope`
- VContainer всегда будет инжектировать экземпляр
- Делать параметр опциональным не имеет смысла
- Упрощает код и делает зависимости явными

## Порядок резолва зависимостей

### До исправления (❌ Ошибка)

```
1. GameLifetimeScope инициализируется
2. VContainer пытается создать DialogueSystem
   └─> Требует ISceneManager
       └─> Пытается создать SceneManager
           └─> Требует ISceneNavigationHistory
               └─> Пытается создать SceneNavigationHistory
                   └─> Ищет регистрацию System.Int32
                       └─> ❌ НЕТ РЕГИСТРАЦИИ - EXCEPTION!
```

### После исправления (✅ Успех)

```
1. GameLifetimeScope инициализируется
2. Регистрации:
   - IAssetManager → AddressablesAssetManager
   - IAudioService → UnityAudioService
   - IInputService → UnityInputService
   - ICharacterAnimatorFactory → CharacterAnimatorFactory
   - ISceneNavigationHistory → SceneNavigationHistory ✅
   - ISceneManager → SceneManager ✅
   - IDialogueSystem → DialogueSystem ✅

3. VContainer создаёт синглтоны:
   a) SceneNavigationHistory()
      └─> ✅ Конструктор без параметров найден
      └─> ✅ Создан экземпляр
   
   b) SceneManager(assetManager, audioService, animatorFactory, navigationHistory)
      └─> ✅ Все зависимости инжектированы
      └─> ✅ Создан экземпляр
   
   c) DialogueSystem(audioService, inputService, sceneManager, assetManager)
      └─> ✅ Все зависимости инжектированы
      └─> ✅ Создан экземпляр

4. Все компоненты успешно инициализированы ✅
```

## Граф зависимостей

```
GameLifetimeScope
├─ IAssetManager (no dependencies)
├─ IAudioService (no dependencies)
├─ IInputService (no dependencies)
├─ ICharacterAnimatorFactory (no dependencies)
├─ ISceneNavigationHistory (no dependencies) ✅ Теперь без параметров
├─ ISceneManager
│   └─ requires: IAssetManager, IAudioService, ICharacterAnimatorFactory, ISceneNavigationHistory ✅
└─ IDialogueSystem
    └─ requires: IAudioService, IInputService, ISceneManager, IAssetManager ✅
```

**Порядок создания:**
1. Классы без зависимостей (листья дерева)
2. SceneNavigationHistory ✅
3. SceneManager (после navigationHistory)
4. DialogueSystem (после sceneManager)

## Лучшие практики VContainer

### ✅ DO: Используйте конструкторы без параметров

```csharp
public class MyService
{
    public MyService() // ✅ VContainer может создать
    {
    }
}
```

### ✅ DO: Используйте зарегистрированные типы

```csharp
public class MyService
{
    public MyService(IOtherService other) // ✅ Если IOtherService зарегистрирован
    {
    }
}
```

### ❌ DON'T: Не используйте default параметры

```csharp
public class MyService
{
    public MyService(int value = 5) // ❌ VContainer попытается инжектировать int
    {
    }
}
```

### ❌ DON'T: Не используйте optional nullable параметры

```csharp
public class MyService
{
    public MyService(IOtherService other = null) // ❌ VContainer не поймёт optional
    {
    }
}
```

### ✅ DO: Используйте перегрузку конструкторов

```csharp
public class MyService
{
    public MyService() : this(5) // ✅ Для DI
    {
    }
    
    public MyService(int value) // ✅ Для тестов
    {
    }
}
```

## Альтернативные решения

### Вариант 1: Регистрация с фабрикой (сложнее)

```csharp
builder.Register<ISceneNavigationHistory>(container => 
{
    return new SceneNavigationHistory(50);
}, Lifetime.Singleton);
```

**Минусы:**
- Более сложный код
- Хардкод значения в другом месте
- Менее понятно для новых разработчиков

### Вариант 2: Использование [Inject] атрибута (не подходит)

```csharp
public class SceneNavigationHistory
{
    [Inject]
    public void Initialize(int maxSize) // ❌ Всё равно нужна регистрация int
    {
    }
}
```

**Минусы:**
- Всё равно требует регистрации `int`
- Проблема не решена

### Вариант 3: Конфигурационный класс (избыточно)

```csharp
public class NavigationHistoryConfig
{
    public int MaxHistorySize { get; set; } = 50;
}

builder.RegisterInstance(new NavigationHistoryConfig());
```

**Минусы:**
- Избыточность для одного параметра
- Дополнительный класс

## Вывод

**Выбранное решение (Вариант с двумя конструкторами)** - самое простое и понятное:
- ✅ Работает с VContainer из коробки
- ✅ Не требует дополнительной конфигурации
- ✅ Сохраняет гибкость для тестов
- ✅ Явные зависимости
- ✅ Чистый код

## Проверка

После исправлений VContainer должен успешно резолвить все зависимости:

```
NovelCore: Initializing GameLifetimeScope...
NovelCore: GameLifetimeScope initialized successfully ✅
NavigationUIManager: Successfully initialized SceneNavigationUI ✅
SceneNavigationUI: Initialized ✅
```

Если ошибка всё ещё есть - проверьте:
1. Все ли зависимости зарегистрированы в `GameLifetimeScope.Configure()`
2. Нет ли других конструкторов с параметрами примитивных типов
3. Нет ли циклических зависимостей (A → B → A)
