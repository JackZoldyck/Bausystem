using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text timerText;

    public void SetFood(ActiveFood food)
    {
        if (food == null || food.item == null)
        {
            Clear();
            return;
        }

        if (icon != null)
        {
            icon.enabled = true;
            icon.sprite = food.item.icon;
        }

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(food.remainingTime);
            timerText.text = seconds.ToString();
        }
    }

    public void Clear()
    {
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        if (timerText != null)
            timerText.text = "";
    }
}