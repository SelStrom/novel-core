# 🔧 Настройка запуска игры (Game Starter Setup)

**Created**: 2026-03-07  
**Updated**: 2026-03-07  
**Status**: Implemented  
**Constitution**: v1.11.0 - Principle VI (Game Entry Point Architecture)

## Что это?

**GameStarter** - это компонент Unity, который служит точкой входа в игру. Он отвечает за:
- Инициализацию VContainer dependency injection
- Загрузку стартовой сцены при запуске игры
- Запуск DialogueSystem и SceneManager
- Поддержку двух режимов preview: Play Mode (полный запуск) и Scene Editor (превью отдельной сцены)

## Пошаговая инструкция

### Шаг 1: Откройте главную Unity сцену

1. В Project window откройте `Assets/Scenes/SampleScene.unity`
2. Это главная сцена, которая загружается при запуске игры

### Шаг 2: Проверьте наличие GameLifetimeScope

1. В Hierarchy окне найдите GameObject с именем "GameLifetimeScope"
2. **Если его нет**, создайте:
   - Правый клик в Hierarchy → Create Empty
   - Назовите "GameLifetimeScope"
   - В Inspector: Add Component → Type "GameLifetimeScope" → Select it
   
> **Важно**: GameLifetimeScope нужен для VContainer dependency injection. Без него сервисы (DialogueSystem, SceneManager) не будут инъецированы.

### Шаг 3: Создайте GameObject для GameStarter

1. В Hierarchy окне: Правый клик → Create Empty
2. Назовите "GameStarter"
3. Убедитесь, что он находится на уровне root (не внутри другого объекта)

### Шаг 4: Добавьте компонент GameStarter

1. Выберите GameObject "GameStarter" в Hierarchy
2. В Inspector: Add Component → Type "GameStarter" → Select it

### Шаг 5: Настройте GameStarter

В Inspector компонента GameStarter вы увидите:

```
┌─────────────────────────────────────────────┐
│ Game Starter (Script)                       │
├─────────────────────────────────────────────┤
│ Starting Scene                              │
│ └─ [None (SceneData)]  ← Drag here          │
│                                             │
│ Starting Scene Reference                    │
│ └─ [None (AssetReference)]                  │
│                                             │
│ ☑ Auto Start                                │
│ Start Delay         0.5                     │
└─────────────────────────────────────────────┘
```

**Настройте поля:**

1. **Starting Scene**: 
   - Откройте Project window
   - Перейдите в `Assets/Content/Projects/Sample/Scenes/`
   - Перетащите `Scene01_Introduction.asset` в поле "Starting Scene"

2. **Auto Start**: 
   - Убедитесь, что галочка стоит (должна быть включена по умолчанию)

3. **Start Delay**: 
   - Оставьте 0.5 секунды (рекомендуется)
   - Это небольшая задержка перед запуском, чтобы все системы успели инициализироваться

### Шаг 6: Сохраните сцену

1. File → Save (или Ctrl+S / Cmd+S)

### Шаг 7: Протестируйте

1. Нажмите кнопку **Play** ▶️ в Unity
2. Через 0.5 секунды должна загрузиться первая сцена
3. Кликайте на экран для продвижения диалога

## Как это работает

```
Unity Scene Start
       ↓
GameLifetimeScope.Awake()
  → Регистрирует все сервисы в VContainer
       ↓
GameStarter.Start()
  → Вызывает Invoke(StartGame, 0.5f)
       ↓
(Через 0.5 секунды)
GameStarter.StartGame()
  → _sceneManager.LoadScene(_startingScene)
  → _dialogueSystem.StartScene(_startingScene)
       ↓
Игра запущена!
```

## Проверка правильной настройки

### В Console должны появиться логи:

```
NovelCore: Initializing GameLifetimeScope...
NovelCore: GameLifetimeScope initialized successfully
GameStarter: Starting game with scene: Введение
SceneManager: Loading scene Введение
DialogueSystem: Starting scene Введение
```

### Если что-то не работает:

**1. "Starting scene is not assigned"**
- Вы забыли назначить SceneData в поле "Starting Scene"
- Перетащите `Scene01_Introduction.asset` в поле

**2. "DialogueSystem not injected"**
- GameLifetimeScope отсутствует или неправильно настроен
- Убедитесь, что GameObject "GameLifetimeScope" с компонентом существует в сцене

**3. "Game already started"**
- Игра уже запущена, игнорируется повторный вызов
- Это нормально, просто информационное сообщение

**4. Черный экран, нет диалогов**
- UI prefabs не созданы (DialogueBox, ChoiceButton)
- Выполните: NovelCore → Generate UI Prefabs → All Prefabs

## Дополнительные функции GameStarter

### Запуск вручную (Manual Start)

Если вы хотите контролировать момент запуска игры:

1. Снимите галочку "Auto Start"
2. Вызовите `GameStarter.StartGame()` из вашего кода:

```csharp
var gameStarter = FindObjectOfType<GameStarter>();
gameStarter.StartGame();
```

### Перезапуск игры (Restart Game)

Чтобы перезапустить игру с самого начала:

```csharp
var gameStarter = FindObjectOfType<GameStarter>();
gameStarter.RestartGame();
```

Это:
- Останавливает текущий диалог
- Выгружает текущую сцену
- Загружает стартовую сцену заново

## Альтернативные варианты

### Вариант 1: Использовать AssetReference вместо прямой ссылки

В поле "Starting Scene Reference" можно указать Addressable asset reference вместо прямой ссылки на SceneData.

**Преимущества:**
- Сцена загружается асинхронно через Addressables
- Можно менять стартовую сцену без перекомпиляции

**Недостаток:**
- Требует настройки Addressables (см. ADDRESSABLES_SETUP.md)

### Вариант 2: Создать свой контроллер

Если вам нужна более сложная логика запуска (например, меню, загрузка сохранений):

```csharp
using VContainer;
using NovelCore.Runtime.Core.DialogueSystem;
using NovelCore.Runtime.Core.SceneManagement;

public class MyGameController : MonoBehaviour
{
    [Inject]
    private IDialogueSystem _dialogueSystem;
    
    [Inject]
    private ISceneManager _sceneManager;

    public void LoadSavedGame(SaveData save)
    {
        // Your custom logic
        var scene = LoadSceneFromSave(save);
        _sceneManager.LoadScene(scene);
        _dialogueSystem.StartScene(scene);
    }
}
```

## FAQ

**Q: Можно ли запустить игру без GameStarter?**  
A: Да, но вам придется вручную вызывать `DialogueSystem.StartScene()` и `SceneManager.LoadScene()` из вашего кода.

**Q: Нужен ли GameStarter в релизной версии игры?**  
A: Зависит от вашей архитектуры. Для простых VN с линейным запуском - да, удобно. Для игр с меню/сохранениями - лучше создать свой контроллер.

**Q: Что если я хочу начать с другой сцены?**  
A: Просто перетащите другой SceneData asset в поле "Starting Scene".

**Q: Можно ли использовать несколько GameStarter?**  
A: Технически да, но рекомендуется использовать только один. Если нужно несколько точек входа, лучше создать разные Unity сцены.

---

**Создано**: 2026-03-07  
**Обновлено**: 2026-03-07  
**Связанные файлы**: 
- `GameStarter.cs` - основной компонент
- `SAMPLE_PROJECT_QUICKSTART.md` - быстрый старт
- `SAMPLE_PROJECT_GUIDE.md` - полное руководство
- `MANUAL_STEPS.md` - ручные шаги настройки
- `.specify/memory/constitution.md` - конституция проекта (Principle VI)
- `specs/001-visual-novel-constructor/plan.md` - план реализации
- `specs/001-visual-novel-constructor/tasks.md` - задачи (Iteration 7.5)

## Changelog

### 2026-03-07 - Initial Implementation

**Added**:
- GameStarter component (`Assets/Scripts/NovelCore/Runtime/Core/GameStarter.cs`)
- Dual-mode preview support (Play Mode full start + Scene Editor preview)
- Auto-start with configurable delay
- RestartGame() method for full reset
- VContainer dependency injection integration
- Starting scene configuration via Inspector

**Documentation**:
- Created GAME_STARTER_SETUP.md (this file)
- Updated SAMPLE_PROJECT_QUICKSTART.md with GameStarter setup steps
- Updated SAMPLE_PROJECT_GUIDE.md with "Предварительная настройка Unity сцены"
- Updated MANUAL_STEPS.md with Step 0 (Game Entry Point setup)
- Updated README.md with GameStarter reference

**Architecture**:
- Updated Constitution v1.10.0 → v1.11.0 (Principle I: Dual-Mode Preview, Principle VI: Game Entry Point)
- Updated plan.md Technical Context with GameStarter entry point
- Updated plan.md Project Structure with GameStarter.cs location
- Added tasks.md Iteration 7.5 (T039, T040.1-T040.3)

**Testing**:
- PlayMode integration test for initialization sequence (T040.1 - planned)
- Manual testing: Sample Project full game start validation

**Rationale**: Implements Constitution Principle VI requirement for explicit game entry point. GameStarter ensures predictable initialization order: Unity Scene Start → GameLifetimeScope (VContainer DI) → GameStarter.Start() → SceneManager.LoadScene() + DialogueSystem.StartScene() → Game Running.

**Breaking Changes**: None. This is new functionality.

**Migration**: For existing projects, add GameStarter GameObject to main Unity scene and configure starting scene. See "Пошаговая инструкция" above.
