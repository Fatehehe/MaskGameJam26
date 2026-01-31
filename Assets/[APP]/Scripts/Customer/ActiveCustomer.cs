using System.Collections.Generic;

public class ActiveCustomer
{
    // Static Data (immutable)
    public MaskData ActiveMaskData { get; private set; }
    public MaskStoryData ActiveMaskStoryData { get; private set; }
    public CharacterVisualData ActiveCharacterVisual { get; private set; }
    public string CharacterName { get; private set; }

    // State Gameplay (mutable)
    public float CurrentCrackPoints { get; set; }
    public MaskState CurrentState { get; set; }
    public List<TeaEssenceData> TeaHistory { get; set; } = new List<TeaEssenceData>();
    public float CrackPercentage => (CurrentCrackPoints / ActiveMaskData.MaxCrackPoints) * 100f;

    public TeaEssenceData GetLastTea()
    {
        if (TeaHistory == null || TeaHistory.Count == 0) return null;
        return TeaHistory[TeaHistory.Count - 1];
    }

    // Constructor for "Factory"
    public ActiveCustomer(MaskData mask, MaskStoryData story, CharacterVisualData visual, string name)
    {
        ActiveMaskData = mask;
        ActiveMaskStoryData = story;
        ActiveCharacterVisual = visual;
        CurrentCrackPoints = 0;
        CurrentState = MaskState.Intact;
        CharacterName = name;
    }
}