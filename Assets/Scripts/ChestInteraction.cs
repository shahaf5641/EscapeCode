using UnityEngine;
using System.Collections;

public class ChestInteraction : MonoBehaviour
{
    public CodeWindowManager codeWindow;
    public Animator chestAnimator;
    public GameObject keyObject;
    public string puzzleType = "chest";
    [SerializeField] private AudioSource successSound;
    private bool chestOpened = false;

    void OnMouseDown()
    {
        if (chestOpened || codeWindow == null) return;

        string problemText =
        @"chest_locked = True
        has_key_code = False

        if has_key_code:
            chest_locked = False

        Can you figure out how to unlock the chest?
        Try writing a line of code that makes the chest unlock.";


        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        string defaultCode ="";

        codeWindow.Open(
            problemText,
            "Unlock The Chest",
            defaultCode,
            CheckChestCode,
            OnChestSolved
        );
    }

    private bool CheckChestCode(string userCode)
    {
        return userCode.Contains("has_key_code = True") || userCode.Contains("has_key_code=True");
    }

    private void OnChestSolved()
    {
        if (chestOpened) return;
        successSound.PlayOneShot(successSound.clip);
        chestOpened = true;
        gameObject.GetComponent<BoxCollider>().enabled = false;

        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Chest solved!");
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("Open");
            StartCoroutine(RevealKeyAfterDelay(1));
        }
    }
    private IEnumerator RevealKeyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }
        yield return new WaitForSeconds(1f);
    }
}
