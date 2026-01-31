public class DialogueManager
{
    public DialogueNode GetBestDialogue(ActiveCustomer customer)
    {
        if (customer == null) return null;

        var nodes = customer.ActiveMaskStoryData.DialogueNodes;
        float crackPercent = customer.CrackPercentage;
        TeaEssenceData lastTea = customer.TeaHistory.Count > 0 ? customer.TeaHistory[customer.TeaHistory.Count - 1] : null;

        if (lastTea != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var n = nodes[i];
                if (n.SpecificTeaReaction == lastTea &&
                    crackPercent >= n.MinCrackPercentage)
                {
                    return n;
                }
            }
        }

        DialogueNode bestNode = null;
        float bestMinCrackPercentage = float.MinValue;
        for (int i = 0; i < nodes.Count; i++)
        {
            var n = nodes[i];
            if (n.SpecificTeaReaction == null &&
                crackPercent >= n.MinCrackPercentage)
            {
                if (n.MinCrackPercentage > bestMinCrackPercentage)
                {
                    bestMinCrackPercentage = n.MinCrackPercentage;
                    bestNode = n;
                }
            }
        }
        return bestNode;
    }
}