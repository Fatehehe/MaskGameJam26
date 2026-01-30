public class CustomerFactory
{
    private readonly MaskDatabase maskDatabase;
    private readonly CharacterVisualDatabase characterVisualDatabase;

    public CustomerFactory(MaskDatabase maskDB, CharacterVisualDatabase charVisualDB)
    {
        maskDatabase = maskDB;
        characterVisualDatabase = charVisualDB;
    }

    public ActiveCustomer GenerateRandomCustomer()
    {
        MaskData selectedMask = maskDatabase.GetRandom();

        MaskStoryData selectedStory = null;
        if (selectedMask.PossibleStories != null && selectedMask.PossibleStories.Count > 0)
        {
            selectedStory = selectedMask.PossibleStories[UnityEngine.Random.Range(0, selectedMask.PossibleStories.Count)];
        }
        else
        {
            UnityEngine.Debug.LogError($"Mask {selectedMask.name} doesn't have any Story! Please fill it in the Inspector.");
            return null;
        }

        CharacterVisualData selectedVisual = characterVisualDatabase.GetRandom();
        string customerName = selectedVisual.GetRandomName();

        return new ActiveCustomer(selectedMask, selectedStory, selectedVisual, customerName);
    }
}