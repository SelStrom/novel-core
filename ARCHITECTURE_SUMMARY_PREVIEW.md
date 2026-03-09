# 🎯 Краткое резюме: Проблемы архитектуры Preview/GameStarter

## Ключевая проблема

**SceneEditorWindow.PreviewScene()** и **GameStarter** не интегрированы — между Editor-time preview и Runtime game flow есть **архитектурный разрыв**.

```
SceneEditorWindow              GameStarter
     ↓                             ↓
"Preview Scene"               StartGame()
     ↓                             ↓
EditorPrefs.Set(...)          LoadScene(_startingScene)
     ↓                             ↓
Play Mode starts              ❌ Игнорирует preview!
```

---

## Что сломано

### 1. Preview не работает
```csharp
// SceneEditorWindow.cs:628
EditorPrefs.SetString("NovelCore_PreviewScene", path);  // ← Записывает
EditorApplication.isPlaying = true;                      // ← Запускает Play

// GameStarter.cs:86
_sceneManager.LoadScene(_startingScene);  // ← Всегда грузит дефолтную сцену!
                                          // ← Никогда не читает EditorPrefs
```

**Результат**: Пользователь нажимает "Preview Scene03b", но грузится "Scene01" из Inspector.

---

## Что нужно исправить

### Минимальное решение (3 задачи)

#### T1: Добавить чтение EditorPrefs в GameStarter

```csharp
// В GameStarter.cs
private SceneData GetSceneToLoad()
{
    #if UNITY_EDITOR
    if (EditorPrefs.HasKey("NovelCore_PreviewScene"))
    {
        string path = EditorPrefs.GetString("NovelCore_PreviewScene");
        SceneData preview = AssetDatabase.LoadAssetAtPath<SceneData>(path);
        
        if (preview != null)
        {
            EditorPrefs.DeleteKey("NovelCore_PreviewScene");  // Очистка
            return preview;
        }
    }
    #endif
    
    return _startingScene;  // Fallback
}

public void StartGame()
{
    SceneData scene = GetSceneToLoad();  // ← Вместо прямого _startingScene
    _sceneManager.LoadScene(scene);
    _dialogueSystem.StartScene(scene);
}
```

**Изменения**: +20 строк кода в `GameStarter.cs`

---

#### T2: Улучшить валидацию в SceneEditorWindow

```csharp
// В SceneEditorWindow.cs
private void PreviewScene()
{
    if (_currentScene == null)
    {
        Debug.LogError("Cannot preview null scene!");
        return;
    }

    string path = AssetDatabase.GetAssetPath(_currentScene);
    EditorPrefs.SetString("NovelCore_PreviewScene", path);
    
    Debug.Log($"[Preview] Set: {_currentScene.SceneName} → Play Mode");
    EditorApplication.isPlaying = true;
}
```

**Изменения**: +5 строк логирования

---

#### T3: Создать PreviewManager (опционально, но рекомендуется)

```csharp
// Новый файл: Runtime/Core/PreviewManager.cs
public static class PreviewManager
{
    public static bool IsPreviewMode { get; }
    public static SceneData GetPreviewScene() { /* ... */ }
    public static void SetPreviewScene(SceneData scene) { /* ... */ }
    public static void ClearPreviewData() { /* ... */ }
}
```

**Преимущества**:
- Централизованная логика preview
- Легко расширяется (preview с конкретного диалога, etc.)
- Убирает дублирование строковых ключей

**Изменения**: +60 строк нового кода

---

## Быстрый старт (для разработчика)

### Что делать прямо сейчас

1. **Открыть** `GameStarter.cs`
2. **Изменить** метод `StartGame()`:
   ```diff
   public void StartGame()
   {
   -   _sceneManager.LoadScene(_startingScene);
   +   SceneData scene = GetSceneToLoad();
   +   _sceneManager.LoadScene(scene);
   }
   ```
3. **Добавить** новый метод `GetSceneToLoad()` (см. код выше)
4. **Тестировать**: Preview → должен грузить правильную сцену

### Время реализации
- **T1 (минимум)**: ~15 минут
- **T1+T2 (рекомендуется)**: ~25 минут
- **T1+T2+T3 (полное решение)**: ~1 час

---

## Почему это важно

### Сейчас (до исправления)
- ❌ Preview Scene не работает вообще
- ❌ Разработчик не может быстро проверить изменения в сцене
- ❌ Приходится вручную менять `_startingScene` в Inspector
- ❌ Workflow: редактировать → assign → play → undo assign → повторить

### После исправления
- ✅ Preview Scene работает из коробки
- ✅ Одна кнопка для проверки любой сцены
- ✅ Workflow: редактировать → Preview (1 клик) → готово
- ✅ Никаких изменений в Inspector

---

## Архитектурные выводы

### Отсутствующие компоненты
1. **Мост Editor↔Runtime**: Нет механизма передачи данных из Editor в Play Mode
2. **Preview State Management**: Нет централизованного управления preview-режимом
3. **Fallback Strategy**: GameStarter не проверяет альтернативные источники сцен

### Нарушенные принципы
- **Separation of Concerns**: SceneEditorWindow знает о EditorPrefs, но GameStarter нет
- **Testability**: Preview-логика размазана между двумя классами
- **User Experience**: Feature не работает, но представлена в UI

---

## Полная документация

См. детальный анализ с кодом, примерами и альтернативными подходами:

📄 **[ARCHITECTURE_ANALYSIS_PREVIEW.md](./ARCHITECTURE_ANALYSIS_PREVIEW.md)**

---

**Статус**: 🔴 Критическая проблема (Preview не работает)  
**Приоритет**: 🔥 Высокий (блокирует удобство разработки)  
**Сложность**: 🟢 Низкая (простое исправление)  
**Время**: ⏱️ 15-60 минут (в зависимости от подхода)
