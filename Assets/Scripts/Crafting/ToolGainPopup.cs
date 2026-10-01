using System.Collections;
using TMPro;
using UnityEngine;

public class ToolGainPopup : MonoBehaviour
{
    public static ToolGainPopup Instance { get; private set; }

    public RectTransform popupRect;
    public TMP_Text popupText;

    public Vector2 hiddenPosition = new Vector2(-400f, -100f);
    public Vector2 visiblePosition = new Vector2(50f, -100f);

    public float slideSpeed = 6f;
    public float showDuration = 2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        if (popupRect == null)
            popupRect = GetComponent<RectTransform>();

        if (popupText == null)
            popupText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        popupRect.anchoredPosition = hiddenPosition;
    }

    public void ShowToolGain(string itemName)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine =
            StartCoroutine(
                ShowRoutine(itemName)
            );
    }

    private IEnumerator ShowRoutine(string itemName)
    {
        popupText.text =
            $"HERGESTELLT: {itemName}";

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * slideSpeed;

            popupRect.anchoredPosition =
                Vector2.Lerp(
                    hiddenPosition,
                    visiblePosition,
                    t
                );

            yield return null;
        }

        popupRect.anchoredPosition =
            visiblePosition;

        yield return new WaitForSecondsRealtime(
            showDuration
        );

        t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * slideSpeed;

            popupRect.anchoredPosition =
                Vector2.Lerp(
                    visiblePosition,
                    hiddenPosition,
                    t
                );

            yield return null;
        }

        popupRect.anchoredPosition =
            hiddenPosition;

        currentRoutine = null;
    }
}