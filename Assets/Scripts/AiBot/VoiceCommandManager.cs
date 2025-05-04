using UnityEngine;
using System.Collections;
using System.Linq;

public class VoiceCommandManager : MonoBehaviour
{
    public VoiceRecorder recorder;
    public WhisperTranscriber transcriber;
    private bool isCoolingDown = false;
    public ChatGPTClient chatGPT;
    public CodeWindowManager codeWindow;
    public TMPro.TextMeshProUGUI userTextDisplay;
    public UnityEngine.UI.Button recordToggleButton;
    public TMPro.TextMeshProUGUI recordToggleLabel;
    private bool isRecording = false;

    public void ToggleRecording()
    {
        if (!isRecording)
        {
            StartVoiceCommand();
            recordToggleLabel.text = "Stop";
            isRecording = true;
        }
        else
        {
            StopVoiceCommand();
            recordToggleLabel.text = "Record";
            isRecording = false;
        }
    }

    public void StartVoiceCommand()
    {
        recorder.StartRecording();
    }

    public void StopVoiceCommand()
    {
        if (isCoolingDown) return;

        isCoolingDown = true;
        StartCoroutine(Cooldown());
        recorder.StopRecordingAndSave();
        StartCoroutine(transcriber.TranscribeAudio(recorder.GetSavedFilePath(), (text) =>
        {
            string input = text.ToLower();
            input = new string(input.Where(c => !char.IsPunctuation(c)).ToArray());
            if (IsHintRequested(input))
            {
                var puzzle = chatGPT.currentPuzzle.GetComponent<PuzzleContextFormatter>();

                if (puzzle != null)
                {
                    if (puzzle.NextHintIndex < puzzle.hints.Length)
                    {
                        string nextHint = $"Hint {puzzle.NextHintIndex + 1}: {puzzle.hints[puzzle.NextHintIndex]}";
                        codeWindow.resultOutput.text += $"\n{nextHint}";
                        puzzle.NextHintIndex++;
                    }
                    else
                    {
                        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("No More Hints Available");
                    }
                }
                return;
            }

            if (IsCodeModeActivation(input))
            {
                chatGPT.isInCodeMode = true;
                FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Code Mode Activated");
                codeWindow.EnableCodeMode();
                return;
            }

            if (IsCodeModeDeactivation(input))
            {
                chatGPT.isInCodeMode = false;
                FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Normal Mode Activated");
                codeWindow.DisableCodeMode();
                return;
            }
            if (IsSubmitRequested(input))
            {
                codeWindow.Submit();
                return;
            }
            StartCoroutine(chatGPT.GetAIHelp(input, (response) =>
            {
                if (chatGPT.isInCodeMode)
                {
                    codeWindow.userInput.text = response;
                }
                else
                {
                    codeWindow.AppendChatLine(input, response);
                }
            }));
        }));
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1f);
        isCoolingDown = false;
    }

    private bool IsHintRequested(string input)
    {
        return input.Contains("hint");
    }

    private bool IsCodeModeActivation(string input)
    {
        string[] activationPhrases = new[]
        {
            "code mode", "codemode", "codmode", "codmod", "cod mod", "cowd mode", "cold mode", "coat mode", "codemood", "cowed mode",
            "start coding", "coding mode", "i want to guess"
        };

        foreach (var phrase in activationPhrases)
        {
            if (input.Contains(phrase))
                return true;
        }

        return false;
    }

    private bool IsCodeModeDeactivation(string input)
    {
        string[] deactivationPhrases = new[]
        {
            "leave mode", "stop coding", "exit coding", "normal mode"
        };
        foreach (var phrase in deactivationPhrases)
        {
            if (input.Contains(phrase))
                return true;
        }

        return false;
    }

    public void TriggerHintManually()
    {
        if (IsHintRequested("hint"))
        {
            var puzzle = chatGPT.currentPuzzle.GetComponent<PuzzleContextFormatter>();

            if (puzzle != null)
            {
                if (puzzle.NextHintIndex < puzzle.hints.Length)
                {
                    string nextHint = $"Hint {puzzle.NextHintIndex + 1}: {puzzle.hints[puzzle.NextHintIndex]}";
                    codeWindow.resultOutput.text += $"\n{nextHint}";
                    puzzle.NextHintIndex++;
                }
                else
                {
                    FindFirstObjectByType<FeedbackUIManager>().ShowMessage("No More Hints Available");
                }
            }
            return;
        }
    }
    private bool IsSubmitRequested(string input)
    {
        string[] submitPhrases = new[]
        {
            "submit", "submit answer", "submit code", "send code", "check", "check my answer", "submit my code"
        };

        foreach (var phrase in submitPhrases)
        {
            if (input.Contains(phrase))
                return true;
        }

        return false;
    }
}