using UnityEngine;

public class SettingsCanvasManager : MonoBehaviour
{
    public static SettingsCanvasManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // avoid duplicates in future scenes
        }
    }

    // You can expose references like:
    // public Canvas settingsCanvas;
}
