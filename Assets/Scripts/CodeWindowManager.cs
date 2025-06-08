using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.IO;

public class CodeWindowManager : MonoBehaviour
{
    public TextMeshProUGUI problemDescText;
    public TextMeshProUGUI problemCodeText;
    public TextMeshProUGUI problemTitleText;
    public TextMeshProUGUI resultOutput;
    public TextMeshProUGUI chatHistoryDisplay;
    public GameObject panel;
    public TMP_InputField userInput;
    private PuzzleContextFormatter lastPuzzleFormatter = null;
    private Func<string, bool> solveCheck;
    private Action onSolved;
    public UISwitcher.UISwitcher switcher2;
    public event Action<bool?> OnValueChangedNullable;
    private bool solved = false;
    public RectTransform modeCircle;
    public Vector3 onLocalPos;
    public Vector3 offLocalPos;
    private GameObject currentSourceObject;
    private Collider currentSourceCollider;
    private HashSet<GameObject> streamedObjects = new();
    private string fullChatLog = "";
    [SerializeField] private ScrollRect chatScrollRect;
    public static bool IsOpen { get; private set; }
    void Start()
    {
        switcher2.isOnNullable = false; // Set visual state to OFF
        FindFirstObjectByType<ChatGPTClient>().isInCodeMode = false; // Ensure logic state is OFF
        switcher2.OnValueChangedNullable += OnSwitcherValueChanged;
        IsOpen = false;
    }

    void OnDestroy()
    {
        switcher2.OnValueChangedNullable -= OnSwitcherValueChanged;
    }
    private void OnSwitcherValueChanged(bool? newValue)
    {
        if (modeCircle != null)
            modeCircle.localPosition = (newValue == true) ? onLocalPos : offLocalPos;

        if (newValue == true)
        {
            FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Code Mode Activated");
            FindFirstObjectByType<ChatGPTClient>().isInCodeMode = true;
        }
        else
        {
            FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Normal Mode Activated");
            FindFirstObjectByType<ChatGPTClient>().isInCodeMode = false;
        }
    }

    public void Open(string problemTitle, string problemDescription, string problemCode, Func<string, bool> checkFunc, Action successCallback, GameObject sourceObject)
    {
        panel.SetActive(true);
        if (sourceObject != null)
        {
            currentSourceObject = sourceObject;
            currentSourceCollider = sourceObject.GetComponent<Collider>();
            if (currentSourceCollider != null)
                currentSourceCollider.enabled = false;
        }

        PlayerController.IsMovementLocked = true;
        BigRobotController.IsMovementLocked = true;

        if (FindFirstObjectByType<ChatGPTClient>().currentPuzzle.TryGetComponent(out PuzzleContextFormatter currentPuzzleFormatter))
        {
            if (lastPuzzleFormatter != currentPuzzleFormatter)
            {
                currentPuzzleFormatter.NextHintIndex = 0;
                lastPuzzleFormatter = currentPuzzleFormatter;
            }
        }

        IsOpen = true;

        bool shouldStream = !streamedObjects.Contains(sourceObject);
        if (shouldStream) streamedObjects.Add(sourceObject);

        StartCoroutine(SetContentDelayed(problemTitle, problemDescription, problemCode, checkFunc, successCallback, shouldStream));
    }


    private IEnumerator TypeText(TextMeshProUGUI target, string fullText, float charDelay = 0.02f)
    {
        target.text = "";
        foreach (char c in fullText)
        {
            target.text += c;
            yield return new WaitForSeconds(charDelay);
        }
    }



    private IEnumerator SetContentDelayed(string problemTitle, string problemDescription, string problemCode, Func<string, bool> checkFunc, Action successCallback, bool stream)
    {
        yield return null;
        problemTitleText.text = problemTitle;

        if (stream)
        {
            // Stream description first
            yield return StartCoroutine(TypeText(problemDescText, problemDescription));

            // Then stream code after description finishes
            yield return StartCoroutine(TypeText(problemCodeText, AddLineNumbers(problemCode), 0.02f));
        }
        else
        {
            problemDescText.text = problemDescription;
            problemCodeText.text = AddLineNumbers(problemCode);
        }

        solveCheck = checkFunc;
        onSolved = successCallback;
        solved = false;
    }




    public void Submit()
    {
        if (solved || solveCheck == null) return;

        if (solveCheck(userInput.text))
        {
            onSolved?.Invoke();
            solved = true;
            chatHistoryDisplay.text = "";
            fullChatLog = "";
            userInput.text = "";
            resultOutput.text = "";
            switcher2.OnValueChangedNullable -= OnSwitcherValueChanged;
            switcher2.isOnNullable = false;
            if (modeCircle != null)
                modeCircle.localPosition = offLocalPos;
            switcher2.OnValueChangedNullable += OnSwitcherValueChanged;
            FindFirstObjectByType<ChatGPTClient>().isInCodeMode = false;
            Close();
        }
        else
        {
            FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Try again");
        }
    }
    public void Close()
    {
        IsOpen = false;
        panel.SetActive(false);
        PlayerController.IsMovementLocked = false;
        BigRobotController.IsMovementLocked = false;

        if (currentSourceCollider != null)
        {
            currentSourceCollider.enabled = true;
            currentSourceCollider = null;
        }

        currentSourceObject = null;
    }


    public void EnableCodeMode() => switcher2.isOnNullable = true;

    public void DisableCodeMode() => switcher2.isOnNullable = false;

    public void ToggleCodeMode()
    {
        bool current = switcher2.isOnNullable ?? false;
        switcher2.isOnNullable = !current;
    }
    public void AppendChatLine(string userMessage, string aiMessage)
    {
        StartCoroutine(StreamChat(userMessage, aiMessage));
    }
    private IEnumerator StreamChat(string userMessage, string aiMessage)
    {
        string userPrefix = "<color=#00c3ff><b>YOU:</b></color> ";
        string aiPrefix = "<color=#ffc300><b>AI:</b></color> ";

        string userLine = userPrefix;
        string aiLine = aiPrefix;

        // Stream user message
        chatHistoryDisplay.text += userLine;
        yield return StartCoroutine(StreamMessage(userMessage, chatHistoryDisplay));

        chatHistoryDisplay.text += "\n" + aiLine;
        yield return StartCoroutine(StreamMessage(aiMessage, chatHistoryDisplay));

        chatHistoryDisplay.text += "\n\n";

        // Scroll to bottom
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }
    private IEnumerator StreamMessage(string message, TextMeshProUGUI target, float charDelay = 0.02f)
    {
        for (int i = 0; i < message.Length; i++)
        {
            target.text += message[i];
            yield return new WaitForSeconds(charDelay);
        }
    }

    public string AddLineNumbers(string text)
    {
        var lines = text.Split('\n');
        List<string> numbered = new();
        int lineNumber = 1;

        foreach (var line in lines)
        {
            string trimmed = line.TrimEnd();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                numbered.Add(""); // Preserve visual spacing without number
            }
            else
            {
                numbered.Add($"{lineNumber++}. {trimmed}");
            }
        }

        return string.Join("\n", numbered);
    }
    public bool RunPythonValidator(string problemId, string userInput)
    {
        string pythonExePath = Path.Combine(Application.dataPath, "..", "Python", "python.exe");
        string scriptPath = Path.Combine(Application.dataPath, "..", "Python", "validate_solution.py");
        userInput = userInput.Replace("\"", "\\\""); // Escape for safety

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = pythonExePath,
            Arguments = $"\"{scriptPath}\" {problemId} \"{userInput}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd().Trim();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                    UnityEngine.Debug.LogError($"Python Error: {error}");

                UnityEngine.Debug.Log($"Python Output: {output}");

                return output == "correct";
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Validation failed: " + ex.Message);
            return false;
        }
    }
}