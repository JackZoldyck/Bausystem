using UnityEngine;
public class RespawnManager : MonoBehaviour

{
    [Header("References")]
    public PlayerHealth playerHealth;
    public CharacterController characterController;

    [Header("Respawn")]
    public Transform respawnPoint;
    public Vector3 fallbackSpawnPosition = Vector3.zero;

    public void RespawnPlayer()
    {
        if (playerHealth == null)
        {
            Debug.LogError("RespawnManager: PlayerHealth fehlt!");
            return;
        }

        Vector3 spawnPosition;

        if (respawnPoint != null)
        {
            spawnPosition = respawnPoint.position;

            Debug.Log(
                $"Respawn am eigenen RespawnPoint: {spawnPosition}"
            );
        }
        else
        {
            spawnPosition = fallbackSpawnPosition;

            Debug.Log(
                $"Kein RespawnPoint vorhanden. Respawn bei: {spawnPosition}"
            );
        }

        spawnPosition =
            GetGroundedSpawnPosition(spawnPosition);

        if (characterController != null)
            characterController.enabled = false;

        playerHealth.transform.position =
            spawnPosition;

        if (characterController != null)
            characterController.enabled = true;

        playerHealth.Respawn();
    }

    private Vector3 GetGroundedSpawnPosition(Vector3 targetPosition)
    {
        Terrain terrain = Terrain.activeTerrain;

        if (terrain != null)
        {
            float terrainY =
                terrain.SampleHeight(targetPosition) +
                terrain.transform.position.y;

            targetPosition.y = terrainY + 0.1f;
        }

        return targetPosition;
    }
}