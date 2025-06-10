using UnityEngine;

public class SFXPlayerManager : MonoBehaviour
{
    public static SFXPlayerManager Instance;

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
}
