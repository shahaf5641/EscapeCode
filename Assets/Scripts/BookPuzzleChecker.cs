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
        
        string = ""t9a52d6am""
        secret_code = string[0] + string[2] + string[5] + string[7] + string[8]

        What is the correct line of code to assign the secret_code variable
        with the final word?
        Use a line like:
        secret_code = ""______""

        Need a hint? Say: “Give me a clue” or “Help me solve this”";

        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

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
        return userCode.Contains("secret_code = \"tadam\"") || userCode.Contains("secret_code=\"tadam\"");
    }

    private void OnBookSolved()
    {
        isSolved = true;
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Book solved!");
        if (chestObject != null)
        {
            chestObject.SetActive(true);
            Animator chestAnim = chestObject.GetComponent<Animator>();
        }
            StartCoroutine(DeactivateAfterDelay(3f));
        }
    
        private IEnumerator DeactivateAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
    }

}
