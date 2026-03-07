# Руководство по Sample Project

## 🎯 Что это?

Sample Project - это готовый демонстрационный сюжет для тестирования visual novel конструктора. Он содержит:

- 4 сцены с диалогами
- Точку выбора с ветвлением сюжета
- 2 разные концовки
- Тестовые фоны и персонажей

## 🚀 Как создать Sample Project

### Шаг 1: Откройте Unity Editor

Убедитесь, что проект `novel-core` открыт в Unity.

### Шаг 2: Запустите генератор

В меню Unity выберите:

```
NovelCore → Generate Sample Project
```

Генератор создаст:
- Папку `Assets/Content/Projects/Sample/`
- 4 сцены (SceneData assets)
- Placeholder текстуры для фонов
- Placeholder спрайты персонажей
- Связи между сценами через выборы

### Шаг 3: Дождитесь завершения

Появится диалоговое окно с подтверждением создания. В консоли Unity вы увидите логи:

```
[SampleProjectGenerator] Created placeholder texture: ...
[SampleProjectGenerator] Created scene: ...
[SampleProjectGenerator] ✅ Successfully created sample project
```

## 📖 Структура Sample Project

```
Assets/Content/Projects/Sample/
├── Scenes/
│   ├── Scene01_Introduction.asset      # Вступление (3 реплики)
│   ├── Scene02_ChoicePoint.asset       # Точка выбора (2 реплики + выбор)
│   ├── Scene03a_PathA.asset            # Путь A: Прогулка (3 реплики)
│   ├── Scene03b_PathB.asset            # Путь B: Дома (3 реплики)
│   └── Choice_MainDecision.asset       # Данные выбора
├── Backgrounds/
│   ├── bg_room.png                     # Бежевый фон (комната)
│   ├── bg_street.png                   # Голубой фон (улица)
│   └── bg_home.png                     # Зелёный фон (дом)
└── Characters/
    └── char_protagonist.png            # Placeholder персонаж
```

## 🎮 Как протестировать

### Вариант 1: Через Scene Editor (рекомендуется)

1. Откройте `Window → NovelCore → Scene Editor` (если окно есть)
2. В Project window выберите `Scene01_Introduction.asset`
3. Нажмите кнопку **"Preview Scene"** в Scene Editor
4. Unity войдёт в Play Mode
5. Кликайте для продвижения диалога
6. Сделайте выбор на сцене 2

### Вариант 2: Прямой запуск Play Mode

1. В Project window выберите `Scene01_Introduction.asset`
2. Убедитесь, что в сцене есть GameObject с `GameLifetimeScope` (VContainer)
3. Нажмите **Play** в Unity
4. Кликайте для продвижения диалога

## 📝 Сюжет Sample Project

### Сцена 1: Введение
```
Реплика 1: "Привет! Это демонстрационная визуальная новелла."
Реплика 2: "Сейчас ты увидишь, как работает система диалогов и выборов."
Реплика 3: "Нажимай на экран, чтобы продолжить..."
```
→ Автоматический переход к Сцене 2

### Сцена 2: Точка выбора
```
Реплика 1: "Пришло время сделать выбор."
Реплика 2: "Куда ты хочешь пойти?"

ВЫБОР:
  [A] Выйти на улицу → Сцена 3a
  [B] Остаться дома  → Сцена 3b
```

### Сцена 3a: Путь A (Прогулка)
```
Фон: Улица (голубой)
Реплика 1: "Ты решил выйти на улицу."
Реплика 2: "Свежий воздух и солнечный день подняли твоё настроение!"
Реплика 3: "[Счастливая концовка - Path A]"
```

### Сцена 3b: Путь B (Дома)
```
Фон: Дом (зелёный)
Реплика 1: "Ты решил остаться дома."
Реплика 2: "Уютный вечер с книгой - тоже неплохой вариант."
Реплика 3: "[Нейтральная концовка - Path B]"
```

## ⚠️ Известные ограничения

### 1. Addressables не настроены

**Проблема**: Текстуры и сцены созданы, но не добавлены в Addressables groups.

**Решение**: 
- Выполните инструкции из `ADDRESSABLES_SETUP.md`
- Добавьте папку `Assets/Content/Projects/Sample/` в Addressables group
- Или дождитесь выполнения задачи **T009**

### 2. Выборы не связаны с целевыми сценами

**Проблема**: В `Choice_MainDecision.asset` поле `targetScene` для опций пустое (требует Addressables).

**Временное решение**:
- Откройте `Choice_MainDecision.asset` в Inspector
- Вручную укажите `targetScene` для каждой опции после настройки Addressables

### 3. Placeholder ассеты

**Примечание**: Фоны и персонажи - это простые цветные прямоугольники для тестирования.

**Для улучшения**:
- Замените текстуры в `Assets/Content/Projects/Sample/Backgrounds/`
- Замените спрайты в `Assets/Content/Projects/Sample/Characters/`
- SceneData автоматически подхватит новые ассеты

## 🔧 Кастомизация

### Изменить текст диалогов

1. Откройте нужный SceneData asset (например, `Scene01_Introduction.asset`)
2. В Inspector разверните секцию **Narrative Content → Dialogue Lines**
3. Измените поле **Fallback Text** для каждой реплики
4. Сохраните (Ctrl+S / Cmd+S)

### Добавить новые реплики

1. Откройте SceneData в Inspector
2. В секции **Dialogue Lines** нажмите **+**
3. Заполните поля:
   - **Line Id**: уникальный ID (например, `line_intro_004`)
   - **Fallback Text**: текст реплики
   - **Emotion**: эмоция персонажа (по умолчанию `neutral`)

### Изменить выборы

1. Откройте `Choice_MainDecision.asset`
2. В секции **Options** измените:
   - **Fallback Text**: текст кнопки
   - **Target Scene**: целевая сцена (после настройки Addressables)

### Добавить новые сцены

1. Правой кнопкой в Project → `Create → NovelCore → Scene Data`
2. Заполните поля аналогично существующим сценам
3. Добавьте ссылку на новую сцену в выборе предыдущей сцены

## 🎨 Замена placeholder ассетов

### Фоны

Требования:
- Формат: PNG или JPG
- Рекомендуемый размер: 1920x1080 (Full HD)
- Aspect ratio: 16:9

Замена:
1. Перетащите новую текстуру в `Assets/Content/Projects/Sample/Backgrounds/`
2. Откройте SceneData в Inspector
3. В поле **Background Image** выберите новую текстуру (если Addressables настроены)

### Персонажи

Требования:
- Формат: PNG с прозрачностью
- Рекомендуемый размер: 1080x1920 (вертикальный)
- Прозрачный фон обязателен

Замена:
1. Перетащите новый спрайт в `Assets/Content/Projects/Sample/Characters/`
2. Откройте SceneData в Inspector
3. В секции **Characters** добавьте CharacterPlacement
4. Укажите character sprite и позицию (0-1 normalized)

## 📚 Дальнейшие шаги

После тестирования Sample Project:

1. **Создайте свой проект**:
   - Скопируйте структуру `Sample/` в новую папку
   - Или используйте `NovelCore → New Project` (когда будет реализовано)

2. **Настройте Addressables** (T009):
   - Следуйте `ADDRESSABLES_SETUP.md`
   - Добавьте ваши папки в Addressables groups

3. **Изучите User Manual**:
   - `specs/001-visual-novel-constructor/user-manual.md`
   - Полное руководство для создателей контента

4. **Продолжите разработку**:
   - Следующие итерации добавят Character Editor, Audio, Transitions
   - См. `specs/001-visual-novel-constructor/tasks.md`

## 🐛 Решение проблем

### "Сцены не загружаются в Play Mode"

**Причина**: Addressables не настроены, SceneManager не может загрузить ассеты.

**Решение**: 
- Выполните настройку Addressables (см. `ADDRESSABLES_SETUP.md`)
- Или дождитесь выполнения T009

### "Диалоги не отображаются"

**Причина**: DialogueBox prefab не создан или не подключен.

**Решение**:
```
NovelCore → Generate UI Prefabs → Dialogue Box
```

### "Выборы не появляются"

**Причина**: ChoiceButton prefab не создан или targetScene не указан.

**Решение**:
1. Создайте prefab: `NovelCore → Generate UI Prefabs → Choice Button`
2. Укажите targetScene в Choice_MainDecision.asset вручную

### "Фоны не отображаются"

**Причина**: Addressables не может загрузить текстуры.

**Временное решение**:
- Используйте прямые ссылки вместо AssetReference (требует изменения кода)
- Или настройте Addressables

## ✅ Чеклист готовности

Перед тестированием убедитесь:

- [ ] Unity Editor открыт с проектом `novel-core`
- [ ] Sample Project создан (`NovelCore → Generate Sample Project`)
- [ ] DialogueBox prefab существует (`Assets/Resources/NovelCore/UI/DialogueBox.prefab`)
- [ ] ChoiceButton prefab существует (`Assets/Resources/NovelCore/UI/ChoiceButton.prefab`)
- [ ] В сцене есть GameLifetimeScope (VContainer)
- [ ] Addressables настроены (опционально, но рекомендуется)

## 📞 Поддержка

Если Sample Project не работает:

1. Проверьте Unity Console на ошибки
2. Убедитесь, что все задачи Iterations 0-9 выполнены
3. Проверьте `MANUAL_STEPS.md` для оставшихся ручных настроек
4. Изучите логи предыдущих компиляций (`unity_*.log`)

---

**Версия**: 1.0.0 (MVP Iteration 7)  
**Последнее обновление**: 2026-03-07  
**Задача**: T040 - Create sample SceneData asset for testing
