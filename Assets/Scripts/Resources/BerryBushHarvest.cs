using System.Collections;
using UnityEngine;

public class BerryBushHarvest : MonoBehaviour
{
    [Header("Berry Settings")]
    public ItemData berryItem;
    public int berryAmount = 3;
    public float respawnTime = 60f;

    [Header("References")]
    public GameObject berriesObject;
    public ResourceGainPopup resourcePopup;

    private bool hasBerries = true;

    public string GetPromptText()
    {
        if (!hasBerries)
            return "";

        return "[E] Beeren pflücken";
    }

    public void Harvest(InventoryGridUI inventoryGrid)
    {
        if (!hasBerries)
            return;

        if (inventoryGrid == null || berryItem == null)
            return;

        inventoryGrid.AddItem(
            berryItem,
            berryAmount
        );

        if (resourcePopup != null)
        {
            resourcePopup.ShowResourceGain(
                berryItem.itemName,
                berryAmount
            );
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        hasBerries = false;

        if (berriesObject != null)
            berriesObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        hasBerries = true;

        if (berriesObject != null)
            berriesObject.SetActive(true);
    }

    public bool HasBerries()
    {
        return hasBerries;
    }
}