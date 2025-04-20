using UnityEngine;
using UnityEngine.UI;

public class SimpleScript : MonoBehaviour
{
    public GameObject Levels;

    public void whenButtonClicked()
    {
        if (Levels.activeInHierachy == true)
            Levels.SetActive(false);
        else Levels.SetActive(true);
    }
}