using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsMenuInGame : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject settingsButton;

    private Collider[] cachedColliders;

    public void ToggleSettings()
    {
        bool isNowActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isNowActive);
        PlayerController.IsMovementLocked = isNowActive;
        BigRobotController.IsMovementLocked = isNowActive;

        if (isNowActive)
            DisableAllWorldColliders();
        else
            EnableAllWorldColliders();
    }

    public void OpenSettings()
    {
        bool isNowActive = !settingsButton.activeSelf;
        settingsButton.SetActive(isNowActive);
    }

    public void ReturnToMainMenu()
    {
        settingsPanel.SetActive(false);
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game pressed - quitting.");
    }

    public void CloseButton()
    {
        settingsPanel.SetActive(false);
        PlayerController.IsMovementLocked = false;
        EnableAllWorldColliders();
    }

    private void DisableAllWorldColliders()
    {
        cachedColliders = GameObject.FindObjectsOfType<Collider>();
        foreach (var col in cachedColliders)
        {
            if (col.enabled && col.gameObject.CompareTag("WorldClickable"))
                col.enabled = false;
        }
    }

    private void EnableAllWorldColliders()
    {
        if (cachedColliders == null) return;

        foreach (var col in cachedColliders)
        {
            if (col != null && col.gameObject.CompareTag("WorldClickable"))
                col.enabled = true;
        }
    }
}
