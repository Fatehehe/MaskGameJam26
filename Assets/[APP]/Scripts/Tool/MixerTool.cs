using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class MixerTool : MonoBehaviour, IEssenceDropTarget, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Components")]
    [SerializeField] private PestleTool pestle;
    [SerializeField] private Image powderResultImage;
    [SerializeField] private List<Image> slotIcons;

    [Header("Visual Feedback")]
    [SerializeField] private Image statusIndicator; // Drag Image kosong/icon status di sini
    [SerializeField] private Sprite iconCheck;      // Gambar Centang Hijau (Brewable)
    [SerializeField] private Sprite iconCross;      // Gambar Silang Merah (Ruined)
    [SerializeField] private Color ruinedColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Warna butek

    private TeaBrewingSystem brewingSystem;
    private bool isPowderReady = false;
    private TeaData readyTeaData;

    private Vector2 initialPos;
    private Vector2 originalPos;
    private RectTransform rectTransform;
    private Canvas canvas;

    private CanvasGroup canvasGroup;

    public void Initialize(TeaBrewingSystem brewingSystem)
    {
        this.brewingSystem = brewingSystem;
        pestle.OnGrindFinished += HandleGrindFinished;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();

        initialPos = originalPos = rectTransform.anchoredPosition;
        powderResultImage.enabled = false;
    }

    private void Awake()
    {
        GameplayEvents.OnPotUpdated += UpdateVisuals;
        GameplayEvents.OnTeaServed += ResetMixerTotal;
    }

    private void OnDestroy()
    {
        GameplayEvents.OnPotUpdated -= UpdateVisuals;
        GameplayEvents.OnTeaServed -= ResetMixerTotal;
    }

    private void UpdateVisuals(List<TeaEssenceData> potContent, BrewingStatus status)
    {
        // 1. Update Ikon Bahan (Looping biasa)
        for (int i = 0; i < slotIcons.Count; i++)
        {
            if (i < potContent.Count)
            {
                slotIcons[i].enabled = true;
                slotIcons[i].sprite = potContent[i].DraggableIcon;

                // Reset warna icon (kalau sebelumnya di-tint ruined)
                slotIcons[i].color = Color.white;
            }
            else
            {
                slotIcons[i].enabled = false;
            }
        }

        // 2. LOGIC BARU: Indikator Status
        if (statusIndicator != null)
        {
            switch (status)
            {
                case BrewingStatus.Brewable:
                    statusIndicator.enabled = true;
                    statusIndicator.sprite = iconCheck;
                    statusIndicator.color = Color.white;
                    pestle.SetGrindable(true); // Boleh diulek
                    readyTeaData = brewingSystem.PreviewRecipe();
                    break;

                case BrewingStatus.Ruined:
                    statusIndicator.enabled = true;
                    statusIndicator.sprite = iconCross;
                    statusIndicator.color = Color.red; // Merah tanda bahaya
                    pestle.SetGrindable(false); // Gak boleh diulek
                    readyTeaData = null;

                    // Opsional: Bikin bahan-bahannya jadi warna butek
                    foreach (var icon in slotIcons) icon.color = ruinedColor;
                    break;

                case BrewingStatus.Incomplete:
                default:
                    statusIndicator.enabled = false; // Sembunyikan kalau belum selesai
                    pestle.SetGrindable(false);
                    readyTeaData = null;
                    break;
            }
        }
        else // Fallback kalau gak pake indikator, pake logic lama
        {
            if (status == BrewingStatus.Brewable)
            {
                readyTeaData = brewingSystem.PreviewRecipe();
                pestle.SetGrindable(true);
            }
            else
            {
                pestle.SetGrindable(false);
                readyTeaData = null;
            }
        }
    }

    public bool TryAccept(TeaEssenceData essence)
    {
        if (essence == null) return false;
        if (brewingSystem.CurrentPotCount >= TeaBrewingSystem.MAX_ESSENCES_IN_POT) return false;
        brewingSystem.AddEssenceToPot(essence);
        return true;
    }

    private void HandleGrindFinished()
    {
        isPowderReady = true;
        for (int i = 0; i < slotIcons.Count; i++)
        {
            Image icon = slotIcons[i];
            icon.enabled = false;
        }

        powderResultImage.enabled = true;
        powderResultImage.sprite = readyTeaData.GrindedIcon;

        Debug.Log("Mixer: Bubuk jadi! Drag Mixer ke Gelas untuk menuang.");
    }

    public void ResetMixerTotal()
    {
        // 1. Reset Logic
        brewingSystem.ClearPot();
        isPowderReady = false;
        powderResultImage.enabled = false;
        pestle.SetGrindable(false);

        // 2. Reset Tampilan Icon Slot
        for (int i = 0; i < slotIcons.Count; i++)
        {
            Image icon = slotIcons[i];
            icon.enabled = false;
        }

        // 3. Reset Posisi Mixer ke Meja
        if (rectTransform != null)
        {
            LeanTween.cancel(gameObject); // Stop animasi tween yang mungkin jalan
            rectTransform.anchoredPosition = initialPos;
            transform.localScale = Vector3.one;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;

        // 1. Cek Drop ke Gelas (Tuang)
        GlassTool glass = eventData.pointerEnter?.GetComponent<GlassTool>();
        if (glass != null && isPowderReady)
        {
            glass.AddPowder(readyTeaData);
            ResetMixerTotal();
            LeanTween.move(rectTransform, originalPos, 0.3f).setEaseOutBack();
            return;
        }

        // 2. Cek Drop ke Sampah (Buang) -- TAMBAHAN BARU --
        TrashTool trash = eventData.pointerEnter?.GetComponent<TrashTool>();
        if (trash != null)
        {
            trash.DisposeItem(this); // Panggil fungsi di TrashTool
            // Mixer otomatis kereset di dalam DisposeItem
            LeanTween.move(rectTransform, originalPos, 0.3f).setEaseOutBack();
            return;
        }

        // Balik ke posisi meja
        LeanTween.move(rectTransform, originalPos, 0.3f).setEaseOutBack();
    }
}
