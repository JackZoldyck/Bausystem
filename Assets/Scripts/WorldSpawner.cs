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

    void Start()
    {
        SpawnObjects(treePrefab, Random.Range(minTrees, maxTrees + 1), treeYOffset);
        SpawnObjects(rockPrefab, Random.Range(minRocks, maxRocks + 1), rockYOffset);
        SpawnObjects(branchPrefab, Random.Range(minBranches, maxBranches + 1), branchYOffset);
        SpawnObjects(stonePrefab, Random.Range(minStones, maxStones + 1), stoneYOffset);
        SpawnObjects(mushroomPrefab, Random.Range(minMushrooms, maxMushrooms + 1), mushroomYOffset);
    }

    void SpawnObjects(GameObject prefab, int amount, float yOffset)
    {
        if (prefab == null)
            return;

        for (int i = 0; i < amount; i++)
        {
            Vector3 position = GetRandomPosition();
            position.y += yOffset;

            Instantiate(
                prefab,
                position,
                Quaternion.Euler(0, Random.Range(0f, 360f), 0)
            );
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(
            -worldSizeX * 0.5f,
             worldSizeX * 0.5f
        );

        float z = Random.Range(
            -worldSizeZ * 0.5f,
             worldSizeZ * 0.5f
        );

        float y = ground.position.y;

        return new Vector3(x, y, z);
    }
}