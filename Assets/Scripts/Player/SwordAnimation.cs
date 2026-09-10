using System.Collections;
using UnityEngine;

public class SwordAnimation : MonoBehaviour
{
    [Header("Swing")]
    public float swingSpeed = 12f;

    public Vector3 windupRotation =
        new Vector3(
            -10f,
            -25f,
            20f
        );

    public Vector3 hitRotation =
        new Vector3(
            15f,
            65f,
            -25f
        );

    [Header("Timing")]
    public float windupDuration = 0.08f;
    public float returnDuration = 0.12f;

    private bool isSwinging = false;

    private Quaternion startRotation;


    private void Start()
    {
        startRotation =
            transform.localRotation;
    }


    public void Swing()
    {
        if (isSwinging)
            return;

        StartCoroutine(
            SwingRoutine()
        );
    }


    private IEnumerator SwingRoutine()
    {
        isSwinging = true;

        Quaternion windup =
            startRotation *
            Quaternion.Euler(
                windupRotation
            );

        Quaternion hit =
            startRotation *
            Quaternion.Euler(
                hitRotation
            );


        float t = 0f;

        while (t < 1f)
        {
            t +=
                Time.deltaTime /
                windupDuration;

            transform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    windup,
                    t
                );

            yield return null;
        }


        t = 0f;

        float attackDuration =
            1f / swingSpeed;

        while (t < 1f)
        {
            t +=
                Time.deltaTime /
                attackDuration;

            transform.localRotation =
                Quaternion.Slerp(
                    windup,
                    hit,
                    t
                );

            yield return null;
        }


        t = 0f;

        while (t < 1f)
        {
            t +=
                Time.deltaTime /
                returnDuration;

            transform.localRotation =
                Quaternion.Slerp(
                    hit,
                    startRotation,
                    t
                );

            yield return null;
        }


        transform.localRotation =
            startRotation;

        isSwinging = false;
    }
}