using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryHotbarSlotUI : HotbarSlotUI,
    IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int slotIndex;
    public HotbarData hotbarData;
    public InventoryGridUI inventoryGrid;

    private GameObject dragIconObject;
    private Image dragIconImage;

    public void OnBeginDrag(PointerEventData eventData)
    {
        HotbarSlotData data = hotbarData.GetSlot(slotIndex);

        if (data == null || data.IsEmpty())
            return;

        dragIconObject = new GameObject("DragIcon");
        dragIconObject.transform.SetParent(transform.root, false);
        dragIconObject.transform.SetAsLastSibling();

        dragIconImage = dragIconObject.AddComponent<Image>();
        dragIconImage.sprite = data.item.icon;
        dragIconImage.preserveAspect = true;
        dragIconImage.raycastTarget = false;

        RectTransform dragRect = dragIconObject.GetComponent<RectTransform>();
        dragRect.sizeDelta = new Vector2(48, 48);

        dragIconObject.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
            dragIconObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
            Destroy(dragIconObject);
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI draggedInventorySlot =
            eventData.pointerDrag.GetComponent<InventorySlotUI>();

        if (draggedInventorySlot != null)
        {
            InventorySlotData inventorySlotData = draggedInventorySlot.GetSlotData();

            if (inventorySlotData == null || inventorySlotData.item == null)
                return;

            hotbarData.SetSlot(slotIndex, inventorySlotData.item, inventorySlotData.amount);
            inventorySlotData.Clear();

            draggedInventorySlot.GetInventoryGrid().RefreshUI();
            return;
        }

        InventoryHotbarSlotUI draggedHotbarSlot =
            eventData.pointerDrag.GetComponent<InventoryHotbarSlotUI>();

        if (draggedHotbarSlot != null)
        {
            if (draggedHotbarSlot == this)
                return;

            HotbarSlotData fromSlot =
                draggedHotbarSlot.hotbarData.GetSlot(draggedHotbarSlot.slotIndex);

            HotbarSlotData toSlot =
                hotbarData.GetSlot(slotIndex);

            if (fromSlot == null || fromSlot.IsEmpty())
                return;

            ItemData tempItem = toSlot.item;
            int tempAmount = toSlot.amount;

            hotbarData.SetSlot(slotIndex, fromSlot.item, fromSlot.amount);
            draggedHotbarSlot.hotbarData.SetSlot(
                draggedHotbarSlot.slotIndex,
                tempItem,
                tempAmount
            );

            return;
        }
    }
}