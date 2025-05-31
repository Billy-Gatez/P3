using UnityEngine;

public class Snow : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowParticles;

    public void SetSnowIntensity(float emissionRate)
    {
        if (snowParticles == null) return;

        var emission = snowParticles.emission;
        emission.rateOverTime = emissionRate;
    }
}
