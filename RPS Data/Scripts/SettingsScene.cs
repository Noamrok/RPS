using UnityEngine;
using UnityEngine.UI;

public class SettingsScene : MonoBehaviour
{
    public Slider volumeSlider;

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }
    void Awake()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        //DontDestroyOnLoad(transform.gameObject);
    }
}
