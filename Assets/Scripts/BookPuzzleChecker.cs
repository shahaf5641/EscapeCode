
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BookPuzzleChecker : MonoBehaviour
{
    public TMP_InputField codeInput;
    public TextMeshProUGUI resultOutput;
    public GameObject chestObject;

    public void RunCode()
    {
        string userCode = codeInput.text;

        bool correct = userCode.Contains("secret_code = \"TaDam\"") || userCode.Contains("secret_code=\"TaDam\"");

        if (correct)
        {
            resultOutput.text = "Correct! You found the secret code.";

            chestObject.SetActive(true); // chest now appears
            Animator animator = chestObject.GetComponent<Animator>();
            animator.Play("ChestDrop"); // starts falling

            gameObject.SetActive(false); // hide code window
        }


        else
        {
            resultOutput.text = "Try again — hint: 5 letters, no numbers.";
        }
    }
}
