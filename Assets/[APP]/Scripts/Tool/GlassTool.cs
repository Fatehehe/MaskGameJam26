using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlassTool : InteractableObject
{
    [Header("Visuals")]
    [SerializeField] private Image contentImage; // Warna air
    [SerializeField] private GameObject powderVisual; // Bubuk di dasar gelas
    [SerializeField] private GameObject spoonSnapPoint; // Tempat sendok nempel

    private TeaData currentTea;
    private bool hasPowder = false;
    private bool hasWater = false;
    private bool isStirred = false;

    public Transform SpoonSnapPoint => spoonSnapPoint.transform;
    public bool IsReadyToServe => isStirred && hasWater && hasPowder;
    public TeaData FinalTea => currentTea;

    protected override void Awake()
    {
        base.Awake();
        ResetGlass();
    }

    // 1. Terima Bubuk dari Mixer
    public void AddPowder(TeaData tea)
    {
        if (hasWater) return; // Gabisa nambah bubuk kalau udah ada air (opsional logic)
        
        currentTea = tea;
        hasPowder = true;
        powderVisual.SetActive(true);
        Debug.Log($"Glass: Bubuk {tea.TeaName} masuk.");
    }

    // 2. Terima Air Panas dari Kettle (Dipanggil KettleTool)
    public void AddHotWater()
    {
        if (!hasPowder) return; // Harus ada bubuk dulu
        
        hasWater = true;
        powderVisual.SetActive(false); // Bubuk larut
        contentImage.color = Color.Lerp(Color.white, Color.brown, 0.5f); // Warna keruh sebelum diaduk
        contentImage.enabled = true;
        Debug.Log("Glass: Air panas dituang.");
    }

    // 3. Selesai Diaduk (Dipanggil SpoonTool)
    public void FinishStirring()
    {
        if (!hasWater) return;
        
        isStirred = true;
        contentImage.color = Color.green; // Ganti warna final teh (bisa ambil dari TeaData)
        Debug.Log("Glass: Teh siap disajikan!");
    }

    // 4. Drag Logic (Sajikan ke Coaster)
    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        CoasterZone coaster = eventData.pointerEnter?.GetComponent<CoasterZone>();
        
        if (coaster != null && IsReadyToServe)
        {
            coaster.Serve(this);
            return true;
        }
        return false;
    }

    public void ResetGlass()
    {
        hasPowder = false;
        hasWater = false;
        isStirred = false;
        powderVisual.SetActive(false);
        contentImage.enabled = false;
        currentTea = null;
        contentImage.color = Color.white;

        ForceReset(); 
        
        Debug.Log("Glass Cleaned & Returned to Table.");
    }
}