using System.Collections;
using UnityEngine;

public class AxeAnimation : MonoBehaviour
{
    public float swingAngle = 60f;
    public float swingSpeed = 10f;

    private bool isSwinging = false;
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    public void Swing()
    {
        if (!isSwinging)
            StartCoroutine(SwingRoutine());
    }

    IEnumerator SwingRoutine()
    {
        isSwinging = true;

        Quaternion hitRotation =
            startRotation * Quaternion.Euler(swingAngle, 0, 0);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;

            transform.localRotation =
                Quaternion.Slerp(startRotation, hitRotation, t);

            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;

            transform.localRotation =
                Quaternion.Slerp(hitRotation, startRotation, t);

            yield return null;
        }

        transform.localRotation = startRotation;

        isSwinging = false;
    }
}