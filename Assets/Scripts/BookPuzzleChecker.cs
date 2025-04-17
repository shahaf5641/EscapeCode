using UnityEngine;
using System.Collections;

public class BookInteraction : MonoBehaviour
{
    private bool isSolved = false;
    public CodeWindowManager codeWindow;
    public GameObject chestObject;

    void OnMouseDown()
    {

    if (isSolved || codeWindow == null) return;
        string problemText =
        @"The Secret Code
        
        string = ""T9a52D6am""
        secret_code = string[0] + string[2] + string[5] + string[7] + string[8]

        What is the correct line of code to assign the secret_code variable
        with the final word?
        Use a line like:
        secret_code = ""______""

        Need a hint? Say: “Give me a clue” or “Help me solve this”";

        FindObjectOfType<ChatGPTClient>().currentPuzzle = this.gameObject;

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
        return userCode.Contains("secret_code = \"TaDam\"") || userCode.Contains("secret_code=\"TaDam\"");
    }

    private void OnBookSolved()
    {
        isSolved = true;
        codeWindow.Close();
        FindObjectOfType<FeedbackUIManager>().ShowMessage("Book solved!");
        if (chestObject != null)
        {
            chestObject.SetActive(true);
            Animator chestAnim = chestObject.GetComponent<Animator>();
            if (chestAnim != null)
            {
                chestAnim.Play("Idle");
            }
        }
            StartCoroutine(DeactivateAfterDelay(3f));
        }
    
        private IEnumerator DeactivateAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
    }

}
