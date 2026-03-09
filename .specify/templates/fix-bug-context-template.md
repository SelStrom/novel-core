# Bug Context Template

Используйте этот шаблон для документирования контекста бага в `.fix/bug_context/bug.md`.

---

# Bug Description

[Детальное описание бага - что идёт не так, когда происходит, влияние на пользователя]

## Expected Behavior

[Что ДОЛЖНО происходить согласно спецификации или ожиданиям пользователя]

## Actual Behavior

[Что ФАКТИЧЕСКИ происходит - конкретные симптомы, сообщения об ошибках, некорректное поведение]

## Steps to Reproduce

1. [Точный шаг 1 - действие пользователя или системное условие]
2. [Точный шаг 2]
3. [Точный шаг 3]
4. [Результат - проявление бага]

## Environment

- **Unity Version**: [e.g., 6.0.0f1]
- **Platform**: [Windows/macOS/iOS/Android]
- **NovelCore Version**: [e.g., 0.2.0]
- **Build Configuration**: [Debug/Release]
- **Additional Context**: [e.g., специфическая конфигурация проекта]

## Frequency

- [ ] Happens every time (100% reproducible)
- [ ] Happens sometimes ([X]% of the time)
- [ ] Happened once
- [ ] Cannot reproduce

## Severity

- [ ] Critical (blocker, system crash, data loss)
- [ ] High (major feature broken, workaround exists)
- [ ] Medium (feature partially broken, minor impact)
- [ ] Low (cosmetic, edge case, minor inconvenience)

## Related Information

- **Related Issue ID**: [if applicable]
- **Related User Story**: [if applicable]
- **Related Spec**: [path to spec.md if applicable]
- **Git Commit (if regression)**: [hash if known]

## Initial Hypothesis

[Опциональное поле - первичная гипотеза о root cause, если очевидна]

## Affected Components

[Список компонентов, которые предположительно затронуты]
- [ ] DialogueSystem
- [ ] SaveSystem
- [ ] SceneManager
- [ ] AssetManager
- [ ] InputHandler
- [ ] UISystem
- [ ] Other: [specify]
