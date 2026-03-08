# Navigation Buttons Implementation - Testing Guide

## Что было исправлено

### Проблема
В Sample Project не создавались и не отображались кнопки переходов (Back/Forward) для навигации по истории сцен.

### Решение

1. **Создан `NavigationUIManager`** (`Runtime/UI/NavigationControls/NavigationUIManager.cs`)
   - Управляет инициализацией `SceneNavigationUI` с dependency injection
   - Связывает VContainer DI с Unity MonoBehaviour lifecycle
   - Автоматически находит и инициализирует компонент при старте

2. **Обновлён `SampleProjectGenerator`** (`Editor/Tools/Generators/SampleProjectGenerator.cs`)
   - Добавлен метод `SetupNavigationUI()` - создаёт UI с кнопками навигации
   - Добавлен метод `CreateNavigationButton()` - создаёт стилизованные кнопки
   - Добавлен метод `LinkScenesLinear()` - связывает сцены через `nextScene` для линейной прогрессии
   - Теперь генератор автоматически создаёт:
     - Canvas с правильными настройками
     - Контейнер NavigationUI с двумя кнопками (Назад/Вперёд)
     - Компонент `SceneNavigationUI` с привязанными кнопками
     - Компонент `NavigationUIManager` для инициализации
     - Связь Scene01 → Scene02 через `nextScene`

3. **Улучшена архитектура**
   - `SceneNavigationUI` теперь правильно инициализируется через DI
   - Кнопки автоматически обновляют своё состояние (enabled/disabled) в зависимости от истории навигации
   - Визуальная обратная связь (цвет кнопок меняется при включении/выключении)

## Как протестировать

### Шаг 1: Пересоздать Sample Project

1. Откройте Unity Editor
2. Выберите меню: `NovelCore → Generate Sample Project`
3. Если Sample Project уже существует, выберите "Overwrite"
4. Дождитесь сообщения об успешном создании

### Шаг 2: Проверить созданные компоненты

1. Откройте сцену `Assets/Scenes/SampleScene.unity`
2. В Hierarchy должны быть следующие объекты:
   - `GameLifetimeScope` (VContainer DI container)
   - `GameStarter` (с назначенной стартовой сценой)
   - `UIManager` (управление UI диалогов)
   - `NavigationUIManager` (управление кнопками навигации)
   - `Canvas` → `NavigationUI` → `BackButton` и `ForwardButton`

3. Проверьте `NavigationUI` в Inspector:
   - Компонент `SceneNavigationUI` должен быть прикреплён
   - Поля `_backButton` и `_forwardButton` должны быть заполнены

4. Проверьте `NavigationUIManager` в Inspector:
   - Поле `_navigationUI` должно указывать на компонент `SceneNavigationUI`

### Шаг 3: Запустить игру

1. Нажмите Play ▶️ в Unity Editor
2. Игра должна автоматически запуститься через 0.5 секунды
3. Вы должны увидеть:
   - Диалоговое окно с текстом
   - **Две кнопки внизу экрана**: "← Назад" и "Вперёд →"

### Шаг 4: Проверить состояние кнопок

**На первой сцене (Scene01_Introduction):**
- Кнопка "Назад" должна быть **неактивна** (серая, полупрозрачная) - истории нет
- Кнопка "Вперёд" должна быть **неактивна** - впереди ничего нет

**Нажмите на экран** чтобы пройти 3 реплики диалога
- После последней реплики должен произойти **автоматический переход** на Scene02_ChoicePoint

**На второй сцене (Scene02_ChoicePoint):**
- Кнопка "Назад" должна быть **АКТИВНА** (темная, непрозрачная) - теперь есть история!
- Кнопка "Вперёд" должна быть **неактивна**

### Шаг 5: Проверить навигацию назад

1. **Нажмите кнопку "← Назад"**
2. Должен произойти переход обратно на Scene01_Introduction
3. Теперь:
   - Кнопка "Назад" должна быть **неактивна** (вернулись в начало)
   - Кнопка "Вперёд" должна быть **АКТИВНА** (есть история вперёд!)

### Шаг 6: Проверить навигацию вперёд

1. **Нажмите кнопку "Вперёд →"**
2. Должен произойти переход обратно на Scene02_ChoicePoint
3. Состояние кнопок должно вернуться:
   - "Назад" - активна
   - "Вперёд" - неактивна

### Шаг 7: Проверить выборы (Choices)

1. На Scene02_ChoicePoint пройдите диалог до появления выбора
2. Выберите один из вариантов:
   - "Выйти на улицу" → переход на Scene03a_PathA
   - "Остаться дома" → переход на Scene03b_PathB
3. После выбора кнопка "Назад" должна быть **активна**

## Ожидаемый результат

✅ **Все функции работают:**
- Кнопки создаются автоматически при генерации Sample Project
- Кнопки отображаются внизу экрана
- Кнопки корректно активны/неактивны в зависимости от истории
- Навигация назад работает (возврат к предыдущей сцене)
- Навигация вперёд работает (переход к следующей сцене в истории)
- Визуальная обратная связь (цвет кнопок) работает
- Линейная прогрессия Scene01 → Scene02 работает автоматически

## Технические детали

### Архитектура

```
GameLifetimeScope (VContainer)
    ↓ инжектит ISceneManager
NavigationUIManager
    ↓ инициализирует
SceneNavigationUI
    ↓ управляет
BackButton + ForwardButton (Unity UI)
```

### Жизненный цикл

1. **Unity Start**: GameLifetimeScope инициализирует DI контейнер
2. **DI Injection**: NavigationUIManager получает ISceneManager через [Inject]
3. **Manager Start**: NavigationUIManager.Start() вызывает Initialize() на SceneNavigationUI
4. **UI Update**: SceneNavigationUI.Update() постоянно проверяет CanNavigateBack/Forward
5. **Button Click**: При клике вызывается SceneManager.NavigateBack/Forward()

### Важные классы

- `SceneNavigationUI.cs` - компонент UI для кнопок навигации
- `NavigationUIManager.cs` - менеджер для инициализации с DI
- `SceneManager.cs` - содержит NavigateBack/Forward методы
- `SceneNavigationHistory.cs` - хранит стек истории навигации

## Логи для отладки

При запуске в Console должны появиться логи:

```
[SampleProjectGenerator] ✅ Navigation UI setup complete!
NavigationUIManager: Successfully initialized SceneNavigationUI
SceneNavigationUI: Initialized
```

Если кнопки не работают, проверьте Console на ошибки:
- `ISceneManager not injected!` - проблема с VContainer DI
- `SceneNavigationUI reference is not assigned!` - не привязана ссылка в Inspector

## Что дальше?

После успешного тестирования можно:
- Настроить визуальный стиль кнопок (цвета, шрифты, иконки)
- Добавить анимацию при появлении/исчезновении кнопок
- Добавить звуковые эффекты при клике
- Создать prefab для переиспользования в других проектах
