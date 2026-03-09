# Исправление кнопок навигации - Краткая инструкция

## Что было исправлено

### Проблема 1: Подписки на события не работали
**Причина**: VContainer инжектит зависимости после `Start()`, поэтому `_sceneManager` был `null` при подписке.

**Решение**: Добавлена корутина с задержкой в один кадр для инициализации после инжекции.

### Проблема 2: Кнопки под окном диалога
**Причина**: NavigationUI рендерился раньше DialogueBox в иерархии Canvas.

**Решение**: Добавлен `SetAsLastSibling()` для перемещения NavigationUI в конец списка (поверх всех).

---

## Быстрый тест

1. **Запустить игру** в Unity Editor
2. **Проверить Console** - должны быть логи:
   ```
   NavigationUIManager: Subscribed to scene navigation events (OnSceneLoadComplete has 1 subscribers)
   NavigationUIManager: Set NavigationUI to render on top (last sibling)
   ```

3. **Перейти на вторую сцену** (кликать по диалогу, выбрать ответ)
4. **Проверить кнопки внизу экрана**:
   - Видны ли они? ✅/❌
   - Кнопка "Назад" активна (темная)? ✅/❌

5. **Кликнуть "Назад"**
6. **Проверить Console**:
   ```
   SceneNavigationUI: Back button clicked!
   NavigationUIManager: OnSceneNavigated called for scene '...'
   SceneManager: Navigating back to scene '...'
   ```

7. **Проверить результат**:
   - Сцена вернулась назад? ✅/❌
   - Диалог перезапустился? ✅/❌

---

## Если кнопки всё ещё не кликаются

### Проблема: DialogueBox блокирует клики

**Решение**:

1. Найти в Hierarchy: `Canvas → DialogueBox → DialoguePanel`
2. Выбрать `DialoguePanel`
3. В Inspector найти компонент **Image**
4. Снять галочку с **"Raycast Target"**

Это позволит кликам проходить сквозь фон диалога к кнопкам навигации.

---

## Измененные файлы

- ✅ `NavigationUIManager.cs` - исправлена инициализация и Z-order
- ✅ `SceneManager.cs` - добавлены диагностические логи

---

## Ожидаемые логи при правильной работе

```
[Старт игры]
NovelCore: Initializing GameLifetimeScope...
NavigationUIManager: Subscribed to scene navigation events (OnSceneLoadComplete has 1 subscribers)
NavigationUIManager: Set NavigationUI to render on top (last sibling)

[Переход на вторую сцену]
SceneManager: Added scene 'scene_02' to navigation history
SceneManager: OnSceneLoadComplete event fired with 1 subscribers
NavigationUIManager: OnSceneNavigated called for scene 'Scene 02'
NavigationUIManager: Restarting dialogue for scene 'Scene 02' (IsPlaying: false, CurrentScene: null)

[Клик на кнопку Назад]
SceneNavigationUI: Back button clicked!
SceneManager: Navigating back to scene 'scene_01' at line 0
SceneNavigationHistory: Navigated back to scene 'scene_01' (index: 0)
NavigationUIManager: OnSceneNavigated called for scene 'Scene 01'
NavigationUIManager: Restarting dialogue for scene 'Scene 01' (IsPlaying: true, CurrentScene: Scene 01)
SceneNavigationUI: Navigated back successfully
```

---

## Если проблема остается

Пожалуйста, отправьте:
1. Скриншот Console логов после запуска игры
2. Скриншот экрана с кнопками (видны ли они?)
3. Описание: что именно не работает?
   - Кнопки не видны?
   - Кнопки видны, но не кликаются?
   - Кнопки кликаются, но сцена не переключается?
   - Сцена переключается, но диалог не перезапускается?
