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
    public GameObject micClickObject;

    private bool isEnabled;
    private bool hasCamera;

    void Start()
    {
        hasCamera = WebCamTexture.devices.Length > 0;

        // Always start with eye tracking OFF
        PlayerPrefs.SetInt("EyeTrackingEnabled", 0);
        PlayerPrefs.Save();

        isEnabled = false;
        eyeTrackingObject.SetActive(false);

        if (!hasCamera)
        {
            DisableAllControls(includeEyeToggle: true);
            buttonText.text = "  off";
        }
        else
        {
            DisableAllControls(includeEyeToggle: false);
            buttonText.text = "  off";
            toggleButton.interactable = true;

            if (buttonText != null)
                buttonText.color = Color.white;

            toggleButton.onClick.AddListener(ToggleEyeTracking);
        }

    }

    void ToggleEyeTracking()
    {
        isEnabled = !isEnabled;
        eyeTrackingObject.SetActive(isEnabled);
        Cursor.visible = !isEnabled;

        if (micClickObject != null)
            micClickObject.SetActive(isEnabled);

        if (micClick != null)
        {
            micClick.enabled = isEnabled;

            if (isEnabled)
            {
                micClick.ActivateMic();
                Debug.Log("🎙️ MicClick enabled with EyeTracking ON");
            }
            else
            {
                Debug.Log("🎙️ MicClick disabled with EyeTracking OFF");
            }
        }

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

    void DisableAllControls(bool includeEyeToggle)
    {
        if (includeEyeToggle && toggleButton != null)
            toggleButton.interactable = false;

        if (buttonText != null)
            buttonText.color = Color.gray;

        if (calibrationButton != null)
            calibrationButton.interactable = false;

        if (calibrationText != null)
            calibrationText.color = Color.gray;

        if (micDropdown != null)
            micDropdown.interactable = false;

        if (micDropdownLabel != null)
            micDropdownLabel.color = Color.gray;

        if (camDropdown != null)
            camDropdown.interactable = false;

        if (camDropdownLabel != null)
            camDropdownLabel.color = Color.gray;

        if (volumeBarUI != null)
            volumeBarUI.SetActive(false);
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
    public void RunCalibration()
    {
        MLP mlp = new MLP();

        // Replace with real calibration data collection in your game
        float[][] inputFeatures = new float[][]
        {
            new float[] { 0.1f, 0.2f },
            new float[] { 0.4f, 0.5f },
            new float[] { 0.7f, 0.8f }
        };

        Vector2[] targetGazePoints = new Vector2[]
        {
            new Vector2(100, 200),
            new Vector2(300, 400),
            new Vector2(500, 600)
        };


        string result = mlp.Train(inputFeatures, targetGazePoints);
        Debug.Log(result);

        mlp.Save("defaultUser.mlp");
        Debug.Log("✅ Calibration complete and saved to defaultUser.mlp");
    }

}


