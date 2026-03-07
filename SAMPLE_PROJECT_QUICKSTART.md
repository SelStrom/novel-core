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

### 3. Запустите тестирование

**Вариант A: Через Play Mode (простой способ)**

1. Выберите `Scene01_Introduction.asset` в Project window
2. Нажмите кнопку **Play** ▶️ в Unity
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
