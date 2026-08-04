using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private int seed = 12345;

    [Header("Large Landscape")]
    [Tooltip("Breite der großen Hügel und Täler. Höher = breiter und ruhiger.")]
    [Min(1f)]
    [SerializeField] private float largeNoiseScale = 180f;

    [Tooltip("Anteil der großen Landschaftsformen.")]
    [Range(0f, 1f)]
    [SerializeField] private float largeNoiseWeight = 0.75f;

    [Header("Smaller Terrain Detail")]
    [Tooltip("Breite der mittleren und kleineren Unebenheiten.")]
    [Min(1f)]
    [SerializeField] private float detailNoiseScale = 70f;

    [Tooltip("Anteil der kleineren Geländeformen.")]
    [Range(0f, 1f)]
    [SerializeField] private float detailNoiseWeight = 0.25f;

    [Tooltip("Tatsächliche maximale Landschaftsauslenkung in Metern.")]
    [Min(0f)]
    [SerializeField] private float hillHeight = 10f;

    [Header("Detail fBM")]
    [Range(1, 8)]
    [SerializeField] private int octaves = 4;

    [Range(0f, 1f)]
    [SerializeField] private float persistence = 0.45f;

    [Min(1f)]
    [SerializeField] private float lacunarity = 2f;

    [Header("Shape")]
    [Tooltip("Unter 1 macht die Landschaft runder. Über 1 verbreitert Täler und betont Hügel.")]
    [Range(0.5f, 3f)]
    [SerializeField] private float heightCurvePower = 1.35f;

    [Tooltip("Hebt das gesamte Gelände in Metern an.")]
    [Min(0f)]
    [SerializeField] private float baseHeightMeters = 1.5f;

    [Header("Spawn Area")]
    [SerializeField] private bool flattenWorldCenter = true;

    [Tooltip("Radius des weitgehend flachen Startbereichs.")]
    [Min(0f)]
    [SerializeField] private float flatCenterRadius = 25f;

    [Tooltip("Breite des weichen Übergangs vom Startbereich zur Landschaft.")]
    [Min(0.01f)]
    [SerializeField] private float flatCenterBlend = 70f;

    private Terrain terrain;
    private TerrainData terrainData;

    private void Awake()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        if (generateOnStart)
            GenerateTerrain();
    }

    [ContextMenu("Generate Terrain")]
    public void GenerateTerrain()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        terrainData = terrain.terrainData;

        int resolution = terrainData.heightmapResolution;
        float[,] heights = new float[resolution, resolution];

        System.Random random = new System.Random(seed);

        float largeOffsetX = random.Next(-100000, 100000);
        float largeOffsetZ = random.Next(-100000, 100000);
        float detailOffsetX = random.Next(-100000, 100000);
        float detailOffsetZ = random.Next(-100000, 100000);

        float terrainHeight = Mathf.Max(0.01f, terrainData.size.y);
        float normalizedHillHeight = hillHeight / terrainHeight;
        float normalizedBaseHeight = baseHeightMeters / terrainHeight;

        Vector2 terrainCenter = new Vector2(
            terrainData.size.x * 0.5f,
            terrainData.size.z * 0.5f
        );

        float totalWeight = Mathf.Max(
            0.0001f,
            largeNoiseWeight + detailNoiseWeight
        );

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float worldX =
                    (float)x / (resolution - 1) * terrainData.size.x;

                float worldZ =
                    (float)z / (resolution - 1) * terrainData.size.z;

                // Große, breite Landschaftsformen.
                float largeNoise = Mathf.PerlinNoise(
                    (worldX + largeOffsetX) / largeNoiseScale,
                    (worldZ + largeOffsetZ) / largeNoiseScale
                );

                // Mittlere und kleine Details über mehrere Oktaven.
                float detailNoise = GetFractalNoise(
                    worldX,
                    worldZ,
                    detailOffsetX,
                    detailOffsetZ
                );

                float combinedNoise =
                    (
                        largeNoise * largeNoiseWeight +
                        detailNoise * detailNoiseWeight
                    )
                    / totalWeight;

                // Formt breitere Täler und weniger gleichmäßige Wellen.
                combinedNoise = Mathf.Pow(
                    Mathf.Clamp01(combinedNoise),
                    heightCurvePower
                );

                float landscapeHeight =
                    combinedNoise * normalizedHillHeight;

                if (flattenWorldCenter)
                {
                    float distanceFromCenter = Vector2.Distance(
                        new Vector2(worldX, worldZ),
                        terrainCenter
                    );

                    float flattenFactor = Mathf.InverseLerp(
                        flatCenterRadius,
                        flatCenterRadius + flatCenterBlend,
                        distanceFromCenter
                    );

                    flattenFactor = Mathf.SmoothStep(
                        0f,
                        1f,
                        flattenFactor
                    );

                    landscapeHeight *= flattenFactor;
                }

                heights[z, x] = Mathf.Clamp01(
                    normalizedBaseHeight + landscapeHeight
                );
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    private float GetFractalNoise(
        float worldX,
        float worldZ,
        float offsetX,
        float offsetZ
    )
    {
        float amplitude = 1f;
        float frequency = 1f;
        float noiseSum = 0f;
        float amplitudeSum = 0f;

        for (int octave = 0; octave < octaves; octave++)
        {
            float sampleX =
                ((worldX + offsetX) / detailNoiseScale) * frequency;

            float sampleZ =
                ((worldZ + offsetZ) / detailNoiseScale) * frequency;

            float noise = Mathf.PerlinNoise(sampleX, sampleZ);

            noiseSum += noise * amplitude;
            amplitudeSum += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return amplitudeSum > 0f
            ? noiseSum / amplitudeSum
            : 0f;
    }

    [ContextMenu("Flatten Terrain")]
    public void FlattenTerrain()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        terrainData = terrain.terrainData;

        int resolution = terrainData.heightmapResolution;
        float[,] heights = new float[resolution, resolution];

        terrainData.SetHeights(0, 0, heights);
    }
}