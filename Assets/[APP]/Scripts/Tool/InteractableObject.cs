using UnityEngine;
using UnityEngine.EventSystems;

public abstract class InteractableObject : MonoBehaviour,
    IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected RectTransform rectTransform;
    protected Canvas canvas;

    protected Vector2 initialPos;
    protected Transform initialParent;
    private Vector2 startAnchoredPos;
    private Transform startParent;
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        initialPos = rectTransform.anchoredPosition;
        initialParent = transform.parent;
    }

    public virtual void ForceReset()
    {
        transform.SetParent(initialParent);
        rectTransform.anchoredPosition = initialPos;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        startAnchoredPos = rectTransform.anchoredPosition;
        startParent = transform.parent;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        OnDragStart(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        OnDragging(eventData);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        bool droppedOnValidTarget = TryHandleDrop(eventData);

        if (!droppedOnValidTarget)
        {
            ReturnToStartPosition();
        }

        OnDragEnd(eventData, droppedOnValidTarget);
    }

    protected void ReturnToStartPosition()
    {
        transform.SetParent(startParent);
        rectTransform.anchoredPosition = startAnchoredPos;
    }

    protected virtual void DetachToOrigin()
    {
        ReturnToStartPosition();
    }

    protected virtual void OnDragStart(PointerEventData eventData) { }
    protected virtual void OnDragging(PointerEventData eventData) { }
    protected abstract bool TryHandleDrop(PointerEventData eventData);
    protected virtual void OnDragEnd(PointerEventData eventData, bool success) { }
}