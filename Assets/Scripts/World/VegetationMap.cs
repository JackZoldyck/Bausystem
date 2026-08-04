using UnityEngine;

public class VegetationMap : MonoBehaviour
{
    private enum DebugMode
    {
        ForestRegion,
        ForestPotential,
        OpenLandPotential,
        Moisture,
        Temperature,
        Fertility,
        Height,
        Slope
    }

    [Header("References")]
    [SerializeField] private Terrain terrain;

    [Header("General")]
    [SerializeField] private int seed = 0;

    [Header("Forest Regions")]
    [SerializeField, Min(0.00001f)]
    private float forestNoiseScale = 0.002f;

    [SerializeField, Range(0f, 1f)]
    private float forestThreshold = 0.45f;

    [SerializeField, Min(0.01f)]
    private float forestContrast = 1.5f;

    [Header("Moisture")]
    [SerializeField, Min(0.00001f)]
    private float moistureNoiseScale = 0.0035f;

    [SerializeField, Range(0f, 1f)]
    private float idealForestMoisture = 0.65f;

    [SerializeField, Min(0.01f)]
    private float moistureTolerance = 0.65f;

    [Header("Temperature")]
    [SerializeField, Range(0f, 1f)]
    private float baseTemperature = 0.85f;

    [SerializeField, Range(0f, 1f)]
    private float altitudeCooling = 0.65f;

    [SerializeField, Range(0f, 1f)]
    private float idealForestTemperature = 0.6f;

    [SerializeField, Min(0.01f)]
    private float temperatureTolerance = 0.75f;

    [Header("Terrain Suitability")]
    [SerializeField]
    private AnimationCurve heightSuitability =
        new AnimationCurve(
            new Keyframe(0f, 0.8f),
            new Keyframe(0.2f, 1f),
            new Keyframe(0.65f, 0.8f),
            new Keyframe(1f, 0f)
        );

    [SerializeField]
    private AnimationCurve slopeSuitability =
        new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.3f, 1f),
            new Keyframe(0.6f, 0.25f),
            new Keyframe(1f, 0f)
        );

    [Header("Open Land")]
    [SerializeField, Range(0f, 1f)]
    private float idealOpenLandMoisture = 0.5f;

    [SerializeField, Min(0.01f)]
    private float openLandMoistureTolerance = 0.75f;

    [Header("Debug")]
    [SerializeField] private bool showDebugMap = true;
    [SerializeField]
    private DebugMode debugMode =
        DebugMode.ForestPotential;

    [SerializeField, Min(2)]
    private int debugResolution = 30;

    [SerializeField, Min(0.01f)]
    private float debugPointSize = 0.5f;

    [SerializeField]
    private float debugHeightOffset = 0.25f;

    private Vector2 forestOffset;
    private Vector2 moistureOffset;

    private void Awake()
    {
        Initialize();
    }

    private void OnValidate()
    {
        forestNoiseScale =
            Mathf.Max(0.00001f, forestNoiseScale);

        moistureNoiseScale =
            Mathf.Max(0.00001f, moistureNoiseScale);

        forestContrast =
            Mathf.Max(0.01f, forestContrast);

        moistureTolerance =
            Mathf.Max(0.01f, moistureTolerance);

        temperatureTolerance =
            Mathf.Max(0.01f, temperatureTolerance);

        openLandMoistureTolerance =
            Mathf.Max(0.01f, openLandMoistureTolerance);

        debugResolution =
            Mathf.Max(2, debugResolution);

        debugPointSize =
            Mathf.Max(0.01f, debugPointSize);

        Initialize();
    }

    private void Initialize()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        if (terrain == null)
            terrain = Terrain.activeTerrain;

        System.Random random =
            new System.Random(seed);

        forestOffset = new Vector2(
            random.Next(-100000, 100000),
            random.Next(-100000, 100000)
        );

        moistureOffset = new Vector2(
            random.Next(-100000, 100000),
            random.Next(-100000, 100000)
        );
    }

    public VegetationSample Sample(
        Vector3 worldPosition)
    {
        if (terrain == null)
        {
            Initialize();
        }

        if (terrain == null)
        {
            return new VegetationSample(
                height: 0f,
                slope: 1f,
                moisture: 0f,
                temperature: 0f,
                fertility: 0f,
                forestPotential: 0f,
                openLandPotential: 0f
            );
        }

        float normalizedHeight =
            GetNormalizedHeight(worldPosition);

        float normalizedSlope =
            GetNormalizedSlope(worldPosition);

        float moisture =
            SampleNoise(
                worldPosition.x,
                worldPosition.z,
                moistureNoiseScale,
                moistureOffset
            );

        float temperature =
            CalculateTemperature(normalizedHeight);

        float fertility =
            CalculateFertility(
                normalizedHeight,
                normalizedSlope,
                moisture
            );

        float forestRegion =
            SampleNoise(
                worldPosition.x,
                worldPosition.z,
                forestNoiseScale,
                forestOffset
            );

        float forestPotential =
            CalculateForestPotential(
                forestRegion,
                normalizedHeight,
                normalizedSlope,
                moisture,
                temperature,
                fertility
            );

        float openLandPotential =
            CalculateOpenLandPotential(
                normalizedHeight,
                normalizedSlope,
                moisture,
                fertility,
                forestPotential
            );

        return new VegetationSample(
            normalizedHeight,
            normalizedSlope,
            moisture,
            temperature,
            fertility,
            forestPotential,
            openLandPotential
        );
    }

    public float GetForestPotential(
        Vector3 worldPosition)
    {
        return Sample(worldPosition)
            .ForestPotential;
    }

    // Übergangsmethode für bestehenden Code.
    // WorldSpawner und GrassRenderer funktionieren dadurch weiter.
    public float GetForestDensity(
        Vector3 worldPosition)
    {
        return GetForestRegion(worldPosition);
    }

    private float GetForestRegion(Vector3 worldPosition)
    {
        return SampleNoise(
            worldPosition.x,
            worldPosition.z,
            forestNoiseScale,
            forestOffset
        );
    }

    private float CalculateTemperature(
        float normalizedHeight)
    {
        return Mathf.Clamp01(
            baseTemperature
            - normalizedHeight * altitudeCooling
        );
    }

    private float CalculateFertility(
        float height,
        float slope,
        float moisture)
    {
        float heightFactor =
            Mathf.Clamp01(
                heightSuitability.Evaluate(height)
            );

        float slopeFactor =
            Mathf.Clamp01(
                slopeSuitability.Evaluate(slope)
            );

        float moistureFactor =
            CalculateSuitability(
                moisture,
                idealForestMoisture,
                moistureTolerance
            );

        return Mathf.Clamp01(
            heightFactor
            * slopeFactor
            * moistureFactor
        );
    }

    private float CalculateForestPotential(
        float forestRegion,
        float height,
        float slope,
        float moisture,
        float temperature,
        float fertility)
    {
        float regionFactor =
            Mathf.InverseLerp(
                forestThreshold,
                1f,
                forestRegion
            );

        regionFactor =
            Mathf.Pow(
                Mathf.Clamp01(regionFactor),
                forestContrast
            );

        float heightFactor =
            Mathf.Clamp01(
                heightSuitability.Evaluate(height)
            );

        float slopeFactor =
            Mathf.Clamp01(
                slopeSuitability.Evaluate(slope)
            );

        float moistureFactor =
            CalculateSuitability(
                moisture,
                idealForestMoisture,
                moistureTolerance
            );

        float temperatureFactor =
            CalculateSuitability(
                temperature,
                idealForestTemperature,
                temperatureTolerance
            );

        return Mathf.Clamp01(
            regionFactor
            * fertility
            * temperatureFactor
);
    }

    private float CalculateOpenLandPotential(
        float height,
        float slope,
        float moisture,
        float fertility,
        float forestPotential)
    {
        float heightFactor =
            Mathf.Clamp01(
                heightSuitability.Evaluate(height)
            );

        float slopeFactor =
            Mathf.Clamp01(
                slopeSuitability.Evaluate(slope)
            );

        float moistureFactor =
            CalculateSuitability(
                moisture,
                idealOpenLandMoisture,
                openLandMoistureTolerance
            );

        float forestSuppression =
            1f - forestPotential;

        return Mathf.Clamp01(
            heightFactor
            * slopeFactor
            * moistureFactor
            * fertility
            * forestSuppression
        );
    }

    private float CalculateSuitability(
        float value,
        float idealValue,
        float tolerance)
    {
        float distance =
            Mathf.Abs(value - idealValue);

        return Mathf.Clamp01(
            1f - distance / tolerance
        );
    }

    private float GetNormalizedHeight(
        Vector3 worldPosition)
    {
        TerrainData terrainData =
            terrain.terrainData;

        float localTerrainHeight =
            terrain.SampleHeight(worldPosition);

        return Mathf.InverseLerp(
            0f,
            terrainData.size.y,
            localTerrainHeight
        );
    }

    private float GetNormalizedSlope(
        Vector3 worldPosition)
    {
        TerrainData terrainData =
            terrain.terrainData;

        Vector3 terrainPosition =
            terrain.transform.position;

        float normalizedX =
            (worldPosition.x - terrainPosition.x)
            / terrainData.size.x;

        float normalizedZ =
            (worldPosition.z - terrainPosition.z)
            / terrainData.size.z;

        normalizedX =
            Mathf.Clamp01(normalizedX);

        normalizedZ =
            Mathf.Clamp01(normalizedZ);

        float slopeDegrees =
            terrainData.GetSteepness(
                normalizedX,
                normalizedZ
            );

        return Mathf.InverseLerp(
            0f,
            90f,
            slopeDegrees
        );
    }

    private float SampleNoise(
        float worldX,
        float worldZ,
        float scale,
        Vector2 offset)
    {
        float sampleX =
            worldX * scale + offset.x;

        float sampleZ =
            worldZ * scale + offset.y;

        return Mathf.PerlinNoise(
            sampleX,
            sampleZ
        );
    }

    private float GetDebugValue(
        VegetationSample sample)
    {
        switch (debugMode)
        {
            case DebugMode.ForestPotential:
                return sample.ForestPotential;

            case DebugMode.OpenLandPotential:
                return sample.OpenLandPotential;

            case DebugMode.Moisture:
                return sample.Moisture;

            case DebugMode.Temperature:
                return sample.Temperature;

            case DebugMode.Fertility:
                return sample.Fertility;

            case DebugMode.Height:
                return sample.Height;

            case DebugMode.Slope:
                return sample.Slope;

            default:
                return 0f;
        }
    }

    private Color GetDebugColor(float value)
    {
        value = Mathf.Clamp01(value);

        switch (debugMode)
        {
            case DebugMode.ForestPotential:
                return Color.Lerp(
                    Color.black,
                    Color.green,
                    value
                );

            case DebugMode.OpenLandPotential:
                return Color.Lerp(
                    Color.black,
                    Color.yellow,
                    value
                );

            case DebugMode.Moisture:
                return Color.Lerp(
                    Color.black,
                    Color.blue,
                    value
                );

            case DebugMode.Temperature:
                return Color.Lerp(
                    Color.blue,
                    Color.red,
                    value
                );

            case DebugMode.Fertility:
                return Color.Lerp(
                    Color.black,
                    new Color(0.4f, 1f, 0.2f),
                    value
                );

            case DebugMode.Height:
                return Color.Lerp(
                    Color.black,
                    Color.white,
                    value
                );

            case DebugMode.Slope:
                return Color.Lerp(
                    Color.green,
                    Color.red,
                    value
                );

            default:
                return Color.white;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugMap)
            return;

        if (terrain == null)
            terrain = GetComponent<Terrain>();

        if (terrain == null)
            terrain = Terrain.activeTerrain;

        if (terrain == null)
            return;

        Initialize();

        Vector3 terrainPosition =
            terrain.transform.position;

        Vector3 terrainSize =
            terrain.terrainData.size;

        for (int x = 0; x < debugResolution; x++)
        {
            for (int z = 0; z < debugResolution; z++)
            {
                float normalizedX =
                    x / (float)(debugResolution - 1);

                float normalizedZ =
                    z / (float)(debugResolution - 1);

                float worldX =
                    terrainPosition.x
                    + normalizedX * terrainSize.x;

                float worldZ =
                    terrainPosition.z
                    + normalizedZ * terrainSize.z;

                Vector3 samplePosition =
                    new Vector3(
                        worldX,
                        0f,
                        worldZ
                    );

                float worldY =
                    terrain.SampleHeight(samplePosition)
                    + terrainPosition.y
                    + debugHeightOffset;

                samplePosition.y = worldY;

                VegetationSample sample =
                    Sample(samplePosition);

                float debugValue;

                if (debugMode == DebugMode.ForestRegion)
                {
                    debugValue = GetForestRegion(samplePosition);
                }
                else
                {
                    VegetationSample vegetationSample =
                        Sample(samplePosition);

                    debugValue =
                        GetDebugValue(vegetationSample);
                }

                Gizmos.color =
                    GetDebugColor(debugValue);

                Gizmos.DrawCube(
                    samplePosition,
                    Vector3.one * debugPointSize
                );
            }
        }
    }
}