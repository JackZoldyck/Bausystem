using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathScreenUI : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public GameObject respawnHint;
    public RespawnManager respawnManager;

    [Header("Timing")]
    public float fadeDelay = 0.5f;
    public float fadeDuration = 1f;
    public float respawnUnlockDelay = 0.8f;

    private bool isDeathScreenActive;
    private bool canRespawn;

    private void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup =
                GetComponent<CanvasGroup>();
        }

        HideInstant();
    }

    private void Update()
    {
        if (!isDeathScreenActive)
            return;

        if (!canRespawn)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Respawn();
        }
    }

    public void ShowDeathScreen()
    {
        if (isDeathScreenActive)
            return;

        isDeathScreenActive = true;
        canRespawn = false;

        if (respawnHint != null)
        {
            respawnHint.SetActive(false);
        }

        StartCoroutine(
            DeathScreenRoutine()
        );

        StartCoroutine(
            UnlockRespawnRoutine()
        );
    }

    private IEnumerator DeathScreenRoutine()
    {
        yield return new WaitForSecondsRealtime(
            fadeDelay
        );

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / fadeDuration
                );

            if (canvasGroup != null)
            {
                canvasGroup.alpha =
                    progress;
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private IEnumerator UnlockRespawnRoutine()
    {
        yield return new WaitForSecondsRealtime(
            respawnUnlockDelay
        );

        canRespawn = true;

        if (respawnHint != null)
        {
            respawnHint.SetActive(true);
        }
    }

    private void Respawn()
    {
        if (!canRespawn)
            return;

        canRespawn = false;

        if (respawnManager == null)
        {
            Debug.LogError(
                "DeathScreenUI: RespawnManager fehlt!"
            );

            return;
        }

        Debug.Log(
            "DEATH SCREEN: Respawn angefordert"
        );

        respawnManager.RespawnPlayer();

        StartCoroutine(
            FadeOutAfterRespawn()
        );
    }

    private IEnumerator FadeOutAfterRespawn()
    {
        float timer = 0f;

        float startAlpha =
            canvasGroup != null
                ? canvasGroup.alpha
                : 1f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / fadeDuration
                );

            if (canvasGroup != null)
            {
                canvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        0f,
                        progress
                    );
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        isDeathScreenActive = false;
        canRespawn = false;

        if (respawnHint != null)
        {
            respawnHint.SetActive(false);
        }
    }

    public void HideDeathScreen()
    {
        StopAllCoroutines();

        isDeathScreenActive = false;
        canRespawn = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (respawnHint != null)
        {
            respawnHint.SetActive(false);
        }
    }

    private void HideInstant()
    {
        isDeathScreenActive = false;
        canRespawn = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (respawnHint != null)
        {
            respawnHint.SetActive(false);
        }
    }
}