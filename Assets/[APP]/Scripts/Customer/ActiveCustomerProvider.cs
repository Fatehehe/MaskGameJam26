using System;

public class ActiveCustomerProvider
{
    public ActiveCustomer CurrentCustomer { get; private set; }
    public event Action OnCustomerChanged;

    public void SetActiveCustomer(ActiveCustomer newCustomer)
    {
        CurrentCustomer = newCustomer;
        OnCustomerChanged?.Invoke();
    }

    public bool HasCustomer => CurrentCustomer != null;
}