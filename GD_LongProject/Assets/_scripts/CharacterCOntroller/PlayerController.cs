using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerValues playerValues;              // ScriptableObject with base values
    public Transform cameraTransform;              // Camera to calculate relative movement
    public Transform lanternHoldPosition;          // Where lanterns are held
    public Transform torcHoldPosition;             // Where torches are held

    [Header("Movement Values")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float turnSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    public bool faceMoveDirection = true;          // Determines if player rotates to match move direction

    // --- Input Actions ---
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _switchAction;
    private InputAction _interactAction;
    private InputAction _switchColourAction;

    // Cached callback delegates (avoid garbage allocation every frame)
    private Action<InputAction.CallbackContext> _jumpHandler;
    private Action<InputAction.CallbackContext> _switchHandler;
    private Action<InputAction.CallbackContext> _interactHandler;
    private Action<InputAction.CallbackContext> _switchColourHandler;

    // --- Movement state ---
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _grounded;
    
    [SerializeField] private float coyoteTime = 0.2f;
    private float _coyoteTimeCounter;

    // --- Interaction state ---
    private GameObject _inPickUpRange;
    private LightSource _lightSource;
    private bool _holdingLight;   

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();

        AssignMovementValues();

        // Bind input actions
        _moveAction        = _playerInput.actions["Move"];
        _jumpAction        = _playerInput.actions["Jump"];
        _switchAction      = _playerInput.actions["Switch"];
        _interactAction    = _playerInput.actions["Interact"];
        _switchColourAction= _playerInput.actions["SwitchColour"];
    }

    private void OnEnable()
    {
        // Assign delegates once and add them
        _jumpHandler        = ctx => Jump();
        _switchHandler      = ctx => LightSwitch();
        _interactHandler    = ctx => Interact();
        _switchColourHandler= ctx => ColourSwitch();

        _jumpAction.performed        += _jumpHandler;
        _switchAction.performed      += _switchHandler;
        _interactAction.performed    += _interactHandler;
        _switchColourAction.performed+= _switchColourHandler;
    }

    private void OnDisable()
    {
        // Remove delegates to avoid memory leaks
        _jumpAction.performed        -= _jumpHandler;
        _switchAction.performed      -= _switchHandler;
        _interactAction.performed    -= _interactHandler;
        _switchColourAction.performed-= _switchColourHandler;
    }

    private void Update()
    {
        // --- Movement ---
        _moveInput = _moveAction.ReadValue<Vector2>();
        _grounded = _controller.isGrounded;

        // Camera-relative movement
        Vector3 forward = cameraTransform.forward; forward.y = 0; forward.Normalize();
        Vector3 right   = cameraTransform.right;   right.y = 0;   right.Normalize();
        Vector3 move    = forward * _moveInput.y + right * _moveInput.x;

        // Smooth rotation toward move direction
        if (faceMoveDirection && move.sqrMagnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * turnSpeed);
        }

        // Gravity
        if (_grounded && _velocity.y < 0) _velocity.y = -2f;
        else _velocity.y += gravityValue * Time.deltaTime;

        // Apply movement + gravity
        _controller.Move((move * moveSpeed + _velocity) * Time.deltaTime);
        
        //CoyoteTime
        if (_grounded)
        {
            _coyoteTimeCounter = coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    // --- Value setup ---
    private void AssignMovementValues()
    {
        moveSpeed    = playerValues.moveSpeed;
        turnSpeed    = playerValues.turnSpeed;
        jumpHeight   = playerValues.jumpHeight;
        gravityValue = playerValues.gravityValue;
        coyoteTime    = playerValues.coyoteTime;
    }

    private void Jump()
    {
        if (_coyoteTimeCounter > 0)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        _coyoteTimeCounter = 0f;
    }

    // --- Interaction logic ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LightSource _))
            _inPickUpRange = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        _inPickUpRange = null;
    }

    private void Interact()
    {
        if (_holdingLight) PutDown();
        else if (_inPickUpRange != null)
        {
            if (!_inPickUpRange.TryGetComponent(out LightSource lightSource)) return;

            // Decide which hold position to use
            Transform holdHere = lightSource.projectionType switch
            {
                lightProperties.ProjectionType.Lantern => lanternHoldPosition,
                lightProperties.ProjectionType.Torch   => torcHoldPosition,
                _ => throw new ArgumentOutOfRangeException()
            };

            PickUp(holdHere, _inPickUpRange.transform);
        }
    }

    private void PickUp(Transform holdHere, Transform holdThis)
    {
        _holdingLight = true;

        // Disable physics + attach to player
        var rb = holdThis.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        holdThis.position = holdHere.position;
        holdThis.rotation = holdHere.rotation;
        holdThis.SetParent(holdHere);

        // Disable colliders (avoids blocking player)
        holdThis.GetComponent<SphereCollider>().enabled = false;
        holdThis.GetComponentInChildren<BoxCollider>().enabled = false;

        _lightSource = holdThis.GetComponent<LightSource>();

        // If the light is already on, stop rotating to move direction
        if (_lightSource.lightOn) faceMoveDirection = false;
    }

    private void PutDown()
    {
        Transform heldLight = _lightSource.transform;

        heldLight.SetParent(null);
        heldLight.position = lanternHoldPosition.position;   // ⚠️ always uses lantern pos (bug if torch?)
        heldLight.rotation = lanternHoldPosition.rotation;

        var rb = heldLight.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        heldLight.GetComponent<SphereCollider>().enabled = true;
        heldLight.GetComponentInChildren<BoxCollider>().enabled = true;

        _holdingLight = false;
        _lightSource = null;
        faceMoveDirection = true;
    }

    private void Strafe()
    {
        faceMoveDirection = !faceMoveDirection;
    }

    private void LightSwitch()
    {
        if (_lightSource == null) return;
        _lightSource.SwitchOnOff();
        Strafe();
    }

    private void ColourSwitch()
    { 
        if (_lightSource != null)  
            _lightSource.GreenBlueSwitch();
    }
}
