using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] sfxClips; // Array for multiple sounds

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxClips.Length && sfxClips[index] != null)
        {
            audioSource.PlayOneShot(sfxClips[index]);
        }
        else
        {
            Debug.LogWarning("Invalid SFX index or clip is missing.");
        }
    }

    public void PlayRandomSFX()
    {
        if (sfxClips.Length > 0)
        {
            int rand = Random.Range(0, sfxClips.Length);
            PlaySFX(rand);
        }
    }
}
