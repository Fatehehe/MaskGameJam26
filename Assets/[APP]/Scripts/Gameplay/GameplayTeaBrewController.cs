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
    [SerializeField] private SpoonTool spoonTool;

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
        if (stationRoot) stationRoot.SetActive(false);

        // Inisialisasi logika alat-alat
        InitializeTools();
        
        // Kita HAPUS SetupIngredientRack() karena data sudah di-drag di Inspector Scene.

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
        
        // Pastikan gelas bersih saat game mulai
        if (glassTool != null) glassTool.ResetGlass();
        
        Debug.Log("Brewing Station Tools Initialized.");
    }

    // --- EVENT HANDLERS (Animasi & Feedback) ---

    private void HandleInterfaceState(bool isOpen)
    {
        if (stationRoot == null) return;

        if (isOpen)
        {
            stationRoot.SetActive(true);
            
            // Animasi Masuk
            stationRoot.transform.localScale = Vector3.one * 0.9f;
            LeanTween.scale(stationRoot, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack);
            
            if (stationCanvasGroup)
            {
                stationCanvasGroup.alpha = 0f;
                LeanTween.alphaCanvas(stationCanvasGroup, 1f, 0.3f);
            }
        }
        else
        {
            // Animasi Keluar
            LeanTween.scale(stationRoot, Vector3.one * 0.95f, 0.3f).setEase(LeanTweenType.easeInBack);
            if (stationCanvasGroup)
            {
                LeanTween.alphaCanvas(stationCanvasGroup, 0f, 0.3f).setOnComplete(() => 
                {
                    stationRoot.SetActive(false);
                    ResetAllTools();
                });
            }
            else
            {
                stationRoot.SetActive(false);
                ResetAllTools();
            }
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
            default:
                break;
        }
    }

    private void ResetAllTools()
    {
        // 1. Reset Mixer (Posisi + Isi)
        if (mixerTool != null) 
        {
            mixerTool.ResetMixerTotal(); 
        }

        // 2. Reset Glass (Posisi + Isi)
        if (glassTool != null) 
        {
            glassTool.ResetGlass(); 
        }

        // 3. Reset Kettle (Posisi + State + Stop Boiling)
        if (kettleTool != null) 
        {
            kettleTool.ForceReset(); 
        }

        // 4. Reset Spoon (Posisi + Detach)
        if (spoonTool != null)
        {
            spoonTool.ForceReset();
        }

        // 5. Reset Stove (Hapus referensi kettle)
        if (stoveTool != null)
        {
            stoveTool.RemoveKettle();
        }
    }
}