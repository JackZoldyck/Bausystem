using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public int woodAmount = 10;
    public int health = 3;

    public void Harvest(PlayerInventory inventory, int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            inventory.wood += woodAmount;

            InventoryUI inventoryUI = FindFirstObjectByType<InventoryUI>();

            if (inventoryUI != null)
                inventoryUI.UpdateUI();

            Destroy(gameObject);
        }
    }
}
