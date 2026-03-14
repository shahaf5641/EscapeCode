using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class WindowsBuild
{
    private const string OutputFolder = "Builds/Windows";
    private const string ExecutableName = "EscapeCode.exe";

    [MenuItem("Build/Build Windows Release")]
    public static void BuildWindowsRelease()
    {
        BuildPlayer(BuildOptions.None);
    }

    public static void BuildWindowsReleaseBatchMode()
    {
        BuildPlayer(BuildOptions.None);
    }

    private static void BuildPlayer(BuildOptions buildOptions)
    {
        string[] scenes =
        {
            "Assets/Scenes/MainMenuScene.unity",
            "Assets/Scenes/FirstRoomScene.unity",
            "Assets/Scenes/SecondRoomScene.unity",
            "Assets/Scenes/ThirdRoomScene.unity"
        };

        string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), OutputFolder);
        Directory.CreateDirectory(outputDirectory);

        string outputPath = Path.Combine(outputDirectory, ExecutableName);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneWindows64,
            options = buildOptions
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Windows build failed: {report.summary.result}");
        }

        UnityEngine.Debug.Log($"Windows build completed successfully: {outputPath}");
    }
}
