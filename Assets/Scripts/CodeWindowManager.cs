using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class CodeWindowManager : MonoBehaviour
{
    public TextMeshProUGUI problemText;
    public TextMeshProUGUI resultOutput;
    public TextMeshProUGUI youSaidText;
    public GameObject panel;
    public TMP_InputField aiResponse;
    public TMP_InputField userInput;
    private PuzzleContextFormatter lastPuzzleFormatter = null;

    
    private Func<string, bool> solveCheck;
    private Action onSolved;

    private bool solved = false;

public void Open(string problemDescription, string defaultCode, Func<string, bool> checkFunc, Action successCallback)
{
    panel.SetActive(true);
    PlayerController.IsMovementLocked = true;

    if (FindFirstObjectByType<ChatGPTClient>().currentPuzzle.TryGetComponent(out PuzzleContextFormatter currentPuzzleFormatter))
    {
        if (lastPuzzleFormatter != currentPuzzleFormatter)
        {
            currentPuzzleFormatter.NextHintIndex = 0; // ✅ Only reset if different puzzle
            lastPuzzleFormatter = currentPuzzleFormatter; // ✅ Update the last opened
        }
    }

    StartCoroutine(SetContentDelayed(problemDescription, defaultCode, checkFunc, successCallback));
}


    private IEnumerator SetContentDelayed(string problemDescription, string defaultCode, Func<string, bool> checkFunc, Action successCallback)
    {
        yield return null;
        problemText.text = problemDescription;
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
            youSaidText.text = "";
            userInput.text = "";
            aiResponse.text = "";
            resultOutput.text = "";
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
        panel.SetActive(false);
        PlayerController.IsMovementLocked = false;
    }
}