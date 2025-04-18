using UnityEngine;
using System.Collections;

public class ChestInteraction : MonoBehaviour
{
    public CodeWindowManager codeWindow;
    public Animator chestAnimator;
    public GameObject keyObject;

    private bool chestOpened = false;

    void OnMouseDown()
    {
        if (chestOpened || codeWindow == null) return;

        string problemText =
        @"Unlock The Chest

        chest_locked = True
        has_key_code = False
        
        if has_key_code:
            chest_locked = False

        if chest_locked == False:
            chest_unlocked = True
        else:
            chest_unlocked = False

        Can you figure out how to unlock the chest?
        Try writing a line of code that makes the chest unlock.

        Need a hint? Say: “Give me a clue” or “Help me solve this”";


        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        string defaultCode ="";

        codeWindow.Open(
            problemText,
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

        chestOpened = true;
        codeWindow.Close();
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Chest solved!");

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
