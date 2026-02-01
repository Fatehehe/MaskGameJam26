using UnityEngine;

public class CoasterZone : MonoBehaviour
{
    private TeaBrewingSystem brewingSystem;

    // Di-inject manual oleh Controller
    public void Initialize(TeaBrewingSystem system)
    {
        brewingSystem = system;
    }

    public void Serve(GlassTool glass)
    {
        if (glass.FinalTea != null)
        {
            Debug.Log("Coaster: Serving Tea...");
            brewingSystem.FinalizeServe(glass.FinalTea);
            
            // Visual: Gelas nempel di coaster, lalu fade out atau sequence selesai
            glass.transform.SetParent(transform);
            glass.transform.localPosition = Vector3.zero;
        }
    }
}