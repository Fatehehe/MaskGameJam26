using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MixerTool : MonoBehaviour, IEssenceDropTarget
{
    [Header("UI")]
    [SerializeField] private Image slot1Icon;
    [SerializeField] private Image slot2Icon;

    private List<TeaEssenceData> essences = new List<TeaEssenceData>();

    public bool TryAccept(TeaEssenceData essence)
    {
        if (essence == null)
            return false;

        if (essences.Count >= 2)
        {
            Debug.Log("Mixer already has 2 essences!");
            return false;
        }

        TeaBrewEvents.OnRequestEssenceAdd?.Invoke(essence);
        return true;
    }

    private void AddEssence(TeaEssenceData data)
    {
        essences.Add(data);

        if (essences.Count == 1 && slot1Icon != null)
            slot1Icon.sprite = data.DraggableIcon;
        else if (essences.Count == 2 && slot2Icon != null)
            slot2Icon.sprite = data.DraggableIcon;
    }

    public void Mix()
    {
        if (essences.Count < 2)
        {
            Debug.Log("Need 2 essences to mix!");
            return;
        }

        ClearMixer();
    }

    public void ClearMixer()
    {
        essences.Clear();

        if (slot1Icon != null) slot1Icon.sprite = null;
        if (slot2Icon != null) slot2Icon.sprite = null;

        TeaBrewEvents.OnRequestPotClear?.Invoke();
    }
}
