using VContainer;
using VContainer.Unity;
using UnityEngine;
using Modules.SoundSystems;

public class GameplayLifetimeScope : LifetimeScope
{
    // References to UI Controllers
    [SerializeField] private GameplayUIController gameplayUIController;
    [SerializeField] private GameplayTeaBrewController gameplayTeaBrewController;

    protected override void Configure(IContainerBuilder builder)
    {
        // State Holder 
        builder.Register<ActiveCustomerProvider>(Lifetime.Singleton);

        // Gameplay Logic Services
        builder.Register<DialogueManager>(Lifetime.Singleton);
        builder.Register<TeaBrewingSystem>(Lifetime.Singleton);

        // UI Controller
        builder.RegisterComponent(gameplayUIController);
        builder.RegisterComponent(gameplayTeaBrewController);

        if (SoundSystem.Instance != null)
        {
            SoundSystem.Instance.PlayAudio("bgm_game");
            SoundSystem.Instance.GlobalMusicVolume = 0.3f;
        }
    }
}
