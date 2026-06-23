using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGridUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public int slotCount = 20;

    private bool slotsCreated = false;
    private List<InventorySlotUI> slots = new List<InventorySlotUI>();

    private Dictionary<Sprite, int> items = new Dictionary<Sprite, int>();

    void Awake()
    {
        CreateSlots();
    }

    void CreateSlots()
    {
        if (slotsCreated)
            return;

        slotsCreated = true;
        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();

            slotUI.ClearSlot();
            slots.Add(slotUI);
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

        if (items.ContainsKey(icon))
            items[icon] += amount;
        else
            items.Add(icon, amount);

        RefreshUI();
    }

    void RefreshUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].ClearSlot();
        }

        int index = 0;

        foreach (KeyValuePair<Sprite, int> item in items)
        {
            if (index >= slots.Count)
                return;

            slots[index].SetSlot(item.Key, item.Value);
            index++;
        }
    }
}