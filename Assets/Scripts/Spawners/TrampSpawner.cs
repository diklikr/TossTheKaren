using System.Collections.Generic;
using UnityEngine;

public class TrampSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] TrampPrefabs;   // list of prefabs to pick from
    [SerializeField] Transform Player;
    [SerializeField] Collider Ground;             // the ground's 3D collider

    [Header("Spacing (X axis)")]
    [SerializeField] float MinGap = 10f;
    [SerializeField] float MaxGap = 30f;
    [SerializeField] float StartX = -4970f;

    [Header("Spawning")]
    [SerializeField] float SpawnAhead = 60f;
    [SerializeField] float DespawnBehind = 30f;
    [SerializeField] float YOffset = 0f;          // adjust so the prefab pivot sits on the ground

    float nextX;
    readonly List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        MaxGap = Mathf.Max(MaxGap, MinGap);
        nextX = StartX;
    }

    void Update()
    {
        if (Player == null || Ground == null || TrampPrefabs == null || TrampPrefabs.Length == 0) return;

        while (nextX < Player.position.x + SpawnAhead)
        {
            SpawnTramp(nextX);
            nextX += Random.Range(MinGap, MaxGap);
        }

        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] == null)
            {
                spawned.RemoveAt(i);
            }
            else if (spawned[i].transform.position.x < Player.position.x - DespawnBehind)
            {
                Destroy(spawned[i]);
                spawned.RemoveAt(i);
            }
        }
    }

    void SpawnTramp(float x)
    {
        GameObject prefab = TrampPrefabs[Random.Range(0, TrampPrefabs.Length)];
        if (prefab == null) return;

        Bounds b = Ground.bounds;
        Vector3 pos = new Vector3(x, b.max.y + YOffset, b.center.z);
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, transform);

        // Collision only: make sure no collider on the spawned object is a trigger
        foreach (Collider c in obj.GetComponentsInChildren<Collider>())
            c.isTrigger = false;

        spawned.Add(obj);
    }
}