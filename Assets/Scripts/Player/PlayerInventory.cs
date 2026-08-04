using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int wood;
    public int stone;

    public bool HasResources(int woodCost, int stoneCost)
    {
        return wood >= woodCost && stone >= stoneCost;
    }

    public void RemoveResources(int woodCost, int stoneCost)
    {
        wood -= woodCost;
        stone -= stoneCost;
    }
}