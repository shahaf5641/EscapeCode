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
            Debug.LogError("openai_config.json not found in Resources!");
        }
        OpenAIConfig config = JsonUtility.FromJson<OpenAIConfig>(jsonFile.text);
        openAIKey = config?.openai_api_key;
    }
    public IEnumerator GetAIHelp(string prompt, Action<string> onResponse)
    {
        string apiUrl = "https://api.openai.com/v1/chat/completions";
        string systemPrompt;
        string puzzleType = "";

        if (currentPuzzle.TryGetComponent(out BookInteraction book))
            puzzleType = "book";
        else if (currentPuzzle.TryGetComponent(out ChestInteraction chest))
            puzzleType = "chest";
        else if (currentPuzzle.TryGetComponent(out DoorInteraction door))
            puzzleType = "door";

        string dynamicExamples = GetDynamicExamples(puzzleType);


    if (isInCodeMode)
    {
        systemPrompt =
            "You are in CODE MODE.\n" +
            "You convert the player's spoken input into one line of valid Python code, based only on what they say.\n\n" +

            "🎯 Your job is to format and correct **only the user's guess**, not to solve the puzzle yourself.\n\n" +

            "✅ WHEN TO RETURN CODE:\n" +
            "- If the player says something code-like (e.g., 'secret code equals t a d a m'), format it.\n" +
            "- If the guess has small errors (wrong case, missing underscores), fix it.\n" +
            "- If the guess is extremely close to the real answer, you may auto-correct it slightly.\n\n" +

            "❌ WHEN **NOT** TO REVEAL THE ANSWER:\n" +
            "- If the guess is far from correct (e.g., 'secret code equals pizza'), do NOT give the real answer.\n" +
            "- If the player says 'I don’t know', do NOT say the answer.\n\n" +

            "💬 OFF-TOPIC input: reply 'That doesn’t sound like a code guess. Try again.'\n\n" +

            "---\n" +
            "🔎 Examples:\n\n" +
            dynamicExamples +
            "\n---\n" +
            "Only return code **if the player speaks a guess**.\n" +
            "Do NOT solve puzzles unless the guess is close.\n" +
            "Do NOT say the correct code unless the user says it directly.\n";
    }
    else
    {
        systemPrompt =
            "You are a friendly AI assistant inside a coding puzzle game. " +
            "The player can ask you questions about the game, puzzles, or general advice. " +
            "Help guide them without giving direct answers.\n\n" +
            "If the player wants to solve a puzzle, they may say \"code mode\" to start.\n\n" +
            "Don't give puzzle answers, hints, or solutions! Tell them to say 'hint' if they want help.\n\n" +
            "---\n" +
            "🔎 Examples:\n\n" +
            dynamicExamples;
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
    private string GetDynamicExamples(string puzzleType)
    {
        switch (puzzleType.ToLower())
        {
            case "book":
                return
                    "Player: secret underscore code equals t a d a m\n" +
                    "You: secret_code = \"tadam\"\n\n" +
                    "Player: secret_code code equals tadam\n" +
                    "You: secret_code = \"tadam\"\n\n" +
                    "Player: secret code equals pizza\n" +
                    "You: secret_code = \"pizza\"\n\n";

            case "chest":
                return
                    "Player: has key code equals\n" +
                    "You: has_key_code =\n\n" +
                    "Player: chest locked equals false\n" +
                    "You: chest_locked = False\n\n" +
                    "Player: has key code equals true\n" +
                    "You: has_key_code = True\n\n";

            case "door":
                return
                    "Player: answer equals 20\n" +
                    "You: answer = 20\n\n" +
                    "Player: answer equals \n" +
                    "You: answer = \n\n" +
                    "Player: entered code equals ten\n" +
                    "You: entered_code = 10\n\n";

            case "laptop":
                return
                    "Player: status equals true\n" +
                    "You: status == True:\n\n" +
                    "Player: status equals\n" +
                    "You: status = :\n\n" +
                    "Player: status euals false \n" +
                    "You: status == False \n\n";

            case "password":
                return
                    "Player: zero\n" +
                    "You: 0:\n\n" +
                    "Player: equals zero\n" +
                    "You: 0:\n\n";
            default:
                return "";
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

