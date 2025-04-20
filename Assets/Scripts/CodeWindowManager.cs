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
    
    private Func<string, bool> solveCheck;
    private Action onSolved;

    private bool solved = false;

    public void Open(string problemDescription, string defaultCode, Func<string, bool> checkFunc, Action successCallback)
    {
        panel.SetActive(true);
        PlayerController.IsMovementLocked = true;
        StartCoroutine(SetContentDelayed(problemDescription, defaultCode, checkFunc, successCallback));
    }

    private IEnumerator SetContentDelayed(string problemDescription, string defaultCode, Func<string, bool> checkFunc, Action successCallback)
    {
        yield return null;
        problemText.text = problemDescription;
        resultOutput.text = "";
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