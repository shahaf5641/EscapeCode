using UnityEngine;

public class GlobalInactiveObjects : MonoBehaviour
{
    public static GlobalInactiveObjects Instance;

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
