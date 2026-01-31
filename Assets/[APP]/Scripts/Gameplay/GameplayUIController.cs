using UnityEngine;
using VContainer;
using System.Collections;

public class GameplayUIController : MonoBehaviour
{
    [Header("Game Session Settings")]
    [SerializeField] private int maxCustomers = 3;

    [Header("Sub-Controllers")]
    [SerializeField] private DialogueDisplayController dialogueDisplay;
    [SerializeField] private CharacterVisualController characterVisual;

    // Dependencies
    private ActiveCustomerProvider customerProvider;
    private DialogueManager dialogueManager;
    private TeaBrewingSystem brewingSystem;
    private CustomerFactory customerFactory;

    private int servedCount = 0;

    [Inject]
    public void Construct(
        ActiveCustomerProvider customerProvider,
        DialogueManager dialogueManager,
        TeaBrewingSystem brewingSystem,
        CustomerFactory customerFactory)
    {
        this.customerProvider = customerProvider;
        this.dialogueManager = dialogueManager;
        this.brewingSystem = brewingSystem;
        this.customerFactory = customerFactory;
    }

    private void Start()
    {
        brewingSystem.OnTeaServed += OnTeaServed;
        brewingSystem.SetInteractable(false);

        servedCount = 0;

        // Start sequence
        characterVisual.HideInstant();
        SpawnNextCustomer();
    }

    private void OnDestroy()
    {
        if (brewingSystem != null) brewingSystem.OnTeaServed -= OnTeaServed;
    }

    // --- PHASE 1: ARRIVAL ---
    private void SpawnNextCustomer()
    {
        if (servedCount >= maxCustomers)
        {
            StartEpilogue();
            return;
        }

        ActiveCustomer newCustomer = customerFactory.GenerateRandomCustomer();
        customerProvider.SetActiveCustomer(newCustomer);

        // 1. Setup Visual (Position at Spawn Point)
        characterVisual.SetupVisuals(newCustomer);

        // 2. Enter Animation
        characterVisual.AnimateEnter(() =>
        {
            ShowCurrentDialogue();
        });
    }

    // --- PHASE 2: TEA REACTION ---
    private void OnTeaServed()
    {
        // Update mask visual (Cracked/Broken) without transition animation
        if (customerProvider.HasCustomer)
        {
            characterVisual.UpdateMaskState(customerProvider.CurrentCustomer);
            ShowCurrentDialogue();
        }
    }

    // --- SHARED: DIALOGUE LOGIC ---
    private void ShowCurrentDialogue()
    {
        // Disable tea interaction while talking
        brewingSystem.SetInteractable(false);

        ActiveCustomer customer = customerProvider.CurrentCustomer;
        DialogueNode dialogNode = dialogueManager.GetBestDialogue(customer);
        string lineToSay = dialogNode != null ? dialogNode.GetRandomLine() : "...";

        dialogueDisplay.ShowText(lineToSay, () =>
        {
            // Callback when Dialog Box is closed (Pop Down)

            if (dialogNode != null && dialogNode.EndsConversation)
            {
                StartCoroutine(HandleDepartureSequence());
            }
            else
            {
                // Open Tea Menu
                brewingSystem.SetInteractable(true);
            }
        });
    }

    // --- PHASE 3: DEPARTURE ---
    private IEnumerator HandleDepartureSequence()
    {
        brewingSystem.SetInteractable(false);
        yield return new WaitForSeconds(0.5f);

        servedCount++;

        // Exit Animation
        characterVisual.AnimateExit(() =>
        {
            // After completely disappearing from the screen, spawn next
            SpawnNextCustomer();
        });
    }

    private void StartEpilogue()
    {
        characterVisual.HideInstant();
        brewingSystem.SetInteractable(false);

        dialogueDisplay.ShowText("All guests have gone home... The fog begins to thin.", () =>
        {
            // Ending callback
            Debug.Log("Game Finished");
        });
    }
}