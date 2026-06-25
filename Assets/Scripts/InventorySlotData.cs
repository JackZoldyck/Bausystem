using UnityEngine;

[System.Serializable]
public class InventorySlotData
{
    public Sprite icon;
    public int amount;

    public bool IsEmpty()
    {
        return icon == null || amount <= 0;
    }

    public void Clear()
    {
        icon = null;
        amount = 0;
    }
}