# Рефакторинг: Объединение логики Sample Project Generator

**Дата**: 2026-03-09  
**Задача**: Унификация инструментов создания Sample Project  
**Статус**: ✅ Завершено

---

## Проблема

Существовало **два отдельных инструмента** для создания Sample Project:

### 1. SampleProjectGenerator
```
Меню: NovelCore → Generate Sample Project
Файл: Assets/Scripts/NovelCore/Editor/Tools/Generators/SampleProjectGenerator.cs
```

**Функции:**
- ✅ Создавал структуру папок
- ✅ Генерировал SceneData ассеты
- ✅ Создавал placeholder изображения (Sprites)
- ✅ Настраивал Unity сцену с UI
- ❌ **НЕ настраивал Addressables для изображений**

### 2. SetupSampleProject ❌ УСТАРЕЛ
```
Меню: NovelCore → Setup Sample Project
Файл: Assets/Scripts/NovelCore/Editor/Tools/SetupSampleProject.cs
```

**Функции:**
- Импортировал спрайты
- Настраивал Addressables для фонов и персонажей
- Требовал **ручного запуска** после генерации

### Последствия:
- 🔴 Пользователь должен был запускать **два инструмента**
- 🔴 Дублирование кода
- 🔴 Путаница в последовательности действий
- 🔴 Неполная автоматизация

---

## Решение

### ✅ Унификация в один инструмент

**Логика `SetupSampleProject` интегрирована в `SampleProjectGenerator`**

### Изменения в коде

#### 1. Добавлены новые методы в SampleProjectGenerator.cs

```csharp
/// <summary>
/// Настраивает фоновое изображение как Addressable с правильным адресом.
/// </summary>
private static void SetupBackgroundAsAddressable(string backgroundName)
{
    string assetPath = $"{BACKGROUNDS_DIR}/{backgroundName}.png";
    string address = $"Backgrounds/{backgroundName}";
    SetupAssetAsAddressable(assetPath, address);
}

/// <summary>
/// Настраивает персонажа как Addressable с правильным адресом.
/// </summary>
private static void SetupCharacterAsAddressable(string characterName)
{
    string assetPath = $"{CHARACTERS_DIR}/{characterName}.png";
    string address = $"Characters/{characterName}";
    SetupAssetAsAddressable(assetPath, address);
}

/// <summary>
/// Универсальный метод для настройки любого ассета как Addressable.
/// </summary>
private static void SetupAssetAsAddressable(string assetPath, string address)
{
    if (!File.Exists(assetPath))
    {
        Debug.LogWarning($"[SampleProjectGenerator] Asset not found: {assetPath}");
        return;
    }

    var settings = AddressableAssetSettingsDefaultObject.Settings;
    if (settings == null)
    {
        Debug.LogError("[SampleProjectGenerator] Addressables settings not found!");
        return;
    }

    string guid = AssetDatabase.AssetPathToGUID(assetPath);
    var entry = settings.FindAssetEntry(guid);
    
    if (entry == null)
    {
        var defaultGroup = settings.DefaultGroup;
        if (defaultGroup == null)
        {
            Debug.LogError("[SampleProjectGenerator] No default Addressables group found");
            return;
        }
        
        entry = settings.CreateOrMoveEntry(guid, defaultGroup, false, false);
    }

    if (entry != null)
    {
        entry.address = address;
        EditorUtility.SetDirty(settings);
        Debug.Log($"[SampleProjectGenerator] ✓ Addressable setup: {address} → {assetPath}");
    }
}
```

#### 2. Обновлены существующие методы

**Было:**
```csharp
private static void GeneratePlaceholderBackgrounds()
{
    CreateColoredTexture("bg_room", new Color(0.8f, 0.7f, 0.6f), BACKGROUNDS_DIR);
    CreateColoredTexture("bg_street", new Color(0.6f, 0.7f, 0.8f), BACKGROUNDS_DIR);
    CreateColoredTexture("bg_home", new Color(0.7f, 0.8f, 0.7f), BACKGROUNDS_DIR);
}
```

**Стало:**
```csharp
private static void GeneratePlaceholderBackgrounds()
{
    // Generate simple colored textures as placeholder backgrounds
    CreateColoredTexture("bg_room", new Color(0.8f, 0.7f, 0.6f), BACKGROUNDS_DIR);
    CreateColoredTexture("bg_street", new Color(0.6f, 0.7f, 0.8f), BACKGROUNDS_DIR);
    CreateColoredTexture("bg_home", new Color(0.7f, 0.8f, 0.7f), BACKGROUNDS_DIR);
    
    // ✨ НОВОЕ: Настроить эти текстуры как Addressables
    SetupBackgroundAsAddressable("bg_room");
    SetupBackgroundAsAddressable("bg_street");
    SetupBackgroundAsAddressable("bg_home");
}
```

**Аналогично для персонажей:**
```csharp
private static void GeneratePlaceholderCharacters()
{
    CreateColoredTexture("char_protagonist", new Color(1f, 0.8f, 0.6f), CHARACTERS_DIR);
    
    // ✨ НОВОЕ: Настроить персонажей как Addressables
    SetupCharacterAsAddressable("char_protagonist");
}
```

#### 3. Удалены устаревшие файлы

- ❌ `Assets/Scripts/NovelCore/Editor/Tools/SetupSampleProject.cs`
- ❌ `Assets/Scripts/NovelCore/Editor/Tools/SetupSampleProject.cs.meta`

---

## Результат

### До рефакторинга

```
1. Меню → NovelCore → Generate Sample Project
   ↓ (создаёт структуру, НО без Addressables)
2. Меню → NovelCore → Setup Sample Project
   ↓ (настраивает Addressables)
3. Готово
```

**2 шага** 🔴

### После рефакторинга

```
1. Меню → NovelCore → Generate Sample Project
   ↓ (создаёт структуру + автоматически настраивает Addressables)
2. Готово ✅
```

**1 шаг** ✅

---

## Преимущества

| Аспект | До | После |
|--------|-----|-------|
| **Шагов для настройки** | 2 | 1 ✅ |
| **Ручная работа** | Требуется | Не требуется ✅ |
| **Дублирование кода** | Да | Нет ✅ |
| **Addressables** | Ручная настройка | Автоматически ✅ |
| **Удобство** | Средне | Отлично ✅ |

---

## Addressables конфигурация

### Автоматически настраиваются адреса:

**Фоновые изображения:**
- `Assets/Content/Projects/Sample/Backgrounds/bg_room.png` → `Backgrounds/bg_room`
- `Assets/Content/Projects/Sample/Backgrounds/bg_street.png` → `Backgrounds/bg_street`
- `Assets/Content/Projects/Sample/Backgrounds/bg_home.png` → `Backgrounds/bg_home`

**Персонажи:**
- `Assets/Content/Projects/Sample/Characters/char_protagonist.png` → `Characters/char_protagonist`

**SceneData:**
- Сцены автоматически добавляются в Addressables по имени сцены

---

## Использование

### Создание Sample Project (один клик):

```
Unity Editor → NovelCore → Generate Sample Project → OK
```

Генератор автоматически:
1. ✅ Создаёт структуру папок
2. ✅ Генерирует placeholder изображения
3. ✅ Импортирует их как Sprites
4. ✅ **Настраивает Addressables с правильными адресами**
5. ✅ Создаёт SceneData ассеты
6. ✅ Связывает сцены через выборы
7. ✅ Настраивает Unity сцену с UI и навигацией

**Никакой дополнительной настройки не требуется!**

---

## Обновлённая документация

Созданы/обновлены файлы:
- ✅ `SAMPLE_PROJECT_GENERATOR_UNIFIED.md` - детали объединения
- ✅ `SAMPLE_PROJECT_ONE_CLICK.md` - краткая инструкция
- ✅ `SAMPLE_PROJECT_STATUS.md` - обновлены ссылки
- ✅ `SAMPLE_PROJECT_READY.md` - обновлены ссылки
- ✅ `SAMPLE_PROJECT_SETUP.md` - обновлены ссылки

---

## Тестирование

### Проверка работы:

```bash
1. Unity Editor → NovelCore → Generate Sample Project
2. Подтвердить создание/перезапись
3. Дождаться сообщения "Sample Project Created"
4. Проверить Addressables:
   - Window → Asset Management → Addressables → Groups
   - Убедиться, что фоны и персонажи в списке с правильными адресами
5. Play ▶️ в SampleScene
6. Проверить:
   ✓ Диалоги работают
   ✓ Выборы работают
   ✓ Навигация работает
```

---

## Совместимость

### Обратная совместимость:
- ✅ Существующие SceneData ассеты не изменились
- ✅ Addressables API остался прежним
- ✅ Unity сцена настраивается так же

### Миграция:
Если вы использовали `Setup Sample Project` ранее:
1. Просто запустите `Generate Sample Project` заново
2. Выберите "Overwrite" при запросе
3. Все настройки будут применены автоматически

---

## Технические детали

### Архитектурное решение:

**Принцип:** Один инструмент = одна ответственность  
**Реализация:** Композиция методов внутри `SampleProjectGenerator`

```
GenerateSampleProject()
├── CreateDirectoryStructure()
├── GeneratePlaceholderBackgrounds()
│   ├── CreateColoredTexture()
│   └── SetupBackgroundAsAddressable()  ← НОВОЕ
├── GeneratePlaceholderCharacters()
│   ├── CreateColoredTexture()
│   └── SetupCharacterAsAddressable()  ← НОВОЕ
├── GenerateStoryScenes()
├── MarkAssetAsAddressable() (для сцен)
├── LinkScenesWithChoices()
└── Setup Unity Scene...
```

### Используемые Unity API:
- `AssetDatabase.AssetPathToGUID()` - получение GUID ассета
- `AddressableAssetSettingsDefaultObject.Settings` - настройки Addressables
- `settings.CreateOrMoveEntry()` - создание Addressable entry
- `EditorUtility.SetDirty()` - пометка для сохранения

---

## Выводы

### Достигнуто:
- ✅ Упрощение workflow для пользователей (1 шаг вместо 2)
- ✅ Устранение дублирования кода
- ✅ Полная автоматизация настройки Sample Project
- ✅ Улучшение maintainability (один инструмент вместо двух)

### Следующие шаги:
- Обновить user manual с новой инструкцией
- Добавить unit-тесты для новых методов (опционально)
- Рассмотреть добавление прогресс-бара для long-running операций

---

**Рефакторинг завершён! 🎉**
