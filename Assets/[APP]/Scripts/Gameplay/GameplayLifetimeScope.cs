using VContainer;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // // State Holder 
        builder.Register<ActiveCustomerProvider>(Lifetime.Singleton);

        // // Gameplay Logic Services
        builder.Register<DialogueManager>(Lifetime.Singleton);
        builder.Register<TeaBrewingSystem>(Lifetime.Singleton);
    }
}
