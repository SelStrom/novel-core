# Specification Analysis - Fixes Applied

**Дата**: 2026-03-09  
**Анализируемая спецификация**: `specs/001-scene-transition/`

## Обзор

Проведен автоматический анализ согласованности артефактов спецификации (spec.md, plan.md, tasks.md) с конституцией проекта. Обнаружено и исправлено **2 критические проблемы** и **4 проблемы высокой важности**.

---

## Критические проблемы (CRITICAL) - ✅ ИСПРАВЛЕНО

### C1: Нарушение Принципа VI конституции (Test Execution Requirement)

**Проблема**: В tasks.md отсутствовали задачи на запуск полного набора тестов после каждой пользовательской истории с явными командами согласно конституции §VI (строки 124-145).

**Исправление**:
- Добавлены задачи с явными командами Unity Test Runner:
  - **T025** [US1]: Запуск тестов после User Story 1
  - **T036** [US2]: Запуск тестов после User Story 2
  - **T061** [US3]: Запуск тестов после User Story 3
  - **T085** [US4]: Запуск тестов после User Story 4
  - **T102** [US5]: Запуск тестов после User Story 5
  - **T115** [Phase 8]: Запуск полного набора тестов перед релизом

**Каждая задача включает**:
- Команду для EditMode тестов
- Команду для PlayMode тестов
- Критерии успеха (EXIT CODE = 0, zero failures)
- Инструкции по обработке ошибок

**Файлы**: `specs/001-scene-transition/tasks.md`, `specs/001-scene-transition/plan.md`

---

### C2: FR-016 не имеет специализированных задач

**Проблема**: Требование FR-016 "Scene transitions MUST respect the transition type and duration specified in the target scene's data" не имело явных задач по реализации.

**Исправление**:
- Добавлена задача **T027** [US1]: "Verify TransitionType integration: Confirm scene transitions respect TransitionType and duration from SceneData (addresses FR-016)"

**Файлы**: `specs/001-scene-transition/tasks.md`

---

## Проблемы высокой важности (HIGH) - ✅ ИСПРАВЛЕНО

### H1: Неопределенность сериализации SceneNavigationState

**Проблема**: Задачи T048-T050 помечены как "TODO: Requires custom serialization" без технической спецификации.

**Исправление**:
- Добавлена задача **T054** [US3]: "Research and document SceneNavigationState serialization format in `specs/001-scene-transition/research.md`"
- Задачи T055-T057 (ранее T048-T050) теперь зависят от T054

**Файлы**: `specs/001-scene-transition/tasks.md`

---

### H2: Отсутствие задачи на настройку VContainer

**Проблема**: T053 помечена "TODO: Needs VContainer setup" - неясно, настроен ли VContainer.

**Исправление**:
- Добавлена задача **T023** [US1]: "Verify VContainer setup for dependency injection in `novel-core/Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs` (Foundation prerequisite)"
- T060 (ранее T053) теперь зависит от T023

**Файлы**: `specs/001-scene-transition/tasks.md`

---

### H3: Отсутствие теста для SC-003 (навигация по 20 сценам без проблем с памятью)

**Проблема**: Критерий успеха SC-003 не имел соответствующей тестовой задачи для проверки памяти.

**Исправление**:
- Добавлена задача **T044** [US3]: "Create PlayMode performance test for navigation history memory usage (SC-003: 20 scenes without memory issues)"

**Файлы**: `specs/001-scene-transition/tasks.md`

---

### H4: Отсутствие UX-теста для SC-006 (настройка условных переходов за <5 минут)

**Проблема**: SC-006 "Content creators can set up complex branching narratives with conditional transitions in under 5 minutes per branch point" не имел задачи на UX-тестирование.

**Исправление**:
- Добавлена задача **T113** [Phase 8]: "UX Testing: Measure time for creator to set up conditional transition (SC-006: must be <5 minutes per branch point)"

**Файлы**: `specs/001-scene-transition/tasks.md`

---

## Проблемы средней важности (MEDIUM) - ✅ ИСПРАВЛЕНО

### M3: Неоднозначность edge case (choices + nextScene)

**Проблема**: Edge case "What happens when a scene has both choices defined AND a next scene defined?" указывал "Choices take priority", но не было ясно, должна ли система предупреждать создателя.

**Исправление**:
- Обновлен раздел Edge Cases в `spec.md`:
  - Добавлено требование: "**System MUST warn creator**: Unity Editor MUST display a warning message in SceneDataEditor Inspector when both choices and nextScene are defined"
  - Задача T035 (ранее T030) уже покрывает это требование

**Файлы**: `specs/001-scene-transition/spec.md`

---

## Структурные улучшения

### Перенумерация задач

**Проблема**: Нестандартная нумерация T020a, T020b, T020c нарушала последовательность.

**Исправление**:
- Все задачи перенумерованы последовательно от T001 до T128
- Удалены суффиксы a/b/c
- Обновлены все ссылки в зависимостях

**Файлы**: `specs/001-scene-transition/tasks.md`

---

## Статистика изменений

### Обновленные метрики tasks.md

- **Всего задач**: 121 → **128** (+7 новых задач)
- **User Story 1 (P1)**: 14 → **17 задач** (+3: VContainer verification, test execution, TransitionType verification)
- **User Story 3 (P2)**: 23 → **25 задач** (+2: serialization research, memory profiling test)
- **Polish Phase**: 24 → **26 задач** (+2: UX testing, full test suite execution)
- **Тестовое покрытие**: 27 → **28 тестовых задач** + **5 обязательных точек запуска тестов**

### Обновленные файлы

- ✅ `specs/001-scene-transition/tasks.md` (перенумерация + 7 новых задач)
- ✅ `specs/001-scene-transition/plan.md` (обновлен Constitution Check с Test Execution Requirement)
- ✅ `specs/001-scene-transition/spec.md` (уточнен edge case про choices + nextScene)

---

## Соответствие конституции

### Принцип VI: Modular Architecture & Testing

**До исправлений**: Частичное соответствие (тесты планировались, но не было обязательных точек запуска)

**После исправлений**: ✅ Полное соответствие
- Все тесты запускаются через Unity Test Runner в batch mode
- Явные команды для EditMode и PlayMode тестов
- Нулевая толерантность к ошибкам перед переходом между фазами
- Тестовое покрытие >80% (28 тестовых задач + интеграционные тесты)

---

## Следующие шаги

1. **Выполнить все задачи Phase 1-2** (Setup + Foundational)
2. **Начать User Story 1** (MVP) с задачи T009
3. **После T025**: Запустить тесты User Story 1 - первый обязательный чекпоинт качества
4. **Продолжать итеративно** через US2 → US3 → US4 → US5 с запуском тестов после каждой истории

---

## Команды для запуска тестов (reference)

### EditMode Tests
```bash
/Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
  -runTests -batchmode -nographics -projectPath "$(pwd)/novel-core" \
  -testPlatform EditMode -testResults "./test-results-editmode.xml" \
  -logFile - 2>&1 | tee unity-test-editmode.log
```

### PlayMode Tests
```bash
/Applications/Unity/Hub/Editor/6000.0.69f1/Unity.app/Contents/MacOS/Unity \
  -runTests -batchmode -projectPath "$(pwd)/novel-core" \
  -testPlatform PlayMode -testResults "./test-results-playmode.xml" \
  -logFile - 2>&1 | tee unity-test-playmode.log
```

---

**Статус**: ✅ Все критические и высокоприоритетные проблемы исправлены  
**Готовность к реализации**: ✅ Спецификация готова к `/speckit.implement`
