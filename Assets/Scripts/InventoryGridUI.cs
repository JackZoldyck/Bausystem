using System.Collections.Generic;
using UnityEngine;

public class InventoryGridUI : MonoBehaviour
{
    public static InventoryGridUI Instance;

    public GameObject slotPrefab;
    public Transform slotParent;
    public int slotCount = 20;

    public bool HasItem(ItemData item, int amount)
    {
        int total = 0;

        for (int i = 0; i < slotData.Count; i++)
        {
            if (!slotData[i].IsEmpty() && slotData[i].item == item)
                total += slotData[i].amount;
        }

        return total >= amount;
    }


    private bool slotsCreated = false;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private List<InventorySlotData> slotData = new List<InventorySlotData>();

    void Awake()
    {
        Instance = this;
        CreateSlots();
    }

    void CreateSlots()
    {
        if (slotsCreated)
            return;

        slotsCreated = true;

        slotUIs.Clear();
        slotData.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            InventorySlotData data = new InventorySlotData();
            slotData.Add(data);

            GameObject slotObject = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();

            slotUI.Setup(this, i);
            slotUI.ClearSlot();

            slotUIs.Add(slotUI);
        }
    }

    public void AddItem(ItemData item, int amount)
    {
        Debug.Log("AddItem AUF: " + gameObject.name);
        Debug.Log("AddItem Item: " + item);
        Debug.Log("AddItem Amount: " + amount);
        Debug.Log("Item Name: " + item.itemName);
        Debug.Log("Item MaxStackSize: " + item.maxStackSize);
        Debug.Log("Item Asset: " + item.name);
        Debug.Log("SlotData Count: " + slotData.Count);
        CreateSlots();

        if (item == null)
        {
            Debug.LogWarning("AddItem wurde ohne Item aufgerufen.");
            return;
        }

        // Vorhandene Stacks auffüllen
        for (int i = 0; i < slotData.Count; i++)
        {
            if (!slotData[i].IsEmpty()
                && slotData[i].item == item
                && slotData[i].amount < item.maxStackSize)
            {
                int space = item.maxStackSize - slotData[i].amount;
                int addAmount = Mathf.Min(space, amount);

                slotData[i].amount += addAmount;
                amount -= addAmount;

                if (amount <= 0)
                {
                    RefreshUI();
                    return;
                }
            }
        }

        // Neue Slots belegen
        for (int i = 0; i < slotData.Count; i++)
        {
            if (slotData[i].IsEmpty())
            {
                int addAmount = Mathf.Min(amount, item.maxStackSize);

                Debug.Log("Neuer Slot belegt: " + i + " mit " + item.itemName + " x" + addAmount);

                slotData[i].item = item;
                slotData[i].amount = addAmount;

                amount -= addAmount;

                if (amount <= 0)
                {
                    RefreshUI();
                    return;
                }
            }
        }

        Debug.LogWarning("Inventar ist voll.");
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
            return;

        InventorySlotData fromSlot = slotData[fromIndex];
        InventorySlotData toSlot = slotData[toIndex];

        if (fromSlot.IsEmpty())
            return;

        if (!toSlot.IsEmpty() && fromSlot.item == toSlot.item)
        {
            toSlot.amount += fromSlot.amount;
            fromSlot.Clear();

            RefreshUI();
            return;
        }

        InventorySlotData temp = slotData[fromIndex];
        slotData[fromIndex] = slotData[toIndex];
        slotData[toIndex] = temp;

        RefreshUI();
    }

    public void SplitStack(int index)
    {
        InventorySlotData sourceSlot = slotData[index];

        if (sourceSlot.IsEmpty())
            return;

        if (sourceSlot.amount <= 1)
            return;

        int emptyIndex = -1;

        for (int i = 0; i < slotData.Count; i++)
        {
            if (slotData[i].IsEmpty())
            {
                emptyIndex = i;
                break;
            }
        }

        if (emptyIndex == -1)
        {
            Debug.Log("Kein freier Slot zum Teilen des Stacks.");
            return;
        }

        int splitAmount = sourceSlot.amount / 2;
        sourceSlot.amount -= splitAmount;

        slotData[emptyIndex].item = sourceSlot.item;
        slotData[emptyIndex].amount = splitAmount;

        RefreshUI();
    }

    public void RemoveItem(ItemData item, int amount)
    {
        int remaining = amount;

        for (int i = 0; i < slotData.Count; i++)
        {
            if (slotData[i].IsEmpty() || slotData[i].item != item)
                continue;

            int removeAmount = Mathf.Min(slotData[i].amount, remaining);

            slotData[i].amount -= removeAmount;
            remaining -= removeAmount;

            if (slotData[i].amount <= 0)
                slotData[i].Clear();

            if (remaining <= 0)
                break;
        }

        RefreshUI();
    }

    public void EatItem(int index)
    {
        if (index < 0 || index >= slotData.Count)
            return;

        InventorySlotData data = slotData[index];

        if (data.IsEmpty())
            return;

        if (data.item.itemType != ItemType.Food)
            return;

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("Kein PlayerStats-Objekt gefunden!");
            return;
        }

        bool eaten = playerStats.EatFood(data.item);

        if (!eaten)
            return;

        data.amount--;

        if (data.amount <= 0)
        {
            data.Clear();
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (slotData[i].IsEmpty())
                slotUIs[i].ClearSlot();
            else
                slotUIs[i].SetSlot(slotData[i].item, slotData[i].amount);
        }
    }
    public InventorySlotData GetSlotData(int index)
    {
        if (index < 0 || index >= slotData.Count)
            return null;

        return slotData[index];
    }

    public void MoveHotbarSlotToInventorySlot(InventoryHotbarSlotUI hotbarSlot, int inventoryIndex)
    {
        HotbarSlotData hotbarSlotData = hotbarSlot.hotbarData.GetSlot(hotbarSlot.slotIndex);

        if (hotbarSlotData == null || hotbarSlotData.IsEmpty())
            return;

        InventorySlotData targetSlot = slotData[inventoryIndex];

        if (targetSlot.IsEmpty())
        {
            targetSlot.item = hotbarSlotData.item;
            targetSlot.amount = hotbarSlotData.amount;

            hotbarSlot.hotbarData.ClearSlot(hotbarSlot.slotIndex);

            RefreshUI();
        }
    }
}