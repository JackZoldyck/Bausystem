using UnityEngine;

public class PlayerGravestoneSpawner : MonoBehaviour
{
    public GameObject gravestonePrefab;

    [Header("Ground Placement")]
    public LayerMask groundMask;
    public float raycastHeight = 3f;
    public float groundOffset = 0f;

    public void SpawnGravestone()
    {
        if (gravestonePrefab == null)
        {
            Debug.LogError(
                "GRABSTEIN: Gravestone Prefab ist NICHT zugewiesen!"
            );

            return;
        }

        Vector3 spawnPosition = transform.position;

        Vector3 rayStart =
            transform.position +
            Vector3.up * raycastHeight;

        if (Physics.Raycast(
            rayStart,
            Vector3.down,
            out RaycastHit hit,
            raycastHeight * 2f,
            groundMask,
            QueryTriggerInteraction.Ignore))
        {
            spawnPosition =
                hit.point +
                Vector3.up * groundOffset;
        }
        else
        {
            Debug.LogWarning(
                "GRABSTEIN: Kein Boden gefunden!"
            );
        }

        Instantiate(
            gravestonePrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}