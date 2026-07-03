using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public Transform slotParent;
    public InventoryGridUI inventoryGrid;
    public int slotCount = 9;
    public bool handleInput = true;

    public HotbarData hotbarData;

    private List<HotbarSlotUI> slots = new List<HotbarSlotUI>();
    private int selectedIndex = -1;

    void Start()
    {
        CreateSlots();
        RefreshUI();
        DeselectAll();
    }

    void Update()
    {
        if (!handleInput)
        {
            RefreshUI();
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }

        RefreshUI();
    }

    void CreateSlots()
    {
        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, slotParent);

            HotbarSlotUI slotUI = slotObject.GetComponent<HotbarSlotUI>();
            InventoryHotbarSlotUI inventoryHotbarSlot =
                slotObject.GetComponent<InventoryHotbarSlotUI>();

            if (slotUI == null)
            {
                Debug.LogError("HotbarSlot Prefab hat kein HotbarSlotUI Script!");
                continue;
            }

            slotUI.SetHotkey((i + 1).ToString());
            slotUI.SetSelected(false);

            if (inventoryHotbarSlot != null)
            {
                inventoryHotbarSlot.slotIndex = i;
                inventoryHotbarSlot.hotbarData = hotbarData;
                inventoryHotbarSlot.inventoryGrid = inventoryGrid;
            }

            slots.Add(slotUI);
        }
    }

    public void RefreshUI()
    {
        if (hotbarData == null)
            return;

        for (int i = 0; i < slots.Count; i++)
        {
            HotbarSlotData data = hotbarData.GetSlot(i);

            if (data == null || data.IsEmpty())
                slots[i].ClearSlot();
            else
                slots[i].SetSlot(data.item, data.amount);
        }
    }

    public void SelectSlot(int index)
    {
        if (selectedIndex == index)
            selectedIndex = -1;
        else
            selectedIndex = index;

        for (int i = 0; i < slots.Count; i++)
            slots[i].SetSelected(i == selectedIndex);
    }

    public void DeselectAll()
    {
        selectedIndex = -1;

        for (int i = 0; i < slots.Count; i++)
            slots[i].SetSelected(false);
    }

    public void SetVisible(bool visible)
    {
        if (hotbarPanel != null)
            hotbarPanel.SetActive(visible);
    }
    public void SetSelectedIndex(int index)
    {
        selectedIndex = index;

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetSelected(i == selectedIndex);
        }
    }
}