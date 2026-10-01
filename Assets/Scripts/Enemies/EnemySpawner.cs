using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Timing")]
    public float corpseLifetime = 5f;
    public float respawnTime = 60f;

    private GameObject currentEnemy;
    private bool respawnRunning;

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: Enemy Prefab fehlt!");
            return;
        }

        currentEnemy = Instantiate(
            enemyPrefab,
            transform.position,
            transform.rotation
        );

        EnemyHealth enemyHealth =
            currentEnemy.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.enemySpawner = this;
        }
    }

    public void EnemyDied(GameObject enemy)
    {
        if (respawnRunning)
            return;

        StartCoroutine(
            HandleDeathAndRespawn(enemy)
        );
    }

    private IEnumerator HandleDeathAndRespawn(
        GameObject enemy
    )
    {
        respawnRunning = true;

        yield return new WaitForSeconds(
            corpseLifetime
        );

        if (enemy != null)
        {
            Destroy(enemy);
        }

        float remainingRespawnTime =
            Mathf.Max(
                0f,
                respawnTime - corpseLifetime
            );

        yield return new WaitForSeconds(
            remainingRespawnTime
        );

        SpawnEnemy();

        respawnRunning = false;
    }
}