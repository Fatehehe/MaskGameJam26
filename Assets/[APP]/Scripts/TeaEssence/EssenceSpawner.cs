using System;
using UnityEngine;

public interface IDraggableData
{
    Sprite DraggableIcon { get; }
}

public class EssenceSpawner : MonoBehaviour
{
    public static event Action<Transform, IDraggableData> OnEssenceSpawnRequested;

    [SerializeField] private EssenceDragItem dragPrefab;
    [SerializeField] private Transform canvas;

    private void OnEnable()
    {
        OnEssenceSpawnRequested += HandleSpawnRequest;
    }

    private void OnDisable()
    {
        OnEssenceSpawnRequested -= HandleSpawnRequest;
    }

    private void HandleSpawnRequest(Transform origin, IDraggableData data)
    {
        if (data == null || dragPrefab == null || canvas == null)
            return;

        EssenceDragItem item = Instantiate(dragPrefab, canvas.transform);
        item.Initialize(data, origin.position);
    }

    // DIPANGGIL OLEH SOURCE
    public static void RequestSpawn(Transform origin, IDraggableData data)
    {
        OnEssenceSpawnRequested?.Invoke(origin, data);
    }
}
