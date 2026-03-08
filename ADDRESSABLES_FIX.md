# Addressables Fix - Краткая инструкция

## Проблема
```
InvalidKeyException: No Location found for Key=5d8ffce10bb864e96ab1270422e174dc
Asset exists at Path=Assets/Content/Projects/Sample/Scenes/Scene02_ChoicePoint.asset, 
verify the asset is marked as Addressable.
```

## Причина
Сцены были созданы, но **не помечены как Addressable** → загрузка через AssetReference не работала.

## Решение
Добавлена автоматическая регистрация сцен в Addressables:

```csharp
// В SampleProjectGenerator.GenerateSampleProject()
var scenes = GenerateStoryScenes();

// ✅ Новое: Автоматически помечаем все сцены как Addressable
foreach (var scene in scenes.Values)
{
    MarkAssetAsAddressable(scene);
}
```

### Функция MarkAssetAsAddressable()
- Получает GUID ассета
- Проверяет Addressables settings
- Добавляет ассет в default group
- Использует имя сцены как адрес

## Что теперь работает
- ✅ Все сцены автоматически Addressable
- ✅ Загрузка через `AssetReference.LoadAssetAsync()` работает
- ✅ Навигация между сценами функциональна
- ✅ Линейная прогрессия Scene01 → Scene02 работает

## Для пользователей

### После обновления кода:
1. **Пересоздайте Sample Project:**
   ```
   NovelCore → Generate Sample Project → Overwrite
   ```

2. **Проверьте Addressables:**
   - Window → Asset Management → Addressables → Groups
   - Должны быть сцены: Scene01_Introduction, Scene02_ChoicePoint, Scene03a_PathA, Scene03b_PathB

3. **Запустите игру:**
   - Play ▶️
   - Пройдите через Scene01
   - Автоматически должен произойти переход на Scene02
   - Ошибок InvalidKeyException быть не должно

### Если старый Sample Project
Если у вас уже существует Sample Project, созданный до этого исправления:

**Вариант 1: Пересоздать (рекомендуется)**
```
NovelCore → Generate Sample Project → Overwrite
```

**Вариант 2: Пометить вручную**
1. Выберите сцену в Project
2. Inspector → Addressable ✓
3. Повторите для всех сцен

## Технические детали

### Порядок инициализации
1. `CreateDirectoryStructure()` - создание папок
2. `GeneratePlaceholderBackgrounds()` - создание фонов
3. `GeneratePlaceholderCharacters()` - создание персонажей
4. `GenerateStoryScenes()` - создание SceneData ассетов
5. **`MarkAssetAsAddressable()`** ✅ - регистрация в Addressables
6. `LinkScenesWithChoices()` - связывание сцен
7. `SetupUnitySceneWithGameStarter()` - настройка Unity сцены
8. `SetupUIManager()` - создание UI
9. `SetupNavigationUI()` - создание кнопок навигации

### Addressables настройки
- **Group:** Default Local Group
- **Address:** Имя сцены (Scene01_Introduction, etc.)
- **Build Path:** Local
- **Load Path:** Local

## Коммит
**e4c9234** - fix: Auto-mark Sample Project scenes as Addressable

Все изменения включены в этот коммит.
