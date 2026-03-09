# Объединение логики создания Sample Project

## Проблема
Ранее существовало **два отдельных инструмента**:

1. **`SampleProjectGenerator`** (`NovelCore/Generate Sample Project`)
   - Создавал структуру Sample Project
   - Генерировал сцены, диалоги, выборы
   - Создавал placeholder изображения (sprites)
   - **НО** не настраивал их как Addressables

2. **`SetupSampleProject`** (`NovelCore/Setup Sample Project`) ❌ УДАЛЁН
   - Настраивал уже созданные спрайты как Addressables
   - Требовал ручной запуск после генерации
   - Дублировал логику

## Решение
Логика `SetupSampleProject` **интегрирована** в `SampleProjectGenerator`.

### Что изменилось

#### Добавлены новые методы:

```csharp
// Настройка фонов как Addressables
private static void SetupBackgroundAsAddressable(string backgroundName)

// Настройка персонажей как Addressables  
private static void SetupCharacterAsAddressable(string characterName)

// Универсальный метод для любых ассетов
private static void SetupAssetAsAddressable(string assetPath, string address)
```

#### Обновлены существующие методы:

```csharp
private static void GeneratePlaceholderBackgrounds()
{
    // 1. Создаём текстуры
    CreateColoredTexture("bg_room", ...);
    CreateColoredTexture("bg_street", ...);
    CreateColoredTexture("bg_home", ...);
    
    // 2. ✨ НОВОЕ: Сразу настраиваем как Addressables
    SetupBackgroundAsAddressable("bg_room");
    SetupBackgroundAsAddressable("bg_street");
    SetupBackgroundAsAddressable("bg_home");
}

private static void GeneratePlaceholderCharacters()
{
    CreateColoredTexture("char_protagonist", ...);
    
    // ✨ НОВОЕ: Сразу настраиваем как Addressables
    SetupCharacterAsAddressable("char_protagonist");
}
```

## Результат

### ✅ Преимущества:
- **Один инструмент** вместо двух
- **Автоматическая настройка** Addressables при генерации
- **Нет дублирования** кода
- **Проще использовать**: один клик `NovelCore → Generate Sample Project`

### 📋 Addressable конфигурация:
- **Фоны**: `Backgrounds/bg_room`, `Backgrounds/bg_street`, `Backgrounds/bg_home`
- **Персонажи**: `Characters/char_protagonist`
- **Сцены**: по имени сцены (например, `Scene01_Introduction`)

## Использование

```
Unity Editor → NovelCore → Generate Sample Project
```

Теперь генератор **автоматически**:
1. ✅ Создаёт директории
2. ✅ Генерирует placeholder изображения
3. ✅ Настраивает их как Sprites
4. ✅ **Добавляет в Addressables с правильными адресами**
5. ✅ Создаёт SceneData ассеты
6. ✅ Связывает сцены через выборы
7. ✅ Настраивает Unity сцену с UI

## Удалённые файлы
- ❌ `SetupSampleProject.cs`
- ❌ `SetupSampleProject.cs.meta`

## Миграция
Если вы использовали `SetupSampleProject` ранее:
- Просто запустите `Generate Sample Project` заново
- Все настройки будут применены автоматически
