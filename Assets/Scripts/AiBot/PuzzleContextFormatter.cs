using UnityEngine;
using System.Text;

public class PuzzleContextFormatter : MonoBehaviour
{
    [HideInInspector]
    public int NextHintIndex = 0;

    [TextArea(1, 10)]
    public string rawString; // Example: string = "T9a52D6am"

    [Tooltip("Hints to guide the player (one per line)")]
    public string[] hints;

    [Tooltip("The correct answer code line, e.g. secret_code = \"TaDam\"")]
    public string answerLine;

    public string GetPromptString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Puzzle:");
        sb.AppendLine(rawString); // Example: string = "T9a52D6am"

        for (int i = 0; i < hints.Length; i++)
        {
            sb.AppendLine("# Hint " + (i + 1) + ": " + hints[i]);
        }

        sb.AppendLine("What is the secret code?");
        sb.AppendLine();
        sb.AppendLine("Answer format: " + answerLine);

        return sb.ToString();
    }

    public string GetRandomHint()
    {
        if (hints == null || hints.Length == 0)
            return "No hints available for this puzzle.";

        return hints[UnityEngine.Random.Range(0, hints.Length)];
    }
}
