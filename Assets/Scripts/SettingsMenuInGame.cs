using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsMenuInGame : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject settingsButton;
    public TMP_Text buttonText;

    void Start()
    {
        buttonText.color = Color.white;
        
    }
    public void ToggleSettings()
    {
        bool isNowActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isNowActive);
        PlayerController.IsMovementLocked = isNowActive;
        BigRobotController.IsMovementLocked = isNowActive;
    }

    public void OpenSettings()
    {
        bool isNowActive = !settingsButton.activeSelf;
        settingsButton.SetActive(isNowActive);
    }

    public void ReturnToMainMenu()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
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
    }
}
