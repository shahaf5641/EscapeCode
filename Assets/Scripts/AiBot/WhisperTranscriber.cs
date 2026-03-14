using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class WhisperTranscriber : MonoBehaviour
{
    private string openAIKey;

    [Serializable]
    public class TranscriptionResult
    {
        public string text;
    }

    void Awake()
    {
        openAIKey = OpenAIKeyLoader.LoadApiKey();
    }

    public IEnumerator TranscribeAudio(string filePath, Action<string> onTranscriptionComplete)
    {
        if (string.IsNullOrWhiteSpace(openAIKey))
        {
            Debug.LogWarning("Transcription skipped because no OpenAI API key is configured.");
            yield break;
        }

        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            Debug.LogError("No recorded audio file was found for transcription.");
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recording.wav", "audio/wav");
        form.AddField("model", "whisper-1");
        form.AddField("language", "en");

        UnityWebRequest www = UnityWebRequest.Post("https://api.openai.com/v1/audio/transcriptions", form);
        www.SetRequestHeader("Authorization", $"Bearer {openAIKey}");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Whisper error: " + www.error);
            Debug.LogError("Whisper response: " + www.downloadHandler.text);
            yield break;
        }

        string result = www.downloadHandler.text;
        TranscriptionResult json = JsonUtility.FromJson<TranscriptionResult>(result);

        if (json == null || string.IsNullOrWhiteSpace(json.text))
        {
            Debug.LogError("Whisper returned an empty transcription: " + result);
            yield break;
        }

        Debug.Log("Whisper transcription: " + json.text);
        onTranscriptionComplete(json.text);
    }
}
