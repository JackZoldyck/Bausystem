using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceGainPopup : MonoBehaviour
{
    public RectTransform popupRect;
    public TMP_Text popupText;

    public Vector2 hiddenPosition = new Vector2(-400f, -100f);
    public Vector2 visiblePosition = new Vector2(50f, -100f);

    public float slideSpeed = 6f;
    public float showDuration = 2f;

    private Coroutine currentRoutine;

    void Awake()
    {
        if (popupRect == null)
            popupRect = GetComponent<RectTransform>();

        if (popupText == null)
            popupText = GetComponent<TMP_Text>();
    }

    void Start()
    {
        popupRect.anchoredPosition = hiddenPosition;
    }

    public void ShowResourceGain(string resourceName, int amount)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(resourceName, amount));
    }

    IEnumerator ShowRoutine(string resourceName, int amount)
    {
        popupText.text = "+" + amount + " " + resourceName;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            popupRect.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, t);
            yield return null;
        }

        popupRect.anchoredPosition = visiblePosition;

        yield return new WaitForSeconds(showDuration);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            popupRect.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, t);
            yield return null;
        }

        popupRect.anchoredPosition = hiddenPosition;
        currentRoutine = null;
    }
}