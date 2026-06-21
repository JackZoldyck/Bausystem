using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    public void SetSlot(Sprite sprite, int amount)
    {
        icon.sprite = sprite;
        icon.enabled = true;

        amountText.text = amount.ToString();
    }

    public void ClearSlot()
    {
        icon.enabled = false;
        amountText.text = "";
    }
}