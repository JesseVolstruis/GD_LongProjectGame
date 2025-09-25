using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;

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

    public Transform holdPosition;

    private System.Action<InputAction.CallbackContext> _jumpHandler;
    private System.Action<InputAction.CallbackContext> _attackHandler;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        // Get this player's private actions (isolated from others)
        _moveAction   = _playerInput.actions["Move"];
        _jumpAction   = _playerInput.actions["Jump"];
        _attackAction = _playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        _jumpHandler = ctx => Jump();
        _attackHandler = ctx => Attack();

        _jumpAction.performed += _jumpHandler;
        _attackAction.performed += _attackHandler;
    }

    private void OnDisable()
    {
        _jumpAction.performed -= _jumpHandler;
        _attackAction.performed -= _attackHandler;
    }

    private void Jump()
    {
        if (_controller.isGrounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
    }

    private LightSource _lightSource;
    private void Attack()
    {
        _lightSource.SwitchOnOff();
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _lightSource = holdPosition.GetChild(0).gameObject.GetComponent<LightSource>();
    }

    private void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();

        // Ground check
        _grounded = _controller.isGrounded;

        // Camera-relative movement
        Vector3 forward = cameraTransform.forward; forward.y = 0; forward.Normalize();
        Vector3 right   = cameraTransform.right;   right.y = 0;   right.Normalize();

        Vector3 move = forward * _moveInput.y + right * _moveInput.x;

        if (faceMoveDirection && move.sqrMagnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }

        // Gravity
        if (_grounded && _velocity.y < 0)
            _velocity.y = -2f;
        else
            _velocity.y += gravityValue * Time.deltaTime;

        // Apply movement + gravity
        _controller.Move((move * playerSpeed + _velocity) * Time.deltaTime);
    }
}
