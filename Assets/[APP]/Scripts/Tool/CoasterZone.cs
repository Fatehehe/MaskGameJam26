using System.Collections;
using UnityEngine;

public class CoasterZone : MonoBehaviour
{
    private TeaBrewingSystem brewingSystem;

    public void Initialize(TeaBrewingSystem system)
    {
        brewingSystem = system;
    }

    public void Serve(GlassTool glass)
    {
        if (glass.FinalTea != null)
        {
            Debug.Log("Coaster: Visual Move First...");
            
            // 1. VISUAL DULU: Pindahkan gelas ke coaster & kunci interaksi
            glass.transform.SetParent(transform);
            glass.transform.localPosition = Vector3.zero;
            
            // Matikan raycast biar gak bisa ditarik user pas lagi animasi
            if (glass.GetComponent<CanvasGroup>() != null)
                glass.GetComponent<CanvasGroup>().blocksRaycasts = false;

            // 2. Jalankan Coroutine untuk animasi & logic akhir
            StartCoroutine(CustomerTakeTea(glass));
        }
    }

    private IEnumerator CustomerTakeTea(GlassTool glass)
    {
        // Simpan data tehnya dulu sebelum hilang (safety)
        TeaData teaToServe = glass.FinalTea;

        // Tunggu animasi sejenak (delay biar berasa 'disajikan')
        yield return new WaitForSeconds(0.5f); 
        
        // Hilangkan visual gelas
        glass.HideVisuals(); 

        // 3. LOGIC TERAKHIR: Baru lapor ke sistem kalau teh sudah diminum
        // Ini akan memicu Dialog -> Menu Tutup -> ResetAllTools -> Gelas Teleport ke Meja
        brewingSystem.FinalizeServe(teaToServe);
    }
}