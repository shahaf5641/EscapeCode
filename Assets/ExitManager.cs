using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class ExitManager : MonoBehaviour
{
    public GameObject exitWindow;
    public Button exitButton;
    public Button yesButton;
    public Button noButton;
    public CanvasGroup otherUIGroup;

    public float fadeDuration = 0.25f;
    public float fadeTargetAlpha = 0.35f;

    void Start()
    {
        if (exitWindow == null)
            exitWindow = FindInactiveGameObjectByName("Exit_Canvas");

        if (exitButton == null)
            exitButton = FindInactiveButtonByName("Exit_Button");

        if (yesButton == null)
            yesButton = FindInactiveButtonByName("Yes_Button");

        if (noButton == null)
            noButton = FindInactiveButtonByName("No_Button");

        if (otherUIGroup == null)
        {
            GameObject found = FindInactiveGameObjectByName("Main_Buttons");
            if (found != null) otherUIGroup = found.GetComponent<CanvasGroup>();
        }

        if (exitWindow == null || exitButton == null || yesButton == null || noButton == null || otherUIGroup == null)
        {
            Debug.LogError("❌ One or more UI references are missing.");
            return;
        }

        exitWindow.SetActive(false);
        otherUIGroup.alpha = 1f;

        exitButton.onClick.AddListener(OpenExitPopup);
        yesButton.onClick.AddListener(ExitGame);
        noButton.onClick.AddListener(CloseExitPopup);
    }

    GameObject FindInactiveGameObjectByName(string name)
    {
        return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == name);
    }

    Button FindInactiveButtonByName(string name)
    {
        return Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(b => b.name == name);
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
