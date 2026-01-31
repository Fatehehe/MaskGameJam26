using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EssenceDragItem : InteractableObject
{
    [SerializeField] private Image icon;
    private IDraggableData data;

    public void Initialize(IDraggableData newData, Vector3 startPos)
    {
        data = newData;
        icon.sprite = data.DraggableIcon;
        rectTransform.position = startPos;
    }

    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        var dropTarget = eventData.pointerEnter?.GetComponent<IDropHandlerTarget>();

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
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
