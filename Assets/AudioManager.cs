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
    public List<AudioSource> sfxSources = new();
    public AudioSource assistantSource;

    private Dictionary<AudioChannel, float> volumes = new();

    void Awake()
    {
        // Apply saved volumes before anything else plays audio
        ApplyInitialVolume(AudioChannel.Music, "volume_music", musicSource);
        ApplyInitialVolume(AudioChannel.SFX, "volume_sfx", sfxSources);
        ApplyInitialVolume(AudioChannel.Assistant, "volume_assistant", assistantSource);
    }

    void Start()
    {
        InitSlider(AudioChannel.Music, musicSlider, "volume_music");
        InitSlider(AudioChannel.SFX, sfxSlider, "volume_sfx");
        InitSlider(AudioChannel.Assistant, assistantSlider, "volume_assistant");
    }

    void ApplyInitialVolume(AudioChannel channel, string key, AudioSource source)
    {
        float volume = PlayerPrefs.GetFloat(key, 0.5f);
        volumes[channel] = volume;
        if (source != null) source.volume = volume;
    }

    void ApplyInitialVolume(AudioChannel channel, string key, List<AudioSource> sources)
    {
        float volume = PlayerPrefs.GetFloat(key, 0.5f);
        volumes[channel] = volume;
        foreach (var src in sources)
            if (src != null) src.volume = volume;
    }

    void InitSlider(AudioChannel channel, Slider slider, string key)
    {
        float volume = volumes.ContainsKey(channel) ? volumes[channel] : 0.5f;
        if (slider != null)
        {
            slider.value = volume;
            slider.onValueChanged.AddListener((v) => SetVolume(channel, v));
        }
    }

    public void SetVolume(AudioChannel channel, float value)
    {
        volumes[channel] = value;
        PlayerPrefs.SetFloat($"volume_{channel.ToString().ToLower()}", value);

        switch (channel)
        {
            case AudioChannel.Music:
                if (musicSource != null) musicSource.volume = value;
                break;
            case AudioChannel.SFX:
                foreach (var src in sfxSources)
                    if (src != null) src.volume = value;
                break;
            case AudioChannel.Assistant:
                if (assistantSource != null) assistantSource.volume = value;
                break;
        }

        PlayerPrefs.Save();
    }

    public float GetVolume(AudioChannel channel)
    {
        return volumes.TryGetValue(channel, out float val) ? val : 1f;
    }
}
