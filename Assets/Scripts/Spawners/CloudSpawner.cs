using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] GameObject CloudPrefab;   // prefab with the Bounce script
    [SerializeField] Transform Player;

    [Header("Spacing (X axis)")]
    [SerializeField] float CloudWidth = 5f;    // width of one cloud
    [SerializeField] float MinGap = 5f;        // empty space between clouds
    [SerializeField] float MaxGap = 20f;       // largest random gap
    [SerializeField] float StartX = -4970f;       // where the first cloud can appear

    [Header("Height (Y axis)")]
    [SerializeField] float MinY = 15f;
    [SerializeField] float MaxY = 50f;

    [Header("Spawning")]
    [SerializeField] float SpawnAhead = 40f;   // keep clouds spawned this far in front of the player
    [SerializeField] float DespawnBehind = 30f; // remove clouds this far behind the player

    float nextX;
    readonly List<GameObject> clouds = new List<GameObject>();

    void Start()
    {
        // Make sure the minimum gap is never smaller than one cloud
        MinGap = Mathf.Max(MinGap, CloudWidth);
        MaxGap = Mathf.Max(MaxGap, MinGap);

        nextX = StartX;
    }

    void Update()
    {
        if (Player == null || CloudPrefab == null) return;

        // Spawn clouds until there are enough ahead of the player
        while (nextX < Player.position.x + SpawnAhead)
        {
            SpawnCloud(nextX);

            // Next cloud: this cloud's width plus a random gap of at least one cloud
            nextX += CloudWidth + Random.Range(MinGap, MaxGap);
        }

        // Remove clouds left far behind
        for (int i = clouds.Count - 1; i >= 0; i--)
        {
            if (clouds[i] == null)
            {
                clouds.RemoveAt(i);
            }
            else if (clouds[i].transform.position.x < Player.position.x - DespawnBehind)
            {
                Destroy(clouds[i]);
                clouds.RemoveAt(i);
            }
        }
    }

    void SpawnCloud(float x)
    {
        Vector3 pos = new Vector3(x, Random.Range(MinY, MaxY), 0f);
        GameObject cloud = Instantiate(CloudPrefab, pos, Quaternion.identity, transform);
        clouds.Add(cloud);
    }
}