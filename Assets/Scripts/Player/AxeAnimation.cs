using System.Collections;
using UnityEngine;

public class AxeAnimation : MonoBehaviour
{
    public Transform axeTransform;

    public float swingAngle = 60f;
    public float swingSpeed = 10f;

    private bool isSwinging = false;

    private Quaternion startRotation;
    private Vector3 startPosition;

    public void Swing()
    {
        if (axeTransform == null || isSwinging)
            return;

        // Ausgangslage unmittelbar vor dem Schlag merken
        startRotation = axeTransform.localRotation;
        startPosition = axeTransform.localPosition;

        StartCoroutine(SwingRoutine());
    }

    IEnumerator SwingRoutine()
    {
        isSwinging = true;

        Quaternion hitRotation =
            startRotation *
            Quaternion.Euler(swingAngle, -20f, -15f);

        float t = 0f;

        // Schlag nach vorne
        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;

            axeTransform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    hitRotation,
                    t
                );

            yield return null;
        }

        t = 0f;

        // Wieder zurück
        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;

            axeTransform.localRotation =
                Quaternion.Slerp(
                    hitRotation,
                    startRotation,
                    t
                );

            yield return null;
        }

        ResetTool();

        isSwinging = false;
    }

    private void ResetTool()
    {
        if (axeTransform == null)
            return;

        axeTransform.localRotation = startRotation;
        axeTransform.localPosition = startPosition;
    }

    private void OnDisable()
    {
        if (isSwinging)
        {
            ResetTool();
            isSwinging = false;
        }
    }
}