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

        if (isInCodeMode)
        {
            systemPrompt =
            "You are in CODE MODE.\n" +
            "You convert the player's spoken input into one line of valid Python code, based only on what they say.\n\n" +

            "\uD83C\uDFAF Your job is to format and correct **only the user's guess**, not to solve the puzzle yourself.\n\n" +

            "\u2705 WHEN TO RETURN CODE:\n" +
            "- If the player says something code-like (e.g., 'secret code equals T A D A M'), format it.\n" +
            "- If the guess has small errors (wrong case, missing underscores), fix it.\n" +
            "- If the guess is extremely close to the real answer, you may auto-correct it slightly.\n\n" +

            "\u274C WHEN **NOT** TO REVEAL THE ANSWER:\n" +
            "- If the guess is far from correct (e.g., 'secret code equals pizza'), do NOT give the real answer.\n" +
            "- If the player says 'I don\u2019t know', do NOT say the answer.\n\n" +

            "\uD83D\uDCAC OFF-TOPIC or GENERAL input: reply with a helpful message like:\n" +
            "'That doesn\u2019t sound like a code guess. Try again.'\n\n" +

            "---\n" +
            "\uD83D\uDD0D Examples:\n\n" +

            "Player: secret underscore code equals T A D A M\n" +
            "You: secret_code = \"TADAM\"\n\n" +

            "Player: secret code equals tadam\n" +
            "You: secret_code = \"TADAM\"\n\n" +

            "Player: code is burger\n" +
            "You: code = \"burger\"\n\n" +

            "Player: secret code equals pizza\n" +
            "You: secret_code = \"pizza\"\n\n" +

            "Player: secret code equals bluh bluh bluh\n" +
            "You: secret_code = \"bluhbluhbluh\"\n\n" +

            "Player: secret code is something like TaDam\n" +
            "You: secret_code = \"TaDam\"\n\n" +

            "Player: I think it\u2019s maybe TADUM\n" +
            "You: secret_code = \"TADUM\"\n\n" +

            "Player: I don\u2019t know the code\n" +
            "You: No worries! Try looking at the string and figuring out what letters are being selected.\n\n" +

            "Player: I want to eat pizza\n" +
            "You: That doesn\u2019t sound like a code guess. Try again.\n\n" +

            "Player: what's the puzzle again?\n" +
            "You: You're trying to extract letters from a string based on index positions to form a word.\n\n" +

            "---\n" +
            "Only return code **if the player speaks a guess**.\n" +
            "Do NOT solve puzzles unless their guess is close to the correct answer.\n" +
            "Do NOT say the correct code unless the user almost says it directly.\n";
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

