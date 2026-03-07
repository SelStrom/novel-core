using UnityEngine;
using VContainer;
using VContainer.Unity;
using NovelCore.Runtime.Core.AssetManagement;
using NovelCore.Runtime.Core.AudioSystem;
using NovelCore.Runtime.Core.InputHandling;
using NovelCore.Runtime.Core.DialogueSystem;
using NovelCore.Runtime.Core.SceneManagement;
using NovelCore.Runtime.Core.Localization;
using NovelCore.Runtime.Animation;

namespace NovelCore.Runtime.Core
{

/// <summary>
/// Root lifetime scope for NovelCore.
/// Registers all core services for dependency injection.
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log("NovelCore: Initializing GameLifetimeScope...");

        // Register core services

        // Asset Management
        builder.Register<IAssetManager, AddressablesAssetManager>(Lifetime.Singleton);

        // Audio System
        builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);

        // Input Handling (MonoBehaviour - will be created as GameObject)
        builder.RegisterComponentOnNewGameObject<UnityInputService>(Lifetime.Singleton)
            .AsImplementedInterfaces();

        // Dialogue System
        builder.Register<IDialogueSystem, DialogueSystem.DialogueSystem>(Lifetime.Singleton);

        // Scene Management
        builder.Register<ISceneManager, SceneManagement.SceneManager>(Lifetime.Singleton);

        // Localization
        builder.Register<ILocalizationService, BasicLocalizationService>(Lifetime.Singleton);

        // Character Animation Factory
        builder.Register<ICharacterAnimatorFactory, CharacterAnimatorFactory>(Lifetime.Singleton);

        // Save System (to be implemented)
        // builder.Register<ISaveSystem, SaveSystem>(Lifetime.Singleton);

        // Platform Services (conditional registration based on build target)
        // RegisterPlatformService(builder);

        Debug.Log("NovelCore: GameLifetimeScope initialized successfully");
    }

    /// <summary>
    /// Registers the appropriate platform service based on the build target.
    /// </summary>
    private void RegisterPlatformService(IContainerBuilder builder)
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        // builder.Register<IPlatformService, SteamPlatformService>(Lifetime.Singleton);
        Debug.Log("NovelCore: Registered Steam platform service");
#elif UNITY_IOS
        // builder.Register<IPlatformService, iOSPlatformService>(Lifetime.Singleton);
        Debug.Log("NovelCore: Registered iOS platform service");
#elif UNITY_ANDROID
        // builder.Register<IPlatformService, AndroidPlatformService>(Lifetime.Singleton);
        Debug.Log("NovelCore: Registered Android platform service");
#else
        Debug.LogWarning("NovelCore: No platform service registered for current platform");
#endif
    }
}

}