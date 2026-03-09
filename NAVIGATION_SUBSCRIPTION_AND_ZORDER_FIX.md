# Navigation Buttons - Исправление проблем с подписками и Z-order

**Дата**: 2026-03-09  
**Проблемы**:
1. Подписки на события не работают
2. Кнопки навигации находятся под окном диалога (Z-order)

## Проблема 1: Подписки не работают

### Причина
VContainer может инжектить зависимости **после** выполнения `Start()`, что приводит к тому, что `_sceneManager` и `_dialogueSystem` еще `null` при попытке подписаться на события.

### Решение
Используем корутину для отложенной инициализации на один кадр после инжекции:

**Файл**: `NavigationUIManager.cs`

```csharp
private void Start()
{
    // Используем корутину для инициализации после VContainer injection
    StartCoroutine(InitializeAfterInjection());
}

private System.Collections.IEnumerator InitializeAfterInjection()
{
    // Ждем один кадр, чтобы VContainer завершил инжекцию
    yield return null;
    
    InitializeNavigationUI();
    SubscribeToNavigationEvents();
}
```

### Дополнительная диагностика
Добавлены логи для проверки:
- Количество подписчиков на событие
- Срабатывание события `OnSceneLoadComplete`
- Вызов `OnSceneNavigated` в NavigationUIManager

```csharp
Debug.Log($"NavigationUIManager: Subscribed to scene navigation events (OnSceneLoadComplete has {_sceneManager.OnSceneLoadComplete?.GetInvocationList().Length ?? 0} subscribers)");
```

---

## Проблема 2: Z-order (кнопки под DialogueBox)

### Причина
В Unity UI элементы рендерятся в порядке их расположения в иерархии Canvas:
- Первый child → рендерится первым (внизу)
- Последний child → рендерится последним (сверху)

Текущий порядок в Canvas:
1. `ChoiceUI`
2. `NavigationUI`
3. `DialogueBox` (создается динамически через UIManager)

Когда DialogueBox создается последним, он закрывает NavigationUI.

### Решение A: Программное изменение sibling order

Добавлен метод `EnsureNavigationUIOnTop()` в `NavigationUIManager.cs`:

```csharp
/// <summary>
/// Ensures NavigationUI renders above other UI elements like DialogueBox.
/// </summary>
private void EnsureNavigationUIOnTop()
{
    if (_navigationUI == null) return;

    var rectTransform = _navigationUI.GetComponent<RectTransform>();
    if (rectTransform != null && rectTransform.parent != null)
    {
        // Move to last in sibling order to render on top
        rectTransform.SetAsLastSibling();
        Debug.Log("NavigationUIManager: Set NavigationUI to render on top (last sibling)");
    }
}
```

Этот метод вызывается в `InitializeNavigationUI()` после инициализации.

### Решение B: Альтернатива - использовать Sorting Order (если потребуется)

Если `SetAsLastSibling()` не помогает, можно использовать отдельный Canvas с более высоким `sortingOrder`:

```csharp
// Пример (не реализовано):
var navigationCanvas = _navigationUI.gameObject.AddComponent<Canvas>();
navigationCanvas.overrideSorting = true;
navigationCanvas.sortingOrder = 1000; // Выше всех других UI
```

---

## Проблема 2.1: DialogueBox блокирует raycasts

### Возможная причина
Если DialogueBox имеет Image компонент с `raycastTarget = true`, он может блокировать клики по кнопкам навигации даже если они визуально "выше".

### Решение (ручная настройка в Unity Editor)

1. **Найти DialogueBox в сцене** (или в префабе):
   - Hierarchy → Canvas → DialogueBox
   
2. **Проверить Image компоненты**:
   - У панели фона (`DialoguePanel`) может быть Image компонент
   - Убедитесь, что `Raycast Target` **отключен** (unchecked) если панель не должна блокировать клики

3. **Альтернатива**: Использовать CanvasGroup:
   ```csharp
   // В DialogueBoxController можно добавить:
   var canvasGroup = _dialoguePanel.GetComponent<CanvasGroup>();
   if (canvasGroup != null)
   {
       canvasGroup.blocksRaycasts = false; // Пропускать клики сквозь панель
   }
   ```

---

## Измененные файлы

### 1. `NavigationUIManager.cs`

**Изменения**:
- ✅ Добавлена корутина `InitializeAfterInjection()` для отложенной инициализации
- ✅ Добавлен метод `EnsureNavigationUIOnTop()` для установки правильного render order
- ✅ Улучшено логирование в `SubscribeToNavigationEvents()`
- ✅ Улучшено логирование в `OnSceneNavigated()` с детальной информацией о состоянии

**Ключевые логи для проверки**:
```
NavigationUIManager: Subscribed to scene navigation events (OnSceneLoadComplete has X subscribers)
NavigationUIManager: Set NavigationUI to render on top (last sibling)
NavigationUIManager: OnSceneNavigated called for scene 'SceneName'
NavigationUIManager: Restarting dialogue for scene 'SceneName' (IsPlaying: true, CurrentScene: ...)
```

### 2. `SceneManager.cs`

**Изменения**:
- ✅ Добавлен лог количества подписчиков при вызове `OnSceneLoadComplete`

**Ключевой лог**:
```
SceneManager: OnSceneLoadComplete event fired with X subscribers
```

Если `X = 0`, значит подписчиков нет → проблема с инжекцией или подпиской.
Если `X >= 1`, значит подписчики есть → событие должно сработать.

---

## Тестирование

### Test 1: Проверка подписки на события

1. Запустить игру в Unity Editor
2. Проверить Console на наличие логов:
   ```
   NavigationUIManager: Subscribed to scene navigation events (OnSceneLoadComplete has 1 subscribers)
   ```
3. Перейти на вторую сцену (через диалог)
4. Проверить Console:
   ```
   SceneManager: OnSceneLoadComplete event fired with 1 subscribers
   NavigationUIManager: OnSceneNavigated called for scene 'SceneName'
   ```

**Ожидаемый результат**: 
- Логи показывают, что подписка работает
- NavigationUIManager получает события

**Если не работает**:
- Проверить, что `NavigationUIManager` в списке `autoInjectGameObjects` в `GameLifetimeScope`
- Проверить, что `GameLifetimeScope` активен в сцене

---

### Test 2: Проверка Z-order (render order)

1. Запустить игру
2. Дождаться появления DialogueBox
3. **Проверить визуально**: видны ли кнопки навигации внизу экрана?
4. **Попробовать кликнуть** на кнопку Back (она будет неактивна, но должна реагировать на hover)

**Ожидаемый результат**:
- Кнопки видны даже когда DialogueBox отображается
- Кнопки реагируют на hover (меняют цвет)
- Console показывает:
  ```
  NavigationUIManager: Set NavigationUI to render on top (last sibling)
  ```

**Если не работает**:
- Проверить в Hierarchy порядок children в Canvas
- NavigationUI должен быть последним (внизу списка)
- Можно вручную перетащить NavigationUI вниз списка в Hierarchy

---

### Test 3: Проверка кликабельности кнопок

1. Запустить игру
2. Перейти на вторую сцену
3. **Попробовать кликнуть Back button** (он должен быть активен)
4. Проверить Console:
   ```
   SceneNavigationUI: Back button clicked!
   SceneManager: Navigating back to scene '...'
   ```

**Ожидаемый результат**:
- Кнопка Back активна (темная, не серая)
- Клик регистрируется
- Сцена переключается назад

**Если кнопка не кликается**:
- Проверить, что у DialoguePanel Image компонент имеет `Raycast Target = false`
- Проверить, что кнопка действительно активна (`interactable = true`)
- Попробовать закрыть DialogueBox и проверить, кликается ли кнопка

---

## Дополнительная диагностика

### Проверка Raycast блокировки

Добавьте временный debug код в `SceneNavigationUI.cs`:

```csharp
private void Update()
{
    if (_sceneManager == null) return;
    
    // Temporary debug - check if buttons are receiving raycasts
    if (_backButton != null && Input.GetMouseButtonDown(0))
    {
        Vector2 mousePos = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _backButton.GetComponent<RectTransform>(), 
            mousePos, 
            null, 
            out Vector2 localPoint);
        
        Debug.Log($"Mouse clicked at {mousePos}, button local pos: {localPoint}, is inside: {_backButton.GetComponent<RectTransform>().rect.Contains(localPoint)}");
    }
    
    UpdateButtonStates();
}
```

Этот код покажет, попадает ли клик в область кнопки.

---

### Проверка иерархии Canvas

Запустите в Console Unity Editor:

```csharp
// В GameStarter или другом MonoBehaviour добавьте:
[ContextMenu("Debug Canvas Hierarchy")]
void DebugCanvasHierarchy()
{
    var canvas = FindFirstObjectByType<Canvas>();
    if (canvas != null)
    {
        Debug.Log("=== Canvas Children (in render order) ===");
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            var child = canvas.transform.GetChild(i);
            Debug.Log($"{i}: {child.name}");
        }
    }
}
```

Правильный порядок (для того чтобы NavigationUI был сверху):
```
0: ChoiceUI
1: [DialogueBox или другие элементы]
2: NavigationUI  ← должен быть последним!
```

---

## Известные ограничения

1. **SetAsLastSibling() вызывается один раз**: Если UIManager создаст DialogueBox после NavigationUIManager инициализации, порядок может снова измениться.
   
   **Решение**: Можно вызывать `EnsureNavigationUIOnTop()` периодически или после создания DialogueBox.

2. **Raycast блокировка**: Если у DialogueBox panel есть Image с `raycastTarget = true`, он будет блокировать клики даже если визуально ниже.
   
   **Решение**: Отключить `Raycast Target` на фоновом Image DialogueBox в Inspector.

---

## Следующие шаги

1. **Запустить тесты** в Unity Editor
2. **Проверить Console логи** на наличие:
   - Сообщений о подписке на события
   - Срабатывания `OnSceneLoadComplete`
   - Вызова `OnSceneNavigated`
3. **Проверить визуально**: видны ли кнопки поверх DialogueBox
4. **Попробовать кликнуть** на кнопку навигации

Если проблемы остаются, отправьте скриншот Console логов и опишите, что именно не работает.
