using Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "TeaData", menuName = "Data/Tea Data")]
public class TeaData : ScriptableObject
{
    [Tooltip("Stable ID derived from the asset name. Rename the asset to change it.")]
    [SerializeField, ReadOnly] private string teaId;
    public string TeaId => string.IsNullOrEmpty(teaId) ? name : teaId;

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

#if UNITY_EDITOR
    private void OnValidate()
    {
        teaId = name;
    }
#endif
}
