using UnityEngine;

public class FloatingKeyPickup : MonoBehaviour
{
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1f;
    public float rotationSpeed = 50f;
    public GameObject chestToHide;
    public GameObject buttonToReveal;
    public Collider doorCollider;


    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Floating
        float y = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, y, 0);

        // Rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnMouseDown()
    {
        FindObjectOfType<FeedbackUIManager>().ShowMessage("Key picked up!");
        Invoke(nameof(HideChest), 3f);
        gameObject.SetActive(false);
        KeyInventory.HasKey = true;
        buttonToReveal.SetActive(true);
        doorCollider.enabled = true;
    }

    private void HideChest()
    {
        chestToHide.SetActive(false);
    }
}
