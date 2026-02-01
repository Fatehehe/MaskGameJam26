using UnityEngine;
using VContainer;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayUIController : MonoBehaviour
{
    [Header("Game Session Settings")]
    [SerializeField] private int maxCustomers = 3;

    [Header("Sub-Controllers")]
    [SerializeField] private DialogueDisplayController dialogueDisplay;
    [SerializeField] private CharacterVisualController characterVisual;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel; // Panel Pop Up
    [SerializeField] private CanvasGroup endGameCanvasGroup; // Buat animasi fade
    [SerializeField] private Button replayButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button homeButton; // Opsional
    [SerializeField] private string creditsSceneName = "Credits";
    [SerializeField] private string mainMenuSceneName = "Home";

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
        // Setup Tombol
        if (replayButton) replayButton.onClick.AddListener(OnReplayClicked);
        if (creditsButton) creditsButton.onClick.AddListener(OnCreditsClicked);
        if (homeButton) homeButton.onClick.AddListener(OnHomeClicked);

        // Sembunyikan panel di awal
        if (endGamePanel) 
        {
            endGamePanel.SetActive(false);
            if (endGameCanvasGroup) endGameCanvasGroup.alpha = 0f;
        }

        GameplayEvents.OnTeaServed += OnTeaServed;
        brewingSystem.SetInteractable(false);

        servedCount = 0;

        // Start sequence
        characterVisual.HideInstant();
        SpawnNextCustomer();
    }

    private void OnDestroy()
    {
        GameplayEvents.OnTeaServed -= OnTeaServed;
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
        // 1. Matikan Gameplay
        characterVisual.HideInstant();
        brewingSystem.SetInteractable(false);

        // 2. Tampilkan Teks Penutup
        dialogueDisplay.ShowText("The mist clears... Only the warmth of the tea remains.", () =>
        {
            // Callback saat teks selesai: TAMPILKAN POP UP
            ShowEndGamePanel();
        });
    }

    private void ShowEndGamePanel()
    {
        if (endGamePanel == null) return;

        endGamePanel.SetActive(true);
        
        // Animasi Fade In & Scale Up
        endGamePanel.transform.localScale = Vector3.one * 0.8f;
        LeanTween.scale(endGamePanel, Vector3.one, 0.5f).setEaseOutBack();
        
        if (endGameCanvasGroup)
        {
            LeanTween.alphaCanvas(endGameCanvasGroup, 1f, 0.5f);
        }
    }

    private void OnReplayClicked()
    {
        // Reload scene saat ini
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCreditsClicked()
    {
        SceneManager.LoadScene(creditsSceneName);
    }
    
    private void OnHomeClicked()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}