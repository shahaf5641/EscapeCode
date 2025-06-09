using UnityEngine;

public class UnitEyeManager : MonoBehaviour
{
    public static UnitEyeManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // You can later expose webcam, gaze state, etc. here
}
