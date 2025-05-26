using UnityEngine;
using TMPro; // ????? ???? ??
using System.Collections.Generic;

public class CameraSettings : MonoBehaviour
{
    public TMP_Dropdown cameraDropdown; // ????? ??TMP_Dropdown
    public static string selectedCameraName;

    void Start()
    {
        PopulateCameraDropdown();
    }

    void PopulateCameraDropdown()
    {
        cameraDropdown.ClearOptions();
        WebCamDevice[] devices = WebCamTexture.devices;
        List<string> options = new List<string>();

        foreach (WebCamDevice device in devices)
        {
            options.Add(device.name);
        }

        cameraDropdown.AddOptions(options);

        string savedCamera = PlayerPrefs.GetString("SelectedCamera", "");
        if (!string.IsNullOrEmpty(savedCamera) && options.Contains(savedCamera))
        {
            cameraDropdown.value = options.IndexOf(savedCamera);
        }

        cameraDropdown.onValueChanged.AddListener(delegate {
            OnCameraSelected(cameraDropdown);
        });
    }

    void OnCameraSelected(TMP_Dropdown dropdown) // ????? ??
    {
        selectedCameraName = dropdown.options[dropdown.value].text;
        PlayerPrefs.SetString("SelectedCamera", selectedCameraName);
        PlayerPrefs.Save();
    }
}
