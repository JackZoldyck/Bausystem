using UnityEngine;

public class BuildRefund : MonoBehaviour
{
    public float refundMultiplier = 1f;

    public ItemData woodItem;
    public InventoryGridUI inventoryGridUI;

    void Awake()
    {
        if (inventoryGridUI == null)
        {
            inventoryGridUI = InventoryGridUI.Instance;

            if (inventoryGridUI == null)
            {
                inventoryGridUI =
                    FindAnyObjectByType<InventoryGridUI>(
                        FindObjectsInactive.Include
                    );
            }
        }
    }

    public void Refund()
    {
        BuildCost cost = GetComponent<BuildCost>();

        if (inventoryGridUI == null || cost == null || woodItem == null)
            return;

        int refundAmount =
            Mathf.RoundToInt(cost.woodCost * refundMultiplier);

        inventoryGridUI.AddItem(woodItem, refundAmount);

    }
}