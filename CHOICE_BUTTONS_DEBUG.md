# Диагностика: Кнопки выбора не отображаются

## Исправления

### ✅ Исправлено: targetScene установлен
**Коммит:** `dcaacc7`

Теперь `ChoiceOption.targetScene` корректно установлен с `AssetReference`:
- Option A → Scene03a_PathA
- Option B → Scene03b_PathB

## Чеклист диагностики

Если кнопки всё ещё не отображаются, проверьте по порядку:

### 1. Пересоздать Sample Project
```
NovelCore → Generate Sample Project → Overwrite
```

**Важно:** Старый Sample Project не будет обновлён автоматически!

### 2. Проверить Unity сцену

Откройте `Assets/Scenes/SampleScene.unity` и проверьте Hierarchy:

```
✅ Должны быть следующие объекты:
- GameLifetimeScope
- GameStarter
- UIManager
- NavigationUIManager
- Canvas
  └─ ChoiceUI
     └─ ChoicePanel
        └─ ChoiceContainer
  └─ NavigationUI
     └─ BackButton
     └─ ForwardButton
```

**Если ChoiceUI отсутствует** → Sample Project создан со старым кодом, пересоздайте.

### 3. Проверить UIManager в Inspector

Выберите `UIManager` в Hierarchy:

```
✅ В Inspector должны быть:
- Canvas: [ссылка на Canvas]
- Dialogue Box Controller: [Auto-найдено или null]
- Choice UI Controller: [ссылка на ChoiceUI/ChoiceUIController] ← ВАЖНО!
```

**Если Choice UI Controller = None** → UIManager не нашёл ChoiceUIController

**Решение:**
1. Найдите ChoiceUI в Hierarchy
2. Вручную перетащите в поле Choice UI Controller
3. Сохраните сцену

### 4. Проверить ChoiceUIController в Inspector

Выберите `ChoiceUI` в Hierarchy → в Inspector должен быть компонент `ChoiceUIController`:

```
✅ Проверьте поля:
- Choice Container: [ссылка на ChoicePanel/ChoiceContainer]
- Choice Button Prefab: [ссылка на Resources/NovelCore/UI/ChoiceButton]
- Choice Panel: [ссылка на ChoicePanel]
```

**Если Choice Button Prefab = None:**

```
NovelCore → Generate UI Prefabs → Choice Button
```

Это создаст prefab в `Assets/Resources/NovelCore/UI/ChoiceButton.prefab`

После создания:
1. Выберите ChoiceUI в Hierarchy
2. Перетащите prefab в поле Choice Button Prefab
3. Сохраните сцену

### 5. Проверить Scene02_ChoicePoint.asset

Откройте `Assets/Content/Projects/Sample/Scenes/Scene02_ChoicePoint.asset`:

```
✅ Должно быть:
- Dialogue Lines: [2 реплики]
- Choices: [1 элемент] ← ВАЖНО!
  └─ Choice_MainDecision
     └─ Options: [2 элемента]
        - Option A: "Выйти на улицу"
          Target Scene: Scene03a_PathA (AssetReference)
        - Option B: "Остаться дома"
          Target Scene: Scene03b_PathB (AssetReference)
```

**Если Choices пустой** → Sample Project создан неправильно, пересоздайте.

**Если Target Scene = None** → Выборы созданы, но не связаны со сценами:
- Пересоздайте Sample Project с новым кодом

### 6. Проверить Addressables

```
Window → Asset Management → Addressables → Groups
```

**Должны быть помечены как Addressable:**
- Scene01_Introduction
- Scene02_ChoicePoint
- Scene03a_PathA ← ВАЖНО для выбора A!
- Scene03b_PathB ← ВАЖНО для выбора B!

**Если какая-то сцена отсутствует:**
1. Выберите её в Project
2. В Inspector поставьте галочку Addressable
3. Или пересоздайте Sample Project

### 7. Проверить Console логи

Запустите игру (Play ▶️) и проверьте Console:

**При старте должны быть:**
```
NovelCore: GameLifetimeScope initialized successfully ✅
UIManager: Initializing UI...
UIManager: DialogueBoxController initialized successfully ✅
UIManager: ChoiceUIController initialized successfully ✅ ← ВАЖНО!
NavigationUIManager: Successfully initialized SceneNavigationUI
```

**Если нет "ChoiceUIController initialized"** → UIManager не смог инициализировать ChoiceUI

**При достижении Scene02 (после 2 реплик) должны быть:**
```
DialogueSystem: Showing choices for choice_main_001 ✅
ChoiceUIController: Displaying 2 choices ✅
```

**Если нет этих логов** → DialogueSystem не достигает точки выбора

### 8. Проверить GameLifetimeScope

Если ChoiceUIController не инициализируется, проверьте:

```cs
// В GameLifetimeScope.cs должны быть зарегистрированы:
builder.Register<IDialogueSystem, DialogueSystem>(Lifetime.Singleton);
builder.Register<ISceneNavigationHistory, SceneNavigationHistory>(Lifetime.Singleton);
builder.Register<ISceneManager, SceneManager>(Lifetime.Singleton);
```

## Распространённые проблемы

### Проблема 1: "ChoiceButton prefab not found"

**Лог:**
```
[SampleProjectGenerator] ChoiceButton prefab not found in Resources
```

**Решение:**
```
NovelCore → Generate UI Prefabs → Choice Button
```

### Проблема 2: Кнопки появляются, но текст пустой

**Причина:** Prefab создан неправильно или отсутствует TextMeshProUGUI

**Решение:**
1. Удалите старый prefab
2. Пересоздайте: `NovelCore → Generate UI Prefabs → Choice Button`

### Проблема 3: Кнопки появляются, но клик не работает

**Проверьте:**
1. EventSystem в сцене (должен быть автоматически)
2. Canvas имеет GraphicRaycaster
3. В Console нет ошибок "targetScene = None"

**Лог должен быть:**
```
ChoiceUIController: Choice 0 selected ✅
DialogueSystem: Selected choice index 0 ✅
```

### Проблема 4: InvalidKeyException при выборе

**Лог:**
```
InvalidKeyException: No Location found for Key=...
```

**Причина:** Целевые сцены не Addressable

**Решение:**
1. Проверьте Addressables Groups
2. Пометьте Scene03a и Scene03b как Addressable
3. Или пересоздайте Sample Project

## Пошаговый тест

1. **Пересоздать Sample Project** (с новым кодом)
2. **Play ▶️**
3. **Кликнуть 3 раза** (Scene01 - 3 реплики)
4. **Автопереход на Scene02**
5. **Кликнуть 2 раза** (Scene02 - 2 реплики)
6. **ДОЛЖНЫ ПОЯВИТЬСЯ 2 КНОПКИ:** ✅
   - "Выйти на улицу"
   - "Остаться дома"
7. **Выбрать любую кнопку**
8. **Переход на Scene03a или Scene03b** ✅

## Если ничего не помогло

1. Удалите папку `Assets/Content/Projects/Sample`
2. Удалите `Assets/Scenes/SampleScene.unity`
3. Создайте новую пустую сцену
4. Пересоздайте Sample Project
5. Проверьте Console на ошибки при генерации

## Коммиты с исправлениями

- `dcaacc7` - fix: Set targetScene for choice options
- `e8553d2` - feat: Add ChoiceUI system to Sample Project
- `e4c9234` - fix: Auto-mark Sample Project scenes as Addressable
- `8fba6f9` - fix: Resolve VContainer DI issues

**Применить все исправления:** Переключитесь на ветку `001-scene-transition` и пересоздайте Sample Project.
