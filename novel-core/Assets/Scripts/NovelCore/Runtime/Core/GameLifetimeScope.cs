using VContainer;
using VContainer.Unity;

namespace NovelCore.Runtime.Core;

/// <summary>
/// Root lifetime scope for NovelCore.
/// Registers all core services for dependency injection.
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log("NovelCore: Initializing GameLifetimeScope...");

        // Register core services (placeholder registrations for now)
        // These will be updated as we implement each service

        // Asset Management
        // builder.Register<IAssetManager, AddressablesAssetManager>(Lifetime.Singleton);

        // Audio System
        // builder.Register<IAudioService, UnityAudioService>(Lifetime.Singleton);

        // Input Handling
        // builder.Register<IInputService, UnityInputService>(Lifetime.Singleton);

        // Dialogue System
        // builder.Register<IDialogueSystem, DialogueSystem>(Lifetime.Singleton);

        // Scene Management
        // builder.Register<ISceneManager, SceneManager>(Lifetime.Singleton);

        // Save System
        // builder.Register<ISaveSystem, SaveSystem>(Lifetime.Singleton);

        // Localization
        // builder.Register<ILocalizationService, UnityLocalizationService>(Lifetime.Singleton);

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
