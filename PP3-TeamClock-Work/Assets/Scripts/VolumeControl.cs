using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] public Slider volumeSlider;

    void Start()
    {
        // Initialize the slider value to the current volume
        volumeSlider.value = gamemanager.instance.GetVolume();
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        gamemanager.instance.SetVolume(volume);
    }
}
