# 🚀 Sample Project: Создание в один клик

## Быстрый старт

```
Unity Editor → NovelCore → Generate Sample Project → OK
```

**Готово!** 🎉

## Что делает генератор

### ✅ Автоматически создаёт:

1. **Структуру папок**
   - `Assets/Content/Projects/Sample/Scenes/`
   - `Assets/Content/Projects/Sample/Backgrounds/`
   - `Assets/Content/Projects/Sample/Characters/`

2. **Placeholder изображения**
   - `bg_room.png`, `bg_street.png`, `bg_home.png` (256x256)
   - `char_protagonist.png` (256x256)
   - Автоматически импортирует как **Sprites**

3. **Addressables конфигурация** ✨
   - Фоны: `Backgrounds/bg_room`, `Backgrounds/bg_street`, `Backgrounds/bg_home`
   - Персонажи: `Characters/char_protagonist`
   - Сцены: по имени сцены

4. **SceneData ассеты**
   - `Scene01_Introduction` (3 диалога)
   - `Scene02_ChoicePoint` (2 варианта выбора)
   - `Scene03a_PathA` (концовка A)
   - `Scene03b_PathB` (концовка B)

5. **Систему выборов**
   - `Choice_MainDecision` с двумя вариантами
   - Автоматическая привязка к сценам

6. **Unity сцену** (`SampleScene.unity`)
   - GameLifetimeScope + GameStarter
   - UIManager + EventSystem
   - ChoiceUI (панель выборов)
   - NavigationUI (кнопки Назад/Вперёд)

## Тестирование

```
1. Unity Editor → Open SampleScene.unity
2. Play ▶️
3. Кликайте для прохода диалогов
4. Выберите вариант в точке выбора
5. Проверьте навигацию (кнопки Назад/Вперёд)
```

## Нет ручной настройки! ✨

Раньше требовалось:
- ❌ Вручную импортировать спрайты
- ❌ Вручную настраивать Addressables
- ❌ Запускать отдельный скрипт `Setup Sample Project`

Теперь:
- ✅ **Всё автоматически** в одном генераторе!

## Полезные команды

| Действие | Команда |
|----------|---------|
| Создать/пересоздать Sample Project | `NovelCore → Generate Sample Project` |
| Открыть тестовую сцену | `Assets/Scenes/SampleScene.unity` |
| Проверить Addressables | `Window → Asset Management → Addressables → Groups` |

## Что нового (по сравнению со старой версией)

### Было (2 шага):
1. `Generate Sample Project` - создавал структуру
2. `Setup Sample Project` - настраивал Addressables ❌ УДАЛЁН

### Стало (1 шаг):
1. `Generate Sample Project` - делает ВСЁ автоматически ✅

## Технические детали

### Созданные методы:
```csharp
SetupBackgroundAsAddressable(string backgroundName)
SetupCharacterAsAddressable(string characterName)
SetupAssetAsAddressable(string assetPath, string address)
```

### Интеграция:
```csharp
GeneratePlaceholderBackgrounds()
{
    CreateColoredTexture(...);  // Создание
    SetupBackgroundAsAddressable(...);  // ✨ Addressables
}
```

## Документация

- [`SAMPLE_PROJECT_GENERATOR_UNIFIED.md`](./SAMPLE_PROJECT_GENERATOR_UNIFIED.md) - детали объединения
- [`SAMPLE_PROJECT_SETUP.md`](./SAMPLE_PROJECT_SETUP.md) - ручная настройка (если нужно)
- [`SAMPLE_PROJECT_STATUS.md`](./SAMPLE_PROJECT_STATUS.md) - статус проекта

---

**Один клик → Готовый Sample Project!** 🚀
