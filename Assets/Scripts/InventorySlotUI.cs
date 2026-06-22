using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    public void SetSlot(Sprite sprite, int amount)
    {
        Debug.Log("Sprite angekommen: " + sprite);

        icon.sprite = sprite;
        icon.enabled = true;
        icon.preserveAspect = true;

        amountText.text = amount.ToString();
    }

    public void ClearSlot()
    {
        icon.enabled = false;
        amountText.text = "";
    }
}