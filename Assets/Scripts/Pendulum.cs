using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public float swingAngle = 45.0f;  // Maximum swing angle in degrees
    public float swingSpeed = 2.0f;   // Swing speed in degrees per second

    private Quaternion initialRotation;

    void Start()
    {
        // Save the initial rotation of the pendulum
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Calculate the rotation angle based on time
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;

        // Apply the rotation to the pendulum
        transform.rotation = initialRotation * Quaternion.Euler(0, 0, angle);
    }
}