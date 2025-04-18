using UnityEngine;
using System.Collections;

public class VoiceCommandManager : MonoBehaviour
{
    public VoiceRecorder recorder;
    public WhisperTranscriber transcriber;
    private bool isCoolingDown = false;
    public ChatGPTClient chatGPT;
    public CodeWindowManager codeWindow;

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

            if (IsCodeModeActivation(input))
            {
                chatGPT.isInCodeMode = true;
                codeWindow.resultOutput.text = "Code Mode Activated";
                return;
            }

            if (IsCodeModeDeactivation(input))
            {
                chatGPT.isInCodeMode = false;
                codeWindow.resultOutput.text = "Left Code Mode";
                return;
            }

            Debug.Log("Transcribed: " + input);

            StartCoroutine(chatGPT.GetAIHelp(input, (response) =>
            {
                codeWindow.codeInput.text = response;
            }));
        }));
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1f); // 1 second cooldown
        isCoolingDown = false;
    }

    private bool IsCodeModeActivation(string input)
    {
        string[] activationPhrases = new[]
        {
            "code mode", "codemode", "cod mod", "cowd mode", "cold mode", "coat mode", "codemood", "cowed mode",
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
            "exit code mode", "leave code mode", "stop coding", "exit coding", "normal mode"
        };

        foreach (var phrase in deactivationPhrases)
        {
            if (input.Contains(phrase))
                return true;
        }

        return false;
    }
}
