using UnityEngine;

public class ResourceHarvester : MonoBehaviour
{
    public Camera playerCamera;
    public float harvestDistance = 4f;
    public PlayerInventory inventory;
    public BuildManager buildManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (buildManager.CurrentPrefab != null)
                return;

            TryHarvest();
        }
    }

    void TryHarvest()
    {
        Debug.Log("Linksklick erkannt");

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, harvestDistance))
        {
            Debug.Log("Getroffen: " + hit.collider.name);

            ResourceNode node = hit.collider.GetComponentInParent<ResourceNode>();

            if (node != null)
            {
                Debug.Log("ResourceNode gefunden");
                node.Harvest(inventory);
            }
        }
        else
        {
            Debug.Log("Nichts getroffen");
        }
    }
}