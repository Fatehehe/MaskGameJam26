using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EssenceDragItem : InteractableObject
{
    [SerializeField] private Image icon;
    private TeaEssenceData data;

    public void Initialize(TeaEssenceData newData, Vector3 startPos)
    {
        data = newData;
        icon.sprite = data.DraggableIcon;
        transform.position = startPos;
    }

    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        var dropTarget = eventData.pointerEnter?.GetComponentInParent<IEssenceDropTarget>();

        if (dropTarget != null && dropTarget.TryAccept(data))
        {
            Destroy(gameObject);
            return true;
        }

        Destroy(gameObject);
        return false;
    }

    protected override void OnDragging(PointerEventData eventData)
    {
        if (canvas == null) return;

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 localCursorPos;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            canvas.worldCamera, // Gunakan kamera canvas (null jika Overlay, tidak masalah)
            out localCursorPos))
        {
            // Set posisi langsung ke titik mouse
            rectTransform.anchoredPosition = localCursorPos;
        }
    }
}
