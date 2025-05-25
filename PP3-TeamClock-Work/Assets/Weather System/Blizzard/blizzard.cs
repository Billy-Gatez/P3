using UnityEngine;

public class blizzard : MonoBehaviour
{

    [SerializeField] private ParticleSystem snowParticles;
    [SerializeField] private AudioSource windAudio;

    public void ActivateBlizzard()
    {
        if (snowParticles != null)
        {
            var emission = snowParticles.emission;
            emission.rateOverTime = 1000f;
        }

        PlayWindAudio();
    }

    public void DeactivateBlizzard()
    {
        if (snowParticles != null)
        {
            var emission = snowParticles.emission;
            emission.rateOverTime = 0f;
        }

        StopWindAudio();
    }

    public void SetBlizzardIntensity(float blizzardEmissionRate)
    {
        if (snowParticles != null)
        {
            var emission = snowParticles.emission;
            emission.rateOverTime = blizzardEmissionRate;
        }

        if (windAudio != null)
        {
            if (blizzardEmissionRate > 0 && !windAudio.isPlaying)
            {
                windAudio.Play();
            }
            else if (blizzardEmissionRate <= 0 && windAudio.isPlaying)
            {
                windAudio.Stop();
            }
        }
    }

    private void PlayWindAudio()
    {
        if (windAudio != null && !windAudio.isPlaying)
        {
            windAudio.Play();
        }
    }

    private void StopWindAudio()
    {
        if (windAudio != null && windAudio.isPlaying)
        {
            windAudio.Stop();
        }
    }
}
