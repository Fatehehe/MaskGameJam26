public class TeaBrewingSystem
{
    private readonly ActiveCustomerProvider _customerProvider;

    private const float CRACKED_THRESHOLD = 40f;
    private const float BROKEN_THRESHOLD = 100f;

    public TeaBrewingSystem(ActiveCustomerProvider customerProvider)
    {
        _customerProvider = customerProvider;
    }

    public void ServeTea(TeaData tea)
    {
        if (!_customerProvider.HasCustomer) return;

        var customer = _customerProvider.CurrentCustomer;
        customer.TeaHistory.Add(tea);

        float impact = tea.CrackImpact;
        customer.CurrentCrackPoints += impact;
        UpdateMaskState(customer);
    }

    private void UpdateMaskState(ActiveCustomer customer)
    {
        float percent = customer.CrackPercentage;

        if (percent >= BROKEN_THRESHOLD)
            customer.CurrentState = MaskState.Broken;
        else if (percent >= CRACKED_THRESHOLD)
            customer.CurrentState = MaskState.Cracked;
        else
            customer.CurrentState = MaskState.Intact;
    }
}