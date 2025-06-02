using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EyeTrackingToggle : MonoBehaviour
{
    public GameObject eyeTrackingObject;
    public Button toggleButton;
    public TMP_Text buttonText;

    public Button calibrationButton;     
    public TMP_Text calibrationText;       

    private bool isEnabled;

    void Start()
    {
        bool hasCamera = WebCamTexture.devices.Length > 0;

        if (!hasCamera)
        {
            toggleButton.interactable = false;
            buttonText.color = Color.gray;

            if (calibrationButton != null)
            {
                calibrationButton.interactable = false;
                if (calibrationText != null)
                    calibrationText.color = Color.gray;
            }

            return;
        }

        isEnabled = PlayerPrefs.GetInt("EyeTrackingEnabled", 0) == 1;
        eyeTrackingObject.SetActive(isEnabled);
        UpdateButtonStates();

        toggleButton.onClick.AddListener(ToggleEyeTracking);
    }

    void ToggleEyeTracking()
    {
        isEnabled = !isEnabled;
        eyeTrackingObject.SetActive(isEnabled);
        PlayerPrefs.SetInt("EyeTrackingEnabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        buttonText.text = isEnabled ? "  on" : "  off";

        if (calibrationButton != null)
        {
            calibrationButton.interactable = isEnabled;

            if (calibrationText != null)
                calibrationText.color = isEnabled ? Color.white : Color.gray;
        }
    }
}
