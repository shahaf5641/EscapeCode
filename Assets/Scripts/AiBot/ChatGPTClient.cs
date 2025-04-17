using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ChatGPTClient : MonoBehaviour
{
    public GameObject currentPuzzle;
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
    public IEnumerator GetAIHelp(string prompt, Action<string> onResponse)
    {
        string apiUrl = "https://api.openai.com/v1/chat/completions";

        string puzzleContext = currentPuzzle.GetComponent<PuzzleContextFormatter>().GetPromptString();

        string systemPrompt =
        "You are an AI assistant inside a coding puzzle game. " +
        "You can help the player by answering questions, giving advice, or helping them understand puzzles. " +
        "But when the player tries to guess the answer to a puzzle, you must switch to strict code-solving mode.\n\n" +

        "When in code-solving mode:\n" +
        "- Only respond with the correct Python code line.\n" +
        "- Do not explain or greet.\n" +
        "- Fix case or syntax issues if the guess is close.\n" +
        "- Example format: secret_code = \"Nana\"\n\n" +

        "When the player asks a general question (like \"what is the puzzle?\" or \"I don’t know the code\"), respond helpfully and conversationally.\n\n" +

        "Only correct the player IF their input is clearly a close attempt — such as slight misspellings, letter case differences, or a phonetically similar version of the answer. If the input includes nonsense words (like 'blah blah') or is too far off, do NOT respond with the code." +

        "Examples:\n\n" +

        "Player: secret code equals TADAM\n" +
        "You: secret_code = \"TaDam\"\n\n" +

        "Player: I don't know the code\n" +
        "You: No worries! Try looking at the string and see which letters are picked. I can help if you get stuck.\n\n" +

        "Player: what is the puzzle again?\n" +
        "You: The puzzle is about extracting the correct letters from a string. Look at the indexes used.\n\n" +

        "Player: I like pizza\n" +
        "You: Let me know when you're ready to guess the code or ask a question.\n\n";

        systemPrompt += puzzleContext;

        var messages = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                { "role", "system" },
                { "content", systemPrompt }
            },
            new Dictionary<string, string>
            {
                { "role", "user" },
                { "content", prompt }
            }
        };

        var requestData = new Dictionary<string, object>
        {
            { "model", "gpt-4o" },
            { "messages", messages },
            { "temperature", 0 }
        };

        string jsonBody = JsonConvert.SerializeObject(requestData);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + openAIKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("ChatGPT error: " + request.error);
            Debug.LogError("ChatGPT response: " + request.downloadHandler.text);
        }
        else
        {
            string json = request.downloadHandler.text;
            var parsed = JsonUtility.FromJson<ChatGPTResponse>(json);
            onResponse(parsed.choices[0].message.content);
        }
    }
}

[System.Serializable]
public class ChatGPTMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class ChatGPTChoice
{
    public ChatGPTMessage message;
}

[System.Serializable]
public class ChatGPTResponse
{
    public ChatGPTChoice[] choices;
}

