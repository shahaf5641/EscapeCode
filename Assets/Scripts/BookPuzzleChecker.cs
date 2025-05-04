 using UnityEngine;
using System.Collections;

public class BookInteraction : MonoBehaviour
{
    private bool isSolved = false;
    public CodeWindowManager codeWindow;
    public GameObject chestObject;
    public string puzzleType = "book";
    public Unity.Cinemachine.CinemachineCamera chestCam;
    public Unity.Cinemachine.CinemachineCamera playerCam;



    void OnMouseDown()
    {
        PlayerController.IsMovementLocked = true;
        if (isSolved || codeWindow == null) return;
        string problemText =
        @"The Secret Code
        
        string = ""t9a52d6am""
        secret_code = string[0] + string[2] + string[5] + string[7] + string[8]

        What is the correct line of code to assign the secret_code variable
        with the final word?
        Use a line like:
        secret_code = ""______""";

        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        string defaultCode ="";
        codeWindow.Open(
            problemText,
            defaultCode,
            CheckBookCode,
            OnBookSolved
        );

    }

    private bool CheckBookCode(string userCode)
    {
        return userCode.Contains("secret_code = \"tadam\"") || userCode.Contains("secret_code=\"tadam\"");
    }

    private void OnBookSolved()
    {
        isSolved = true;

        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Book solved!");

        if (chestObject != null)
        {
            chestObject.SetActive(true);
            Animator chestAnim = chestObject.GetComponent<Animator>();
            // You can optionally play the chest opening animation here
        }

        chestCam.Priority = 20;     // Focus on chest
        playerCam.Priority = 10;    // Keep player cam lower

        StartCoroutine(WaitAndReturnCamera());
        StartCoroutine(DeactivateAfterDelay(3f));
    }

    private IEnumerator WaitAndReturnCamera()
    {
        yield return new WaitForSeconds(3f);

        chestCam.Priority = 5;      // Lower chest cam priority
        playerCam.Priority = 15;    // Reactivate player cam
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
