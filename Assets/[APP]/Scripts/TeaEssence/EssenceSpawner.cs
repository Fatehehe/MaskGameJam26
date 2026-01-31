using System;
using UnityEngine;

public class EssenceSpawner : MonoBehaviour
{
    public static event Func<Transform, TeaEssenceData, EssenceDragItem> OnEssenceSpawnRequested;

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

    private EssenceDragItem HandleSpawnRequest(Transform origin, TeaEssenceData data)
    {
        if (data == null || dragPrefab == null || canvas == null)
            return null;

        EssenceDragItem item = Instantiate(dragPrefab, canvas);
        item.Initialize(data, origin.position);
        return item;
    }

    public static EssenceDragItem RequestSpawn(Transform origin, TeaEssenceData data)
    {
        return OnEssenceSpawnRequested?.Invoke(origin, data);
    }
}
