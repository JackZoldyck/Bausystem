using System.Collections;
using UnityEngine;

public class PickupRespawn : MonoBehaviour
{
    public float respawnTime = 60f;

    private Collider objectCollider;
    private Renderer[] renderers;

    void Awake()
    {
        objectCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Collect()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        // Unsichtbar machen
        foreach (Renderer r in renderers)
            r.enabled = false;

        if (objectCollider != null)
            objectCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // Wieder anzeigen
        foreach (Renderer r in renderers)
            r.enabled = true;

        if (objectCollider != null)
            objectCollider.enabled = true;
    }
}