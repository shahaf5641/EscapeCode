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

    void Awake()
    {
        if (glowPrefab != null)
            glowPrefab.SetActive(false);
    }

    void Start()
    {
        if (startOnAwake)
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
        yield return new WaitForSeconds(delayBeforeGlow);

        if (!hasInteracted && glowPrefab != null)
            glowPrefab.SetActive(true);
    }

    // Optional if you want to re-allow glow later
    public void ResetInteraction()
    {
        hasInteracted = false;
        if (glowPrefab != null)
            glowPrefab.SetActive(false);
    }
}
