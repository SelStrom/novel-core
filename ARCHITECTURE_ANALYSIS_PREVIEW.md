# Анализ архитектуры: SceneEditorWindow.PreviewScene и GameStarter

**Дата**: 2026-03-07  
**Статус**: Архитектурный анализ

## 🎯 Краткое резюме

**Проблема**: `SceneEditorWindow.PreviewScene()` и `GameStarter` не интегрированы — между Editor-time preview и Runtime game flow есть разрыв.

**Текущее состояние**: Preview-функция только устанавливает EditorPrefs и запускает Play Mode, но не гарантирует, что правильная сцена загрузится.

---

## 📊 Текущая архитектура

### 1. SceneEditorWindow.PreviewScene() (Editor-Time)

```csharp
// Файл: SceneEditorWindow.cs, строки 622-634
private void PreviewScene()
{
    if (_currentScene == null)
        return;

    // Store current scene for preview
    EditorPrefs.SetString("NovelCore_PreviewScene", AssetDatabase.GetAssetPath(_currentScene));

    // Enter Play mode
    EditorApplication.isPlaying = true;

    Debug.Log($"Previewing scene: {_currentScene.SceneName}");
}
```

**Что делает**:
- ✅ Сохраняет путь к сцене в `EditorPrefs`
- ✅ Запускает Play Mode
- ❌ Не гарантирует загрузку этой сцены в runtime

---

### 2. GameStarter (Runtime)

```csharp
// Файл: GameStarter.cs, строки 15-113
public class GameStarter : MonoBehaviour
{
    [SerializeField]
    private SceneData _startingScene;  // ← Задается в Inspector вручную
    
    [SerializeField]
    private bool _autoStart = true;
    
    [SerializeField]
    private float _startDelay = 0.5f;

    private void Start()
    {
        if (_autoStart)
        {
            Invoke(nameof(StartGame), _startDelay);
        }
    }

    public void StartGame()
    {
        // Загружает _startingScene (который настроен в Inspector)
        _sceneManager.LoadScene(_startingScene);
        _dialogueSystem.StartScene(_startingScene);
    }
}
```

**Что делает**:
- ✅ Загружает сцену через `ISceneManager`
- ✅ Запускает диалоговую систему
- ❌ **Не читает `EditorPrefs` с preview-сценой**
- ❌ Всегда грузит `_startingScene`, игнорируя preview-запрос

---

## 🔴 Выявленные проблемы

### Проблема 1: Отсутствие моста между Editor и Runtime

**Симптом**:
```
SceneEditorWindow → EditorPrefs["NovelCore_PreviewScene"] = "path/to/scene"
                                        ↓
                                   [РАЗРЫВ]
                                        ↓
GameStarter → всегда грузит _startingScene из Inspector
```

**Последствия**:
- При клике "Preview Scene" в редакторе запускается Play Mode
- Но грузится не preview-сцена, а дефолтная `_startingScene` из GameStarter
- Пользователь видит не ту сцену, которую хотел проверить

---

### Проблема 2: GameStarter не знает о preview-режиме

`GameStarter.Start()` не проверяет:
- Запущен ли Play Mode из Editor (через Preview)?
- Есть ли в `EditorPrefs` путь к preview-сцене?
- Нужно ли переопределить `_startingScene`?

**Текущий код**:
```csharp
public void StartGame()
{
    // Всегда грузит _startingScene, независимо от контекста
    _sceneManager.LoadScene(_startingScene);
}
```

---

### Проблема 3: Нет механизма обнаружения preview-режима

В архитектуре отсутствует:
- Флаг "IsPreviewMode"
- Способ передать preview-сцену из Editor в Runtime
- Fallback-логика: если preview-сцена не найдена → грузить дефолтную

---

## ✅ Что нужно доработать

### 1. **Добавить Preview Mode Handler в GameStarter**

```csharp
// В GameStarter.cs
public void StartGame()
{
    SceneData sceneToLoad = GetSceneToLoad();
    
    if (sceneToLoad == null)
    {
        Debug.LogError("No scene to load!");
        return;
    }

    _sceneManager.LoadScene(sceneToLoad);
    _dialogueSystem.StartScene(sceneToLoad);
}

private SceneData GetSceneToLoad()
{
    #if UNITY_EDITOR
    // Проверяем, запущен ли preview-режим
    if (EditorPrefs.HasKey("NovelCore_PreviewScene"))
    {
        string previewPath = EditorPrefs.GetString("NovelCore_PreviewScene");
        
        // Загружаем SceneData по пути
        SceneData previewScene = AssetDatabase.LoadAssetAtPath<SceneData>(previewPath);
        
        if (previewScene != null)
        {
            Debug.Log($"[Preview Mode] Loading scene: {previewScene.SceneName}");
            
            // Очищаем EditorPrefs после использования
            EditorPrefs.DeleteKey("NovelCore_PreviewScene");
            
            return previewScene;
        }
    }
    #endif
    
    // Fallback: грузим дефолтную сцену
    return _startingScene;
}
```

**Преимущества**:
- ✅ GameStarter автоматически определяет preview-режим
- ✅ Если preview-сцена установлена → грузит её
- ✅ Если нет → грузит дефолтную
- ✅ После запуска preview EditorPrefs очищается

---

### 2. **Улучшить SceneEditorWindow.PreviewScene()**

Добавить валидацию и информативные сообщения:

```csharp
private void PreviewScene()
{
    if (_currentScene == null)
    {
        Debug.LogError("SceneEditorWindow: Cannot preview null scene!");
        return;
    }

    string scenePath = AssetDatabase.GetAssetPath(_currentScene);
    
    if (string.IsNullOrEmpty(scenePath))
    {
        Debug.LogError("SceneEditorWindow: Scene asset path is invalid!");
        return;
    }

    // Сохраняем путь к preview-сцене
    EditorPrefs.SetString("NovelCore_PreviewScene", scenePath);
    
    Debug.Log($"[Preview Mode] Set preview scene: {_currentScene.SceneName}");
    Debug.Log($"[Preview Mode] Path: {scenePath}");
    Debug.Log($"[Preview Mode] Entering Play Mode...");

    // Запускаем Play Mode
    EditorApplication.isPlaying = true;
}
```

---

### 3. **Альтернативный подход: PreviewManager (более масштабируемый)**

Если функций preview будет больше (например, preview с конкретной строки диалога, preview выборов), стоит создать отдельный компонент:

```csharp
// Файл: Runtime/Core/PreviewManager.cs
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NovelCore.Runtime.Core
{
    /// <summary>
    /// Управляет preview-режимом при запуске из Editor.
    /// </summary>
    public static class PreviewManager
    {
        private const string PREVIEW_SCENE_KEY = "NovelCore_PreviewScene";
        private const string PREVIEW_DIALOGUE_INDEX_KEY = "NovelCore_PreviewDialogueIndex";
        
        public static bool IsPreviewMode
        {
            get
            {
                #if UNITY_EDITOR
                return EditorPrefs.HasKey(PREVIEW_SCENE_KEY);
                #else
                return false;
                #endif
            }
        }
        
        public static SceneData GetPreviewScene()
        {
            #if UNITY_EDITOR
            if (!IsPreviewMode)
                return null;
                
            string path = EditorPrefs.GetString(PREVIEW_SCENE_KEY);
            SceneData scene = AssetDatabase.LoadAssetAtPath<SceneData>(path);
            
            // Очищаем после использования
            ClearPreviewData();
            
            return scene;
            #else
            return null;
            #endif
        }
        
        public static void SetPreviewScene(SceneData scene)
        {
            #if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(scene);
            EditorPrefs.SetString(PREVIEW_SCENE_KEY, path);
            #endif
        }
        
        public static void ClearPreviewData()
        {
            #if UNITY_EDITOR
            EditorPrefs.DeleteKey(PREVIEW_SCENE_KEY);
            EditorPrefs.DeleteKey(PREVIEW_DIALOGUE_INDEX_KEY);
            #endif
        }
        
        // Будущее расширение: preview с конкретной строки диалога
        public static int? GetPreviewDialogueIndex()
        {
            #if UNITY_EDITOR
            if (EditorPrefs.HasKey(PREVIEW_DIALOGUE_INDEX_KEY))
                return EditorPrefs.GetInt(PREVIEW_DIALOGUE_INDEX_KEY);
            #endif
            return null;
        }
    }
}
```

**Использование в GameStarter**:
```csharp
private SceneData GetSceneToLoad()
{
    // Проверяем preview-режим
    if (PreviewManager.IsPreviewMode)
    {
        SceneData previewScene = PreviewManager.GetPreviewScene();
        if (previewScene != null)
        {
            Debug.Log($"[Preview Mode] Loading: {previewScene.SceneName}");
            return previewScene;
        }
    }
    
    // Fallback
    return _startingScene;
}
```

**Использование в SceneEditorWindow**:
```csharp
private void PreviewScene()
{
    if (_currentScene == null) return;
    
    PreviewManager.SetPreviewScene(_currentScene);
    EditorApplication.isPlaying = true;
}
```

---

## 🎨 Дополнительные улучшения (опционально)

### 4. **Preview с конкретной строки диалога**

Добавить возможность запустить preview не с начала сцены, а с конкретного диалога:

```csharp
// В SceneEditorWindow.cs
private void DrawDialogueLine(SerializedProperty lineProp)
{
    // ... существующий код ...
    
    // Добавить кнопку "Preview from here"
    if (GUILayout.Button("Preview from this line"))
    {
        PreviewSceneFromDialogue(_selectedDialogueIndex);
    }
}

private void PreviewSceneFromDialogue(int dialogueIndex)
{
    PreviewManager.SetPreviewScene(_currentScene);
    PreviewManager.SetPreviewDialogueIndex(dialogueIndex);
    EditorApplication.isPlaying = true;
}
```

**Требует доработки в DialogueSystem**:
```csharp
public void StartScene(SceneData scene, int? startDialogueIndex = null)
{
    if (startDialogueIndex.HasValue)
    {
        // Пропустить диалоги до указанного индекса
        _currentDialogueIndex = startDialogueIndex.Value;
    }
    // ... остальная логика
}
```

---

### 5. **Visual feedback в Editor**

Добавить индикатор, что сцена готова к preview:

```csharp
private void DrawToolbar()
{
    // ... существующий код ...
    
    EditorGUI.BeginDisabledGroup(_currentScene == null);
    
    // Изменить стиль кнопки в зависимости от состояния
    GUIStyle previewButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
    if (_currentScene != null)
    {
        previewButtonStyle.normal.textColor = Color.green;
        previewButtonStyle.fontStyle = FontStyle.Bold;
    }
    
    if (GUILayout.Button("▶ Preview Scene", previewButtonStyle, GUILayout.Width(110)))
    {
        PreviewScene();
    }
    
    EditorGUI.EndDisabledGroup();
}
```

---

### 6. **Auto-stop Play Mode после preview**

Добавить возможность автоматически останавливать Play Mode после завершения сцены:

```csharp
// В PreviewManager
public static bool AutoStopAfterPreview
{
    get
    {
        #if UNITY_EDITOR
        return EditorPrefs.GetBool("NovelCore_AutoStopPreview", false);
        #else
        return false;
        #endif
    }
    set
    {
        #if UNITY_EDITOR
        EditorPrefs.SetBool("NovelCore_AutoStopPreview", value);
        #endif
    }
}
```

**В SceneEditorWindow**:
```csharp
private void DrawToolbar()
{
    // ... кнопка Preview ...
    
    bool autoStop = PreviewManager.AutoStopAfterPreview;
    bool newAutoStop = GUILayout.Toggle(autoStop, "Auto-stop", EditorStyles.toolbarButton);
    if (newAutoStop != autoStop)
    {
        PreviewManager.AutoStopAfterPreview = newAutoStop;
    }
}
```

**В DialogueSystem** (при завершении сцены):
```csharp
private void OnSceneComplete()
{
    #if UNITY_EDITOR
    if (PreviewManager.AutoStopAfterPreview)
    {
        Debug.Log("[Preview Mode] Scene complete, stopping Play Mode");
        EditorApplication.isPlaying = false;
    }
    #endif
}
```

---

## 📋 Итоговый чек-лист изменений

### Минимальные изменения (must-have)
- [ ] **T1**: Добавить `GetSceneToLoad()` в `GameStarter.cs`
- [ ] **T2**: Добавить проверку `EditorPrefs["NovelCore_PreviewScene"]` в `GameStarter`
- [ ] **T3**: Улучшить валидацию в `SceneEditorWindow.PreviewScene()`

### Рекомендуемые изменения (should-have)
- [ ] **T4**: Создать `PreviewManager.cs` для централизованного управления preview
- [ ] **T5**: Добавить очистку `EditorPrefs` после использования preview-сцены
- [ ] **T6**: Добавить логирование для отладки preview-режима

### Опциональные улучшения (nice-to-have)
- [ ] **T7**: Preview с конкретной строки диалога
- [ ] **T8**: Visual feedback (цветные кнопки, индикаторы)
- [ ] **T9**: Auto-stop Play Mode после preview
- [ ] **T10**: Настройки preview в отдельном окне Preferences

---

## 🔍 Пример сценария использования (After fix)

### До исправления:
```
1. Пользователь открывает SceneEditorWindow
2. Редактирует Scene03b_PathB
3. Нажимает "Preview Scene"
4. Play Mode запускается
5. ❌ Грузится Scene01_Introduction (из GameStarter._startingScene)
6. Пользователь в замешательстве
```

### После исправления:
```
1. Пользователь открывает SceneEditorWindow
2. Редактирует Scene03b_PathB
3. Нажимает "Preview Scene"
   → EditorPrefs["NovelCore_PreviewScene"] = "path/to/Scene03b_PathB.asset"
4. Play Mode запускается
5. GameStarter.Start() вызывается
   → GetSceneToLoad() проверяет EditorPrefs
   → Находит "NovelCore_PreviewScene"
   → Загружает Scene03b_PathB
6. ✅ Пользователь видит именно ту сцену, которую хотел проверить
7. EditorPrefs очищается автоматически
```

---

## 🏗️ Архитектурные принципы

### Соблюдение Single Responsibility
- `SceneEditorWindow` → Editor UI и инициация preview
- `GameStarter` → Инициализация игры и определение стартовой сцены
- `PreviewManager` → Мост между Editor и Runtime

### Избежание платформенной зависимости
- `#if UNITY_EDITOR` директивы для Editor-only кода
- Runtime код не зависит от EditorPrefs в production builds

### Fail-safe design
- Всегда есть fallback на `_startingScene`
- Если preview-сцена не найдена → игра не крашится
- Автоматическая очистка EditorPrefs после использования

---

## 💡 Альтернативные подходы (отклонены)

### ❌ Подход 1: ScriptableObject для preview-состояния
**Идея**: Создать `PreviewState.asset` для хранения preview-сцены.

**Почему не подходит**:
- Требует создания asset-файла
- Asset может быть случайно закоммичен в git
- EditorPrefs проще и не засоряет проект

### ❌ Подход 2: CustomPlayModeInitializer
**Идея**: Использовать `[RuntimeInitializeOnLoadMethod]` для кастомной инициализации.

**Почему не подходит**:
- Сложнее отладка
- Конфликтует с существующей логикой GameStarter
- Избыточно для простой задачи

---

## 📚 Ссылки на код

- `SceneEditorWindow.PreviewScene()`: [SceneEditorWindow.cs:622-634](novel-core/Assets/Scripts/NovelCore/Editor/Windows/SceneEditorWindow.cs)
- `GameStarter.StartGame()`: [GameStarter.cs:55-90](novel-core/Assets/Scripts/NovelCore/Runtime/Core/GameStarter.cs)
- `ISceneManager`: [ISceneManager.cs](novel-core/Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/ISceneManager.cs)
- `SceneManager`: [SceneManager.cs](novel-core/Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/SceneManager.cs)

---

## 📝 Заметки для реализации

1. **Тестирование**: После изменений проверить:
   - Preview из редактора грузит правильную сцену
   - Обычный запуск игры (не preview) грузит дефолтную сцену
   - EditorPrefs очищается после preview
   - Build не содержит Editor-зависимостей

2. **Документация**: Обновить:
   - `user-manual.md`: добавить раздел "Preview Scene в Editor"
   - `SAMPLE_PROJECT_QUICKSTART.md`: упомянуть Preview функцию

3. **Будущие расширения**:
   - Preview с сохранением состояния (variables, flags)
   - Preview с конкретного выбора (ChoiceData)
   - Multi-scene preview (цепочка сцен)

---

**Дата создания**: 2026-03-07  
**Версия документа**: 1.0  
**Автор анализа**: AI Assistant (Claude Sonnet 4.5)
