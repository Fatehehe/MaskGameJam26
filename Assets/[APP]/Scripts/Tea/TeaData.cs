using System.Collections.Generic;
using Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "Tea Data", menuName = "Data/Tea Data")]
public class TeaData : ScriptableObject
{
    [Tooltip("Stable ID derived from the asset name. Rename the asset to change it.")]
    [SerializeField, ReadOnly] private string teaId;
    public string TeaId => string.IsNullOrEmpty(teaId) ? name : teaId;

    [Header("Display Properties")]
    [SerializeField] private string teaName;
    public string TeaName => teaName;
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;
    [SerializeField] private string description;
    public string Description => description;

    [Header("Gameplay Properties")]
    [SerializeField] private List<TeaEssenceData> requiredEssences = new List<TeaEssenceData>();
    public List<TeaEssenceData> RequiredEssences => requiredEssences;
    [SerializeField] private EmotionType emotionType;
    public EmotionType EmotionType => emotionType;
    [SerializeField, Range(0f, 100f)] private float crackImpact;
    public float CrackImpact => crackImpact;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        teaId = name;
    }
#endif
}
