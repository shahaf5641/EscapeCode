using UnityEngine;

public class BookInteraction : MonoBehaviour
{
    private bool isSolved = false;

    public CodeWindowManager codeWindow;
    public GameObject chestObject;

    void OnMouseDown()
    {

    if (isSolved || codeWindow == null) return;
        string problemText =
        @"string = ""T9a52D6am""

        secret_code = string[0] + string[1] + string[5]

        # Hint 1: The code does not contain numbers
        # Hint 2: The code contains 5 letters

        print(""The secret code is:"", secret_code)";

        string defaultCode ="";
        codeWindow.Open(
            problemText,
            defaultCode, // 👈 empty string for no default code
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

    if (chestObject != null)
    {
        chestObject.SetActive(true);

        Animator chestAnim = chestObject.GetComponent<Animator>();
        if (chestAnim != null)
        {
            chestAnim.Play("Idle");
        }
    }
}

}
