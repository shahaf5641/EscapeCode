using UnityEngine;
using System.Diagnostics;
using System.IO;

public class VoiceActivator : MonoBehaviour
{
    Process voiceProcess;

    public void StartVoiceClick()
    {
        string exePath = Path.Combine(Application.dataPath, "../voice_click.exe");

        UnityEngine.Debug.Log("🔍 Checking path: " + exePath);

        if (File.Exists(exePath))
        {
            UnityEngine.Debug.Log("✅ Found voice_click.exe, attempting to start...");

            voiceProcess = new Process();
            voiceProcess.StartInfo.FileName = exePath;
            voiceProcess.StartInfo.UseShellExecute = false;
            voiceProcess.StartInfo.CreateNoWindow = true;
            voiceProcess.Start();

            UnityEngine.Debug.Log("🚀 voice_click.exe launched silently!");
        }
        else
        {
            UnityEngine.Debug.LogError("❌ voice_click.exe NOT FOUND at: " + exePath);
        }
    }

    public void StopVoiceClick()
    {
        if (voiceProcess != null && !voiceProcess.HasExited)
        {
            voiceProcess.Kill();
            voiceProcess.Dispose();
            voiceProcess = null;
            UnityEngine.Debug.Log("🛑 voice_click.exe stopped.");
        }
    }

    void OnApplicationQuit()
    {
        StopVoiceClick();
    }
}
