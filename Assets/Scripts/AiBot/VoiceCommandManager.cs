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
    public float silenceThreshold = 0.05f;
    public float silenceTimeout = 2f;
    private float lastLoudTime;

    private bool isRecording = false;
    private bool isCoolingDown = false;
    private bool wasMicClickOnBeforeRecording = false;
    private AudioClip micMonitorClip;
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
            micClick.enabled = false;
        }

        recorder.StartRecording();
        isRecording = true;
        recordToggleLabel.text = "Stop";

        lastUsedMic = Microphone.devices.FirstOrDefault();
        micMonitorClip = Microphone.Start(lastUsedMic, true, 1, 44100);
        lastLoudTime = Time.time;
        if (silenceMonitorRoutine != null)
            StopCoroutine(silenceMonitorRoutine);
        silenceMonitorRoutine = StartCoroutine(MonitorSilence());
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

        if (!string.IsNullOrEmpty(lastUsedMic))
        {
            Microphone.End(lastUsedMic);
            micMonitorClip = null;
        }

        recorder.StopRecordingAndSave();

        StartCoroutine(transcriber.TranscribeAudio(recorder.GetSavedFilePath(), (text) =>
        {
            if (micClick != null)
            {
                micClick.enabled = wasMicClickOnBeforeRecording;
                if (micClick.enabled)
                {
                    micClick.ActivateMic(); // Reactivate listening logic
                }
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
    private IEnumerator MonitorSilence()
    {
        float[] samples = new float[128];

        while (true)
        {
            if (micMonitorClip == null || !Microphone.IsRecording(lastUsedMic))
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            int micPos = Microphone.GetPosition(lastUsedMic);
            if (micPos < samples.Length)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            micMonitorClip.GetData(samples, micPos - samples.Length);
            float sum = samples.Sum(sample => sample * sample);
            float volume = Mathf.Sqrt(sum / samples.Length);

            if (volume > silenceThreshold)
                lastLoudTime = Time.time;

            if (Time.time - lastLoudTime > silenceTimeout)
            {
                Debug.Log("🛑 Silence detected. Auto-stopping voice command.");
                StopVoiceCommand();
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
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
