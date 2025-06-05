using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using System.Linq;

public class MicToVirtualClick : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

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

    void Start()
    {
        PopulateMicDropdown();

        if (micDropdown != null)
        {
            micDropdown.onValueChanged.AddListener(OnMicDropdownChanged);
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
        var micList = Microphone.devices.ToList();
        if (micList.Count == 0)
        {
            Debug.LogError("❌ No microphones found.");
            return;
        }

        string currentSelection = micDropdown.options.Count > 0 && micDropdown.value < micDropdown.options.Count
            ? micDropdown.options[micDropdown.value].text
            : null;

        micDropdown.ClearOptions();
        micDropdown.AddOptions(micList);

        // Restore selection if still available
        if (!string.IsNullOrEmpty(currentSelection) && micList.Contains(currentSelection))
        {
            micDropdown.value = micList.IndexOf(currentSelection);
        }

        micDropdown.RefreshShownValue();
    }


    void OnMicDropdownChanged(int index)
    {
        string newMic = micDropdown.options[index].text;
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
            if (volume > loudnessThreshold && Time.time - lastClickTime > clickCooldown)
            {
                Debug.Log("🔊 Mic triggered real click");
                TriggerVirtualClick();
                lastClickTime = Time.time;
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    float GetLoudness()
    {
        if (micClip == null) return 0f;

        int micPosition = Microphone.GetPosition(micName) - sampleWindow;
        if (micPosition < 0) return 0;

        float[] samples = new float[sampleWindow];
        micClip.GetData(samples, Mathf.Max(micPosition, 0));

        float sum = 0f;
        foreach (float sample in samples)
            sum += sample * sample;

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
}
