using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EssenceObject : InteractableObject
{
    [SerializeField] private Image icon;
    [SerializeField] private ScriptableObject dataAsset;

    private IDraggableData data;
    public IDraggableData Data => data;

    protected override void Awake()
    {
        base.Awake();

        data = dataAsset as IDraggableData;

        if (data == null)
        {
            Debug.LogError($"{name} dataAsset does not implement IDraggableData!");
            return;
        }

        icon.sprite = data.DraggableIcon;
    }

    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        var dropTarget = eventData.pointerEnter?.GetComponent<IDropHandlerTarget>();
        if (dropTarget != null)
        {
            return dropTarget.TryAccept(data);
        }
        return false;
    }

    protected override void OnDragStart(PointerEventData eventData)
    {
        if (data == null) return;
        EssenceSpawner.RequestSpawn(rectTransform, data);
    }

    protected override void OnDragging(PointerEventData eventData)
    {
        // rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    protected override void OnDragEnd(PointerEventData eventData, bool success)
    {

    }
}
