using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    private enum SurfaceAlignment
    {
        Upright,
        FollowTerrain
    }

    [Header("Ground")]
    [SerializeField] private Terrain terrain;

    [SerializeField] private VegetationMap vegetationMap;

    [Header("Prefabs")]
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject branchPrefab;
    public GameObject stonePrefab;
    public GameObject mushroomPrefab;
    public GameObject berryBushPrefab;

    [Header("Height Offsets")]
    public float treeYOffset = 0f;
    public float rockYOffset = 0f;
    public float branchYOffset = 0.15f;
    public float stoneYOffset = 0.15f;
    public float mushroomYOffset = 0f;
    public float berryBushYOffset = 0f;

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
    [Header("Forest Generation")]
    [SerializeField] private int worldSeed = 12345;

    [SerializeField, Min(5f)]
    private float forestCellSize = 80f;

    [SerializeField, Range(0f, 1f)]
    private float forestCenterChance = 0.75f;

    [SerializeField, Range(0f, 1f)]
    private float minimumCenterDensity = 0.52f;

    [SerializeField, Range(0f, 1f)]
    private float minimumTreeDensity = 0.48f;

    [SerializeField, Min(1f)]
    private float minimumForestRadius = 18f;

    [SerializeField, Min(1f)]
    private float maximumForestRadius = 35f;

    [SerializeField, Min(1)]
    private int minimumTreesPerForest = 20;

    [SerializeField, Min(1)]
    private int maximumTreesPerForest = 55;

    [SerializeField, Min(0f)]
    private float minimumTreeDistance = 2.5f;

    [SerializeField, Min(1)]

    [Header("Berry Bush Generation")]

    [Tooltip("Wie viele Berry-Cluster in der Welt erzeugt werden sollen.")]
    public int minBerryClusters = 12;
    public int maxBerryClusters = 20;

    [Tooltip("Anzahl Büsche innerhalb eines Clusters.")]
    public int minBerryBushesPerCluster = 3;
    public int maxBerryBushesPerCluster = 6;

    [Tooltip("Radius eines einzelnen Berry-Clusters.")]
    public float berryClusterRadius = 5f;

    [Tooltip("In diesem Radius wird die Baumdichte geprüft.")]
    public float berryTreeCheckRadius = 15f;

    [Tooltip("Maximal erlaubte Anzahl Bäume im Prüfradius.")]
    public int maxTreesNearBerryCluster = 4;

    [Tooltip("Mindestabstand zwischen einzelnen Berry Bushes.")]
    public float minimumBerryBushDistance = 1.5f;

    [Tooltip("Wie oft nach einem geeigneten Clusterplatz gesucht wird.")]
    public int berryClusterPlacementAttempts = 30;
    private int attemptsPerTree = 8;

    public float branchMinDistanceFromTree = 2f;
    public float branchMaxDistanceFromTree = 10f;

    public float stoneMinDistanceFromRock = 2f;
    public float stoneMaxDistanceFromRock = 9f;

    public float mushroomMinDistance = 2f;
    public float mushroomMaxDistance = 8f;

    public float rockMinDistanceFromTrees = 10f;

    private readonly List<Vector3> treePositions = new();
    private readonly List<Vector3> rockPositions = new();

    private void Awake()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;
    }

    private void Start()
    {
        if (terrain == null)
        {
            enabled = false;
            return;
        }

        SpawnTrees();
        SpawnRocks();

        SpawnNearAnchors(
            branchPrefab,
            Random.Range(minBranches, maxBranches + 1),
            treePositions,
            branchYOffset,
            branchMinDistanceFromTree,
            branchMaxDistanceFromTree,
            SurfaceAlignment.FollowTerrain
        );

        SpawnNearAnchors(
            stonePrefab,
            Random.Range(minStones, maxStones + 1),
            rockPositions,
            stoneYOffset,
            stoneMinDistanceFromRock,
            stoneMaxDistanceFromRock,
            SurfaceAlignment.FollowTerrain
        );

        SpawnMushrooms();
        SpawnBerryBushClusters();
    }

    private void SpawnTrees()
    {
        treePositions.Clear();

        if (treePrefab == null)
        {
            Debug.LogWarning(
                "WorldSpawner: Kein Tree Prefab zugewiesen.",
                this
            );

            return;
        }

        if (vegetationMap == null)
        {
            Debug.LogWarning(
                "WorldSpawner: Keine VegetationMap zugewiesen.",
                this
            );

            return;
        }

        Vector3 terrainPosition =
            terrain.transform.position;

        Vector3 terrainSize =
            terrain.terrainData.size;

        int cellsX = Mathf.CeilToInt(
            terrainSize.x / forestCellSize
        );

        int cellsZ = Mathf.CeilToInt(
            terrainSize.z / forestCellSize
        );

        for (int cellX = 0; cellX < cellsX; cellX++)
        {
            for (int cellZ = 0; cellZ < cellsZ; cellZ++)
            {
                TrySpawnForest(
                    cellX,
                    cellZ,
                    terrainPosition
                );
            }
        }

        Debug.Log(
            $"WorldSpawner: {treePositions.Count} Bäume erzeugt.",
            this
        );
    }

    private void TrySpawnForest(
    int cellX,
    int cellZ,
    Vector3 terrainPosition)
    {
        int cellSeed = CombineSeed(
            worldSeed,
            cellX,
            cellZ
        );

        System.Random random =
            new System.Random(cellSeed);

        if (NextFloat(random) > forestCenterChance)
            return;

        float cellStartX =
            terrainPosition.x
            + cellX * forestCellSize;

        float cellStartZ =
            terrainPosition.z
            + cellZ * forestCellSize;

        float centerX =
            cellStartX
            + NextFloat(random) * forestCellSize;

        float centerZ =
            cellStartZ
            + NextFloat(random) * forestCellSize;

        Vector3 center = new Vector3(
            centerX,
            0f,
            centerZ
        );

        if (!IsInsideTerrainBounds(center))
            return;

        float centerDensity =
            vegetationMap.GetForestDensity(center);

        if (centerDensity < minimumCenterDensity)
            return;

        float radius = Mathf.Lerp(
            minimumForestRadius,
            maximumForestRadius,
            NextFloat(random)
        );

        int treeCount = random.Next(
            minimumTreesPerForest,
            maximumTreesPerForest + 1
        );

        float densityStrength =
            Mathf.InverseLerp(
                minimumCenterDensity,
                1f,
                centerDensity
            );

        treeCount = Mathf.RoundToInt(
            treeCount
            * Mathf.Lerp(
                0.65f,
                1.35f,
                densityStrength
            )
        );

        SpawnForestTrees(
            center,
            radius,
            treeCount,
            random
        );
    }

    private void SpawnForestTrees(
    Vector3 center,
    float radius,
    int targetTreeCount,
    System.Random random)
    {
        int spawnedTrees = 0;
        int attempts = 0;

        int maximumAttempts =
            targetTreeCount * attemptsPerTree;

        while (
            spawnedTrees < targetTreeCount &&
            attempts < maximumAttempts
        )
        {
            attempts++;

            Vector2 offset =
                GetRandomPointInCircle(
                    random,
                    radius
                );

            Vector3 position = new Vector3(
                center.x + offset.x,
                0f,
                center.z + offset.y
            );

            if (!IsInsideTerrainBounds(position))
                continue;

            float forestDensity =
                vegetationMap.GetForestDensity(position);

            if (forestDensity < minimumTreeDensity)
                continue;

            float acceptance =
                Mathf.InverseLerp(
                    minimumTreeDensity,
                    1f,
                    forestDensity
                );

            if (NextFloat(random) > acceptance)
                continue;

            if (!IsFarEnoughFromTrees(position))
                continue;

            position =
                PlaceOnTerrain(
                    position,
                    treeYOffset
                );

            SpawnTreeDeterministic(
                position,
                random
            );

            treePositions.Add(position);
            spawnedTrees++;
        }
    }

    private Vector2 GetRandomPointInCircle(
    System.Random random,
    float radius)
    {
        float angle =
            NextFloat(random)
            * Mathf.PI
            * 2f;

        // Sqrt sorgt für eine gleichmäßige Verteilung
        // über die gesamte Kreisfläche.
        float distance =
            Mathf.Sqrt(NextFloat(random))
            * radius;

        return new Vector2(
            Mathf.Cos(angle) * distance,
            Mathf.Sin(angle) * distance
        );
    }

    private bool IsInsideTerrainBounds(
    Vector3 position)
    {
        Vector3 terrainPosition =
            terrain.transform.position;

        Vector3 terrainSize =
            terrain.terrainData.size;

        return
            position.x >= terrainPosition.x &&
            position.x <= terrainPosition.x + terrainSize.x &&
            position.z >= terrainPosition.z &&
            position.z <= terrainPosition.z + terrainSize.z;
    }

    private bool IsFarEnoughFromTrees(
    Vector3 position)
    {
        if (minimumTreeDistance <= 0f)
            return true;

        float minimumDistanceSquared =
            minimumTreeDistance
            * minimumTreeDistance;

        foreach (Vector3 treePosition in treePositions)
        {
            float deltaX =
                position.x - treePosition.x;

            float deltaZ =
                position.z - treePosition.z;

            float distanceSquared =
                deltaX * deltaX
                + deltaZ * deltaZ;

            if (distanceSquared < minimumDistanceSquared)
                return false;
        }

        return true;
    }

    private void SpawnTreeDeterministic(
    Vector3 position,
    System.Random random)
    {
        if (treePrefab == null)
            return;

        float randomYaw =
            NextFloat(random) * 360f;

        Quaternion rotation =
            Quaternion.Euler(
                0f,
                randomYaw,
                0f
            );

        Instantiate(
            treePrefab,
            position,
            rotation
        );
    }

    private static float NextFloat(
    System.Random random)
    {
        return (float)random.NextDouble();
    }

    private static int CombineSeed(
    int seed,
    int cellX,
    int cellZ)
    {
        unchecked
        {
            int hash = seed;

            hash = hash * 397 ^ cellX;
            hash = hash * 397 ^ cellZ;

            return hash;
        }
    }

    private void SpawnRocks()
    {
        int amount = Random.Range(minRocks, maxRocks + 1);

        for (int i = 0; i < amount; i++)
        {
            Vector3 position = GetOpenPositionAwayFromTrees();
            position = PlaceOnTerrain(position, rockYOffset);

            Spawn(
                rockPrefab,
                position,
                SurfaceAlignment.FollowTerrain
            );

            rockPositions.Add(position);
        }
    }

    private void SpawnMushrooms()
    {
        int amount = Random.Range(
            minMushrooms,
            maxMushrooms + 1
        );

        List<Vector3> anchors = new();
        anchors.AddRange(treePositions);
        anchors.AddRange(rockPositions);

        SpawnNearAnchors(
            mushroomPrefab,
            amount,
            anchors,
            mushroomYOffset,
            mushroomMinDistance,
            mushroomMaxDistance,
            SurfaceAlignment.Upright
        );
    }

    private void SpawnBerryBushClusters()
    {
        if (berryBushPrefab == null)
        {
            Debug.LogWarning(
                "WorldSpawner: Kein Berry Bush Prefab zugewiesen.",
                this
            );

            return;
        }

        int targetClusterCount = Random.Range(
            minBerryClusters,
            maxBerryClusters + 1
        );

        int spawnedClusters = 0;
        int attempts = 0;

        int maximumAttempts =
            targetClusterCount * berryClusterPlacementAttempts;

        List<Vector3> berryBushPositions = new();

        while (
            spawnedClusters < targetClusterCount &&
            attempts < maximumAttempts
        )
        {
            attempts++;

            Vector3 clusterCenter =
                GetRandomPosition();

            // Cluster nur dort zulassen,
            // wo nicht zu viele Bäume stehen.
            if (!IsGoodBerryClusterPosition(clusterCenter))
                continue;

            int bushCount = Random.Range(
                minBerryBushesPerCluster,
                maxBerryBushesPerCluster + 1
            );

            int spawnedInCluster = 0;

            for (int i = 0; i < bushCount; i++)
            {
                const int bushPlacementAttempts = 10;

                for (
                    int bushAttempt = 0;
                    bushAttempt < bushPlacementAttempts;
                    bushAttempt++
                )
                {
                    Vector2 offset =
                        Random.insideUnitCircle *
                        berryClusterRadius;

                    Vector3 position =
                        clusterCenter +
                        new Vector3(
                            offset.x,
                            0f,
                            offset.y
                        );

                    if (!IsInsideTerrainBounds(position))
                        continue;

                    if (!IsFarEnoughFromBerryBushes(
                        position,
                        berryBushPositions))
                    {
                        continue;
                    }

                    position = PlaceOnTerrain(
                        position,
                        berryBushYOffset
                    );

                    Spawn(
                        berryBushPrefab,
                        position,
                        SurfaceAlignment.Upright
                    );

                    berryBushPositions.Add(position);

                    spawnedInCluster++;

                    break;
                }
            }

            // Nur als erfolgreicher Cluster zählen,
            // wenn wenigstens ein Busch gespawnt wurde.
            if (spawnedInCluster > 0)
                spawnedClusters++;
        }

        Debug.Log(
            $"WorldSpawner: {berryBushPositions.Count} Berry Bushes " +
            $"in {spawnedClusters} Clustern erzeugt.",
            this
        );
    }

    private bool IsGoodBerryClusterPosition(
    Vector3 position)
    {
        int nearbyTrees = 0;

        float checkRadiusSquared =
            berryTreeCheckRadius *
            berryTreeCheckRadius;

        foreach (Vector3 treePosition in treePositions)
        {
            float deltaX =
                position.x - treePosition.x;

            float deltaZ =
                position.z - treePosition.z;

            float distanceSquared =
                deltaX * deltaX +
                deltaZ * deltaZ;

            if (distanceSquared <= checkRadiusSquared)
            {
                nearbyTrees++;

                if (nearbyTrees >
                    maxTreesNearBerryCluster)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool IsFarEnoughFromBerryBushes(
    Vector3 position,
    List<Vector3> existingBushes)
    {
        float minimumDistanceSquared =
            minimumBerryBushDistance *
            minimumBerryBushDistance;

        foreach (Vector3 bushPosition in existingBushes)
        {
            float deltaX =
                position.x - bushPosition.x;

            float deltaZ =
                position.z - bushPosition.z;

            float distanceSquared =
                deltaX * deltaX +
                deltaZ * deltaZ;

            if (distanceSquared <
                minimumDistanceSquared)
            {
                return false;
            }
        }

        return true;
    }

    private void SpawnNearAnchors(
        GameObject prefab,
        int amount,
        List<Vector3> anchors,
        float yOffset,
        float minDistance,
        float maxDistance,
        SurfaceAlignment alignment
    )
    {
        if (
            prefab == null ||
            anchors == null ||
            anchors.Count == 0
        )
        {
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            Vector3 anchor =
                anchors[Random.Range(0, anchors.Count)];

            Vector3 position = GetRandomPositionNear(
                anchor,
                minDistance,
                maxDistance
            );

            position = PlaceOnTerrain(position, yOffset);

            Spawn(prefab, position, alignment);
        }
    }

    private Vector3 GetOpenPositionAwayFromTrees()
    {
        const int maximumAttempts = 30;

        for (int attempt = 0; attempt < maximumAttempts; attempt++)
        {
            Vector3 position = GetRandomPosition();
            bool tooCloseToTree = false;

            foreach (Vector3 treePosition in treePositions)
            {
                if (
                    GetHorizontalDistance(position, treePosition) <
                    rockMinDistanceFromTrees
                )
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

    private Vector3 GetRandomPositionNear(
        Vector3 center,
        float minDistance,
        float maxDistance
    )
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(minDistance, maxDistance);

        Vector3 position = center + new Vector3(
            Mathf.Cos(angle) * distance,
            0f,
            Mathf.Sin(angle) * distance
        );

        ClampToTerrainBounds(ref position);

        return position;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        float x = Random.Range(
            terrainPosition.x,
            terrainPosition.x + terrainSize.x
        );

        float z = Random.Range(
            terrainPosition.z,
            terrainPosition.z + terrainSize.z
        );

        return new Vector3(x, 0f, z);
    }

    private Vector3 PlaceOnTerrain(Vector3 position, float yOffset)
    {
        float sampledHeight = terrain.SampleHeight(position);

        position.y =
            sampledHeight +
            terrain.transform.position.y +
            yOffset;

        return position;
    }

    private Vector3 GetTerrainNormal(Vector3 worldPosition)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        float normalizedX =
            (worldPosition.x - terrainPosition.x) /
            terrainSize.x;

        float normalizedZ =
            (worldPosition.z - terrainPosition.z) /
            terrainSize.z;

        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedZ = Mathf.Clamp01(normalizedZ);

        return terrain.terrainData.GetInterpolatedNormal(
            normalizedX,
            normalizedZ
        );
    }

    private Quaternion GetSpawnRotation(
        Vector3 position,
        SurfaceAlignment alignment
    )
    {
        float randomYaw = Random.Range(0f, 360f);

        if (alignment == SurfaceAlignment.Upright)
        {
            return Quaternion.Euler(
                0f,
                randomYaw,
                0f
            );
        }

        Vector3 terrainNormal = GetTerrainNormal(position);

        Quaternion surfaceRotation =
            Quaternion.FromToRotation(
                Vector3.up,
                terrainNormal
            );

        Quaternion yawRotation =
            Quaternion.AngleAxis(
                randomYaw,
                terrainNormal
            );

        return yawRotation * surfaceRotation;
    }

    private void Spawn(
     GameObject prefab,
     Vector3 position,
     SurfaceAlignment alignment)
    {
        if (prefab == null)
            return;

        GameObject instance = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        float randomYaw = Random.Range(0f, 360f);

        instance.transform.rotation =
            Quaternion.Euler(0f, randomYaw, 0f);

        if (alignment == SurfaceAlignment.FollowTerrain)
        {
            Transform visual =
                instance.transform.Find("Visual");

            if (visual != null)
            {
                Vector3 terrainNormal =
                    GetTerrainNormal(position);

                Quaternion slopeRotation =
                    Quaternion.FromToRotation(
                        Vector3.up,
                        terrainNormal
                    );

                visual.rotation =
                    slopeRotation * visual.rotation;
            }
        }
    }

    private void ClampToTerrainBounds(ref Vector3 position)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        position.x = Mathf.Clamp(
            position.x,
            terrainPosition.x,
            terrainPosition.x + terrainSize.x
        );

        position.z = Mathf.Clamp(
            position.z,
            terrainPosition.z,
            terrainPosition.z + terrainSize.z
        );
    }

    private static float GetHorizontalDistance(
        Vector3 first,
        Vector3 second
    )
    {
        float deltaX = first.x - second.x;
        float deltaZ = first.z - second.z;

        return Mathf.Sqrt(
            deltaX * deltaX +
            deltaZ * deltaZ
        );
    }
}