using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlassTool : InteractableObject
{
    [Header("Visuals")]
    [SerializeField] private Sprite defaultGlassSprite; // Gambar gelas kosong
    [SerializeField] private Image glassImage; // Gambar gelas
    [SerializeField] private Image contentImage; // Gambar air teh
    [SerializeField] private GameObject powderVisual; // Gambar bubuk di dasar
    // Hapus spoonSnapPoint karena gak pake sendok lagi

    private TeaData currentTea;
    private bool hasPowder = false;
    private bool hasWater = false;

    // Logic baru: Siap saji kalau ada Bubuk + Air (Gak perlu diaduk)
    public bool IsReadyToServe => hasWater && hasPowder;
    public TeaData FinalTea => currentTea;

    protected override void Awake()
    {
        base.Awake();
        ResetGlass();
    }

    // 1. Terima Bubuk (Dari Mixer)
    public void AddPowder(TeaData tea)
    {
        if (hasWater) return;

        currentTea = tea;
        hasPowder = true;
        powderVisual.SetActive(true);
        Debug.Log($"Glass: Bubuk {tea.TeaName} masuk.");
    }

    // 2. Terima Air Panas (Dari Kettle)
    public void AddHotWater()
    {
        if (!hasPowder) return;

        hasWater = true;
        powderVisual.SetActive(false); // Bubuk larut
        contentImage.enabled = true;
        glassImage.sprite = defaultGlassSprite;

        // Langsung set warna teh final (Gak perlu keruh dulu)
        // Kalau mau canggih: Ambil warna dari TeaData.LiquidColor
        if (currentTea.Icon != null)
        {
            glassImage.sprite = currentTea.Icon;
            contentImage.color = Color.white;
            contentImage.enabled = false;
        }
        else
        {
            contentImage.color = new Color(0.6f, 0.3f, 0f, 1f); // Coklat teh default
        }

        Debug.Log("Glass: Air panas dituang. Teh Siap Saji!");
    }

    // 3. Logic Drag ke Coaster (Hijau)
    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        // 1. Cek Coaster (Sajikan)
        CoasterZone coaster = eventData.pointerEnter?.GetComponent<CoasterZone>();
        if (coaster != null && IsReadyToServe)
        {
            coaster.Serve(this);
            return true;
        }

        // 2. Cek Trash (Buang Isi) -- TAMBAHAN BARU --
        TrashTool trash = eventData.pointerEnter?.GetComponent<TrashTool>();
        if (trash != null)
        {
            trash.DisposeItem(this); // Gelas jadi bersih lagi
            return false; // Return false biar dia balik ke meja (animasi DetachToOrigin)
        }

        return false;
    }

    public void HideVisuals()
    {
        // Matikan gambar air dan bubuk
        contentImage.enabled = false;
        powderVisual.SetActive(false);

        // Matikan gambar gelasnya sendiri (CanvasGroup alpha 0)
        // Pastikan Glass punya CanvasGroup ya (dari InteractableObject pasti punya)
        GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void ResetGlass()
    {
        // 1. Reset Data
        hasPowder = false;
        hasWater = false;
        powderVisual.SetActive(false);
        contentImage.enabled = false;
        currentTea = null;
        contentImage.color = Color.white;
        glassImage.sprite = defaultGlassSprite;

        // 2. Munculkan Visualnya Lagi (Alpha 1)
        GetComponent<CanvasGroup>().alpha = 1f;

        // 3. Reset Posisi Fisik
        ForceReset();

        Debug.Log("Glass Cleaned & Returned to Table.");
    }

    protected override void OnDragging(PointerEventData eventData)
    {
        // Update posisi berdasarkan gerakan mouse dan scale factor canvas
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
}