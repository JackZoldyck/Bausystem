using System.Collections.Generic;
using UnityEngine;

public class InventoryGridUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public int slotCount = 20;

    private bool slotsCreated = false;
    private List<InventorySlotUI> slots = new List<InventorySlotUI>();

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

            if (slotUI == null)
            {
                Debug.LogError("Slot Prefab hat kein InventorySlotUI Script!");
                continue;
            }

            slotUI.ClearSlot();
            slots.Add(slotUI);
        }

        Debug.Log("Inventar Slots erstellt: " + slots.Count);
    }

    public void AddItem(Sprite icon, int amount)
    {
        CreateSlots();

        if (slots.Count == 0)
        {
            Debug.LogError("Keine Inventar-Slots vorhanden!");
            return;
        }

        slots[0].SetSlot(icon, amount);
    }
}