using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsCanvasManager : MonoBehaviour
{
    public static SettingsCanvasManager Instance;

    [SerializeField] private GameObject globalButtonMenu;
    [SerializeField] private GameObject globalPanelSettings;

    private Collider[] cachedColliders;
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

        if (globalButtonMenu != null)
            globalButtonMenu.SetActive(false);

        if (globalPanelSettings != null)
            globalPanelSettings.SetActive(false);

        DisableAllWorldColliders();
    }

    void OnDisable()
    {
        if (!IsInRoomScene) return;

        if (globalButtonMenu != null)
            globalButtonMenu.SetActive(true);

        if (globalPanelSettings != null)
            globalPanelSettings.SetActive(true);

        EnableAllWorldColliders();
    }

    void DisableAllWorldColliders()
    {
        cachedColliders = GameObject.FindObjectsOfType<Collider>();
        foreach (var col in cachedColliders)
        {
            if (col.enabled && col.gameObject.CompareTag("WorldClickable"))
                col.enabled = false;
        }
    }

    void EnableAllWorldColliders()
    {
        if (cachedColliders == null) return;

        foreach (var col in cachedColliders)
        {
            if (col != null && col.gameObject.CompareTag("WorldClickable"))
                col.enabled = true;
        }
    }
}
