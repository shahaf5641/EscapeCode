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
    [SerializeField] private AudioSource successSound;
    [SerializeField] private AudioClip chestFallSound;
    [SerializeField] private AudioSource audioSource;

    void OnMouseDown()
    {
        PlayerController.IsMovementLocked = true;
        if (isSolved || codeWindow == null) return;

        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        string problemTitle = "Secret Code";

        string problemDescription = 
@"Buried within the pages of an old journal lies a secret word—
but it’s scattered, hidden among the letters like a riddle.
Can you reconstruct the message by picking the right characters?";

        string problemCode =
@"string = ""t9a52d6am""
secret_code = string[0] + string[2] + string[5] + string[7] + string[8]
secret_code = ""________""";

        codeWindow.Open(
            problemTitle,
            problemDescription,
            problemCode,
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
        successSound.PlayOneShot(successSound.clip);
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Book solved!");
        audioSource.PlayOneShot(chestFallSound);
        if (chestObject != null)
        {
            chestObject.SetActive(true);
            Animator chestAnim = chestObject.GetComponent<Animator>();
        }
        gameObject.GetComponent<BoxCollider>().enabled = false;

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
