using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitManager : MonoBehaviour
{
    public GameObject exitWindow;         // Regular Canvas for the popup panel containing YES and NO buttons
    public Button exitButton;             // The Exit button
    public Button yesButton;              // YES button
    public Button noButton;               // NO button

    public CanvasGroup otherUIGroup;      // Group for the rest of the UI (to disable interaction)
    public float fadeDuration = 0.25f;    // Duration for fade in/out
    public float fadeTargetAlpha = 0.35f;  // Target alpha for partial fade

    void Start()
    {
        exitWindow.SetActive(false);
        otherUIGroup.alpha = 1f;

        exitButton.onClick.AddListener(OpenExitPopup);
        yesButton.onClick.AddListener(ExitGame);
        noButton.onClick.AddListener(CloseExitPopup);
    }

    void OpenExitPopup()
    {
        exitWindow.SetActive(true);
        StartCoroutine(FadeCanvasGroup(otherUIGroup, 1f, fadeTargetAlpha));
        otherUIGroup.interactable = false;
        otherUIGroup.blocksRaycasts = false;
    }

    void CloseExitPopup()
    {
        exitWindow.SetActive(false);
        StartCoroutine(FadeCanvasGroup(otherUIGroup, fadeTargetAlpha, 1f));
        otherUIGroup.interactable = true;
        otherUIGroup.blocksRaycasts = true;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end)
    {
        float elapsed = 0f;
        while (elapsed <= fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
