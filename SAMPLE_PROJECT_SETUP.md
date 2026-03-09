# Настройка Sample Project - Инструкция

## Проблема

В Sample Project не настроены:
1. ❌ Фоновые изображения для сцен
2. ❌ Персонажи
3. ❌ Музыка
4. ❌ Кнопки навигации не работают (нет истории сцен для навигации)

## Созданные ресурсы

### Фоновые изображения
Созданы placeholder-изображения:
- `Assets/Content/Projects/Sample/Art/Backgrounds/bg_scene1.png` (1920x1080)
- `Assets/Content/Projects/Sample/Art/Backgrounds/bg_scene2.png` (1920x1080)
- `Assets/Content/Projects/Sample/Art/Backgrounds/bg_scene3.png` (1920x1080)
- `Assets/Content/Projects/Sample/Art/Backgrounds/bg_scene4.png` (1920x1080)

### Персонажи
- `Assets/Content/Projects/Sample/Art/Characters/character_placeholder.png` (512x1024)

## Что нужно сделать в Unity Editor

### 1. Импортировать изображения как Sprites

1. Выбрать все `.png` файлы в папках Backgrounds и Characters
2. В Inspector изменить **Texture Type** на **Sprite (2D and UI)**
3. Нажать **Apply**

### 2. Сделать ресурсы Addressable

#### Фоновые изображения:
1. Выбрать `bg_scene1.png` → Inspector → ✓ **Addressable**
2. Установить Address: `Backgrounds/bg_intro`
3. Повторить для остальных:
   - `bg_scene2.png` → `Backgrounds/bg_choice`
   - `bg_scene3.png` → `Backgrounds/bg_pathA`
   - `bg_scene4.png` → `Backgrounds/bg_pathB`

#### Персонажи:
1. Выбрать `character_placeholder.png` → Inspector → ✓ **Addressable**
2. Address: `Characters/character_test`

### 3. Создать CharacterData asset

1. В папке `Assets/Content/Projects/Sample/Data/Characters/`
2. ПКМ → Create → NovelCore → Character Data
3. Назвать: `CharacterTest`
4. Заполнить:
   - Character ID: `char_test`
   - Character Name: `Тестовый персонаж`
   - Animation Type: `Static` (или Spine если есть)
   - Default Scale: `(1, 1)`

### 4. Обновить SceneData assets

#### Scene01_Introduction.asset:
1. Открыть в Inspector
2. **Background Image**:
   - Нажать на кружок справа
   - Выбрать `bg_intro` из Addressables
3. **Characters** (опционально):
   - Size: 1
   - Element 0:
     - Character: выбрать `CharacterTest` (Addressable)
     - Position: `(0.5, 0.3)` - по центру внизу
     - Initial Emotion: `neutral`

#### Scene02_ChoicePoint.asset:
1. Background Image → `bg_choice`
2. Characters (если нужен персонаж)

#### Scene03a_PathA.asset:
1. Background Image → `bg_pathA`

#### Scene03b_PathB.asset:
1. Background Image → `bg_pathB`

### 5. Проверить кнопки навигации

#### В SampleScene.unity:
1. Найти GameObject `NavigationUIManager`
2. Убедиться, что:
   - `SceneNavigationUI` component привязан
   - В дочернем объекте `SceneNavigationUI` есть:
     - `BackButton` - ссылка на кнопку "◀ Назад"
     - `ForwardButton` - ссылка на кнопку "Вперёд →"

#### Важно:
Кнопки навигации станут активными только после того, как вы:
1. Пройдете первую сцену (Scene01)
2. Перейдете на вторую сцену (Scene02)
3. Тогда кнопка "Назад" станет активной (можно вернуться к Scene01)

## ✨ Автоматический способ (РЕКОМЕНДУЕТСЯ)

Используйте встроенный генератор Sample Project:

```
Unity Editor → Меню → NovelCore → Generate Sample Project
```

Генератор автоматически:
- ✅ Создаёт все ассеты (сцены, диалоги, выборы)
- ✅ Генерирует placeholder изображения (256x256)
- ✅ Импортирует их как Sprites
- ✅ Настраивает Addressables с правильными адресами
- ✅ Связывает сцены через систему выборов
- ✅ Настраивает Unity сцену с UI и навигацией

**Никакой ручной настройки не требуется!**

## Быстрый тест

После настройки:

1. Открыть `SampleScene`
2. Нажать Play ▶️
3. Проверить:
   - ✓ Виден фоновый спрайт
   - ✓ Виден персонаж (если добавлен)
   - ✓ Диалоговое окно с текстом
   - ✓ Кнопки навигации (серые на первой сцене)
   - ✓ После перехода на Scene02 кнопка "Назад" становится активной

## Почему кнопки не работали

1. **Нет истории навигации**: Кнопки навигации работают на основе `SceneNavigationHistory`
2. При запуске игры история пустая → кнопка "Назад" неактивна
3. После перехода на следующую сцену предыдущая добавляется в историю
4. Теперь можно вернуться назад

Это правильное поведение! Кнопки не должны быть активны, если некуда возвращаться.
