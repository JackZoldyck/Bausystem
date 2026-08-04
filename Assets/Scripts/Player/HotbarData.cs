using UnityEngine;

[System.Serializable]
public class HotbarSlotData
{
    public ItemData item;
    public int amount;

    public bool IsEmpty()
    {
        return item == null || amount <= 0;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }
}

public class HotbarData : MonoBehaviour
{
    public HotbarSlotData[] slots = new HotbarSlotData[9];

    void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                slots[i] = new HotbarSlotData();
        }
    }

    public HotbarSlotData GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
            return null;

        return slots[index];
    }

    public void SetSlot(int index, ItemData item, int amount)
    {
        if (index < 0 || index >= slots.Length)
            return;

        slots[index].item = item;
        slots[index].amount = amount;
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        slots[index].Clear();
    }
}