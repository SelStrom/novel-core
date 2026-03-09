# Быстрый запуск сцен в Play Mode

**Обновление**: 2026-03-08  
**Новая функция**: Автоматический запуск игры с выделенной SceneData

## Что это даёт?

Теперь вы можете **быстро тестировать любую сцену**, просто выделив её в Project Window и нажав Play ▶️. Не нужно каждый раз менять настройки GameStarter в Inspector!

## Как использовать

### Способ 1: Быстрый тест выделенной сцены

1. В Project Window найдите SceneData, которую хотите протестировать
2. Кликните на неё мышью (выделите)
3. Нажмите кнопку Play ▶️ в Unity Editor
4. **Готово!** Игра стартует с этой сцены

**Пример**:
```
Assets/Content/Projects/Sample/Scenes/
├── Scene01_Introduction.asset      ← Выделите эту сцену
├── Scene02_ChoicePoint.asset
└── Scene03a_PathA.asset
```

После выделения `Scene02_ChoicePoint.asset` и нажатия Play, игра стартует с этой сцены.

### Способ 2: Превью из Scene Editor (приоритет)

Если вы используете кнопку **"Preview Scene"** в Scene Editor Window, она имеет **наивысший приоритет** над выделением в Project Window.

**Порядок приоритета**:
1. 🥇 **Preview Scene** (кнопка в Scene Editor)
2. 🥈 **Selected Scene** (выделенная в Project Window)
3. 🥉 **Starting Scene** (настроенная в GameStarter Inspector)

### Способ 3: Обычный запуск (fallback)

Если ничего не выделено и Preview не активен, игра стартует с **Starting Scene**, настроенной в Inspector поля GameStarter.

## Логи в Console

При запуске игры вы увидите один из логов:

```
[Preview Mode] Loading preview scene: Scene02_ChoicePoint
```
↑ Запуск через Scene Editor Preview

```
GameStarter: Loading selected scene: Scene02_ChoicePoint
```
↑ Запуск с выделенной SceneData

```
GameStarter: Loading default starting scene: Scene01_Introduction
```
↑ Fallback на Starting Scene из Inspector

## Часто задаваемые вопросы

### Q: Я выделил сцену, но игра стартует с другой

**A**: Проверьте Console. Если видите `[Preview Mode] Loading preview scene: ...`, значит активен Preview Mode из Scene Editor. Preview имеет приоритет над Selection.

**Решение**: Запустите игру ещё раз — Preview Mode очищается после первого использования.

### Q: Можно ли выделить несколько сцен сразу?

**A**: Нет, будет использована первая выделенная SceneData из `Selection.activeObject`. Это стандартное поведение Unity.

### Q: Работает ли это в билде?

**A**: Нет, эта функция работает **только в Unity Editor**. В билде всегда используется Starting Scene из GameStarter. Это сделано специально через `#if UNITY_EDITOR` для безопасности.

### Q: Нужно ли очищать выделение после теста?

**A**: Нет, выделение остаётся активным до тех пор, пока вы не выделите что-то другое. Это удобно для повторных тестов одной и той же сцены.

### Q: Что будет, если я выделю не SceneData?

**A**: Игра стартует с Starting Scene из Inspector (fallback). Выделение не-SceneData asset просто игнорируется.

## Технические детали

### Изменённые файлы

- `novel-core/Assets/Scripts/NovelCore/Runtime/Core/GameStarter.cs` - метод `GetSceneToLoad()`

### Используемые технологии

- `UnityEditor.Selection.activeObject` - API Unity для получения выделенного asset
- `#if UNITY_EDITOR` - условная компиляция (работает только в Editor)
- `PreviewManager` - существующий класс для Editor-Runtime коммуникации

### Constitution Compliance

✅ **Principle I (Creator-First Design)**: Упрощает workflow для создателей контента  
✅ **Principle VIII (Editor-Runtime Bridge)**: Использует Selection API для Editor→Runtime коммуникации  
✅ **No Code Required**: Работает без написания кода, через визуальный интерфейс Unity

## Примеры использования

### Пример 1: Тестирование диалога в середине игры

Вы работаете над сценой `Scene05_Climax.asset` и хотите протестировать диалог.

**Старый способ** (до обновления):
1. Открыть SampleScene.unity
2. Найти GameStarter в Hierarchy
3. В Inspector изменить Starting Scene на Scene05_Climax
4. Нажать Play
5. После теста вернуть Starting Scene обратно

**Новый способ**:
1. Выделить `Scene05_Climax.asset` в Project Window
2. Нажать Play ▶️
3. **Готово!** Не нужно ничего менять в Inspector

### Пример 2: Быстрое переключение между сценами

Вы тестируете разные концовки игры:

```
Assets/Content/Projects/Sample/Scenes/
├── Scene10_EndingA.asset   ← Тест 1: выделить и Play
├── Scene10_EndingB.asset   ← Тест 2: выделить и Play
└── Scene10_EndingC.asset   ← Тест 3: выделить и Play
```

**Workflow**:
1. Выделить Scene10_EndingA → Play → проверить
2. Exit Play Mode
3. Выделить Scene10_EndingB → Play → проверить
4. Exit Play Mode
5. Выделить Scene10_EndingC → Play → проверить

Быстро и удобно! 🚀

---

**Обратная связь**: Если у вас есть предложения по улучшению этой функции, создайте issue в репозитории.
