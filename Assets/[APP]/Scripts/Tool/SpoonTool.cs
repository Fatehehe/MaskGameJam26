using UnityEngine;
using UnityEngine.EventSystems;

public class SpoonTool : InteractableObject
{
    [SerializeField] private float stirDistanceThreshold = 1500f;
    
    private GlassTool currentGlass;
    private float currentStirDistance = 0f;
    private Vector2 lastMousePos;

    // Cache posisi meja agar bisa pulang
    private Vector2 originalPos;
    private Transform originalParent;

    protected override void Awake()
    {
        base.Awake();
        // Simpan posisi & parent awal (Meja/Coaster)
        originalPos = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    // --- LOGIC MOVEMENT & STIRRING ---
    protected override void OnDragging(PointerEventData eventData)
    {
        // Kondisi 1: Sedang nempel di gelas (NGADUK)
        if (currentGlass != null)
        {
            // Jangan gerak posisi (tetap di snap point), tapi rotasi
            currentStirDistance += Vector2.Distance(eventData.position, lastMousePos);
            lastMousePos = eventData.position;

            // Visual effect: Putar sendok sesuai gerakan mouse X
            float rotationAmount = -eventData.delta.x;
            transform.Rotate(Vector3.forward, rotationAmount);

            if (currentStirDistance >= stirDistanceThreshold)
            {
                currentGlass.FinishStirring();
                // Opsional: Reset distance biar gak spam finish
                currentStirDistance = 0; 
            }
        }
        // Kondisi 2: Sedang di-drag dari meja (ANGKUT)
        else
        {
            base.OnDragging(eventData); // Panggil base kosong (good practice)
            // Manual movement karena InteractableObject.cs kamu OnDragging-nya kosong
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        lastMousePos = eventData.position;
    }

    // --- LOGIC DROP & ATTACH ---
    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        // Cek target di bawah mouse
        GlassTool targetGlass = eventData.pointerEnter?.GetComponent<GlassTool>();

        // Skenario A: Drag dari Meja ke Gelas
        if (currentGlass == null)
        {
            if (targetGlass != null)
            {
                AttachToGlass(targetGlass);
                return true; // Sukses nempel
            }
        }
        // Skenario B: Drag saat sudah di Gelas (Lagi ngaduk)
        else
        {
            // Kalau dilepas MASIH di dalam gelas yang sama -> Tetap di situ
            if (targetGlass == currentGlass)
            {
                return true; 
            }
            // Kalau dilepas di LUAR gelas -> False (nanti OnDragEnd yang urus pulang)
        }

        return false;
    }

    private void AttachToGlass(GlassTool glass)
    {
        currentGlass = glass;
        transform.SetParent(glass.SpoonSnapPoint);
        rectTransform.anchoredPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity; // Reset rotasi biar rapi pas masuk
        currentStirDistance = 0;
    }

    // --- LOGIC PULANG ---
    protected override void OnDragEnd(PointerEventData eventData, bool success)
    {
        // Logic: Kalau gagal drop (lepas di ruang kosong/luar gelas), 
        // DAN kita tadinya lagi di gelas -> Pulang ke meja.
        if (!success && currentGlass != null)
        {
            DetachToOrigin();
            currentGlass = null;
        }
        
        // Note: Kalau success (lepas di dalam gelas), kita diam saja (stay attached).
    }

    // Override biar pulangnya ke Meja, bukan ke Gelas
    protected override void DetachToOrigin()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPos;
        transform.localRotation = Quaternion.identity; // Reset rotasi pas balik ke meja
    }

    public override void ForceReset()
    {
        // 1. Detach dari Gelas (Penting!)
        transform.SetParent(initialParent); // Balik ke parent asli (meja/coaster)
        
        // 2. Reset Variabel
        currentGlass = null;
        currentStirDistance = 0;

        // 3. Reset Posisi
        base.ForceReset();
    }
}