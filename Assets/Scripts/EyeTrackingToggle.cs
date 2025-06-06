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
    public MicToVirtualClick micClick;
    public TMP_Dropdown micDropdown;
    public TMP_Dropdown camDropdown;
    public TMP_Text micDropdownLabel;
    public TMP_Text camDropdownLabel;
    public GameObject volumeBarUI;

    private bool isEnabled;

    void Start()
    {
        bool hasCamera = WebCamTexture.devices.Length > 0;

        // Always force OFF when scene starts
        PlayerPrefs.SetInt("EyeTrackingEnabled", 0);
        PlayerPrefs.Save();

        isEnabled = false;
        eyeTrackingObject.SetActive(false);

        if (!hasCamera)
        {
            if (calibrationButton != null)
            {
                calibrationButton.interactable = false;
                if (calibrationText != null)
                    calibrationText.color = Color.gray;
            }

            return;
        }

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

    if (micDropdown != null)
    {
        micDropdown.interactable = isEnabled;
        if (micDropdownLabel != null)
            micDropdownLabel.color = isEnabled ? Color.white : Color.gray;
    }

    if (camDropdown != null)
    {
        camDropdown.interactable = isEnabled;
        if (camDropdownLabel != null)
            camDropdownLabel.color = isEnabled ? Color.white : Color.gray;
    }
        if (volumeBarUI != null)
            volumeBarUI.SetActive(isEnabled);
    }

    public void ToggleVoiceAndMicClick()
    {
        micClick.enabled = !micClick.enabled;

        if (micClick.enabled)
        {
            micClick.ActivateMic();
        }

        Debug.Log("🎙️ Mic Click is now " + (micClick.enabled ? "ON" : "OFF"));
    }
}
