using UnityEngine;

public class ObjectSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        if (source != null && clickSound != null && !source.isPlaying)
        {
            source.PlayOneShot(clickSound);
        }
    }

}
