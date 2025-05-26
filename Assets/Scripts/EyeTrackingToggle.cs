using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EyeTrackingToggle : MonoBehaviour
{
    public GameObject eyeTrackingObject;
    public Button toggleButton;
    public TMP_Text buttonText;

    private bool isEnabled;

    void Start()
    {
        isEnabled = PlayerPrefs.GetInt("EyeTrackingEnabled", 0) == 1;
        eyeTrackingObject.SetActive(isEnabled);
        UpdateButtonText();

        toggleButton.onClick.AddListener(ToggleEyeTracking);
    }

    void ToggleEyeTracking()
    {
        isEnabled = !isEnabled;
        eyeTrackingObject.SetActive(isEnabled);
        PlayerPrefs.SetInt("EyeTrackingEnabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        buttonText.text = isEnabled ? "  on" : "  off";
    }
}
