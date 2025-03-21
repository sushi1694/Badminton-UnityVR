using UnityEngine;

public class ContinuousMovement2 : MonoBehaviour
{
    public float moveSpeed = 3.0f; // Speed of movement
    public float rotationSpeed = 90.0f; // Speed of rotation

    void Update()
    {
        // Move forward/backward using the left joystick
        MovePlayer();

        // Rotate left/right using the right joystick
        RotatePlayer();
    }

    void MovePlayer()
    {
        // Get vertical input from left joystick (forward/backward)
        float verticalInput = Input.GetAxis("Vertical");

        // Create movement vector based on forward direction
        Vector3 movement = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
        movement.y = 0; // Prevent movement in Y-axis (no flying)

        // Apply movement
        transform.position += movement;
    }

    void RotatePlayer()
    {
        // Get horizontal input from right joystick (rotation)
        float rotationInput = Input.GetAxis("Horizontal Secondary Thumbstick");

        // Calculate rotation amount
        float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;

        // Apply rotation around Y-axis
        transform.Rotate(0, rotationAmount, 0);
    }
}
