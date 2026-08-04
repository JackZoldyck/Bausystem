using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public Image icon;
    public TMP_Text amountText;
    public InventoryGridUI GetInventoryGrid()
    {
        return inventoryGrid;
    }

    private InventoryGridUI inventoryGrid;
    private int slotIndex;

    private GameObject dragIconObject;
    private Image dragIconImage;

    public void Setup(InventoryGridUI grid, int index)
    {
        inventoryGrid = grid;
        slotIndex = index;
    }

    public void SetSlot(ItemData item, int amount)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        icon.enabled = true;
        icon.preserveAspect = true;

        amountText.text = amount > 1 ? amount.ToString() : "";
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
        amountText.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (icon.sprite == null)
            return;

        dragIconObject = new GameObject("DragIcon");
        dragIconObject.transform.SetParent(transform.root, false);
        dragIconObject.transform.SetAsLastSibling();

        dragIconImage = dragIconObject.AddComponent<Image>();
        dragIconImage.sprite = icon.sprite;
        dragIconImage.preserveAspect = true;
        dragIconImage.raycastTarget = false;

        RectTransform dragRect = dragIconObject.GetComponent<RectTransform>();
        dragRect.sizeDelta = new Vector2(48, 48);

        dragIconObject.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
        {
            dragIconObject.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
        {
            Destroy(dragIconObject);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI draggedSlot =
            eventData.pointerDrag.GetComponent<InventorySlotUI>();

        if (draggedSlot != null)
        {
            inventoryGrid.SwapSlots(draggedSlot.slotIndex, slotIndex);
            return;
        }

        InventoryHotbarSlotUI draggedHotbarSlot =
            eventData.pointerDrag.GetComponent<InventoryHotbarSlotUI>();

        if (draggedHotbarSlot != null)
        {
            inventoryGrid.MoveHotbarSlotToInventorySlot(draggedHotbarSlot, slotIndex);
            return;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        InventorySlotData data = GetSlotData();

        if (data != null &&
            data.item != null &&
            data.item.itemType == ItemType.Food)
        {
            inventoryGrid.EatItem(slotIndex);
            return;
        }

        inventoryGrid.SplitStack(slotIndex);
    }
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    public InventorySlotData GetSlotData()
    {
        return inventoryGrid.GetSlotData(slotIndex);
    }
}