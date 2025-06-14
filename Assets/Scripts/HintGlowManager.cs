using UnityEngine;

public class HintGlowManager : MonoBehaviour
{
    public static HintGlowManager Instance { get; private set; }
    public bool HintsEnabled { get; private set; } = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetHintsEnabled(bool enabled)
    {
        HintsEnabled = enabled;

        foreach (var glow in FindObjectsOfType<GlowEffect>(true)) // true includes inactive
        {
            glow.OnHintStateChanged(enabled);
        }
    }
}
