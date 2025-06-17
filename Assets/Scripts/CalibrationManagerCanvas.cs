using UnityEngine;

public class CalibrationManagerCanvas : MonoBehaviour
{
    public static CalibrationManagerCanvas Instance;

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
