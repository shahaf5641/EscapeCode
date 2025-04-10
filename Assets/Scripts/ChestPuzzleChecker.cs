using UnityEngine;
using TMPro;

public class ChestPuzzleChecker : MonoBehaviour
{
    public TMP_InputField codeInput;
    public TextMeshProUGUI resultOutput;
    public ChestInteraction chestInteraction;

    private bool chestOpened = false;

    public void RunCode()
    {
        string userCode = codeInput.text;

        bool correct = userCode.Contains("box_locked = False") || userCode.Contains("box_locked=False");

        if (correct && !chestOpened)
        {
            resultOutput.text = "Success! The chest opens!";
            chestInteraction.OpenChestAndRevealKey();
            chestOpened = true;
            gameObject.SetActive(false); // hide CodeWindow
        }
        else
        {
            resultOutput.text = "The chest is still locked...";
        }
    }
}
