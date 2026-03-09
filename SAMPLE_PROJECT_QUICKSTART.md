# 🚀 Быстрый старт: Запуск Sample Project

## Пошаговая инструкция

### 1. Создайте Sample Project

В Unity Editor выберите меню:

```
NovelCore → Generate Sample Project
```

Дождитесь сообщения об успешном создании.

**✨ Новое в T040.3**: Генератор теперь автоматически настраивает Unity сцену:
- Создаёт или находит GameObject с `GameLifetimeScope`
- Создаёт или находит GameObject с `GameStarter`
- Автоматически назначает `Scene01_Introduction.asset` как стартовую сцену
- Настраивает Auto Start и задержку запуска

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

### 3. ✅ Unity сцена настроена автоматически!

Генератор уже настроил `Assets/Scenes/SampleScene.unity`:
- ✓ GameObject `GameLifetimeScope` создан и настроен
- ✓ GameObject `GameStarter` создан
- ✓ Стартовая сцена назначена (`Scene01_Introduction.asset`)
- ✓ Auto Start включен
- ✓ Start Delay = 0.5 секунды

**Проверить настройку вручную** (опционально):
1. Откройте Unity сцену `Assets/Scenes/SampleScene.unity`
2. В Hierarchy должны быть GameObjects: `GameLifetimeScope` и `GameStarter`
3. Выберите `GameStarter` в Inspector:
   - Поле "Starting Scene" должно содержать `Scene01_Introduction`
   - "Auto Start" должен быть включен (галочка)
   - "Start Delay" должен быть 0.5

### 4. Запустите тестирование

**Вариант A: Автоматический запуск (рекомендуется)**

1. Нажмите кнопку **Play** ▶️ в Unity
2. Игра автоматически загрузит первую сцену через 0.5 секунды
3. Кликайте на экран для продвижения диалога
4. На сцене 2 появятся кнопки выбора - выберите путь

**Вариант B: Через Scene Editor (если реализован)**

1. Откройте `Window → NovelCore → Scene Editor`
2. Выберите `Scene01_Introduction.asset`
3. Нажмите **"Preview Scene"**

### 5. Что вы увидите

- **Сцена 1**: 3 диалоговые реплики (вступление)
- **Сцена 2**: 2 реплики + выбор из 2 вариантов
- **Сцена 3a или 3b**: Концовка в зависимости от выбора

## ⚠️ Если не работает

### Проблема: "Ничего не происходит при запуске Play Mode"

**Решение**: 
- Убедитесь, что генератор успешно завершил работу (проверьте Console на сообщения `[SampleProjectGenerator] ✅`)
- Откройте `SampleScene.unity` и проверьте наличие GameObjects
- Если GameStarter отсутствует, запустите генератор заново: `NovelCore → Generate Sample Project`
- Проверьте Console на ошибки инициализации VContainer

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
- Проверьте, что VContainer зарегистрировал все сервисы (Console должен показать "NovelCore: GameLifetimeScope initialized successfully")
- Посмотрите Console на ошибки инициализации
- Убедитесь, что Main Camera присутствует в сцене

### Проблема: "GameStarter не создаётся автоматически"

**Решение**:
- Проверьте, что файл `Assets/Scenes/SampleScene.unity` существует
- Если сцена отсутствует, создайте её вручную: File → New Scene → Save As → `Assets/Scenes/SampleScene.unity`
- Запустите генератор снова после создания сцены

## 📖 Полное руководство

Для детальной информации см. `SAMPLE_PROJECT_GUIDE.md`

---

**MVP Iteration 7.5** | Задача T040.3 ✅ | Обновлено: 2026-03-07
