using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SettingsCanvasManager : MonoBehaviour
{
    public static SettingsCanvasManager Instance;

    [SerializeField] private GameObject globalButtonMenu;
    [SerializeField] private GameObject globalPanelSettings;
    [SerializeField] private GameObject calibrationCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GazeCalibration scriptToEnable;


    private Dictionary<Collider, bool> colliderStates = new();
    private bool IsInRoomScene =>
        SceneManager.GetActiveScene().name == "FirstRoomScene" ||
        SceneManager.GetActiveScene().name == "SecondRoomScene";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        if (!IsInRoomScene) return;

        globalButtonMenu?.SetActive(false);
        globalPanelSettings?.SetActive(false);

        DisableAllWorldColliders();
    }

    void OnDisable()
    {
        if (!IsInRoomScene) return;

        globalButtonMenu?.SetActive(true);
        globalPanelSettings?.SetActive(true);

        RestoreOriginalColliderStates();
    }

    void DisableAllWorldColliders()
    {
        colliderStates.Clear();

        foreach (var col in GameObject.FindObjectsOfType<Collider>())
        {
            if (col.gameObject.CompareTag("WorldClickable"))
            {
                colliderStates[col] = col.enabled;  // save current state
                col.enabled = false;                // disable
            }
        }
    }

    void RestoreOriginalColliderStates()
    {
        foreach (var kvp in colliderStates)
        {
            if (kvp.Key != null)
                kvp.Key.enabled = kvp.Value;  // restore previous state
        }
    }

    public void CalibrationButtonHideSettingsCanvas()
    {
        if (calibrationCanvas != null)
            calibrationCanvas.SetActive(!calibrationCanvas.activeSelf);

        if (settingsCanvas != null)
            settingsCanvas.SetActive(!settingsCanvas.activeSelf);

        if (scriptToEnable != null)
            scriptToEnable.enabled = !scriptToEnable.enabled;

        // Only toggle these if the scene is not MainMenuScene
        if (SceneManager.GetActiveScene().name != "MainMenuScene")
        {
            if (globalButtonMenu != null)
                globalButtonMenu.SetActive(!globalButtonMenu.activeSelf);

            if (globalPanelSettings != null)
                globalPanelSettings.SetActive(!globalPanelSettings.activeSelf);
        }
    }
}
