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
        string problemDescription = 
@"The chest won’t budge — it’s sealed by logic, not a lock.
Somewhere inside, a real key is waiting... but to reach it, you'll need to write the correct line of code.
Can you crack the command that opens the chest?";

        string problemCode =
@"chest_locked = True
has_key_code = False
_________________
if has_key_code:
    chest_locked = False";

        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        codeWindow.Open(
            "Unlock The Chest",
            problemDescription,
            problemCode,
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
