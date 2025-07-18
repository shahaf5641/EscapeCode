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
        GetComponent<GlowEffect>()?.MarkInteracted();
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
            OnBookSolved,
            this.gameObject
        );
    }

    private bool CheckBookCode(string userCode)
    {
        // Use CodeWindowManager's Python validator with problem ID "p0"
        return codeWindow.RunPythonValidator("p0", userCode);
    }

    private void OnBookSolved()
    {
        isSolved = true;
        successSound.PlayOneShot(successSound.clip);
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Book solved!");
        audioSource.PlayOneShot(chestFallSound);
        chestObject.GetComponent<GlowEffect>().StartGlow();          // start Chest glow


        if (chestObject != null)
        {
            chestObject.SetActive(true);
            Animator chestAnim = chestObject.GetComponent<Animator>();
        }

        gameObject.GetComponent<BoxCollider>().enabled = false;

        chestCam.Priority = 20;     // Focus on chest
        playerCam.Priority = 10;    // Keep player cam lower
        StartCoroutine(DisableColliderAfterDelay(0.5f));
        StartCoroutine(WaitAndReturnCamera());
        StartCoroutine(DeactivateAfterDelay(3f));
    }
    IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    private IEnumerator WaitAndReturnCamera()
    {
        yield return new WaitForSeconds(3f);
        chestCam.Priority = 5;
        playerCam.Priority = 15;
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
