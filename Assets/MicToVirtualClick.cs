using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class MicToVirtualClick : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
    public Slider thresholdSlider;
    private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const int MOUSEEVENTF_LEFTUP = 0x0004;
    public float loudnessThreshold = 0.1f;
    public float checkInterval = 0.1f;
    public float clickCooldown = 0.5f;
    public TMP_Dropdown micDropdown;
    private float lastClickTime = 0f;
    private AudioClip micClip;
    private string micName;
    private int sampleWindow = 128;
    private Coroutine micCheckRoutine;
    public Slider volumeBar;
    public RectTransform thresholdMarker;
    void Start()
    {
        // Volume bar setup
        if (volumeBar != null)
        {
            volumeBar.minValue = 0f;
            volumeBar.maxValue = 1f;
        }

        // Load threshold from PlayerPrefs if available
        loudnessThreshold = Mathf.Clamp01(PlayerPrefs.GetFloat("threshold_value", loudnessThreshold));

        // Set up threshold slider (if assigned)
        if (thresholdSlider != null)
        {
            thresholdSlider.minValue = 0f;
            thresholdSlider.maxValue = 1f;
            thresholdSlider.value = loudnessThreshold;
            thresholdSlider.onValueChanged.AddListener(UpdateThresholdFromSlider);
        }

        // Initial marker and value
        UpdateThresholdUI();

        // Populate mic dropdown
        PopulateMicDropdown();

        if (micDropdown != null)
        {
            micDropdown.onValueChanged.AddListener(OnMicDropdownChanged);

            string savedMic = PlayerPrefs.GetString("selected_mic", null);
            if (!string.IsNullOrEmpty(savedMic) && Microphone.devices.Contains(savedMic))
            {
                micName = savedMic;
                int index = Microphone.devices.ToList().IndexOf(savedMic);
                micDropdown.value = index;
                micDropdown.RefreshShownValue();
            }
        }
    }


    public void ActivateMic()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("❌ No microphone detected.");
            return;
        }

        if (micDropdown == null || micDropdown.options.Count == 0)
        {
            Debug.LogError("❌ Microphone dropdown is empty or unassigned.");
            return;
        }

        string selectedMic = GetCurrentDropdownMic();
        StopMic();
        StartMic(selectedMic);
    }

    void OnDisable()
    {
        StopMic();
    }

    void PopulateMicDropdown()
    {
        if (micDropdown == null) return;

        var micList = Microphone.devices.ToList();
        if (micList.Count == 0)
        {
            Debug.LogError("❌ No microphones found.");
            return;
        }

        micDropdown.ClearOptions();
        micDropdown.AddOptions(micList);
        micDropdown.RefreshShownValue();
    }

    void OnMicDropdownChanged(int index)
    {
        string newMic = micDropdown.options[index].text;
        PlayerPrefs.SetString("selected_mic", newMic);
        PlayerPrefs.Save();

        StopMic();
        StartMic(newMic);
    }

    string GetCurrentDropdownMic()
    {
        if (micDropdown == null || micDropdown.options.Count == 0)
            return null;

        return micDropdown.options[micDropdown.value].text;
    }

    void StartMic(string newMic)
    {
        if (string.IsNullOrEmpty(newMic) || !Microphone.devices.Contains(newMic))
        {
            Debug.LogError("❌ Selected mic not valid.");
            return;
        }

        micName = newMic;
        micClip = Microphone.Start(micName, true, 1, 44100);
        micCheckRoutine = StartCoroutine(CheckMicVolume());
        Debug.Log($"🎤 Mic click listening on: {micName}");
    }

    void StopMic()
    {
        if (!string.IsNullOrEmpty(micName) && Microphone.IsRecording(micName))
        {
            Microphone.End(micName);
        }

        if (micCheckRoutine != null)
        {
            StopCoroutine(micCheckRoutine);
            micCheckRoutine = null;
        }

        micClip = null;
    }
    IEnumerator CheckMicVolume()
    {
        while (true)
        {
            float volume = GetLoudness();

            // Update volume bar
            if (volumeBar != null)
                volumeBar.value = volume;

            // Update threshold marker position
            UpdateThresholdMarker();

            // Click only if volume passes threshold
            if (volume > loudnessThreshold && Time.time - lastClickTime > clickCooldown)
            {
                Debug.Log("🔊 Mic triggered real click");
                TriggerVirtualClick();
                lastClickTime = Time.time;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }
    void UpdateThresholdMarker()
    {
        if (thresholdMarker == null || volumeBar == null)
            return;

        // Normalize threshold
        float normalizedThreshold = Mathf.Clamp01(loudnessThreshold / volumeBar.maxValue);

        // Set anchors for vertical placement (Y only)
        thresholdMarker.anchorMin = new Vector2(0, normalizedThreshold);
        thresholdMarker.anchorMax = new Vector2(1, normalizedThreshold);
        thresholdMarker.anchoredPosition = Vector2.zero; // Reset offset
    }
public float GetLoudness()
{
    if (micClip == null || !Microphone.IsRecording(null)) return 0f;

    const int sampleWindow = 128;
    float[] samples = new float[sampleWindow];

    int micPosition = Microphone.GetPosition(null) - sampleWindow;
    if (micPosition < 0) return 0f;

    try
    {
        micClip.GetData(samples, micPosition);
    }
    catch
    {
        return 0f; // Prevent crash from GetData
    }

    float sum = 0f;
    for (int i = 0; i < sampleWindow; i++)
    {
        sum += samples[i] * samples[i];
    }
    return Mathf.Sqrt(sum / sampleWindow);
}




    void TriggerVirtualClick()
    {
        Vector3 pos = Input.mousePosition;
        int x = (int)pos.x;
        int y = (int)pos.y;

        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        Debug.Log("🖱️ Real mouse click simulated at: " + pos);
    }
    void UpdateThresholdFromSlider(float value)
    {
        loudnessThreshold = value;
        UpdateThresholdUI();
    }

    void UpdateThresholdUI()
    {
        UpdateThresholdMarker();
    }
}
