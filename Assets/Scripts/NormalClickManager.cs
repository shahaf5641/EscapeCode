using UnityEngine;

public class NormalClickManager : MonoBehaviour
{
    public static NormalClickManager Instance;

    [SerializeField] private AudioSource clickAudioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // prevent duplication!
        }
    }

    public void PlayClickSound()
    {
        if (clickAudioSource != null)
            clickAudioSource.Play();
        else
            Debug.LogWarning("❌ Click AudioSource not assigned on NormalClickManager.");
    }
}
