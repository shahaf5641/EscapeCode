using UnityEngine;
using TMPro;

public class UIScript : MonoBehaviour
{
    public TextMeshProUGUI output;
    public TMP_InputField userInput;

    public void ButtonDemo()
    {
        output.text = userInput.text;
    }
}
