using UnityEngine;
using System.IO;
using TMPro;
using System.Linq;
using System.Collections;

public class VoiceRecorder : MonoBehaviour
{
    public string fileName = "recordedAudio.wav";
    private AudioClip recordedClip;
    private bool isRecording = false;
    private string filePath;
    private string currentMic;

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

        if (micDropdown != null && micDropdown.options.Count > 0)
        {
            currentMic = micDropdown.options[micDropdown.value].text;
        }
        else
        {
            currentMic = Microphone.devices[0];
            Debug.LogWarning("🎤 Microphone dropdown missing or empty. Falling back to default mic: " + currentMic);
        }

        Debug.Log("🎤 Selected device: " + currentMic);
        Debug.Log("📦 Available devices: " + string.Join(", ", Microphone.devices));

        StartCoroutine(ResetAndRecord(currentMic));
    }

    private IEnumerator ResetAndRecord(string micName)
    {
        Microphone.End(null);
        yield return new WaitForSeconds(0.1f);
        recordedClip = Microphone.Start(micName, false, 10, 44100);
        isRecording = true;
        Debug.Log("🎬 Recording started using: " + micName);
    }

    public void StopRecordingAndSave()
    {
        if (!isRecording) return;

        Microphone.End(currentMic);
        isRecording = false;

        if (recordedClip == null)
        {
            Debug.LogError("❌ Recorded clip is null!");
            return;
        }

        float[] samples = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(samples, 0);

        if (samples.All(s => Mathf.Approximately(s, 0)))
        {
            Debug.LogError("❌ Recording has only silence!");
            return;
        }

        byte[] wavBytes = WavUtility.FromAudioClip(recordedClip);
        File.WriteAllBytes(filePath, wavBytes);
        Debug.Log("✅ Audio saved to: " + filePath);
    }

    public string GetSavedFilePath() => filePath;
}
