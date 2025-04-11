using UnityEngine;

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
        @"has_key_code = False
        chest_locked = True

        if has_key_code:
            chest_locked = False

        if chest_locked == False:
            print(""The chest unlocks!"")
        else:
            print(""The chest is still locked."")";

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
        codeWindow.Close(); // Closes the window & unlocks movement

        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("Open"); // Plays animation
        }

        if (keyObject != null)
        {
            keyObject.SetActive(true); // Reveals the key
        }

    }
}
