using VContainer;
using VContainer.Unity;
using UnityEngine;
using Modules.SoundSystems;

public class HomeLifetimeScope : LifetimeScope
{
    [SerializeField] private HomeButtonUI homeButtonUIController;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(homeButtonUIController);
        if (SoundSystem.Instance != null)
        {
            SoundSystem.Instance.PlayAudio("bgm_home");
            SoundSystem.Instance.GlobalMusicVolume = 0.3f;
        }
    }
}
