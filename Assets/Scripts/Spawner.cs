using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Serializable]
    public class ObjectType
    {
        public string Name;
        public GameObject[] Prefabs;

        [Header("Spacing")]
        [Min(0f)] public float Width = 0f;
        [Min(0.01f)] public float MinGap = 10f;
        [Min(0.01f)] public float MaxGap = 30f;

        [Header("Height")]
        public bool PlaceOnGround;
        public float MinY = 15f;
        public float MaxY = 50f;
        public float YOffset;

        [Header("Spawning")]
        public float SpawnAhead = 40f;
        public float DespawnBehind = 30f;
        public bool DisableTriggers;

        [NonSerialized] public float NextX;
        [NonSerialized] public bool Enabled;
        [NonSerialized] public List<GameObject> Spawned = new List<GameObject>();
    }

    [SerializeField] Transform Player;
    [SerializeField] Collider Ground;
    [SerializeField] float StartX = -4950f;
    [SerializeField] float MaxX = 4900f;
    [SerializeField] ObjectType[] ObjectTypes;

    void Start()
    {
        if (Player == null)
        {
            Debug.LogError("Spawner requires a player.", this);
            enabled = false;
            return;
        }

        if (ObjectTypes == null)
            return;

        foreach (ObjectType type in ObjectTypes)
        {
            if (type == null)
                continue;

            if (!IsValid(type))
                continue;

            type.NextX = StartX;
            type.Enabled = true;
        }
    }

    bool IsValid(ObjectType type)
    {
        if (type.Prefabs == null || type.Prefabs.Length == 0)
        {
            Debug.LogError($"Spawner type '{type.Name}' needs at least one prefab.", this);
            return false;
        }

        foreach (GameObject prefab in type.Prefabs)
        {
            if (prefab == null)
            {
                Debug.LogError($"Spawner type '{type.Name}' has an empty prefab slot.", this);
                return false;
            }
        }

        if (type.Width < 0f || type.MinGap <= 0f || type.MaxGap < type.MinGap)
        {
            Debug.LogError($"Spawner type '{type.Name}' has invalid spacing.", this);
            return false;
        }

        if (type.SpawnAhead < 0f || type.DespawnBehind < 0f)
        {
            Debug.LogError($"Spawner type '{type.Name}' has invalid spawn distances.", this);
            return false;
        }

        if (type.PlaceOnGround && Ground == null)
        {
            Debug.LogError($"Spawner type '{type.Name}' requires a ground collider.", this);
            return false;
        }

        if (!type.PlaceOnGround && type.MaxY < type.MinY)
        {
            Debug.LogError($"Spawner type '{type.Name}' has an invalid Y range.", this);
            return false;
        }

        return true;
    }

    void Update()
    {
        if (Player == null || ObjectTypes == null)
            return;

        foreach (ObjectType type in ObjectTypes)
        {
            if (type == null || !type.Enabled)
                continue;

            while (type.NextX <= MaxX &&
                   type.NextX < Player.position.x + type.SpawnAhead)
            {
                if (type.PlaceOnGround && Ground == null)
                {
                    Debug.LogError($"Spawner type '{type.Name}' lost its ground collider.", this);
                    type.Enabled = false;
                    break;
                }

                Spawn(type, type.NextX);

                float next = type.NextX + type.Width +
                             UnityEngine.Random.Range(type.MinGap, type.MaxGap);

                if (next <= type.NextX)
                {
                    Debug.LogError($"Spawner type '{type.Name}' cannot advance its X position.", this);
                    type.Enabled = false;
                    break;
                }

                type.NextX = next;
            }

            for (int i = type.Spawned.Count - 1; i >= 0; i--)
            {
                GameObject obj = type.Spawned[i];

                if (obj == null)
                {
                    type.Spawned.RemoveAt(i);
                }
                else if (obj.transform.position.x < Player.position.x - type.DespawnBehind)
                {
                    Destroy(obj);
                    type.Spawned.RemoveAt(i);
                }
            }
        }
    }

    void Spawn(ObjectType type, float x)
    {
        GameObject prefab = type.Prefabs[
            UnityEngine.Random.Range(0, type.Prefabs.Length)
        ];

        Vector3 position;
        if (type.PlaceOnGround)
        {
            Bounds bounds = Ground.bounds;
            position = new Vector3(x, bounds.max.y + type.YOffset, bounds.center.z);
        }
        else
        {
            position = new Vector3(
                x,
                UnityEngine.Random.Range(type.MinY, type.MaxY),
                0f
            );
        }

        GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);

        if (type.DisableTriggers)
        {
            foreach (Collider collider in obj.GetComponentsInChildren<Collider>())
                collider.isTrigger = false;
        }

        type.Spawned.Add(obj);
    }
}