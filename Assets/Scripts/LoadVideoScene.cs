using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadVideoScene : MonoBehaviour
{
    public string sceneToLoadAfterVideo; // תקבע באינספקטור

    public void StartVideoScene()
    {
        VideoSession.nextSceneAfterVideo = sceneToLoadAfterVideo;
        SceneManager.LoadScene("VideoScene"); // או "TutorialScene" אם זה השם שלך
    }
}
