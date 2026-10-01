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

    private bool hasBerries = true;

    public string GetPromptText()
    {
        if (!hasBerries)
            return "";

        return "[E] Beeren pflücken";
    }

    public bool Harvest(InventoryGridUI inventoryGrid)
    {
        if (!hasBerries)
            return true;

        if (inventoryGrid == null || berryItem == null)
            return false;

        int addedAmount =
            inventoryGrid.AddItem(
                berryItem,
                berryAmount
            );

        if (addedAmount <= 0)
            return false;

        if (ResourceGainPopup.Instance != null)
        {
            ResourceGainPopup.Instance.ShowResourceGain(
                berryItem.itemName,
                addedAmount
            );
        }

        if (addedAmount < berryAmount)
            return false;

        StartCoroutine(
            RespawnRoutine()
        );

        return true;
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