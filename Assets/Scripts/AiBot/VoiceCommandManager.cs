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

            if (input.Contains("code mode"))
            {
                chatGPT.isInCodeMode = true;
                codeWindow.resultOutput.text = "🧠 Code Mode Activated. Speak your code guess.";
                return;
            }

            if (input.Contains("exit code mode") || input.Contains("leave code mode"))
            {
                chatGPT.isInCodeMode = false;
                codeWindow.resultOutput.text = "👋 Left Code Mode. You can now talk normally.";
                return;
            }

            Debug.Log("Transcribed: " + text);

            StartCoroutine(chatGPT.GetAIHelp(text, (response) =>
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
}
