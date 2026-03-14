using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThirdRoomEndingOverlay : MonoBehaviour
{
    private const string TargetSceneName = "ThirdRoomScene";
    private const string MainMenuSceneName = "MainMenuScene";
    private const string OverlayObjectName = "ThirdRoomEndingCanvas";
    private const float ReturnDelaySeconds = 5f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryCreateOverlay(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryCreateOverlay(scene);
    }

    private static void TryCreateOverlay(Scene scene)
    {
        if (scene.name != TargetSceneName)
            return;

        if (GameObject.Find(OverlayObjectName) != null)
            return;

        GameObject canvasObject = new GameObject(OverlayObjectName);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject panelObject = new GameObject("Panel");
        panelObject.transform.SetParent(canvasObject.transform, false);

        Image panel = panelObject.AddComponent<Image>();
        panel.color = new Color(0f, 0f, 0f, 0.55f);

        RectTransform panelRect = panel.rectTransform;
        panelRect.anchorMin = new Vector2(0.2f, 0.35f);
        panelRect.anchorMax = new Vector2(0.8f, 0.65f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject textObject = new GameObject("Message");
        textObject.transform.SetParent(panelObject.transform, false);

        Text message = textObject.AddComponent<Text>();
        message.text = "To Be Continued";
        message.alignment = TextAnchor.MiddleCenter;
        message.color = Color.white;
        message.fontSize = 64;
        message.fontStyle = FontStyle.Bold;
        message.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        message.resizeTextForBestFit = true;
        message.resizeTextMinSize = 28;
        message.resizeTextMaxSize = 72;

        RectTransform textRect = message.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(40f, 40f);
        textRect.offsetMax = new Vector2(-40f, -40f);

        canvasObject.AddComponent<ReturnToMainMenuAfterDelay>();
    }

    private sealed class ReturnToMainMenuAfterDelay : MonoBehaviour
    {
        private float elapsedTime;

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= ReturnDelaySeconds)
            {
                SceneManager.LoadScene(MainMenuSceneName);
            }
        }
    }
}
