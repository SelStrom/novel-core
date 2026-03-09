# Анализ MD файлов в корне проекта

**Дата**: 2026-03-09  
**Задача**: Интеграция временных MD файлов со spec-kit структурой

## Классификация файлов (48 файлов)

### 🔴 Категория 1: УДАЛИТЬ (дублируют spec-kit или устарели)

#### Implementation Reports (10 файлов)
- `IMPLEMENTATION_COMPLETE_PREVIEW_BRIDGE.md` - дублирует info в spec
- `IMPLEMENTATION_ITERATION_4.1.md` - временный отчет
- `IMPLEMENTATION_SCRIPTABLEOBJECT_FIX.md` - временный фикс
- `IMPLEMENTATION_SUMMARY.md` - дублирует tasks.md progress
- `IMPLEMENTATION_T040.3.md` - временная задача
- `IMPLEMENTATION_UI_MANAGER_FIX.md` - временный фикс
- `SCENE_TRANSITION_IMPLEMENTATION_COMPLETE.md` - дублирует spec

**Обоснование**: Эти файлы - промежуточные отчеты о выполнении задач. Информация есть в `tasks.md` и git history.

#### Status Reports (6 файлов)
- `SAMPLE_PROJECT_STATUS.md` - устарел
- `SAMPLE_PROJECT_READY.md` - устарел
- `COMPILATION_READY.md` - устарел
- `ANALYSIS_FIXES_APPLIED.md` - временный отчет
- `TEST_EXECUTION_RESULTS.md` - устарел (есть в testing-strategy)
- `TEST_RESULTS_FINAL.md` - устарел

**Обоснование**: Статус-репорты актуальны только на момент создания. Git commits содержат ту же информацию.

#### Troubleshooting Guides (9 файлов)
- `ADDRESSABLES_FIX.md` - можно смержить в research.md
- `UNITY_COMPILATION_ERRORS_FIX.md` - временный фикс
- `VCONTAINER_DI_FIX.md` - временный фикс
- `CHOICE_BUTTONS_DEBUG.md` - временный дебаг
- `NAVIGATION_BUTTONS_FIX.md` - временный фикс
- `NAVIGATION_BUTTONS_TROUBLESHOOTING.md` - временный фикс
- `NAVIGATION_FINAL_FIX.md` - временный фикс
- `NAVIGATION_FINAL_FIX_RU.md` - дубликат
- `NAVIGATION_FIXES_REPORT.md` - временный фикс
- `NAVIGATION_FIX_QUICK_RU.md` - дубликат
- `NAVIGATION_FIX_SUMMARY.md` - временный фикс
- `NAVIGATION_SUBSCRIPTION_AND_ZORDER_FIX.md` - временный фикс

**Обоснование**: Фиксы уже применены. Если проблемы повторятся, их можно восстановить из git history.

#### Duplicate Testing Instructions (3 файла)
- `TESTING_PREVIEW_BRIDGE.md` - дублирует testing-strategy
- `TEST_FIXES.md` - дублирует testing-strategy
- `NAVIGATION_QUICK_TEST.md` - дублирует testing-strategy

**Обоснование**: Информация есть в `.specify/memory/testing-strategy.md`.

**Итого категория 1**: **28 файлов** к удалению

---

### 🟡 Категория 2: СМЕРЖИТЬ (содержат ценную информацию)

#### Setup & Configuration (6 файлов)
- `ADDRESSABLES_SETUP.md` → Смержить в `specs/001-visual-novel-constructor/quickstart.md` (раздел "Addressables Setup")
- `PACKAGE_INSTALLATION.md` → Смержить в `quickstart.md` (раздел "Dependencies")
- `GAME_STARTER_SETUP.md` → Смержить в `quickstart.md` (раздел "GameStarter Configuration")
- `MANUAL_STEPS.md` → Смержить в `quickstart.md` (раздел "Manual Configuration")
- `CSHARP_VERSION.md` → Информация уже в `tech-stack.md` (проверить актуальность)
- `UNITY_START_INSTRUCTIONS.md` → Смержить в `user-manual-ru.md` (раздел "Первый запуск")

**Действия**:
- Извлечь ценные setup инструкции
- Добавить в соответствующие разделы quickstart/user-manual
- Убедиться, что нет дублирования

#### Sample Project Guides (6 файлов)
- `SAMPLE_PROJECT_QUICKSTART.md` → Смержить в `user-manual-ru.md` (раздел "Быстрый старт")
- `SAMPLE_PROJECT_GUIDE.md` → Смержить в `user-manual-ru.md` (раздел "Демо-проект")
- `SAMPLE_PROJECT_ONE_CLICK.md` → Смержить в `user-manual-ru.md`
- `SAMPLE_PROJECT_GENERATOR_UNIFIED.md` → Информация для `quickstart.md` (раздел "Sample Project Generator")
- `SAMPLE_PROJECT_SETUP.md` → Смержить в `quickstart.md`
- `SCENE_SELECTION_QUICKSTART.md` → Смержить в `user-manual-ru.md` (раздел "Выбор сцены")

**Действия**:
- Объединить все Sample Project инструкции в единый раздел user-manual
- Убедиться, что нет противоречий между версиями

#### Testing Instructions (2 файла)
- `TESTING_INSTRUCTIONS.md` → Смержить в `.specify/memory/testing-strategy.md` (раздел "Manual Testing")
- `SCENE_SELECTION_TESTING.md` → Смержить в `testing-strategy.md` (раздел "Feature Testing")

**Действия**:
- Добавить manual testing procedures в testing-strategy
- Добавить feature-specific testing steps

#### Architecture Analysis (2 файла)
- `ARCHITECTURE_ANALYSIS_PREVIEW.md` → Смержить в `specs/001-visual-novel-constructor/research.md`
- `ARCHITECTURE_SUMMARY_PREVIEW.md` → Смержить в `research.md`

**Действия**:
- Добавить архитектурные решения в research.md
- Обновить plan.md если есть важные изменения

#### Refactoring Reports (2 файла)
- `REFACTORING_REPORT.md` → **ОСТАВИТЬ** (это актуальный отчет о недавнем рефакторинге)
- `REFACTORING_LOCALIZATION_API.md` → Смержить в `research.md` (раздел "Localization Strategy")
- `REFACTORING_SAMPLE_PROJECT_GENERATOR.md` → Смержить в `quickstart.md`

**Действия**:
- REFACTORING_REPORT.md сохранить как есть (ценный документ)
- Остальные смержить в research/quickstart

#### Unity Troubleshooting (1 файл)
- `UNITY_TROUBLESHOOTING.md` → Смержить в `quickstart.md` (раздел "Troubleshooting")

**Действия**:
- Добавить common issues в quickstart

**Итого категория 2**: **19 файлов** к мержу

---

### 🟢 Категория 3: ОСТАВИТЬ (актуальные/критичные)

- `README.md` → **ОСТАВИТЬ И ОБНОВИТЬ** (главный entry point проекта)
  - Добавить ссылки на `.specify/memory/` структуру
  - Упростить, убрать дублирование
  - Четко обозначить spec-kit workflow

- `REFACTORING_REPORT.md` → **ОСТАВИТЬ** (актуальный отчет от 2026-03-09)

**Итого категория 3**: **2 файла** к сохранению (с обновлениями)

---

## План действий

### Фаза 1: Мерж ценной информации (Категория 2)

1. **Обновить `.specify/memory/testing-strategy.md`**:
   - Добавить manual testing procedures из TESTING_INSTRUCTIONS.md
   - Добавить feature testing из SCENE_SELECTION_TESTING.md
   - Добавить troubleshooting scenarios

2. **Обновить `specs/001-visual-novel-constructor/quickstart.md`**:
   - Раздел "Dependencies": из PACKAGE_INSTALLATION.md
   - Раздел "Addressables Setup": из ADDRESSABLES_SETUP.md
   - Раздел "GameStarter Configuration": из GAME_STARTER_SETUP.md
   - Раздел "Manual Configuration": из MANUAL_STEPS.md
   - Раздел "Troubleshooting": из UNITY_TROUBLESHOOTING.md
   - Раздел "Sample Project Generator": из SAMPLE_PROJECT_GENERATOR_UNIFIED.md

3. **Обновить `specs/001-visual-novel-constructor/user-manual-ru.md`**:
   - Раздел "Первый запуск": из UNITY_START_INSTRUCTIONS.md
   - Раздел "Быстрый старт": из SAMPLE_PROJECT_QUICKSTART.md
   - Раздел "Демо-проект": из SAMPLE_PROJECT_GUIDE.md, SAMPLE_PROJECT_ONE_CLICK.md
   - Раздел "Выбор сцены": из SCENE_SELECTION_QUICKSTART.md

4. **Обновить `specs/001-visual-novel-constructor/research.md`**:
   - Добавить architecture analysis из ARCHITECTURE_ANALYSIS_PREVIEW.md, ARCHITECTURE_SUMMARY_PREVIEW.md
   - Добавить localization strategy из REFACTORING_LOCALIZATION_API.md
   - Добавить addressables insights из ADDRESSABLES_FIX.md (если релевантно)

### Фаза 2: Обновить README.md

- Упростить структуру
- Добавить четкие ссылки на spec-kit документацию
- Убрать дублирование с quickstart/user-manual
- Добавить раздел "Documentation Structure" со ссылками на `.specify/memory/`

### Фаза 3: Удалить устаревшие файлы (Категория 1)

Удалить 28 файлов:
- 10 implementation reports
- 6 status reports
- 9 troubleshooting guides
- 3 duplicate testing instructions

### Фаза 4: Верификация

1. Убедиться, что вся ценная информация сохранена
2. Проверить, что ссылки между документами работают
3. Прогнать тесты (если есть)
4. Создать коммит с изменениями

---

## Метрики

- **Всего файлов**: 48
- **К удалению**: 28 (58%)
- **К мержу**: 19 (40%)
- **К сохранению**: 2 (4%) (README + REFACTORING_REPORT)

**Ожидаемый результат**: 
- Централизованная документация в spec-kit структуре
- README.md как единая точка входа
- Все временные файлы удалены
- История сохранена в git

---

## Решения по спорным файлам

### REFACTORING_REPORT.md
**Решение**: ОСТАВИТЬ

**Обоснование**: 
- Создан недавно (2026-03-09)
- Содержит актуальный анализ рефакторинга документации
- Описывает создание `.specify/memory/` структуры
- Полезен для понимания эволюции проекта
- Можно переместить в `.specify/memory/refactoring-report-2026-03-09.md` для истории

### CSHARP_VERSION.md
**Решение**: Проверить, затем удалить

**Обоснование**:
- Информация должна быть в `tech-stack.md`
- Если нет - добавить перед удалением

### Sample Project Guides (6 файлов)
**Решение**: Смержить в один раздел user-manual

**Обоснование**:
- Множество дублирующихся инструкций
- Создать единый authoritative guide
- Убрать противоречия между версиями

---

## Следующий шаг

Начать с Фазы 1: мержить ценную информацию в spec-kit структуру.

**Рекомендуемый порядок**:
1. testing-strategy.md (добавить manual testing)
2. quickstart.md (добавить setup инструкции)
3. user-manual-ru.md (добавить user-facing guides)
4. research.md (добавить architecture notes)
5. README.md (упростить и добавить ссылки)
6. Удалить устаревшие файлы
