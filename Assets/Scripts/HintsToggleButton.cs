using UnityEngine;
using TMPro; // for TextMeshPro

public class HintsToggleButton : MonoBehaviour
{
    public TextMeshProUGUI hintStatusText; // assign the "on/off" text object

    void Start()
    {
        UpdateButtonText();
    }

    public void ToggleHints()
    {
        bool newState = !HintGlowManager.Instance.HintsEnabled;
        HintGlowManager.Instance.SetHintsEnabled(newState);
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (hintStatusText != null)
        {
            hintStatusText.text = HintGlowManager.Instance.HintsEnabled ? "on" : "off";
        }
    }
}
