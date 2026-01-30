using System.Collections.Generic;
using Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "MaskData", menuName = "Data/Mask Data")]
public class MaskData : ScriptableObject
{
    [Tooltip("Stable ID derived from the asset name. Rename the asset to change it.")]
    [SerializeField, ReadOnly] private string maskId;
    public string MaskId => string.IsNullOrEmpty(maskId) ? name : maskId;

    [Header("Display Properties")]
    [SerializeField] private string maskName;
    public string MaskName => maskName;

    [Header("Gameplay Properties")]
    [SerializeField] private float maxCrackPoints = 100f;
    public float MaxCrackPoints => maxCrackPoints;

    [SerializeField] private EmotionType emotionType;
    public EmotionType EmotionType => emotionType;

    [SerializeField] private List<MaskStoryData> possibleStories = new List<MaskStoryData>();
    public List<MaskStoryData> PossibleStories => possibleStories;

    [SerializeField] private Sprite normalMaskSprite;
    public Sprite NormalMaskSprite => normalMaskSprite;

    [SerializeField] private Sprite crackedMaskSprite;
    public Sprite CrackedMaskSprite => crackedMaskSprite;

    [SerializeField] private Sprite brokenMaskSprite;
    public Sprite BrokenMaskSprite => brokenMaskSprite;

#if UNITY_EDITOR
    private void OnValidate()
    {
        maskId = name;
    }
#endif
}