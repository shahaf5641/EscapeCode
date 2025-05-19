using UnityEngine;
using TMPro;
using System.Collections;

public class FeedbackUIManager : MonoBehaviour
{
    public GameObject panel;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI feedbackText;
    public float fadeDuration = 0.5f;
    public float displayTime = 2f;
    [SerializeField] private AudioSource messageSound;

    private Coroutine fadeRoutine;

    public void ShowMessage(string message)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        panel.SetActive(true);
        feedbackText.text = message;
        if (message == "Try again" || message == "No More Hints Available")
            messageSound.Play();
        fadeRoutine = StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // Fade In
        yield return FadeCanvas(0, 1, fadeDuration);

        yield return new WaitForSeconds(displayTime);

        // Fade Out
        yield return FadeCanvas(1, 0, fadeDuration);

        panel.SetActive(false);
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
