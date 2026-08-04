using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlotUI : MonoBehaviour
{
    public Image background;
    public Image icon;
    public TMP_Text amountText;
    public TMP_Text hotkeyText;
    public Outline selectionOutline;

    void Start()
    {
        if (icon != null)
        {
            icon.enabled = false;
            icon.color = Color.white;
            icon.raycastTarget = false;
        }

        if (amountText != null)
            amountText.text = "";

        if (selectionOutline != null)
            selectionOutline.enabled = false;
    }

    public void SetHotkey(string text)
    {
        if (hotkeyText != null)
            hotkeyText.text = text;
    }

    public void SetSelected(bool selected)
    {
        if (selectionOutline != null)
            selectionOutline.enabled = selected;
    }

    public void SetIcon(Sprite sprite)
    {
        if (icon == null)
            return;

        icon.enabled = sprite != null;
        icon.sprite = sprite;
        icon.preserveAspect = true;
    }
    public void SetSlot(ItemData item, int amount)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }

        icon.enabled = true;
        icon.sprite = item.icon;
        icon.preserveAspect = true;

        amountText.text = amount > 1 ? amount.ToString() : "";
    }

    public void ClearSlot()
    {
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        if (amountText != null)
            amountText.text = "";
    }
}