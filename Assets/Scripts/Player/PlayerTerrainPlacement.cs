using UnityEngine;

public class PlayerTerrainPlacement : MonoBehaviour
{
    [Header("Ground")]
    [SerializeField] private Terrain terrain;

    [Header("Placement")]
    [Tooltip("Kleiner Abstand über dem Boden, damit der Collider nicht im Terrain startet.")]
    [Min(0f)]
    [SerializeField] private float groundOffset = 0.1f;

    private void Awake()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;
    }

    private void Start()
    {
        PlacePlayerOnTerrain();
    }

    [ContextMenu("Place Player On Terrain")]
    public void PlacePlayerOnTerrain()
    {
        if (terrain == null)
        {
            Debug.LogError(
                "PlayerTerrainPlacement: Kein Terrain gefunden.",
                this
            );
            return;
        }

        Vector3 position = transform.position;

        float terrainHeight =
            terrain.SampleHeight(position) +
            terrain.transform.position.y;

        position.y = terrainHeight + GetRequiredOffset();
        transform.position = position;
    }

    private float GetRequiredOffset()
    {
        CharacterController controller =
            GetComponent<CharacterController>();

        if (controller != null)
        {
            return controller.height * 0.5f
                 - controller.center.y
                 + controller.skinWidth;
        }

        return groundOffset;
    }
}