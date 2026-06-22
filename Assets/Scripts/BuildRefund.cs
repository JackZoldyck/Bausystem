using UnityEngine;

public class BuildRefund : MonoBehaviour
{
    public float refundMultiplier = 1f;

    public void Refund()
    {
        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
        BuildCost cost = GetComponent<BuildCost>();

        if (inventory == null || cost == null)
            return;

        inventory.wood += Mathf.RoundToInt(cost.woodCost * refundMultiplier);
    }
}