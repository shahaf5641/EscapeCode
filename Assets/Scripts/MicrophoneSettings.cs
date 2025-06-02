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

        if (options.Count == 0)
        {
            options.Add("No microphone found");
            microphoneDropdown.interactable = false;

            Transform arrow = microphoneDropdown.transform.Find("Arrow");
            if (arrow != null) arrow.gameObject.SetActive(false);

            TMP_Text label = microphoneDropdown.transform.Find("Label").GetComponent<TMP_Text>();
            label.color = Color.gray;

            selectedMicrophone = "";
        }
        else
        {
            microphoneDropdown.AddOptions(options);
            microphoneDropdown.interactable = true;

            string savedMicrophone = PlayerPrefs.GetString("SelectedMicrophone", "");
            if (!string.IsNullOrEmpty(savedMicrophone) && options.Contains(savedMicrophone))
            {
                microphoneDropdown.value = options.IndexOf(savedMicrophone);
            }

            microphoneDropdown.onValueChanged.AddListener(delegate {
                OnMicrophoneSelected(microphoneDropdown);
            });
        }
    }

    void OnMicrophoneSelected(TMP_Dropdown dropdown)
    {
        selectedMicrophone = dropdown.options[dropdown.value].text;
        PlayerPrefs.SetString("SelectedMicrophone", selectedMicrophone);
        PlayerPrefs.Save();
    }
}
