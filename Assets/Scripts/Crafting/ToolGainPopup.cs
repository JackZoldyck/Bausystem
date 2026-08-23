using TMPro;
using UnityEngine;

public class ToolGainPopup : MonoBehaviour
{
    public TMP_Text popupText;
    public float visibleDuration = 2f;

    private float timer;

    private void Start()
    {
        if (popupText != null)
            popupText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (popupText == null)
            return;

        if (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;

            if (timer <= 0f)
            {
                popupText.gameObject.SetActive(false);
            }
        }
    }

    public void ShowToolGain(string itemName)
    {
        if (popupText == null)
            return;

        popupText.text = $"HERGESTELLT: {itemName}";

        popupText.gameObject.SetActive(true);

        timer = visibleDuration;
    }
}