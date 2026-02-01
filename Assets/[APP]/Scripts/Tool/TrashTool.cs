using UnityEngine;

public class TrashTool : MonoBehaviour, IEssenceDropTarget
{
    [Header("Settings")]
    [SerializeField] private string resetSoundName = "TrashDump"; // Opsional buat SFX

    public void DisposeItem(MonoBehaviour tool)
    {
        if (tool is MixerTool mixer)
        {
            Debug.Log("Trash: Mengosongkan Mixer...");
            mixer.ResetMixerTotal(); // Panggil fungsi reset mixer
            // PlaySFX(resetSoundName);
        }
        else if (tool is GlassTool glass)
        {
            Debug.Log("Trash: Mengosongkan Gelas...");
            glass.ResetGlass(); // Panggil fungsi reset gelas
            // PlaySFX(resetSoundName);
        }
    }

    public bool TryAccept(TeaEssenceData draggable)
    {
        return true; 
    }
}