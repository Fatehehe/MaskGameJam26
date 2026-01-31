using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VContainer;
using System.Collections.Generic;

public class GameplayTeaBrewController : MonoBehaviour
{
    // --- UI ELEMENTS ---
    [Header("Main Containers")]
    [SerializeField] private GameObject teaMenuPanel; // Main Brewing UI panel
    [SerializeField] private Transform essenceButtonContainer; // Container for essence buttons
    [SerializeField] private Transform potIconContainer; // Container for added ingredient icons (Pot)

    [Header("Prefabs")]
    [SerializeField] private Button essenceButtonPrefab;
    [SerializeField] private Image potIconPrefab;

    [Header("Action Controls")]
    [SerializeField] private Button brewButton;  // "Brew" button
    [SerializeField] private Button trashButton; // "Trash" button
    [SerializeField] private TMP_Text statusLabel; // Info: Incomplete/Brewable/Ruined

    // Dependencies
    private TeaBrewingSystem brewingSystem;

    [Inject]
    public void Construct(TeaBrewingSystem brewingSystem)
    {
        this.brewingSystem = brewingSystem;
    }

    private void Start()
    {
        // 1. Initial setup (UI is off at start)
        teaMenuPanel.SetActive(false);
        brewButton.interactable = false;

        // 2. Setup persistent action buttons
        brewButton.onClick.AddListener(OnBrewClicked);
        trashButton.onClick.AddListener(OnTrashClicked);

        // 3. Generate Essence Menu
        // GenerateEssenceMenu();

        // 4. PROPER SUBSCRIBE
        brewingSystem.OnInteractableStateChanged += HandleInteractableState;
        brewingSystem.OnPotUpdated += HandlePotUpdated;
    }

    private void OnDestroy()
    {
        // 5. PROPER CLEANUP (Very important for Game Jam to avoid errors when restarting/changing scene)
        if (brewingSystem != null)
        {
            brewingSystem.OnInteractableStateChanged -= HandleInteractableState;
            brewingSystem.OnPotUpdated -= HandlePotUpdated;
        }

        // Cleanup button listeners (standard Unity)
        brewButton.onClick.RemoveAllListeners();
        trashButton.onClick.RemoveAllListeners();
    }

    // --- HANDLERS (CALLBACKS) ---

    private void HandleInteractableState(bool isVisible)
    {
        // Show/hide animation
        if (isVisible)
        {
            // Mulai GameplayTeaBrewController.cs
        }
        else
        {
            
        }
    }

    private void HandlePotUpdated(List<TeaEssenceData> essences, BrewingStatus status)
    {
        // 1. Update Pot Icon Visuals
        RefreshPotVisuals(essences);

        // 2. Update Button Status & Label Based on Enum
        switch (status)
        {
            case BrewingStatus.Incomplete:
                brewButton.interactable = false;
                statusLabel.text = "Mix the ingredients...";
                statusLabel.color = Color.white;
                break;

            case BrewingStatus.Brewable:
                brewButton.interactable = true;
                statusLabel.text = "Ready to brew!";
                statusLabel.color = Color.cyan;
                
                // Small bounce animation so player knows the recipe is ready
                LeanTween.scale(brewButton.gameObject, Vector3.one * 1.1f, 0.15f).setLoopPingPong(1);
                break;

            case BrewingStatus.Ruined:
                brewButton.interactable = false;
                statusLabel.text = "Recipe Failed! Discard!";
                statusLabel.color = Color.red;

                // Shake animation on trash button
                LeanTween.rotateZ(trashButton.gameObject, 10f, 0.1f).setLoopPingPong(3);
                break;
        }
    }

    private void RefreshPotVisuals(List<TeaEssenceData> essences)
    {
        // Cleanup old icons (Use regular for-loop, no LINQ)
        foreach (Transform child in potIconContainer) Destroy(child.gameObject);

        for (int i = 0; i < essences.Count; i++)
        {
            Image icon = Instantiate(potIconPrefab, potIconContainer);
            icon.sprite = essences[i].Icon;

            // Animate icon entering the pot
            icon.transform.localScale = Vector3.zero;
            LeanTween.scale(icon.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
        }
    }

    // --- BUTTON ACTIONS ---
    private void OnBrewClicked()
    {
        brewingSystem.BrewAndServe();
    }

    private void OnTrashClicked()
    {
        brewingSystem.ClearPot();
        
        // Reset animation: Scale down icon container briefly
        LeanTween.scale(potIconContainer.gameObject, Vector3.one * 0.8f, 0.1f).setLoopPingPong(1);
    }
}