using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickByTag : MonoBehaviour
{
    public AudioClip clickSound;   // צליל הלחיצה
    public string buttonTag = "PlayButton";  // תג שאתה רוצה לבדוק (שנה לפי הצורך)

    private AudioSource sfxSource;  // מאגר הקול של הכפתור

    void Start()
    {
        sfxSource = GetComponent<AudioSource>();  // מקבל את ה־AudioSource מהאובייקט

        // בודק אם ה־AudioSource קיים
        if (sfxSource == null)
        {
            Debug.LogWarning("No AudioSource found on this object.");
        }

        // מוסיף את הפונקציה שתתבצע בעת לחיצה על הכפתור
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // בודק אם הכפתור תואם לתג שהגדרת
        if (gameObject.CompareTag(buttonTag))
        {
            PlayClickSound();
        }
    }

    void PlayClickSound()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);  // מנגן את הצליל שהגדרת
        }
    }
}
