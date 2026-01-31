using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public interface IDropHandlerTarget
{
    bool TryAccept(IDraggableData essence);
}

public class MixerTool : MonoBehaviour, IDropHandlerTarget
{
    [Header("UI")]
    [SerializeField] private Image slot1Icon;
    [SerializeField] private Image slot2Icon;
    private TeaBrewingSystem teaSystem;

    private List<IDraggableData> essences = new List<IDraggableData>(2);

    private void Start()
    {
        // teaSystem.OnInteractableStateChanged +=
    }

    public bool TryAccept(IDraggableData essence)
    {
        if (essence == null)
            return false;

        if (essences.Count >= 2)
        {
            Debug.Log("Mixer already has 2 essences!");
            return false;
        }

        AddEssence(essence);
        return true; // object tidak dihancurkan
    }

    private void AddEssence(IDraggableData data)
    {
        essences.Add(data);

        if (essences.Count == 1 && slot1Icon != null)
            slot1Icon.sprite = data.DraggableIcon;
        else if (essences.Count == 2 && slot2Icon != null)
            slot2Icon.sprite = data.DraggableIcon;

        // Debug.Log($"Added essence: {data.EssenceName}");
    }

    public void Mix()
    {
        if (essences.Count < 2)
        {
            Debug.Log("Need 2 essences to mix!");
            return;
        }

        // Debug.Log($"Mixing {essences[0].EssenceName} + {essences[1].EssenceName}");

        // TODO: cek resep di sini

        ClearMixer();
    }

    public void ClearMixer()
    {
        essences.Clear();

        if (slot1Icon != null) slot1Icon.sprite = null;
        if (slot2Icon != null) slot2Icon.sprite = null;
    }
}
