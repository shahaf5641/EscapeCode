using UnityEngine;
using UnityEngine.UI;

public class AudioPopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public AudioSource popupAudio;
    public Button closeButton;

    private static bool hasShownPopupThisSession = false;

    void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);

        if (!hasShownPopupThisSession)
        {
            popupPanel.SetActive(true);
            popupAudio.Play();
            hasShownPopupThisSession = true;
        }
        else
        {
            popupPanel.SetActive(false);
        }
    }

    public void ClosePopup()
    {
        popupAudio.Stop();
        popupPanel.SetActive(false);
    }
}
