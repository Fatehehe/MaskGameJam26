using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EssenceObject : InteractableObject
{
    [SerializeField] private Image icon;
    [SerializeField] private TeaEssenceData data;
    public TeaEssenceData Data => data;

    protected override void Awake()
    {
        base.Awake();

        if (data == null)
        {
            Debug.LogError($"{name} dataAsset is not a valid TeaEssenceData!");
            return;
        }

        icon.sprite = data.Icon;
    }

    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        var dropTarget = eventData.pointerEnter?.GetComponent<IEssenceDropTarget>();
        if (dropTarget != null)
        {
            return dropTarget.TryAccept(data);
        }
        return false;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        OnDragStart(eventData);
    }

    protected override void OnDragStart(PointerEventData eventData)
    {
        if (data == null) return;

        EssenceDragItem spawnedItem = EssenceSpawner.RequestSpawn(rectTransform, data);

        if (spawnedItem != null)
        {
            eventData.pointerDrag = spawnedItem.gameObject;

            spawnedItem.OnBeginDrag(eventData);
        }
    }
}
