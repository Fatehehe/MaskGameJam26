using UnityEngine;
using UnityEngine.EventSystems;

public class PestleTool : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Grind Settings")]
    [SerializeField] private float grindThreshold = 1000f; // Total gesekan yang dibutuhkan
    
    [Header("Movement Limits (Local Position)")]
    // Sesuaikan angka ini di Inspector pas Play Mode biar pas sama gambar mangkukmu
    [SerializeField] private float minX = -50f; 
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minY = -20f;
    [SerializeField] private float maxY = 30f;

    [Header("Visual Juice")]
    [SerializeField] private float tiltAmount = 15f; // Miring dikit pas digerakin

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
        // Opsional: Reset posisi pestle ke tengah
        rectTransform.anchoredPosition = initialPos;
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        // 1. Hitung Posisi Baru
        Vector2 delta = eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
        Vector2 targetPos = rectTransform.anchoredPosition + delta;

        // 2. BATASI GERAKAN (CLAMP) - Biar gak keluar dari mangkuk oranye
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        rectTransform.anchoredPosition = targetPos;

        // 3. Efek Visual: Miringkan ulekan sesuai posisi X
        // Kalau di kiri miring kiri, di kanan miring kanan
        float normalizedX = Mathf.InverseLerp(minX, maxX, targetPos.x); // 0 (kiri) s/d 1 (kanan)
        float targetAngle = Mathf.Lerp(tiltAmount, -tiltAmount, normalizedX);
        rectTransform.localRotation = Quaternion.Euler(0, 0, targetAngle);

        // 4. Hitung Progress Ngulek
        if (isGrindable)
        {
            // Kita pakai magnitude delta biar gerakan bolak-balik dihitung
            currentGrindProgress += delta.magnitude;

            if (currentGrindProgress >= grindThreshold)
            {
                isGrindable = false; // Stop grinding
                OnGrindFinished?.Invoke();
                
                // Efek visual selesai (misal balik tegak)
                LeanTween.rotateZ(gameObject, 0f, 0.2f);
                Debug.Log("Grinding Complete!");
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Balik ke posisi istirahat (tengah mangkuk)
        LeanTween.move(rectTransform, initialPos, 0.3f).setEaseOutBack();
        LeanTween.rotateZ(gameObject, 0f, 0.3f);
    }
}