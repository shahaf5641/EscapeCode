using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoorInteraction : MonoBehaviour
{
    [SerializeField] private CodeWindowManager codeWindow;
    [SerializeField] private Animator finalDoorAnimator;
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip finishRoomSound;
    
    public string puzzleType = "finaldoor";
    private bool isSolved = false;
void OnMouseDown()
{
    if (isSolved || codeWindow == null) return;

    if (sound != null)
        sound.Play();

    string problemTitle = "Target Signal Match";

    string problemDescription =
@"You're one step away from unlocking the path forward.
The system requires a precise input — the final condition in the sequence.
Complete it correctly, and the door will open.";


    string problemCode =
@"nums = [3, 2, 4]
target = 6
for i in range(len(nums)):
    for j in range(i + 1, len(nums)):
        if __________:
            answer = [i, j]
if answer == [1, 2]:
    unlock_final_door()";

    FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

    codeWindow.Open(
        problemTitle,
        problemDescription,
        problemCode,
        CheckAnswer,
        OnSolved
    );
}


    private bool CheckAnswer(string userCode)
    {
        return codeWindow.RunPythonValidator("p6", userCode);
    }
    private void OnSolved()
    {
        isSolved = true;
        if (sound != null)
            sound.PlayOneShot(doorOpenSound);
            sound.PlayOneShot(finishRoomSound);

        if (finalDoorAnimator != null)
            finalDoorAnimator.SetTrigger("Open");

        FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage("Room Solved!");
        // 🎯 Start scene transition
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(3f); // let the animation play
        SceneManager.LoadScene("ThirdRoomScene");
    }
}
