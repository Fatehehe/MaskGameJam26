using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private GameplayUIController gameplayUIController;
    protected override void Configure(IContainerBuilder builder)
    {
        // State Holder 
        builder.Register<ActiveCustomerProvider>(Lifetime.Singleton);

        // Gameplay Logic Services
        builder.Register<DialogueManager>(Lifetime.Singleton);
        builder.Register<TeaBrewingSystem>(Lifetime.Singleton);

        // UI Controller
        builder.RegisterComponent(gameplayUIController);
    }
}
