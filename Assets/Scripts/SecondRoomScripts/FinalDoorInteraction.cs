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

        string problemText =
        @"Final Door: Target Signal Match
        Find two indices such that their values add up to the target.

        nums = [3, 2, 4]
        target = 6

        for i in range(len(nums)):
            for j in range(i + 1, len(nums)):
                if __________:
                    answer = [i, j]

        if answer == [1, 2]:
            unlock_final_door()";
        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        string defaultCode = "";

        codeWindow.Open(
            problemText,
            defaultCode,
            CheckAnswer,
            OnSolved
        );
    }

    private bool CheckAnswer(string userCode)
    {
        return userCode.Equals("nums[i] + nums[j] == target");
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
