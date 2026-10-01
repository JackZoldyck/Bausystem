using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;
    public string pickupName = "Item";

    public string GetPromptText()
    {
        return "[E] " + pickupName + " aufsammeln";
    }

    public int Pickup(InventoryGridUI inventoryGrid)
    {
        if (item == null || inventoryGrid == null)
            return 0;

        int requestedAmount = amount;

        int addedAmount =
            inventoryGrid.AddItem(
                item,
                requestedAmount
            );

        if (addedAmount <= 0)
            return 0;

        amount -= addedAmount;

        if (ResourceGainPopup.Instance != null)
        {
            ResourceGainPopup.Instance.ShowResourceGain(
                item.itemName,
                addedAmount
            );
        }

        if (amount <= 0)
        {
            PickupRespawn respawn =
                GetComponent<PickupRespawn>();

            if (respawn != null)
                respawn.Collect();
            else
                Destroy(gameObject);
        }

        return addedAmount;
    }
}