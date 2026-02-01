using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MixerTool : MonoBehaviour, IEssenceDropTarget, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Components")]
    [SerializeField] private PestleTool pestle;
    [SerializeField] private Image powderResultImage;
    [SerializeField] private List<Image> slotIcons;

    private TeaBrewingSystem brewingSystem;
    private bool isPowderReady = false;
    private TeaData readyTeaData;

    private Vector2 initialPos;
    private Vector2 originalPos;
    private RectTransform rectTransform;
    private Canvas canvas;

    public void Initialize(TeaBrewingSystem brewingSystem)
    {
        this.brewingSystem = brewingSystem;
        pestle.OnGrindFinished += HandleGrindFinished;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
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
        for (int i = 0; i < slotIcons.Count; i++)
        {
            if (i < potContent.Count)
            {
                slotIcons[i].enabled = true;
                slotIcons[i].sprite = potContent[i].Icon;
            }
            else
            {
                slotIcons[i].enabled = false;
            }
        }

        if (status == BrewingStatus.Brewable)
        {
            readyTeaData = brewingSystem.PreviewRecipe();
            pestle.SetGrindable(true);
            Debug.Log("Mixer: Siap diulek!");
        }
        else
        {
            pestle.SetGrindable(false);
            readyTeaData = null;
        }
    }

    public bool TryAccept(TeaEssenceData essence)
    {
        if (essence == null)
            return false;

        if (brewingSystem.CurrentPotCount >= TeaBrewingSystem.MAX_ESSENCES_IN_POT)
        {
            Debug.Log("Mixer Full!");
            return false;
        }

        brewingSystem.AddEssenceToPot(essence);
        return true;
    }

    private void HandleGrindFinished()
    {
        isPowderReady = true;
        foreach(var icon in slotIcons) icon.enabled = false;
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
        foreach(var icon in slotIcons) icon.enabled = false;

        // 3. Reset Posisi Mixer ke Meja
        if (rectTransform != null)
        {
            LeanTween.cancel(gameObject); // Stop animasi tween yang mungkin jalan
            rectTransform.anchoredPosition = initialPos;
            transform.localScale = Vector3.one;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) { }
    
    public void OnDrag(PointerEventData eventData) 
    {

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Cek apakah didrop di atas Gelas?
        GlassTool glass = eventData.pointerEnter?.GetComponent<GlassTool>();
        
        if (glass != null && isPowderReady)
        {
            // Tuang bubuk ke gelas
            glass.AddPowder(readyTeaData);
            ResetMixerTotal();
        }

        // Balik ke posisi meja
        LeanTween.move(rectTransform, originalPos, 0.3f).setEaseOutBack();
    }
}
