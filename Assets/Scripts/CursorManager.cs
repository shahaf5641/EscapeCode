using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public GameObject regularCursor;      
    public GameObject eyeTrackingCursor;  // ??? ??Eye Tracking (Prefab)

    private bool isEyeTrackingEnabled = false;

    void Start()
    {
        // ??? ???????? ?? ??Eye Tracking ???? ?? ????
        isEyeTrackingEnabled = PlayerPrefs.GetInt("EyeTrackingEnabled", 0) == 1;
        UpdateCursor();
    }

    public void SetEyeTrackingEnabled(bool enabled)
    {
        isEyeTrackingEnabled = enabled;
        PlayerPrefs.SetInt("EyeTrackingEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (isEyeTrackingEnabled)
        {
            regularCursor.SetActive(false);
            eyeTrackingCursor.SetActive(true);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // ???? ????? ?? ????? ?????
        }
        else
        {
            regularCursor.SetActive(true);
            eyeTrackingCursor.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public bool IsEyeTrackingEnabled()
    {
        return isEyeTrackingEnabled;
    }
}
