using System.Collections.Generic;
using Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "MaskStoryData", menuName = "Data/Mask Story Data")]
public class MaskStoryData : ScriptableObject
{
    [Tooltip("Stable ID derived from the asset name. Rename the asset to change it.")]
    [SerializeField, ReadOnly] private string storyId;
    public string StoryId => string.IsNullOrEmpty(storyId) ? name : storyId;

    [Header("Dialogue Nodes")]
    [SerializeField] private List<DialogueNode> dialogueNodes = new List<DialogueNode>();
    public List<DialogueNode> DialogueNodes => dialogueNodes;

#if UNITY_EDITOR
    private void OnValidate()
    {
        storyId = name;
    }
#endif
}

[System.Serializable]
public class DialogueNode
{
    [SerializeField, Range(0f, 100f)] private float minCrackPercentage = 0f;
    public float MinCrackPercentage => minCrackPercentage;
    [SerializeField] private TeaData specificTeaReaction;
    public TeaData SpecificTeaReaction => specificTeaReaction;

    [Header("Content")]
    [SerializeField]
    [TextArea(3, 10)] private List<string> lines = new List<string>();
    public string GetRandomLine()
    {
        if (lines == null || lines.Count == 0)
            return string.Empty;
        int idx = Random.Range(0, lines.Count);
        return lines[idx];
    }

    [SerializeField] private bool endsConversation;
    public bool EndsConversation => endsConversation;
}