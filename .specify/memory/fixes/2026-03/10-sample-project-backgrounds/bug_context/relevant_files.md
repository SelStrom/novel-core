# Relevant Files

## Primary Files (directly affected)

- `novel-core/Assets/Scripts/NovelCore/Runtime/Core/GameLifetimeScope.cs`
  - Location: `NovelCore.Runtime.Core.GameLifetimeScope`
  - Method: `Configure()` (line 33)
  - Issue: `builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton)` — неправильная регистрация MonoBehaviour
  - Reason: UnityAudioService — это MonoBehaviour, который требует RegisterComponentOnNewGameObject

- `novel-core/Assets/Scripts/NovelCore/Runtime/Core/AudioSystem/UnityAudioService.cs`
  - Location: `NovelCore.Runtime.Core.AudioSystem.UnityAudioService`
  - Line: 13 - `public class UnityAudioService : MonoBehaviour, IAudioService`
  - Reason: MonoBehaviour не может быть создан через обычный constructor

## Secondary Files (dependencies)

- `novel-core/Assets/Scripts/NovelCore/Runtime/Core/SceneManagement/SceneManager.cs`
  - Constructor: принимает `IAudioService` как dependency
  - Impact: получает `null` вместо валидного UnityAudioService

- `novel-core/Assets/Scripts/NovelCore/Runtime/Core/DialogueSystem/DialogueSystem.cs`
  - Constructor: принимает `IAudioService` как dependency
  - Impact: получает `null` вместо валидного UnityAudioService

## Working Example (for comparison)

- `novel-core/Assets/Scripts/NovelCore/Runtime/Core/InputHandling/UnityInputService.cs`
  - Registration: `builder.RegisterComponentOnNewGameObject<UnityInputService>(Lifetime.Singleton).AsImplementedInterfaces();`
  - Reason: Это правильный паттерн для регистрации MonoBehaviour в VContainer
