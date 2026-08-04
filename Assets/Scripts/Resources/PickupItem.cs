using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;
    public string pickupName = "Item";
    public ResourceGainPopup resourcePopup;

    public string GetPromptText()
    {
        return "[E] " + pickupName + " aufsammeln";
    }

    public void Pickup(InventoryGridUI inventoryGrid)
    {
        if (item == null || inventoryGrid == null)
            return;

        inventoryGrid.AddItem(item, amount);
        if (resourcePopup != null)
        {
            resourcePopup.ShowResourceGain(item.itemName, amount);
        }

        PickupRespawn respawn = GetComponent<PickupRespawn>();

        if (respawn != null)
            respawn.Collect();
        else
            Destroy(gameObject);
    }
}