# Инструкция: Как запустить Unity и проверить исправления

## Шаг 1: Запустить Unity Editor

1. **Открыть Unity Hub**
2. **Найти проект** `novel-core` в списке проектов
3. **Кликнуть** на проект для запуска Unity Editor
4. **Дождаться** окончания компиляции (снизу справа будет крутиться индикатор)

---

## Шаг 2: Проверить ошибки компиляции

### Если есть ошибки компиляции:

1. **Открыть Console**:
   - Menu: `Window → General → Console`
   - Или горячая клавиша: `Cmd+Shift+C` (Mac) / `Ctrl+Shift+C` (Windows)

2. **Проверить наличие красных ошибок** (красный восклицательный знак)

3. **Кликнуть на ошибку** для просмотра деталей

4. **Скопировать текст ошибки** и отправить мне

### Ожидаемый результат:
✅ Нет ошибок компиляции (красных сообщений)  
⚠️ Могут быть желтые предупреждения (warnings) - это нормально

---

## Шаг 3: Открыть тестовую сцену

1. **В Project Window** найти:
   ```
   Assets/Scenes/SampleScene.unity
   ```

2. **Двойной клик** на `SampleScene.unity` для открытия

3. **Проверить Hierarchy**:
   - Должен быть `Canvas`
   - Внутри Canvas: `NavigationUI`, `ChoiceUI`
   - Должен быть `GameLifetimeScope`
   - Должен быть `NavigationUIManager`

---

## Шаг 4: Запустить игру

1. **Нажать Play** ▶️ (вверху по центру)

2. **Проверить Console** на наличие логов:
   ```
   ✅ NovelCore: Initializing GameLifetimeScope...
   ✅ NovelCore: GameLifetimeScope initialized successfully
   ✅ NavigationUIManager: Subscribed to scene navigation events (OnSceneLoadComplete has 1 subscribers)
   ✅ NavigationUIManager: Set NavigationUI to render on top (last sibling)
   ✅ SceneNavigationUI: Back button listener added
   ✅ SceneNavigationUI: Forward button listener added
   ✅ SceneNavigationUI: Initialized successfully
   ```

3. **Проверить визуально**:
   - Внизу экрана видны две кнопки: "◀ Назад" и "Вперёд →"
   - Обе кнопки серые (неактивные) на первой сцене
   - Диалог отображается правильно

---

## Шаг 5: Тест навигации

1. **Кликать на экран** или **Space** для продвижения диалога

2. **Выбрать любой ответ** в choice menu

3. **Проверить после перехода на вторую сцену**:
   - ✅ Кнопка "◀ Назад" стала **темной** (активной)
   - ✅ Кнопка "Вперёд →" остается **серой** (неактивной)
   - ✅ Кнопки **видны** даже когда показан диалог

4. **Кликнуть кнопку "◀ Назад"**

5. **Проверить результат**:
   - ✅ Вернулись на первую сцену
   - ✅ Диалог перезапустился с начала
   - ✅ Кнопка "Вперёд →" стала **активной**
   - ✅ В Console логи навигации:
     ```
     SceneNavigationUI: Back button clicked!
     SceneManager: Navigating back to scene '...'
     NavigationUIManager: OnSceneNavigated called for scene '...'
     ```

---

## Возможные ошибки и решения

### Ошибка 1: Ошибка компиляции в NavigationUIManager

**Если видите**:
```
error CS0117: 'System.Collections' does not contain a definition for 'IEnumerator'
```

**Решение**: Добавить using в начало файла:
```csharp
using System.Collections;
```

---

### Ошибка 2: NavigationUIManager не в списке autoInjectGameObjects

**Симптом**: В Console:
```
NavigationUIManager: ISceneManager not injected!
```

**Решение**:
1. В Hierarchy выбрать `GameLifetimeScope`
2. В Inspector найти секцию `Auto Inject Game Objects`
3. Если `NavigationUIManager` отсутствует:
   - Нажать `+` для добавления элемента
   - Перетащить `NavigationUIManager` из Hierarchy в новое поле

---

### Ошибка 3: Кнопки не кликаются (под диалогом)

**Решение**:
1. Найти в Hierarchy: `Canvas → DialogueBox → DialoguePanel`
2. Выбрать `DialoguePanel`
3. В Inspector → Image компонент
4. **Снять галочку** с "Raycast Target"

---

## Что отправить, если есть проблемы

1. **Скриншот Console** (весь список сообщений)
2. **Скриншот Game View** (с кнопками внизу)
3. **Описание проблемы**:
   - На каком шаге возникла проблема?
   - Что именно не работает?
   - Есть ли красные ошибки в Console?

---

## Быстрая проверка (чек-лист)

Перед тем как отправлять проблему, проверьте:

- [ ] Unity открылся без ошибок компиляции
- [ ] В Console нет красных ошибок
- [ ] SampleScene.unity открыта
- [ ] При нажатии Play появляются логи инициализации
- [ ] Кнопки навигации видны внизу экрана
- [ ] После перехода на вторую сцену кнопка "Назад" активна
- [ ] Клик на "Назад" возвращает на первую сцену

Если все пункты ✅ - **всё работает правильно!**

---

## Дополнительно: Горячие клавиши Unity

- `Cmd/Ctrl + Shift + C` - открыть Console
- `Space` или `Click` - продвинуть диалог
- `Cmd/Ctrl + P` - Play/Stop
- `Cmd/Ctrl + Shift + F` - Focus на выбранном объекте в Scene

---

**Готово!** Теперь запустите Unity и следуйте инструкции. Если возникнут ошибки, отправьте мне детали.
