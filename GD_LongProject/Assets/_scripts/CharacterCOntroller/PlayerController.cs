using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset actionAsset;
    private InputAction _moveAction;
    private InputAction _jumpAction;

    private Vector2 _moveInput;

    [Header("Movement")]
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityValue = -9.81f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _grounded;

    [Header("Camera")]
    public Transform cameraTransform;
    public bool faceMoveDirection = true;

    void OnEnable() => actionAsset.Enable();
    void OnDisable() => actionAsset.Disable();

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = actionAsset.FindAction("Move");
        _jumpAction = actionAsset.FindAction("Jump");
    }

    void Update()
    {
        // Ground check
        _grounded = _controller.isGrounded;

        // Read input
        _moveInput = _moveAction.ReadValue<Vector2>();

        // Camera-relative movement
        Vector3 forward = cameraTransform.forward; forward.y = 0; forward.Normalize();
        Vector3 right   = cameraTransform.right;   right.y = 0;   right.Normalize();

        Vector3 move = forward * _moveInput.y + right * _moveInput.x;

        if (faceMoveDirection && move.sqrMagnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }

        // Jump
        if (_jumpAction.triggered && _grounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // Gravity
        if (_grounded && _velocity.y < 0)
            _velocity.y = -2f; // small downward force to stick to ground
        else
            _velocity.y += gravityValue * Time.deltaTime;

        // Apply movement + gravity
        _controller.Move((move * playerSpeed + _velocity) * Time.deltaTime);
    }
}
