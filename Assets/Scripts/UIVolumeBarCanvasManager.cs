using UnityEngine;

public class UIVolumeBarCanvasManager : MonoBehaviour
{
    public static UIVolumeBarCanvasManager Instance;

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
}
