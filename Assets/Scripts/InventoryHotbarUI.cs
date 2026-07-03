using System.Collections.Generic;
using UnityEngine;

public class InventoryHotbarUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public int slotCount = 9;
    public HotbarData hotbarData;
    public HotbarUI gameplayHotbarUI;

    private List<HotbarSlotUI> slots = new List<HotbarSlotUI>();

    void Start()
    {
        CreateSlots();
        RefreshUI();
    }

    void CreateSlots()
    {
        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, slotParent);
            HotbarSlotUI slotUI = slotObject.GetComponent<HotbarSlotUI>();

            slotUI.SetHotkey((i + 1).ToString());
            slotUI.SetSelected(false);

            slots.Add(slotUI);
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            HotbarSlotData data = hotbarData.GetSlot(i);

            if (data == null || data.IsEmpty())
                slots[i].ClearSlot();
            else
                slots[i].SetSlot(data.item, data.amount);
        }

        gameplayHotbarUI.RefreshUI();
    }
}
