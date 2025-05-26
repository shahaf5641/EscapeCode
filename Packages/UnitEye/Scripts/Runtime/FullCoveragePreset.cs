using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calibration preset that covers both edges and central grid
/// </summary>
public class FullCoveragePreset : CalibrationPreset
{
    private int edgeSteps = 10;
    private int gridCols = 4;
    private int gridRows = 3;

    public FullCoveragePreset(float padding) : base(padding) { }

    public override List<Vector2> GetPoints()
    {
        List<Vector2> points = new List<Vector2>();

        float width = Screen.width;
        float height = Screen.height;

        // --- 1. Add Edge Points ---

        // Top edge
        for (int i = 0; i <= edgeSteps; i++)
        {
            float x = Mathf.Lerp(padding, width - padding, i / (float)edgeSteps);
            points.Add(new Vector2(x, padding));
        }

        // Right edge
        for (int i = 1; i <= edgeSteps; i++)
        {
            float y = Mathf.Lerp(padding, height - padding, i / (float)edgeSteps);
            points.Add(new Vector2(width - padding, y));
        }

        // Bottom edge
        for (int i = 1; i <= edgeSteps; i++)
        {
            float x = Mathf.Lerp(width - padding, padding, i / (float)edgeSteps);
            points.Add(new Vector2(x, height - padding));
        }

        // Left edge
        for (int i = 1; i < edgeSteps; i++)
        {
            float y = Mathf.Lerp(height - padding, padding, i / (float)edgeSteps);
            points.Add(new Vector2(padding, y));
        }

        // --- 2. Add Grid Points in Center Area ---

        for (int row = 0; row < gridRows; row++)
        {
            float y = Mathf.Lerp(padding, height - padding, (row + 1) / (float)(gridRows + 1));
            for (int col = 0; col < gridCols; col++)
            {
                float x = Mathf.Lerp(padding, width - padding, (col + 1) / (float)(gridCols + 1));
                points.Add(new Vector2(x, y));
            }
        }

        return points;
    }
}
