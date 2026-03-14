using System;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class VoiceRecorder : MonoBehaviour
{
    public string fileName = "recordedAudio.wav";
    public AudioClip recordedClip;

    private bool isRecording = false;
    private string filePath;
    private string currentMic;
    private string lastSavedFilePath;
    private TMP_Dropdown micDropdown;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        lastSavedFilePath = null;

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
            Debug.LogError("No microphone detected.");
            return;
        }

        lastSavedFilePath = null;

        if (micDropdown != null && micDropdown.options.Count > 0)
        {
            currentMic = micDropdown.options[micDropdown.value].text;
        }
        else
        {
            currentMic = Microphone.devices[0];
            Debug.LogWarning("Microphone dropdown missing or empty. Falling back to default mic: " + currentMic);
        }

        Debug.Log("Selected device: " + currentMic);
        Debug.Log("Available devices: " + string.Join(", ", Microphone.devices));

        StartCoroutine(ResetAndRecord(currentMic));
    }

    private IEnumerator ResetAndRecord(string micName)
    {
        Microphone.End(null);
        yield return new WaitForSeconds(0.1f);
        recordedClip = Microphone.Start(micName, false, 10, 44100);
        isRecording = true;
        Debug.Log("Recording started using: " + micName);
    }

    public bool StopRecordingAndSave()
    {
        if (!isRecording)
            return false;

        int recordedSamples = Microphone.GetPosition(currentMic);

        Microphone.End(currentMic);
        isRecording = false;

        if (recordedClip == null)
        {
            Debug.LogError("Recorded clip is null.");
            return false;
        }

        if (recordedSamples <= 0)
        {
            Debug.LogError("Recording captured 0 samples.");
            return false;
        }

        float[] samples = new float[recordedSamples * recordedClip.channels];
        recordedClip.GetData(samples, 0);

        if (samples.All(s => Mathf.Approximately(s, 0f)))
        {
            Debug.LogError("Recording has only silence.");
            return false;
        }

        AudioClip trimmedClip = AudioClip.Create(
            $"{fileName}_{DateTime.UtcNow:yyyyMMddHHmmss}",
            recordedSamples,
            recordedClip.channels,
            recordedClip.frequency,
            false
        );
        trimmedClip.SetData(samples, 0);

        byte[] wavBytes = WavUtility.FromAudioClip(trimmedClip);
        File.WriteAllBytes(filePath, wavBytes);
        lastSavedFilePath = filePath;
        Debug.Log("Audio saved to: " + filePath);

        return true;
    }

    public string GetSavedFilePath() => lastSavedFilePath;

    public AudioClip GetRecordedClip()
    {
        return recordedClip;
    }

    public bool IsRecording()
    {
        return isRecording;
    }
}
