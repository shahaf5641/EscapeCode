using UnityEngine;
using System.Collections;

public class PasswordSphereeInteraction : MonoBehaviour
{
    private bool isSolved = false;
    public CodeWindowManager codeWindow;
    public string puzzleType = "password";
    [SerializeField] private AudioSource successSound;
    [SerializeField] private GameObject handRailToDisable;


    void OnMouseDown()
    {
        if (isSolved || codeWindow == null) return;

        PlayerController.IsMovementLocked = true;

        string problemText =
        @"Robot Activation Code
        Complete the condition to find the even password

        passwords = [8271, 1235, 4312, 9001]
        i = 0
        password = 1

        while i < 4:
            if passwords[i] % 2 == ___:
                password = passwords[i]
            i += 1

        if password == 4312:
            activate_robot()";

        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        string defaultCode = "";

        codeWindow.Open(
            problemText,
            defaultCode,
            CheckPasswordCode,
            OnPasswordSolved
        );
    }

    private bool CheckPasswordCode(string userCode)
    {
        return userCode.Equals("0");
    }

    private void OnPasswordSolved()
    {
        isSolved = true;
                var thisCollider = GetComponent<BoxCollider>();
        if (thisCollider != null)
            thisCollider.enabled = false;
        if (successSound != null && successSound.clip != null)
            successSound.PlayOneShot(successSound.clip);
        handRailToDisable.SetActive(false);
        FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage("Robot Activated!");
        FindFirstObjectByType<RobotSphereController>()?.StartRollingToTarget();
        DeactivateAfterDelay(2f);
    }
        private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
