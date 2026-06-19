using System.Collections;
using UnityEngine;

public class TreeHitFeedback : MonoBehaviour
{
    public float shakeDuration = 0.15f;
    public float shakeStrength = 3f;
    public ParticleSystem hitParticles;

    private Quaternion startRotation;
    private Coroutine shakeRoutine;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    public void PlayHitFeedback(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (hitParticles != null)
        {
            hitParticles.transform.position = hitPoint;

            hitParticles.transform.rotation =
                Quaternion.LookRotation(hitNormal);

            hitParticles.Play();
        }

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float x = Random.Range(-shakeStrength, shakeStrength);
            float z = Random.Range(-shakeStrength, shakeStrength);

            transform.localRotation = startRotation * Quaternion.Euler(x, 0f, z);

            yield return null;
        }

        transform.localRotation = startRotation;
    }
}