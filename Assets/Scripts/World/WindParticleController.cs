using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WindParticleController : MonoBehaviour
{
    public float velocityMultiplier = 1f;

    private ParticleSystem particleSystemComponent;

    void Awake()
    {
        particleSystemComponent = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (WindManager.Instance == null)
            return;

        Vector3 windVelocity =
            WindManager.Instance.GetWindVelocity()
            * velocityMultiplier;

        ParticleSystem.VelocityOverLifetimeModule velocityModule =
            particleSystemComponent.velocityOverLifetime;

        velocityModule.enabled = true;
        velocityModule.space = ParticleSystemSimulationSpace.World;

        velocityModule.x = windVelocity.x;
        velocityModule.y = windVelocity.y;
        velocityModule.z = windVelocity.z;
    }
}