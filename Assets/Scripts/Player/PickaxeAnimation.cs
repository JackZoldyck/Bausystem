using System.Collections;
using UnityEngine;

public class PickaxeAnimation : MonoBehaviour
{
    public Transform pickaxeTransform;

    public float swingAngle = 60f;
    public float swingSpeed = 10f;

    private bool isSwinging = false;

    private Quaternion startRotation;
    private Vector3 startPosition;

    public void Swing()
    {
        if (pickaxeTransform == null || isSwinging)
            return;

        // Aktuelle Ausgangslage direkt vor dem Schlag merken
        startRotation = pickaxeTransform.localRotation;
        startPosition = pickaxeTransform.localPosition;

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

            pickaxeTransform.localRotation =
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

            pickaxeTransform.localRotation =
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
        if (pickaxeTransform == null)
            return;

        pickaxeTransform.localRotation = startRotation;
        pickaxeTransform.localPosition = startPosition;
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