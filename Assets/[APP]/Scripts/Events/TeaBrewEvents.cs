using System;
using UnityEngine;

public static class TeaBrewEvents
{
    public static Action<TeaEssenceData> OnRequestEssenceAdd;
    public static Action OnRequestPotClear;
    public static Action<bool> OnLevelUpPanelShown;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        OnLevelUpPanelShown = null;
        OnRequestEssenceAdd = null;
    }
}
