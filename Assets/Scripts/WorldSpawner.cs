using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [Header("Ground")]
    public Transform ground;
    public float worldSizeX = 400f;
    public float worldSizeZ = 400f;

    [Header("Prefabs")]
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject branchPrefab;
    public GameObject stonePrefab;
    public GameObject mushroomPrefab;

    [Header("Height Offsets")]
    public float treeYOffset = 0f;
    public float rockYOffset = 0f;
    public float branchYOffset = 0.15f;
    public float stoneYOffset = 0.15f;
    public float mushroomYOffset = 0f;

    [Header("Spawn Counts")]
    public int minTrees = 20;
    public int maxTrees = 40;
    public int minRocks = 10;
    public int maxRocks = 20;
    public int minBranches = 20;
    public int maxBranches = 40;
    public int minStones = 20;
    public int maxStones = 40;
    public int minMushrooms = 10;
    public int maxMushrooms = 20;

    [Header("Biome Rules")]
    public float treeClusterRadius = 18f;
    public float treeClusterChance = 0.65f;

    public float branchMinDistanceFromTree = 2f;
    public float branchMaxDistanceFromTree = 10f;

    public float stoneMinDistanceFromRock = 2f;
    public float stoneMaxDistanceFromRock = 9f;

    public float mushroomMinDistance = 2f;
    public float mushroomMaxDistance = 8f;

    public float rockMinDistanceFromTrees = 10f;

    private List<Vector3> treePositions = new();
    private List<Vector3> rockPositions = new();

    void Start()
    {
        SpawnTrees();
        SpawnRocks();

        SpawnNearAnchors(branchPrefab, Random.Range(minBranches, maxBranches + 1), treePositions, branchYOffset, branchMinDistanceFromTree, branchMaxDistanceFromTree);
        SpawnNearAnchors(stonePrefab, Random.Range(minStones, maxStones + 1), rockPositions, stoneYOffset, stoneMinDistanceFromRock, stoneMaxDistanceFromRock);
        SpawnMushrooms();
    }

    void SpawnTrees()
    {
        int amount = Random.Range(minTrees, maxTrees + 1);

        for (int i = 0; i < amount; i++)
        {
            Vector3 position;

            if (treePositions.Count > 0 && Random.value < treeClusterChance)
            {
                Vector3 anchor = treePositions[Random.Range(0, treePositions.Count)];
                position = GetRandomPositionNear(anchor, 4f, treeClusterRadius);
            }
            else
            {
                position = GetRandomPosition();
            }

            position.y += treeYOffset;
            Spawn(treePrefab, position);
            treePositions.Add(position);
        }
    }

    void SpawnRocks()
    {
        int amount = Random.Range(minRocks, maxRocks + 1);

        for (int i = 0; i < amount; i++)
        {
            Vector3 position = GetOpenPositionAwayFromTrees();
            position.y += rockYOffset;

            Spawn(rockPrefab, position);
            rockPositions.Add(position);
        }
    }

    void SpawnMushrooms()
    {
        int amount = Random.Range(minMushrooms, maxMushrooms + 1);

        List<Vector3> anchors = new();
        anchors.AddRange(treePositions);
        anchors.AddRange(rockPositions);

        SpawnNearAnchors(
            mushroomPrefab,
            amount,
            anchors,
            mushroomYOffset,
            mushroomMinDistance,
            mushroomMaxDistance
        );
    }

    void SpawnNearAnchors(
        GameObject prefab,
        int amount,
        List<Vector3> anchors,
        float yOffset,
        float minDistance,
        float maxDistance
    )
    {
        if (prefab == null || anchors == null || anchors.Count == 0)
            return;

        for (int i = 0; i < amount; i++)
        {
            Vector3 anchor = anchors[Random.Range(0, anchors.Count)];
            Vector3 position = GetRandomPositionNear(anchor, minDistance, maxDistance);

            position.y = ground.position.y + yOffset;

            Spawn(prefab, position);
        }
    }

    Vector3 GetOpenPositionAwayFromTrees()
    {
        for (int attempt = 0; attempt < 30; attempt++)
        {
            Vector3 position = GetRandomPosition();

            bool tooCloseToTree = false;

            foreach (Vector3 treePosition in treePositions)
            {
                if (Vector3.Distance(position, treePosition) < rockMinDistanceFromTrees)
                {
                    tooCloseToTree = true;
                    break;
                }
            }

            if (!tooCloseToTree)
                return position;
        }

        return GetRandomPosition();
    }

    Vector3 GetRandomPositionNear(Vector3 center, float minDistance, float maxDistance)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(minDistance, maxDistance);

        Vector3 position = center + new Vector3(
            Mathf.Cos(angle) * distance,
            0f,
            Mathf.Sin(angle) * distance
        );

        position.x = Mathf.Clamp(position.x, -worldSizeX * 0.5f, worldSizeX * 0.5f);
        position.z = Mathf.Clamp(position.z, -worldSizeZ * 0.5f, worldSizeZ * 0.5f);
        position.y = ground.position.y;

        return position;
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-worldSizeX * 0.5f, worldSizeX * 0.5f);
        float z = Random.Range(-worldSizeZ * 0.5f, worldSizeZ * 0.5f);
        float y = ground.position.y;

        return new Vector3(x, y, z);
    }

    void Spawn(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
            return;

        Instantiate(
            prefab,
            position,
            Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
        );
    }
}