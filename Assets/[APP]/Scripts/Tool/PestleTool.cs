using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PestleTool : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Settings")]
    [SerializeField] private float grindRadius = 50f;
    [SerializeField] private float grindThreshold = 1000f;

    private RectTransform rectTransform;
    private Vector2 initialPos;
    private float currentGrindProgress = 0f;
    private bool isGrindable = false;
    
    public System.Action OnGrindFinished;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPos = rectTransform.anchoredPosition;
    }

    public void SetGrindable(bool state)
    {
        isGrindable = state;
        currentGrindProgress = 0;
        // Opsional: Ganti warna/visual pestle kalau siap ngulek
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        // 1. Logic Gerak Terbatas (Clamped Position)
        Vector2 targetPos = rectTransform.anchoredPosition + (eventData.delta / GetComponentInParent<Canvas>().scaleFactor);
        
        // Batasi jarak dari titik tengah (Vector2.zero asusmsi parentnya di tengah mortar)
        if (targetPos.magnitude > grindRadius)
        {
            targetPos = targetPos.normalized * grindRadius;
        }
        rectTransform.anchoredPosition = targetPos;

        // 2. Logic Grinding
        if (isGrindable)
        {
            // Tambahkan jarak yang ditempuh mouse ke progress
            currentGrindProgress += eventData.delta.magnitude;

            if (currentGrindProgress >= grindThreshold)
            {
                isGrindable = false;
                OnGrindFinished?.Invoke();
                Debug.Log("Grinding Complete!");
                // Feedback Visual: Partikel debu / suara ulek
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Kembalikan ke posisi tengah biar rapi
        LeanTween.move(rectTransform, initialPos, 0.2f).setEaseOutBack();
    }
}