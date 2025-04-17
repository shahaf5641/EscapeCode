using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System;

public class WhisperTranscriber : MonoBehaviour
{
    private string openAIKey;

    void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("openai_config");

        if (jsonFile == null)
        {
            Debug.LogError("❌ openai_config.json not found in Resources!");
        }
        else
        {
            Debug.Log("✅ Loaded raw config: " + jsonFile.text);
        }
        OpenAIConfig config = JsonUtility.FromJson<OpenAIConfig>(jsonFile.text);
        openAIKey = config?.openai_api_key;

    }
    public IEnumerator TranscribeAudio(string filePath, Action<string> onTranscriptionComplete)
    {
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
        }
        else
        {
            string result = www.downloadHandler.text;
            var json = JsonUtility.FromJson<TranscriptionResult>("{\"text\":" + result.Split(new[] { "\"text\":" }, StringSplitOptions.None)[1]);
            onTranscriptionComplete(json.text);
        }
    }

    [Serializable]
    public class TranscriptionResult
    {
        public string text;
    }
}
