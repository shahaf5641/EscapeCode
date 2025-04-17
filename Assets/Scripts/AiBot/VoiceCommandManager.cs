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
            Debug.Log("Transcribed: " + text);
            StartCoroutine(chatGPT.GetAIHelp(text, (response) =>
            {
                codeWindow.codeInput.text = response;
            }));
        }));
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(10); // 10 second cooldown
        isCoolingDown = false;
    }

}
