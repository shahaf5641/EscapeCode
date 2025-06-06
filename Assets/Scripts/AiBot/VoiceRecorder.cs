using UnityEngine;
using System.IO;
using TMPro;

public class VoiceRecorder : MonoBehaviour
{
    public string fileName = "recordedAudio.wav";
    private AudioClip recordedClip;
    private bool isRecording = false;
    private string filePath;

    private TMP_Dropdown micDropdown;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        MicToVirtualClick micClick = FindObjectOfType<MicToVirtualClick>(true);
        if (micClick != null)
        {
            micDropdown = micClick.micDropdown;
        }
    }

    public void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("❌ No microphone detected.");
            return;
        }

        string selectedMic = null;

        if (micDropdown != null && micDropdown.options.Count > 0)
        {
            selectedMic = micDropdown.options[micDropdown.value].text;
        }
        else
        {
            selectedMic = Microphone.devices[0];
            Debug.LogWarning("🎤 Microphone dropdown missing or empty. Falling back to default mic: " + selectedMic);
        }

        recordedClip = Microphone.Start(selectedMic, false, 10, 44100);
        isRecording = true;
        Debug.Log("🎬 Recording started using: " + selectedMic);
    }

    public void StopRecordingAndSave()
    {
        if (!isRecording) return;

        Microphone.End(null);
        isRecording = false;

        if (recordedClip == null)
        {
            Debug.LogError("❌ Recorded clip is null!");
            return;
        }

        byte[] wavBytes = WavUtility.FromAudioClip(recordedClip);
        File.WriteAllBytes(filePath, wavBytes);
        Debug.Log("✅ Audio saved to: " + filePath);
    }

    public string GetSavedFilePath() => filePath;
}
