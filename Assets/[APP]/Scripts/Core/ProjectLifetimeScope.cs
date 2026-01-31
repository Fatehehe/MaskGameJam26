using VContainer;
using VContainer.Unity;
using UnityEngine;
using Modules.SoundSystems;

public class ProjectLifetimeScope : LifetimeScope
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] protected GameObject loadingPrefab;
    [SerializeField] protected MaskDatabase maskDatabase;
    [SerializeField] protected TeaDatabase teaDatabase;
    [SerializeField] protected TeaEssenceDatabase teaEssenceDatabase;
    [SerializeField] protected CharacterVisualDatabase characterVisualDatabase;

    protected override void Configure(IContainerBuilder builder)
    {
        // Databases
        builder.RegisterInstance(maskDatabase);
        builder.RegisterInstance(teaDatabase);
        builder.RegisterInstance(teaEssenceDatabase);
        builder.RegisterInstance(characterVisualDatabase);

        // Global Services
        Instantiate(soundSystem, transform);
        builder.RegisterComponentInHierarchy<SoundSystem>().AsSelf();

        builder.RegisterEntryPoint<ProjectSavingSystem>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<PlayerInputSystem>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<LoadingService>(Lifetime.Singleton).AsSelf().WithParameter(loadingPrefab);

        // Factory
        builder.Register<CustomerFactory>(Lifetime.Singleton);
    }
}
