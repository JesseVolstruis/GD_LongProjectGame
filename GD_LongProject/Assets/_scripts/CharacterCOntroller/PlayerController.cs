using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _switchAction;
    private InputAction _interactAction;
    private InputAction _switchColourAction;

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
    private System.Action<InputAction.CallbackContext> _switchHandler;
    private System.Action<InputAction.CallbackContext> _interactHandler;
    private System.Action<InputAction.CallbackContext> _switchColourHandler;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction   = _playerInput.actions["Move"];
        _jumpAction   = _playerInput.actions["Jump"];
        _switchAction = _playerInput.actions["Switch"];
        _interactAction = _playerInput.actions["Interact"];
        _switchColourAction = _playerInput.actions["SwitchColour"];
    }
    private void OnEnable()
    {
        _jumpHandler = ctx => Jump();
        _switchHandler = ctx => Switch();
        _interactHandler = ctx => Interact();
        _switchColourHandler = ctx => ChangeColour();
        
        _jumpAction.performed += _jumpHandler;
        _switchAction.performed += _switchHandler;
        _interactAction.performed += _interactHandler;
        _switchColourAction.performed += _switchColourHandler;
    }

    private void OnDisable()
    {
        _jumpAction.performed -= _jumpHandler;
        _switchAction.performed -= _switchHandler;
        _interactAction.performed -= _interactHandler;
        _switchColourAction.performed -= _switchColourHandler;
    }

    private void Jump()
    {
        if (_controller.isGrounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
    }

    private GameObject _inPickUpRange;
    private LightSource _lightSource;
    private bool _holdingLight;    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LightSource lightSource))
        {
            _inPickUpRange =  other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        _inPickUpRange = null;
    }
    private void Interact()
    {
        if (_holdingLight)
        {
            PutDown();
        }
        else if (_inPickUpRange != null)
        {
            if (_holdingLight) return;
            PickUp(holdPosition,_inPickUpRange.transform);
            _holdingLight = true;
            _lightSource = _inPickUpRange.GetComponent<LightSource>();
        }
    }
    private void PickUp(Transform holdHere, Transform holdThis)
    {
		holdThis.GetComponent<Rigidbody>().isKinematic = true;
        holdThis.position = holdHere.position;
        holdThis.SetParent(holdHere);
    }
    private void PutDown()
    {
        Transform heldLight = _lightSource.transform;
        heldLight.SetParent(null);
        heldLight.GetComponent<Rigidbody>().isKinematic = false;
        _holdingLight = false;
        _lightSource = null;
    }
    private void Switch()
    {
        if(_lightSource != null)  _lightSource.SwitchOnOff();
    }

    private void ChangeColour()
    { 
        if(_lightSource != null)  _lightSource.ChangeColour(3);
    }
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        //_lightSource = holdPosition.GetChild(0).gameObject.GetComponent<LightSource>();
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
