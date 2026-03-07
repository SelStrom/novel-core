# 🚀 Быстрый старт: Запуск Sample Project

## Пошаговая инструкция

### 1. Создайте Sample Project

В Unity Editor выберите меню:

```
NovelCore → Generate Sample Project
```

Дождитесь сообщения об успешном создании.

### 2. Проверьте созданные файлы

В Project window должны появиться:

```
Assets/Content/Projects/Sample/
├── Scenes/
│   ├── Scene01_Introduction.asset
│   ├── Scene02_ChoicePoint.asset
│   ├── Scene03a_PathA.asset
│   └── Scene03b_PathB.asset
├── Backgrounds/
│   ├── bg_room.png
│   ├── bg_street.png
│   └── bg_home.png
└── Characters/
    └── char_protagonist.png
```

### 3. Настройте Unity сцену

Перед запуском игры нужно настроить основную Unity сцену:

1. Откройте Unity сцену `Assets/Scenes/SampleScene.unity`
2. Убедитесь, что в сцене есть GameObject с компонентом `GameLifetimeScope` (VContainer)
   - Если его нет, создайте: GameObject → Create Empty, назовите "GameLifetimeScope"
   - Добавьте компонент: Add Component → Game Lifetime Scope
3. Создайте GameObject для запуска игры:
   - GameObject → Create Empty, назовите "GameStarter"
   - Добавьте компонент: Add Component → Game Starter
4. В компоненте `GameStarter`:
   - Перетащите `Scene01_Introduction.asset` из Project window в поле "Starting Scene"
   - Убедитесь, что "Auto Start" включен (галочка стоит)

### 4. Запустите тестирование

**Вариант A: Через Play Mode (рекомендуемый способ)**

1. Нажмите кнопку **Play** ▶️ в Unity
2. Игра автоматически загрузит первую сцену
3. Кликайте на экран для продвижения диалога
4. На сцене 2 появятся кнопки выбора - выберите путь

**Вариант B: Через Scene Editor (если реализован)**

1. Откройте `Window → NovelCore → Scene Editor`
2. Выберите `Scene01_Introduction.asset`
3. Нажмите **"Preview Scene"**

### 4. Что вы увидите

- **Сцена 1**: 3 диалоговые реплики (вступление)
- **Сцена 2**: 2 реплики + выбор из 2 вариантов
- **Сцена 3a или 3b**: Концовка в зависимости от выбора

## ⚠️ Если не работает

### Проблема: "Ничего не происходит при запуске Play Mode"

**Решение**: 
- Убедитесь, что в сцене есть GameObject с компонентом `GameStarter`
- Проверьте, что в `GameStarter` назначена стартовая сцена (поле "Starting Scene")
- Убедитесь, что "Auto Start" включен
- Проверьте Console на ошибки (особенно "Starting scene is not assigned")

### Проблема: "Не могу найти меню NovelCore"

**Решение**: 
- Убедитесь, что файл `SampleProjectGenerator.cs` скомпилирован
- Перезапустите Unity Editor
- Проверьте Console на ошибки компиляции

### Проблема: "Диалоги не отображаются"

**Решение**: Создайте UI prefabs:
```
NovelCore → Generate UI Prefabs → All Prefabs
```

Или по отдельности:
```
NovelCore → Generate UI Prefabs → Dialogue Box
NovelCore → Generate UI Prefabs → Choice Button
```

### Проблема: "Addressables ошибки"

**Решение**: 
- Сейчас можно протестировать диалоги без Addressables
- Для полного функционала выполните настройку из `ADDRESSABLES_SETUP.md`
- Или дождитесь выполнения задачи T009

### Проблема: "Черный экран в Play Mode"

**Решение**:
- Убедитесь, что в сцене есть GameObject с `GameLifetimeScope`
- Проверьте, что VContainer зарегистрировал все сервисы
- Посмотрите Console на ошибки инициализации

## 📖 Полное руководство

Для детальной информации см. `SAMPLE_PROJECT_GUIDE.md`

---

**MVP Iteration 7** | Задача T040 ✅
