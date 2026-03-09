# Тестирование автоматического запуска с выделенной SceneData

**Дата**: 2026-03-08  
**Функционал**: При нажатии Play в Unity Editor игра автоматически стартует с выделенной SceneData

## Что было изменено

Модифицирован метод `GameStarter.GetSceneToLoad()` в файле:
- `novel-core/Assets/Scripts/NovelCore/Runtime/Core/GameStarter.cs` (строки 133-168)

### Приоритет загрузки сцен

1. **Preview Scene** (наивысший приоритет) - из Scene Editor кнопки "Preview"
2. **Selected Scene** (новый функционал) - выделенная в Project Window
3. **Starting Scene** (fallback) - из Inspector поля `_startingScene`

## Сценарии тестирования

### Scenario 1: Загрузка выделенной SceneData

**Цель**: Проверить, что игра стартует с SceneData, выделенной в Project Window

**Шаги**:
1. Открыть Unity Editor
2. В Project Window найти любую SceneData (например, `Assets/Content/Projects/Sample/Scenes/Scene02_ChoicePoint.asset`)
3. Выделить SceneData кликом мыши
4. Нажать кнопку Play ▶️ в Unity Editor
5. Проверить Console на наличие лога:
   ```
   GameStarter: Loading selected scene: [Имя сцены]
   ```

**Ожидаемый результат**:
- Игра запускается с выделенной сцены
- В Console видно: `GameStarter: Loading selected scene: ...`
- Диалог и визуальное содержимое соответствует выбранной сцене

---

### Scenario 2: Приоритет Preview над Selection

**Цель**: Проверить, что Preview Scene имеет приоритет над выделенной SceneData

**Шаги**:
1. Выделить одну SceneData в Project Window (например, Scene01_Introduction)
2. Открыть Scene Editor: `Window → NovelCore → Scene Editor`
3. Загрузить в редакторе **другую** SceneData (например, Scene02_ChoicePoint)
4. Нажать кнопку "Preview Scene" в Scene Editor
5. Проверить Console на наличие лога:
   ```
   [Preview Mode] Loading preview scene: Scene02_ChoicePoint
   ```

**Ожидаемый результат**:
- Игра запускается с preview сцены (Scene02_ChoicePoint), а НЕ с выделенной (Scene01_Introduction)
- В Console видно: `[Preview Mode] Loading preview scene: Scene02_ChoicePoint`
- Preview имеет приоритет над Selection

---

### Scenario 3: Fallback на Starting Scene

**Цель**: Проверить, что при отсутствии выделения используется Starting Scene из Inspector

**Шаги**:
1. Открыть главную сцену: `Assets/Scenes/SampleScene.unity`
2. Выделить GameObject "GameStarter" в Hierarchy
3. В Inspector проверить, что поле "Starting Scene" заполнено (например, Scene01_Introduction)
4. В Project Window **снять выделение** (кликнуть на пустое место или на папку)
5. Нажать кнопку Play ▶️
6. Проверить Console на наличие лога:
   ```
   GameStarter: Loading default starting scene: Scene01_Introduction
   ```

**Ожидаемый результат**:
- Игра запускается с дефолтной Starting Scene из Inspector
- В Console видно: `GameStarter: Loading default starting scene: ...`
- Функционал не сломан для случая, когда ничего не выделено

---

### Scenario 4: Выделение не-SceneData asset

**Цель**: Проверить, что при выделении других типов assets используется Starting Scene

**Шаги**:
1. В Project Window выделить НЕ-SceneData asset (например, текстуру, материал, или папку)
2. Нажать кнопку Play ▶️
3. Проверить Console на наличие лога:
   ```
   GameStarter: Loading default starting scene: ...
   ```

**Ожидаемый результат**:
- Игра запускается с дефолтной Starting Scene
- Выделение не-SceneData asset игнорируется
- Нет ошибок в Console

---

## Проверка кода

### Изменённый метод GetSceneToLoad()

```csharp
private SceneData GetSceneToLoad()
{
    // 1. Check for preview mode (Constitution Principle VIII: Editor-Runtime Bridge)
    SceneData previewScene = PreviewManager.GetPreviewScene();
    if (previewScene != null)
    {
        Debug.Log($"[Preview Mode] Loading preview scene: {previewScene.SceneName}");
        return previewScene;
    }
    
    // 2. Check for selected SceneData in Project Window (Editor only)
    #if UNITY_EDITOR
    if (UnityEditor.Selection.activeObject is SceneData selectedScene)
    {
        Debug.Log($"GameStarter: Loading selected scene: {selectedScene.SceneName}");
        return selectedScene;
    }
    #endif
    
    // 3. Fallback to default starting scene from Inspector
    if (_startingScene != null)
    {
        Debug.Log($"GameStarter: Loading default starting scene: {_startingScene.SceneName}");
    }
    
    return _startingScene;
}
```

### Ключевые моменты реализации

✅ **#if UNITY_EDITOR** - проверка Selection работает только в Editor, в билде используется fallback  
✅ **Pattern matching** - `is SceneData selectedScene` проверяет тип и сразу приводит к SceneData  
✅ **Приоритет** - Preview → Selection → Starting Scene  
✅ **Логирование** - каждый путь загрузки логируется для отладки  

## Результаты тестирования

_Заполните после выполнения тестов:_

- [ ] Scenario 1: Загрузка выделенной SceneData - ✅/❌
- [ ] Scenario 2: Приоритет Preview над Selection - ✅/❌
- [ ] Scenario 3: Fallback на Starting Scene - ✅/❌
- [ ] Scenario 4: Выделение не-SceneData asset - ✅/❌

**Примечания**:
[Ваши комментарии по результатам тестирования]

## Соответствие Constitution

- ✅ **Principle I (Creator-First Design)**: Упрощает workflow - не нужно вручную настраивать GameStarter
- ✅ **Principle VIII (Editor-Runtime Bridge)**: Использует Selection.activeObject для Editor→Runtime коммуникации
- ✅ **Dual-Mode Preview**: Сохраняет приоритет Scene Editor Preview над автоматическим выбором
- ✅ **Zero Code Changes for Builds**: `#if UNITY_EDITOR` гарантирует, что в билде используется _startingScene

## Troubleshooting

### Игра не стартует с выделенной сценой

**Проблема**: Выделена SceneData, но игра стартует с дефолтной Starting Scene

**Решение**:
1. Проверьте Console - должен быть лог `GameStarter: Loading selected scene: ...`
2. Если лог другой, проверьте:
   - Действительно ли выделена SceneData (не папка, не другой asset)
   - Не активен ли Preview Mode (проверьте EditorPrefs: `NovelCore_PreviewScene`)

### Preview Scene не работает

**Проблема**: Нажата кнопка Preview в Scene Editor, но игра стартует с другой сцены

**Решение**:
1. Проверьте, что PreviewManager корректно записывает EditorPrefs
2. Проверьте Console на наличие `[Preview Mode] Loading preview scene: ...`
3. Preview имеет наивысший приоритет - если он не работает, проблема в PreviewManager

### Нет логов в Console

**Проблема**: При нажатии Play нет логов о загрузке сцены

**Решение**:
1. Проверьте, что GameStarter инициализируется (`GameStarter.StartGame()` вызывается)
2. Проверьте, что GameLifetimeScope существует в сцене
3. Проверьте Console Filter - возможно, логи скрыты фильтром
