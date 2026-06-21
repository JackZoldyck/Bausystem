using System.Collections;
using TMPro;
using UnityEngine;

public class WoodGainPopup : MonoBehaviour
{
    public RectTransform popupRect;
    public TMP_Text popupText;

    public Vector2 hiddenPosition = new Vector2(-400f, -100f);
    public Vector2 visiblePosition = new Vector2(120f, -100f);

    public float slideSpeed = 6f;
    public float showDuration = 1.5f;

    Coroutine currentRoutine;

    public void ShowWoodGain(int amount)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(amount));
    }

    IEnumerator ShowRoutine(int amount)
    {
        popupText.text = "+" + amount + " Holz";

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;

            popupRect.anchoredPosition =
                Vector2.Lerp(hiddenPosition, visiblePosition, t);

            yield return null;
        }

        yield return new WaitForSeconds(showDuration);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;

            popupRect.anchoredPosition =
                Vector2.Lerp(visiblePosition, hiddenPosition, t);

            yield return null;
        }
    }
}