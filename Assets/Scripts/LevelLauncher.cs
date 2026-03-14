using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LevelLauncher : MonoBehaviour
{
    public VideoClip[] tutorialClips;   
    public string nextScene;
    public void LaunchTutorial()
    {
        TutorialVideoConfig.clipsToPlay = tutorialClips;
        TutorialVideoConfig.nextSceneName = nextScene;
        TutorialVideoConfig.currentIndex = 0;

        SceneManager.LoadScene("TutorialScene");
    }
}
