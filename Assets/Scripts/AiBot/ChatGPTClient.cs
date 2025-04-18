using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ChatGPTClient : MonoBehaviour
{
    public bool isInCodeMode = false;
    public GameObject currentPuzzle;
    private string openAIKey;
    
    void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("openai_config");

        if (jsonFile == null)
        {
            Debug.LogError("❌ openai_config.json not found in Resources!");
        }
        OpenAIConfig config = JsonUtility.FromJson<OpenAIConfig>(jsonFile.text);
        openAIKey = config?.openai_api_key;
    }
    public IEnumerator GetAIHelp(string prompt, Action<string> onResponse)
    {
        string apiUrl = "https://api.openai.com/v1/chat/completions";
        string systemPrompt;

        if (isInCodeMode)
        {
            systemPrompt =
            "You are in CODE MODE.\n" +
            "Your job is to take the player's spoken input and convert it into one line of valid Python code that reflects what they meant to write — even if the guess is incorrect.\n\n" +

            "Focus on clarity, accuracy, and clean syntax.\n\n" +

            "You must:\n" +
            "- Extract the intent from the spoken phrase\n" +
            "- Preserve letter casing if specified (capitalize spelled-out letters like T A D A M → TADAM)\n" +
            "- Respect variable names like 'secret code' → secret_code\n" +
            "- If the input sounds like code (contains 'code', 'equals', 'underscore', spelled letters, etc.), translate it into a Python assignment\n\n" +

            "Examples:\n\n" +

            "Player: secret underscore code equals T A D A M\n" +
            "You: secret_code = \"TADAM\"\n\n" +

            "Player: secret code equals ta-ba-v\n" +
            "You: secret_code = \"tabav\"\n\n" +

            "Player: code equals burger\n" +
            "You: code = \"burger\"\n\n" +

            "Player: secret code is blah blah blah\n" +
            "You: secret_code = \"blahblahblah\"\n\n" +

            "If the input is completely off-topic (e.g. \"I like pizza\"), respond with:\n" +
            "\"That doesn’t sound like a code guess. Try saying the code again.\"";
        }
        else
        {
            systemPrompt =
            "You are a friendly AI assistant inside a coding puzzle game. " +
            "The player can ask you questions about the game, the puzzles, or anything in general. " +
            "Answer helpfully and in-character as an assistant guiding them through a mysterious game world.\n\n" +
            "If the player wants to solve a puzzle, they may say \"code mode\" to begin submitting code-like guesses.";
        }

        string puzzleContext = currentPuzzle.GetComponent<PuzzleContextFormatter>().GetPromptString();
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

