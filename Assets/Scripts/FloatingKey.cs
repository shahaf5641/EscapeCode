using UnityEngine;

public class FloatingKey : MonoBehaviour
{
    public float floatAmplitude = 0.25f;    // how high it floats
    public float floatFrequency = 1f;       // speed of the float
    public float rotationSpeed = 50f;       // degrees per second

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Floating up and down
        float y = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, y, 0);

        // Rotating
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
