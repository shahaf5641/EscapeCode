using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum AudioChannel { Music, SFX, Assistant }

public class AudioManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider assistantSlider;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource assistantSource;

    private Dictionary<AudioChannel, float> volumes = new();

    void Start()
    {
        LoadVolume(AudioChannel.Music, musicSlider, musicSource, "volume_music");
        LoadVolume(AudioChannel.SFX, sfxSlider, sfxSource, "volume_sfx");
        LoadVolume(AudioChannel.Assistant, assistantSlider, assistantSource, "volume_assistant");
    }

    private void LoadVolume(AudioChannel channel, Slider slider, AudioSource source, string key)
    {
        float savedVolume = PlayerPrefs.GetFloat(key, 0.5f);
        volumes[channel] = savedVolume;
        source.volume = savedVolume;
        slider.value = savedVolume;

        slider.onValueChanged.AddListener((value) => {
            SetVolume(channel, value);
        });
    }

    public void SetVolume(AudioChannel channel, float value)
    {
        volumes[channel] = value;

        switch (channel)
        {
            case AudioChannel.Music:
                musicSource.volume = value;
                PlayerPrefs.SetFloat("volume_music", value);
                break;
            case AudioChannel.SFX:
                sfxSource.volume = value;
                PlayerPrefs.SetFloat("volume_sfx", value);
                break;
            case AudioChannel.Assistant:
                assistantSource.volume = value;
                PlayerPrefs.SetFloat("volume_assistant", value);
                break;
        }

        PlayerPrefs.Save();
    }

    public float GetVolume(AudioChannel channel)
    {
        return volumes.ContainsKey(channel) ? volumes[channel] : 1f;
    }
}
