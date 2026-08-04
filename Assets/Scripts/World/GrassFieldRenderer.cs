using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrassFieldRenderer : MonoBehaviour
{
    private const int MaxInstancesPerBatch = 1023;

    [Header("References")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private Mesh grassMesh;
    [SerializeField] private Material grassMaterial;
    [SerializeField] private VegetationMap vegetationMap;

    [Header("Area")]
    [SerializeField, Min(1f)] private float areaSize = 50f;
    [SerializeField, Min(1)] private int grassCount = 10000;
    [SerializeField] private Vector3 areaCenter;

    [Header("Variation")]
    [SerializeField] private Vector2 heightScaleRange = new(0.75f, 1.2f);
    [SerializeField] private Vector2 widthScaleRange = new(0.85f, 1.15f);
    [SerializeField, Range(0f, 90f)] private float maximumSlope = 35f;
    [SerializeField] private float groundOffset = 0.01f;
    [SerializeField] private int seed = 12345;

    [Header("Rendering")]
    [SerializeField] private bool castShadows = true;
    [SerializeField] private bool receiveShadows = true;

    [Header("Vegetation")]
    [SerializeField, Range(0f, 1f)]
    private float minimumGrassChanceInForest = 0.15f;

    private readonly List<Matrix4x4[]> batches = new();

    private void Start()
    {
        Generate();
    }

    private void LateUpdate()
    {
        Draw();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        batches.Clear();

        if (terrain == null || grassMesh == null || grassMaterial == null)
        {
            Debug.LogError("Terrain, Grass Mesh und Grass Material müssen gesetzt sein.", this);
            return;
        }

        Random.State previousState = Random.state;
        Random.InitState(seed);

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;

        List<Matrix4x4> currentBatch =
            new List<Matrix4x4>(MaxInstancesPerBatch);

        int created = 0;
        int attempts = 0;
        int maximumAttempts = grassCount * 5;

        while (created < grassCount && attempts < maximumAttempts)
        {
            attempts++;

            float halfSize = areaSize * 0.5f;

            float worldX = areaCenter.x + Random.Range(-halfSize, halfSize);
            float worldZ = areaCenter.z + Random.Range(-halfSize, halfSize);

            float normalizedX =
                (worldX - terrainPosition.x) / terrainData.size.x;

            float normalizedZ =
                (worldZ - terrainPosition.z) / terrainData.size.z;

            if (normalizedX < 0f || normalizedX > 1f ||
                normalizedZ < 0f || normalizedZ > 1f)
            {
                continue;
            }

            Vector3 normal =
                terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);

            float slope = Vector3.Angle(normal, Vector3.up);

            if (slope > maximumSlope)
                continue;

            float worldY =
                terrain.SampleHeight(new Vector3(worldX, 0f, worldZ))
                + terrainPosition.y
                + groundOffset;

            Vector3 position = new Vector3(worldX, worldY, worldZ);

            float forest =
                vegetationMap.GetForestDensity(position);

            float grassChance =
                Mathf.Lerp(1f, minimumGrassChanceInForest, forest);

            if (Random.value > grassChance)
                continue;

            Quaternion rotation =
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            float heightScale =
                Random.Range(heightScaleRange.x, heightScaleRange.y);

            float widthScale =
                Random.Range(widthScaleRange.x, widthScaleRange.y);

            Vector3 scale =
                new Vector3(widthScale, heightScale, widthScale);

            currentBatch.Add(
                Matrix4x4.TRS(position, rotation, scale)
            );

            created++;

            if (currentBatch.Count == MaxInstancesPerBatch)
            {
                batches.Add(currentBatch.ToArray());
                currentBatch.Clear();
            }
        }

        if (currentBatch.Count > 0)
            batches.Add(currentBatch.ToArray());

        Random.state = previousState;
    }

    private void Draw()
    {
        if (grassMesh == null || grassMaterial == null)
            return;

        ShadowCastingMode shadowMode =
            castShadows
                ? ShadowCastingMode.TwoSided
                : ShadowCastingMode.Off;

        foreach (Matrix4x4[] batch in batches)
        {
            Graphics.DrawMeshInstanced(
                grassMesh,
                0,
                grassMaterial,
                batch,
                batch.Length,
                null,
                shadowMode,
                receiveShadows,
                gameObject.layer
            );
        }
    }
}