using UnityEngine;
using TMPro;
using VContainer;
using System.Collections.Generic;

public class GameplayTeaBrewController : MonoBehaviour
{
    [Header("Brewing Station Panel")]
    [SerializeField] private GameObject stationRoot;
    [SerializeField] private CanvasGroup stationCanvasGroup;

    [Header("Physical Tools")]
    [SerializeField] private MixerTool mixerTool;
    [SerializeField] private GlassTool glassTool;
    [SerializeField] private CoasterZone coasterZone;
    [SerializeField] private KettleTool kettleTool;
    [SerializeField] private StoveTool stoveTool;

    [Header("Ingredient Rack")]
    [SerializeField] private List<EssenceObject> essenceRackSlots;

    // Dependencies
    private TeaBrewingSystem brewingSystem;

    [Inject]
    public void Construct(TeaBrewingSystem brewingSystem)
    {
        this.brewingSystem = brewingSystem;
    }

    private void Start()
    {
        if (stationRoot) stationRoot.SetActive(true);

        SetStationInteractable(false);

        InitializeTools();

        GameplayEvents.OnBrewingInterfaceStateChanged += HandleInterfaceState;
        if (kettleTool != null) kettleTool.OnStateChanged += UpdateHintFromKettle;
    }

    private void OnDestroy()
    {
        GameplayEvents.OnBrewingInterfaceStateChanged -= HandleInterfaceState;
        if (kettleTool != null) kettleTool.OnStateChanged -= UpdateHintFromKettle;
    }

    // --- INITIALIZATION ---

    private void InitializeTools()
    {
        if (mixerTool != null) mixerTool.Initialize(brewingSystem);
        if (coasterZone != null) coasterZone.Initialize(brewingSystem);
        if (glassTool != null) glassTool.ResetGlass();

        Debug.Log("Brewing Station Tools Initialized.");
    }

    // --- EVENT HANDLERS (Animasi & Feedback) ---

    private void SetStationInteractable(bool isInteractable)
    {
        if (stationCanvasGroup != null)
        {
            stationCanvasGroup.blocksRaycasts = isInteractable;

            // stationCanvasGroup.alpha = isInteractable ? 1f : 0.6f;

            stationCanvasGroup.alpha = 1f;
        }
    }

    private void HandleInterfaceState(bool isOpen)
    {
        if (stationRoot == null) return;

        if (isOpen)
        {
            SetStationInteractable(true);

            // LeanTween.scale(stationRoot, Vector3.one * 1.02f, 0.1f).setLoopPingPong(1);
        }
        else
        {
            SetStationInteractable(false);

            ResetAllTools();
        }
    }

    private void UpdateHintFromKettle(KettleTool.KettleState state)
    {
        switch (state)
        {
            case KettleTool.KettleState.Boiling:
                Debug.Log("Kettle: Air sedang mendidih...");
                break;
            case KettleTool.KettleState.Boiled:
                Debug.Log("Kettle: Air Panas Siap! Tuangkan ke Gelas.");
                break;
        }
    }

    private void ResetAllTools()
    {
        if (mixerTool != null) mixerTool.ResetMixerTotal();
        if (glassTool != null) glassTool.ResetGlass();
        if (kettleTool != null) kettleTool.ForceReset();
        if (stoveTool != null) stoveTool.RemoveKettle();
    }
}