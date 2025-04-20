using UnityEngine;
using UnityEngine.UI;

public class OpenLevelsMenu : MonoBehaviour
{
    public GameObject Levels;              
    public Button PlayButton;              
    public Button[] OtherButtons;          

    void Start()
    {
        Levels.SetActive(false);

        // Toggle Levels visibility when Play is clicked
        PlayButton.onClick.AddListener(() =>
        {
            Levels.SetActive(!Levels.activeSelf);
        });

        // Hide Levels when any other button is clicked
        foreach (Button btn in OtherButtons)
        {
            btn.onClick.AddListener(() =>
            {
                Levels.SetActive(false);
            });
        }
    }
}

