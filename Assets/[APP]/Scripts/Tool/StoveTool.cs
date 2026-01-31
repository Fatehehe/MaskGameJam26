using UnityEngine;
using UnityEngine.EventSystems;

public class StoveTool : MonoBehaviour, IPointerClickHandler
{
    public Transform snapPoint;

    private KettleTool currentKettle;

    public void PlaceKettle(KettleTool kettle)
    {
        currentKettle = kettle;
    }

    public void RemoveKettle()
    {
        currentKettle = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentKettle != null)
        {
            currentKettle.StartBoilingFromStove();
        }
    }
}
