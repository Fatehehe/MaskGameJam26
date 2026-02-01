using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    // --- CUSTOMER EVENTS ---
    public static Action<ActiveCustomer> OnCustomerChanged; // Kirim data customer baru
    public static Action OnCustomerDeparture; // Saat customer pergi

    // --- TEA BREWING EVENTS ---
    public static Action OnTeaServed; // Saat teh selesai diseduh
    public static Action<bool> OnBrewingInterfaceStateChanged; // Buka/Tutup Menu Teh
    
    // Event Panci (Pot)
    // Param 1: List essence saat ini
    // Param 2: Status racikan (Incomplete/Brewable/Ruined)
    public static Action<List<TeaEssenceData>, BrewingStatus> OnPotUpdated;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        OnCustomerChanged = null;
        OnCustomerDeparture = null;
        OnTeaServed = null;
        OnBrewingInterfaceStateChanged = null;
        OnPotUpdated = null;
    }
}