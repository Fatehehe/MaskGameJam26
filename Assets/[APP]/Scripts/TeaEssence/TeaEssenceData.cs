using Modules;
using UnityEngine;


[CreateAssetMenu(fileName = "TeaEssenceData", menuName = "Data/Tea Essence Data")]
public class TeaEssenceData : ScriptableObject
{
    [Tooltip("Stable ID derived from the asset name. Rename the asset to change it.")]
    [SerializeField, ReadOnly] private string essenceId;
    public string EssenceId => string.IsNullOrEmpty(essenceId) ? name : essenceId;

    [Header("Display Properties")]
    [SerializeField] private string essenceName;
    public string EssenceName => essenceName;

    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;

    [SerializeField] private Sprite draggableIcon;
    public Sprite DraggableIcon => draggableIcon != null ? draggableIcon : icon;

#if UNITY_EDITOR
    private void OnValidate()
    {
        essenceId = name;
    }
#endif
}
