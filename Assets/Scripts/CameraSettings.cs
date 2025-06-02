using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CameraSettings : MonoBehaviour
{
    public TMP_Dropdown cameraDropdown;
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

        if (devices.Length == 0)
        {
            options.Add("No camera found");
            cameraDropdown.AddOptions(options);
            cameraDropdown.interactable = false;
            selectedCameraName = "";

            // Disable arrow
            Transform arrow = cameraDropdown.transform.Find("Arrow");
            if (arrow != null) arrow.gameObject.SetActive(false);

            // Change label text color to gray
            TMP_Text label = cameraDropdown.transform.Find("Label").GetComponent<TMP_Text>();
            label.color = Color.gray;
        }
        else
        {
            foreach (WebCamDevice device in devices)
            {
                options.Add(device.name);
            }

            cameraDropdown.AddOptions(options);
            cameraDropdown.interactable = true;

            string savedCamera = PlayerPrefs.GetString("SelectedCamera", "");
            if (!string.IsNullOrEmpty(savedCamera) && options.Contains(savedCamera))
            {
                cameraDropdown.value = options.IndexOf(savedCamera);
                selectedCameraName = savedCamera;
            }
            else
            {
                selectedCameraName = options[0];
            }

            cameraDropdown.onValueChanged.AddListener(delegate {
                OnCameraSelected(cameraDropdown);
            });
        }
    }

    void OnCameraSelected(TMP_Dropdown dropdown)
    {
        selectedCameraName = dropdown.options[dropdown.value].text;
        PlayerPrefs.SetString("SelectedCamera", selectedCameraName);
        PlayerPrefs.Save();
    }
}
