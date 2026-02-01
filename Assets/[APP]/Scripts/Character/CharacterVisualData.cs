using System.Collections.Generic;
using Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterVisualData", menuName = "Data/Character Visual Data")]
public class CharacterVisualData : ScriptableObject
{
    [SerializeField, ReadOnly] private string visualId;
    public string VisualId => string.IsNullOrEmpty(visualId) ? name : visualId;

    [Header("Visual Assets")]
    [SerializeField] private Sprite bodySprite;
    public Sprite BodySprite => bodySprite;

    [Header("Identity")]
    [SerializeField] private GenderType gender;
    public GenderType Gender => gender;

    [SerializeField] private List<string> possibleNames;
    public List<string> PossibleNames => possibleNames;

    public string GetRandomName()
    {
        if (possibleNames == null || possibleNames.Count == 0) return "Unknown Soul";
        return possibleNames[UnityEngine.Random.Range(0, possibleNames.Count)];
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        visualId = name;
    }
#endif
}

public enum GenderType { Male, Female, NonBinary, Unknown }
