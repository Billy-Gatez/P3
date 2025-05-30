using UnityEngine;

public class rain : MonoBehaviour
{
    public AudioSource rainAudio;
    [Range(0f, 1f)] public float volume = 1f;
    void Start()
    {
        if (rainAudio == null)
        {
            rainAudio = GetComponent<AudioSource>();
        }

        if (rainAudio != null)
        {
            rainAudio.volume = volume;
        }
    }

    void Update()
    {
        if (rainAudio != null)
        {
            rainAudio.volume = volume;

        }
    }
}