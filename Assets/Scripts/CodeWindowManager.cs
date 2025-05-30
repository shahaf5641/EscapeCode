using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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

    public void Open(string problemTitle, string problemDescription, string problemCode, Func<string, bool> checkFunc, Action successCallback)
    {
        panel.SetActive(true);
        PlayerController.IsMovementLocked = true;
        BigRobotController.IsMovementLocked = true;
        if (FindFirstObjectByType<ChatGPTClient>().currentPuzzle.TryGetComponent(out PuzzleContextFormatter currentPuzzleFormatter))
        {
            if (lastPuzzleFormatter != currentPuzzleFormatter)
            {
                currentPuzzleFormatter.NextHintIndex = 0; // ✅ Only reset if different puzzle
                lastPuzzleFormatter = currentPuzzleFormatter; // ✅ Update the last opened
            }
        }
        IsOpen = true;
        StartCoroutine(SetContentDelayed(problemTitle, problemDescription, problemCode, checkFunc, successCallback));
    }


    private IEnumerator SetContentDelayed(string problemTitle, string problemDescription, string problemCode, Func<string, bool> checkFunc, Action successCallback)
    {
        yield return null;
        problemTitleText.text = problemTitle;
        problemDescText.text = problemDescription;
        problemCodeText.text = AddLineNumbers(problemCode);
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
        fullChatLog += $"<color=#00c3ff><b>YOU:</b></color> {userMessage}\n<color=#ffc300><b>AI:</b></color> {aiMessage}\n\n";
        chatHistoryDisplay.text = fullChatLog;

        // Scroll to bottom after canvas updates
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
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

}