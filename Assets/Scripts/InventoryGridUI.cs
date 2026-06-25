using System.Collections.Generic;
using UnityEngine;

public class InventoryGridUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public int slotCount = 20;

    private bool slotsCreated = false;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private List<InventorySlotData> slotData = new List<InventorySlotData>();

    void Awake()
    {
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

    public void AddItem(Sprite icon, int amount)
    {
        CreateSlots();

        if (icon == null)
        {
            Debug.LogWarning("AddItem wurde ohne Icon aufgerufen.");
            return;
        }

        for (int i = 0; i < slotData.Count; i++)
        {
            if (!slotData[i].IsEmpty() && slotData[i].icon == icon)
            {
                slotData[i].amount += amount;
                RefreshUI();
                return;
            }
        }

        for (int i = 0; i < slotData.Count; i++)
        {
            if (slotData[i].IsEmpty())
            {
                slotData[i].icon = icon;
                slotData[i].amount = amount;
                RefreshUI();
                return;
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

        if (!toSlot.IsEmpty() && fromSlot.icon == toSlot.icon)
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

        slotData[emptyIndex].icon = sourceSlot.icon;
        slotData[emptyIndex].amount = splitAmount;

        RefreshUI();
    }

    void RefreshUI()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (slotData[i].IsEmpty())
                slotUIs[i].ClearSlot();
            else
                slotUIs[i].SetSlot(slotData[i].icon, slotData[i].amount);
        }
    }
}