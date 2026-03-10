# Bug Description

При инжекте зависимостей через VContainer объект `UnityAudioService` равен `null`, что приводит к `NullReferenceException` при попытке использования аудио системы.

## Expected Behavior

1. `UnityAudioService` должен быть зарегистрирован в `GameLifetimeScope` как Singleton
2. При инжекте в другие сервисы (например, `SceneManager`, `DialogueSystem`) должен быть валидный экземпляр
3. Метод `IAudioService` должен быть доступен для вызова без `NullReferenceException`

## Actual Behavior

1. `UnityAudioService` регистрируется в VContainer
2. При попытке инжекта в другие сервисы получается `null`
3. При вызове методов аудио сервиса возникает `NullReferenceException`

## Steps to Reproduce

1. Запустить игру (Play Mode)
2. VContainer инициализирует `GameLifetimeScope`
3. Сервисы инжектируются в конструкторы (например, `SceneManager`)
4. При попытке использовать `_audioService` → `NullReferenceException`

## Environment

- Unity Version: 6000.0.69f1
- Platform: macOS
- NovelCore Version: dev (current)
- VContainer Version: 1.14+

## Impact

- Критический баг: аудио система не функционирует
- Затрагивает все системы, зависящие от `IAudioService` (SceneManager, DialogueSystem)
- Делает невозможным воспроизведение музыки и звуковых эффектов
