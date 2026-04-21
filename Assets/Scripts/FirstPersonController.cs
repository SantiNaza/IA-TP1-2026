using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    public CharacterController characterController;
    public Transform cameraHolder;

    [Header("Movement")]
    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;

    private float xRotation;
    private float verticalVelocity;

    private void Start()
    {
        // Hide and lock the cursor for an FPS-style camera.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Try to auto-assign CharacterController if it was not set in the Inspector.
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        LookAround();
        HandleMovement();
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Horizontal mouse movement rotates the player body.
        transform.Rotate(Vector3.up * mouseX);

        // Vertical mouse movement rotates only the camera holder.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (cameraHolder != null)
        {
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        if (characterController == null)
        {
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Convert local input (WASD) into world-space movement direction.
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Keep a slight downward force when grounded so the controller stays snapped to ground.
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Jump using kinematic equation: v = sqrt(2 * jumpHeight * -gravity).
        if (characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity over time.
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }
}
