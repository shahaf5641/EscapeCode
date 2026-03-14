using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceCommandManager : MonoBehaviour
{
    private const string VoiceUnavailableMessage = "This game build does not support voice recording because it requires an OpenAI API key.";

    public VoiceRecorder recorder;
    public WhisperTranscriber transcriber;
    public ChatGPTClient chatGPT;
    public CodeWindowManager codeWindow;
    public MicToVirtualClick micClick;
    public TextMeshProUGUI userTextDisplay;
    public Button recordToggleButton;
    public TextMeshProUGUI recordToggleLabel;

    [SerializeField] private AudioSource aiReplySound;

    private Coroutine silenceMonitorRoutine;
    private float lastLoudTime;
    private bool isRecording = false;
    private bool isCoolingDown = false;
    private bool wasMicClickOnBeforeRecording = false;
    private string lastUsedMic;
    private bool hasApiKey;

    public float silenceThreshold = 0.005f;
    public float silenceTimeout = 2f;

    void Awake()
    {
        hasApiKey = OpenAIKeyLoader.HasApiKey();

        if (micClick == null)
        {
            micClick = FindObjectOfType<MicToVirtualClick>();
            if (micClick == null)
            {
                Debug.LogError("MicToVirtualClick not found in scene.");
            }
            else
            {
                Debug.Log("MicToVirtualClick assigned dynamically.");
            }
        }

        if (!hasApiKey)
        {
            if (recordToggleLabel != null)
                recordToggleLabel.text = "Voice Disabled";

            if (recordToggleButton != null)
            {
                recordToggleButton.onClick.RemoveAllListeners();
                recordToggleButton.onClick.AddListener(ShowVoiceUnavailableMessage);
            }
        }
    }

    public void ToggleRecording()
    {
        if (!hasApiKey)
        {
            ShowVoiceUnavailableMessage();
            return;
        }

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
        if (!hasApiKey)
        {
            ShowVoiceUnavailableMessage();
            return;
        }

        if (micClick != null)
        {
            wasMicClickOnBeforeRecording = micClick.enabled;
            micClick.StopMicrophone();
            micClick.enabled = false;
        }

        string micName = micClick != null ? micClick.GetSelectedMicName() : Microphone.devices.FirstOrDefault();
        lastUsedMic = micName;

        StartCoroutine(RecordWithDelay());
    }

    private IEnumerator RecordWithDelay()
    {
        Microphone.End(null);
        yield return new WaitForSeconds(0.2f);

        recorder.StartRecording();
        isRecording = true;

        if (recordToggleLabel != null)
            recordToggleLabel.text = "Stop";

        lastLoudTime = Time.time;

        if (silenceMonitorRoutine != null)
            StopCoroutine(silenceMonitorRoutine);

        silenceMonitorRoutine = StartCoroutine(MonitorSilenceFromRecorder());
    }

    public void StopVoiceCommand()
    {
        if (isCoolingDown || !isRecording)
            return;

        isCoolingDown = true;
        isRecording = false;

        if (recordToggleLabel != null)
            recordToggleLabel.text = hasApiKey ? "Record" : "Voice Disabled";

        StartCoroutine(Cooldown());

        if (silenceMonitorRoutine != null)
        {
            StopCoroutine(silenceMonitorRoutine);
            silenceMonitorRoutine = null;
        }

        bool saved = recorder.StopRecordingAndSave();
        if (!saved)
        {
            RestoreMicClickIfNeeded();
            FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage("Voice recording failed");
            return;
        }

        StartCoroutine(FinalizeVoiceCommand());
    }

    private IEnumerator FinalizeVoiceCommand()
    {
        yield return new WaitForSeconds(0.1f);

        string savedFilePath = recorder.GetSavedFilePath();
        if (string.IsNullOrWhiteSpace(savedFilePath))
        {
            RestoreMicClickIfNeeded();
            FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage("Voice recording failed");
            yield break;
        }

        yield return StartCoroutine(transcriber.TranscribeAudio(savedFilePath, text =>
        {
            RestoreMicClickIfNeeded();

            string input = text.ToLowerInvariant();
            input = new string(input.Where(c => !char.IsPunctuation(c)).ToArray());

            var words = input.Split(' ');
            if (words.Length <= 3 && words.Any(w => w == "stop" || w == "listening"))
            {
                FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Voice input stopped.");
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
            Debug.LogWarning("No clip to monitor.");
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
            float sum = 0f;
            float peak = 0f;

            foreach (float sample in samples)
            {
                float absoluteValue = Mathf.Abs(sample);
                sum += absoluteValue;
                if (absoluteValue > peak)
                    peak = absoluteValue;
            }

            float avg = sum / samples.Length;
            float rms = Mathf.Sqrt(sum / samples.Length);

            Debug.Log($"Voice RMS={rms:F5} | Peak={peak:F5} | Avg={avg:F5} | Silence={Time.time - lastLoudTime:F2}s");

            if (rms > silenceThreshold)
                lastLoudTime = Time.time;

            if (Time.time - lastLoudTime > silenceTimeout)
            {
                Debug.Log("Silence detected. Stopping voice recording.");
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

        StartCoroutine(chatGPT.GetAIHelp(input, response =>
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
        string[] phrases =
        {
            "code mode", "codemode", "codmode", "codmod", "cod mod", "cowd mode", "cold mode",
            "coat mode", "codemood", "cowed mode", "start coding", "coding mode", "i want to guess"
        };

        return phrases.Any(input.Contains);
    }

    private bool IsCodeModeDeactivation(string input)
    {
        string[] phrases =
        {
            "leave mode", "stop coding", "exit coding", "normal mode"
        };

        return phrases.Any(input.Contains);
    }

    private bool IsSubmitRequested(string input)
    {
        string[] phrases =
        {
            "submit", "submit answer", "submit code", "send code", "check", "check my answer", "submit my code"
        };

        return phrases.Any(input.Contains);
    }

    public void TriggerHintManually()
    {
        if (!IsHintRequested("hint"))
            return;

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

    private void RestoreMicClickIfNeeded()
    {
        if (micClick != null && wasMicClickOnBeforeRecording)
        {
            micClick.enabled = true;
            micClick.ActivateMic();
        }
    }

    private void ShowVoiceUnavailableMessage()
    {
        FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage(VoiceUnavailableMessage);
    }
}
