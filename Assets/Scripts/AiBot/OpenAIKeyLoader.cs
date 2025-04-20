using UnityEngine;
using System;
public static class OpenAIKeyLoader
{
    public static string LoadApiKey()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("openai_config");

        if (jsonFile == null)
        {
            Debug.LogError("❌ openai_config.json not found in Resources!");
            return null;
        }

        OpenAIConfig config = JsonUtility.FromJson<OpenAIConfig>(jsonFile.text);
        return config?.openai_api_key;
    }
}
