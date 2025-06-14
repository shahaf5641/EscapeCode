using UnityEngine;
using System.Collections;

public class GlowEffect : MonoBehaviour
{
    [Header("Glow Settings")]
    public GameObject glowPrefab;
    public float delayBeforeGlow = 1f;
    public bool startOnAwake = false;

    private bool hasInteracted = false;
    private Coroutine glowRoutine;
    private float elapsedTime = 0f;
    private bool isPaused = false;

    void Awake()
    {
        if (glowPrefab != null)
            glowPrefab.SetActive(false);
    }

    void Start()
    {
        if (startOnAwake && HintGlowManager.Instance?.HintsEnabled == true)
            StartGlow();
    }

    public void StartGlow()
    {
        if (!gameObject.activeInHierarchy)
        {
            Invoke(nameof(StartGlow), 0.5f);
            return;
        }

        if (glowRoutine != null)
            StopCoroutine(glowRoutine);

        isPaused = false;
        glowRoutine = StartCoroutine(GlowAfterDelay());
    }

    public void MarkInteracted()
    {
        hasInteracted = true;
        if (glowRoutine != null)
            StopCoroutine(glowRoutine);

        if (glowPrefab != null)
            glowPrefab.SetActive(false);
    }

    IEnumerator GlowAfterDelay()
    {
        while (elapsedTime < delayBeforeGlow)
        {
            if (!isPaused)
                elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (!hasInteracted && glowPrefab != null)
            glowPrefab.SetActive(true);
    }

    public void OnHintStateChanged(bool hintsOn)
    {
        if (!gameObject.activeInHierarchy)
        {
            // Skip coroutine on inactive objects
            return;
        }

        isPaused = !hintsOn;

        if (hintsOn)
        {
            if (glowRoutine == null && !hasInteracted)
                glowRoutine = StartCoroutine(GlowAfterDelay());
        }
        else
        {
            if (glowPrefab != null)
                glowPrefab.SetActive(false);
        }
    }


    public void ResetInteraction()
    {
        hasInteracted = false;
        elapsedTime = 0f;
        isPaused = false;

        if (glowRoutine != null)
            StopCoroutine(glowRoutine);

        if (glowPrefab != null)
            glowPrefab.SetActive(false);
    }
}
