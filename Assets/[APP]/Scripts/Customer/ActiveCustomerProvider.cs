using System;

public class ActiveCustomerProvider
{
    public ActiveCustomer CurrentCustomer { get; private set; }

    public void SetActiveCustomer(ActiveCustomer newCustomer)
    {
        CurrentCustomer = newCustomer;
        if (newCustomer != null)
            GameplayEvents.OnCustomerChanged?.Invoke(newCustomer);
        else
            GameplayEvents.OnCustomerDeparture?.Invoke();
    }

    public bool HasCustomer => CurrentCustomer != null;
}