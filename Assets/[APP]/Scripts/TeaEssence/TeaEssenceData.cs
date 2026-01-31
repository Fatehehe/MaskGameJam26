using Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "TeaEssenceData", menuName = "Data/Tea Essence Data")]
public class TeaEssenceData : ScriptableObject
{
    [Tooltip("Stable ID derived from the asset name. Rename the asset to change it.")]
    [SerializeField, ReadOnly] private string teaEssenceId;
    public string TeaEssenceId => string.IsNullOrEmpty(teaEssenceId) ? name : teaEssenceId;

    [Header("Display Properties")]
    [SerializeField] private string teaName;
    public string TeaName => teaName;

    [SerializeField] private Sprite teaSprite;
    public Sprite TeaSprite => teaSprite;

    [Header("Gameplay Properties")]
    [SerializeField] private EmotionType emotionType;
    public EmotionType EmotionType => emotionType;
    [SerializeField] private float crackImpact = 10f;
    public float CrackImpact => crackImpact;
    [SerializeField] private float brewTemp = 50f;
    public float BrewTemp => brewTemp;
#if UNITY_EDITOR
    private void OnValidate()
    {
        teaEssenceId = name;
    }
#endif
}
