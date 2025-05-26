using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MicrophoneSettings : MonoBehaviour
{
    public TMP_Dropdown microphoneDropdown;
    public static string selectedMicrophone;

    void Start()
    {
        PopulateMicrophoneDropdown();
    }

    void PopulateMicrophoneDropdown()
    {
        microphoneDropdown.ClearOptions();
        List<string> options = new List<string>(Microphone.devices);

        microphoneDropdown.AddOptions(options);

        string savedMicrophone = PlayerPrefs.GetString("SelectedMicrophone", "");
        if (!string.IsNullOrEmpty(savedMicrophone) && options.Contains(savedMicrophone))
        {
            microphoneDropdown.value = options.IndexOf(savedMicrophone);
        }

        microphoneDropdown.onValueChanged.AddListener(delegate {
            OnMicrophoneSelected(microphoneDropdown);
        });
    }

    void OnMicrophoneSelected(TMP_Dropdown dropdown)
    {
        selectedMicrophone = dropdown.options[dropdown.value].text;
        PlayerPrefs.SetString("SelectedMicrophone", selectedMicrophone);
        PlayerPrefs.Save();
    }
}
