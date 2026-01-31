using System.Collections.Generic;
using UnityEngine;
using Modules;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TeaEssenceDatabase", menuName = "Databases/Tea Essence Database")]
public class TeaEssenceDatabase : BaseDatabase<TeaEssenceData>
{
    [System.Serializable]
    public class TeaPair : DatabaseItemPair<TeaEssenceData>
    {
        public TeaPair(string key, TeaEssenceData value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Header("Teas")]
    [SerializeField] private List<TeaPair> items = new List<TeaPair>();

    private readonly Dictionary<string, TeaEssenceData> lookup = new Dictionary<string, TeaEssenceData>();

    public override TeaEssenceData[] GetAllItems()
    {
        TeaEssenceData[] arr = new TeaEssenceData[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            arr[i] = items[i]?.Value;
        }
        return arr;
    }

    public override TeaEssenceData GetItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return null;
        return items[index]?.Value;
    }

    public override TeaEssenceData GetItem(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        EnsureIndex();
        if (lookup.TryGetValue(id, out var val))
            return val;

        return null;
    }

    public override TeaEssenceData GetRandom()
    {
        if (items == null || items.Count == 0)
            return null;
        int idx = UnityEngine.Random.Range(0, items.Count);
        return items[idx]?.Value;
    }

#if UNITY_EDITOR
    protected override void Add(TeaEssenceData value)
    {
        if (value == null)
            return;

        var key = GetKey(value);
        if (string.IsNullOrEmpty(key))
            return;

        // Prevent duplicates by key
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].Key == key)
                return;
        }

        items.Add(new TeaPair(key, value));
        // update lookup so runtime queries work without extra rebuilds
        if (!lookup.ContainsKey(key))
            lookup.Add(key, value);
    }

    protected override void Clear()
    {
        items.Clear();
        lookup.Clear();
    }

    protected override void Sort()
    {
        items.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return string.Compare(a.Key, b.Key, System.StringComparison.Ordinal);
        });

        // Rebuild lookup to match sorted items
        RebuildIndex();
    }
#endif

    private static string GetKey(TeaEssenceData data)
    {
        return data != null ? data.EssenceId : null;
    }

    private void EnsureIndex()
    {
        if (lookup.Count == items.Count)
            return;
        RebuildIndex();
    }

    private void RebuildIndex()
    {
        lookup.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            var p = items[i];
            if (p == null || p.Value == null)
                continue;
            var key = p.Key;
            if (string.IsNullOrEmpty(key))
                key = GetKey(p.Value);
            if (string.IsNullOrEmpty(key))
                continue;
            if (!lookup.ContainsKey(key))
                lookup.Add(key, p.Value);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TeaEssenceDatabase))]
public class TeaEssenceDatabaseEditor : Editor
{
    private TeaEssenceDatabase script;

    private void OnEnable()
    {
        script = (TeaEssenceDatabase)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.Space();
        if (GUILayout.Button("Setup"))
        {
            script.Setup();
            EditorUtility.SetDirty(script);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
