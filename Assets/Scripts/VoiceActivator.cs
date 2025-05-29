using UnityEngine;
using System.Diagnostics;
using System.IO;

public class VoiceActivator : MonoBehaviour
{
    private Process voiceProcess;
    private string exePath;
    private bool isRunning = false;

    void Start()
    {
        exePath = Path.Combine(Application.dataPath, "../voice_click.exe");
    }

    public void ToggleVoice()
    {
        if (!isRunning)
        {
            StartVoiceClick();
        }
        else
        {
            StopVoiceClick();
        }
    }

    private void StartVoiceClick()
    {
        if (File.Exists(exePath))
        {
            if (voiceProcess == null || voiceProcess.HasExited)
            {
                voiceProcess = new Process();
                voiceProcess.StartInfo.FileName = exePath;
                voiceProcess.StartInfo.UseShellExecute = false;
                voiceProcess.StartInfo.CreateNoWindow = true;
                voiceProcess.Start();
                isRunning = true;
                UnityEngine.Debug.Log("🎤 voice_click.exe started.");
            }
        }
    }

    private void StopVoiceClick()
    {
        // Kill tracked process (if it's still alive)
        if (voiceProcess != null && !voiceProcess.HasExited)
        {
            voiceProcess.Kill();
            voiceProcess.Dispose();
            voiceProcess = null;
        }

        // Force kill ALL matching processes by name as backup
        foreach (var p in Process.GetProcessesByName("voice_click"))
        {
            p.Kill();
            UnityEngine.Debug.Log("🔪 Force-killed stray voice_click.exe");
        }
        isRunning = false;
    }


    void OnApplicationQuit()
    {
        StopVoiceClick(); // Clean up on game exit
    }
}
