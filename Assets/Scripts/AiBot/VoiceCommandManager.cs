using UnityEngine;
using System.Collections;
using System.Linq;

public class VoiceCommandManager : MonoBehaviour
{
    public VoiceRecorder recorder;
    public WhisperTranscriber transcriber;
    public ChatGPTClient chatGPT;
    public CodeWindowManager codeWindow;
    public MicToVirtualClick micClick;
    public TMPro.TextMeshProUGUI userTextDisplay;
    public UnityEngine.UI.Button recordToggleButton;
    public TMPro.TextMeshProUGUI recordToggleLabel;
    [SerializeField] private AudioSource aiReplySound;
    private Coroutine silenceMonitorRoutine;
    public float silenceThreshold = 0.005f;
    public float silenceTimeout = 2f;
    private float lastLoudTime;
    private bool isRecording = false;
    private bool isCoolingDown = false;
    private bool wasMicClickOnBeforeRecording = false;
    private string lastUsedMic;
    
    void Awake()
    {
        if (micClick == null)
        {
            micClick = FindObjectOfType<MicToVirtualClick>();
            if (micClick == null)
            {
                Debug.LogError("❌ MicToVirtualClick not found in scene!");
            }
            else
            {
                Debug.Log("✅ MicToVirtualClick assigned dynamically.");
            }
        }
    }

    public void ToggleRecording()
    {
        if (!isRecording)
        {
            StartVoiceCommand();
        }
        else
        {
            StopVoiceCommand();
        }
    }

    public void StartVoiceCommand()
    {
        if (micClick != null)
        {
            wasMicClickOnBeforeRecording = micClick.enabled;
            micClick.StopMicrophone();
            micClick.enabled = false;
        }

        string micName = micClick != null ? micClick.GetSelectedMicName() : Microphone.devices.FirstOrDefault();
        lastUsedMic = micName;

        StartCoroutine(RecordWithDelay(micName));
    }

    private IEnumerator RecordWithDelay(string micName)
    {
        // Stop all microphone instances first
        Microphone.End(null);
        yield return new WaitForSeconds(0.2f);

        // Start recording - let VoiceRecorder handle the microphone
        recorder.StartRecording();
        isRecording = true;
        recordToggleLabel.text = "Stop";

        lastLoudTime = Time.time;

        if (silenceMonitorRoutine != null)
            StopCoroutine(silenceMonitorRoutine);
        silenceMonitorRoutine = StartCoroutine(MonitorSilenceFromRecorder());
    }

    public void StopVoiceCommand()
    {
        if (isCoolingDown || !isRecording) return;

        isCoolingDown = true;
        isRecording = false;
        recordToggleLabel.text = "Record";
        StartCoroutine(Cooldown());

        if (silenceMonitorRoutine != null)
        {
            StopCoroutine(silenceMonitorRoutine);
            silenceMonitorRoutine = null;
        }

        recorder.StopRecordingAndSave();
        StartCoroutine(FinalizeVoiceCommand());
    }

    private IEnumerator FinalizeVoiceCommand()
    {
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(transcriber.TranscribeAudio(recorder.GetSavedFilePath(), (text) =>
        {
            if (micClick != null && wasMicClickOnBeforeRecording)
            {
                micClick.enabled = true;
                micClick.ActivateMic();
            }

            string input = text.ToLower();
            input = new string(input.Where(c => !char.IsPunctuation(c)).ToArray());

            var words = input.Split(' ');
            if (words.Length <= 3 && words.Any(w => w == "stop" || w == "listening"))
            {
                FindFirstObjectByType<FeedbackUIManager>().ShowMessage("🎤 Voice input stopped.");
                return;
            }

            HandleTranscribedInput(input);
        }));
    }

private IEnumerator MonitorSilenceFromRecorder()
{
    yield return new WaitForSeconds(0.5f);

    const int sampleWindow = 1024;
    float[] samples = new float[sampleWindow];
    AudioClip micClip = recorder.GetRecordedClip();

    if (micClip == null)
    {
        Debug.LogWarning("⚠️ No clip to monitor.");
        yield break;
    }

    while (recorder.IsRecording())
    {
        int micPos = Microphone.GetPosition(lastUsedMic);
        if (micPos < sampleWindow)
        {
            yield return new WaitForSeconds(0.1f);
            continue;
        }

        micClip.GetData(samples, micPos - sampleWindow);
        float sum = 0f, peak = 0f;
        foreach (var s in samples)
        {
            float abs = Mathf.Abs(s);
            sum += abs;
            if (abs > peak) peak = abs;
        }

        float avg = sum / samples.Length;
        float rms = Mathf.Sqrt(sum / samples.Length);

        Debug.Log($"🎙️ RMS={rms:F5} | Peak={peak:F5} | Avg={avg:F5} | Silence={Time.time - lastLoudTime:F2}s");

        if (rms > silenceThreshold)
            lastLoudTime = Time.time;

        if (Time.time - lastLoudTime > silenceTimeout)
        {
            Debug.Log("🛑 Silence detected. Stopping...");
            StopVoiceCommand();
            yield break;
        }

        yield return new WaitForSeconds(0.2f);
    }
}



    private void HandleTranscribedInput(string input)
    {
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
            aiReplySound.Play();
            if (chatGPT.isInCodeMode)
                codeWindow.userInput.text = response;
            else
                codeWindow.AppendChatLine(input, response);
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
        string[] phrases = {
            "code mode", "codemode", "codmode", "codmod", "cod mod", "cowd mode", "cold mode",
            "coat mode", "codemood", "cowed mode", "start coding", "coding mode", "i want to guess"
        };
        return phrases.Any(input.Contains);
    }

    private bool IsCodeModeDeactivation(string input)
    {
        string[] phrases = {
            "leave mode", "stop coding", "exit coding", "normal mode"
        };
        return phrases.Any(input.Contains);
    }

    private bool IsSubmitRequested(string input)
    {
        string[] phrases = {
            "submit", "submit answer", "submit code", "send code", "check", "check my answer", "submit my code"
        };
        return phrases.Any(input.Contains);
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
        }
    }
}