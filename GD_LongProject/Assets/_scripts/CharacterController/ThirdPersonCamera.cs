using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform target;         // The player to follow
    public PlayerInput playerInput;  // PlayerInput component for this player

    [Header("Settings")]
    public float distance = 5f;
    public float height = 2f;
    public float rotationSpeed = 5f;
    public float smoothTime = 0.1f;

    private Vector2 lookInput;
    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponentInParent<PlayerInput>();
    }

    void LateUpdate()
    {
        if (target == null || playerInput == null) return;

        // Read look input
        var lookAction = playerInput.actions["Look"]; // should be Vector2
        lookInput = lookAction.ReadValue<Vector2>();

        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // limits vertical rotation

        // Desired camera position
        Vector3 desiredPosition = target.position
                                  - Quaternion.Euler(pitch, yaw, 0) * Vector3.forward * distance
                                  + Vector3.up * height;

        // Smoothly move camera
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

        // Always look at the player
        transform.LookAt(target.position + Vector3.up * 1.5f); // aim a bit above player center
    }
}