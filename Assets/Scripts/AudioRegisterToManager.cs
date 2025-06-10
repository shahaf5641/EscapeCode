using UnityEngine;

public class AudioRegisterToManager : MonoBehaviour
{
    public AudioChannel channel = AudioChannel.SFX;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioSource src = GetComponent<AudioSource>();
            if (src != null)
            {
                AudioManager.Instance.RegisterExternalSource(channel, src);
            }
        }
    }
}