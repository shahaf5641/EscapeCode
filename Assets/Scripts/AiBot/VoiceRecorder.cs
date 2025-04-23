using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class VoiceRecorder : MonoBehaviour
{
    public string fileName = "recordedAudio.wav";
    private AudioClip recordedClip;
    private bool isRecording = false;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    public void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected.");
            return;
        }

        string selectedMic = Microphone.devices[0]; // try index 0 or 1
        Debug.Log("Using mic: " + selectedMic);
        recordedClip = Microphone.Start(selectedMic, false, 10, 44100);
        isRecording = true;
        Debug.Log("Recording started...");
    }

    public void StopRecordingAndSave()
    {
        if (!isRecording) return;

        Microphone.End(null);
        isRecording = false;

        if (recordedClip == null)
        {
            Debug.LogError("Recorded clip is null!");
            return;
        }

        var samples = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(samples, 0);
        // Save the audio to WAV
        byte[] wavBytes = WavUtility.FromAudioClip(recordedClip);
        File.WriteAllBytes(filePath, wavBytes);
        Debug.Log("Saved to: " + filePath);
    }


    public string GetSavedFilePath() => filePath;
}
