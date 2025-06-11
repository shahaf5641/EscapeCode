using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SettingsMenuInGame : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject settingsButton;

    private List<Collider> collidersTemporarilyDisabled = new();

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
        collidersTemporarilyDisabled.Clear();

        Collider[] allColliders = GameObject.FindObjectsOfType<Collider>();
        foreach (var col in allColliders)
        {
            if (col.enabled && col.gameObject.CompareTag("WorldClickable"))
            {
                col.enabled = false;
                collidersTemporarilyDisabled.Add(col);
            }
        }
    }

    private void EnableAllWorldColliders()
    {
        foreach (var col in collidersTemporarilyDisabled)
        {
            if (col != null)
                col.enabled = true;
        }

        collidersTemporarilyDisabled.Clear();
    }
}
