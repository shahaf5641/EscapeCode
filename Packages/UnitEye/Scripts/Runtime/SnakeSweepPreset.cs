using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Snake pattern calibration preset: first horizontal snake, then vertical snake
/// </summary>
public class SnakeSweepPreset : CalibrationPreset
{
    private int horizontalSteps = 10;
    private int verticalStepsDown = 5;
    private int verticalStepsUp = 5;

    public SnakeSweepPreset(float padding) : base(padding) { }

    public override List<Vector2> GetPoints()
    {
        List<Vector2> points = new List<Vector2>();

        float width = Screen.width;
        float height = Screen.height;

        float horizontalStep = width / horizontalSteps;
        float verticalStepDown = height / verticalStepsDown;
        float verticalStepUp = height / verticalStepsUp;

        bool goRight = true;

        // Horizontal snake: top to bottom
        for (int v = 0; v <= verticalStepsDown; v++)
        {
            float y = v * verticalStepDown;

            if (goRight)
            {
                for (int h = 0; h <= horizontalSteps; h++)
                {
                    float x = h * horizontalStep;
                    points.Add(new Vector2(x, y));
                }
            }
            else
            {
                for (int h = horizontalSteps; h >= 0; h--)
                {
                    float x = h * horizontalStep;
                    points.Add(new Vector2(x, y));
                }
            }

            goRight = !goRight;
        }

        // Vertical snake: left to right (bottom to top)
        bool goUp = true;
        for (int h = 0; h <= horizontalSteps; h++)
        {
            float x = h * horizontalStep;

            if (goUp)
            {
                for (int v = verticalStepsUp; v >= 0; v--)
                {
                    float y = v * verticalStepUp;
                    points.Add(new Vector2(x, y));
                }
            }
            else
            {
                for (int v = 0; v <= verticalStepsUp; v++)
                {
                    float y = v * verticalStepUp;
                    points.Add(new Vector2(x, y));
                }
            }

            goUp = !goUp;
        }

        return points;
    }
}
