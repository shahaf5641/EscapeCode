using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("FirstRoomScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked");
    }

    public void OpenTutorial()
    {
        Debug.Log("Tutorial button clicked");
    }
}
