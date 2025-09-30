using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //INPUT ACTIONS
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _switchAction;
    private InputAction _interactAction;
    private InputAction _switchColourAction;
    //CALLBACK HANDLERS
    private System.Action<InputAction.CallbackContext> _jumpHandler;
    private System.Action<InputAction.CallbackContext> _switchHandler;
    private System.Action<InputAction.CallbackContext> _interactHandler;
    private System.Action<InputAction.CallbackContext> _switchColourHandler;
    
    
    public PlayerValues playerValues;
    
    [Header("Movement Values")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float turnSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    
    [Header("Camera")]
    public Transform cameraTransform;
    
     public Transform lanternHoldPosition;
    public Transform torcHoldPosition;
     public bool faceMoveDirection = true;
    
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _grounded;
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        AssignMovementValues();
        _moveAction   = _playerInput.actions["Move"];
        _jumpAction   = _playerInput.actions["Jump"];
        _switchAction = _playerInput.actions["Switch"];
        _interactAction = _playerInput.actions["Interact"];
        _switchColourAction = _playerInput.actions["SwitchColour"];
    }
    private void OnEnable()
    {
        _jumpHandler = ctx => Jump();
        _switchHandler = ctx => LightSwitch();
        _interactHandler = ctx => Interact();
        _switchColourHandler = ctx => ColourSwitch();
        
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
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * turnSpeed);
        }

        // Gravity
        if (_grounded && _velocity.y < 0)
            _velocity.y = -2f;
        else
            _velocity.y += gravityValue * Time.deltaTime;

        // Apply movement + gravity
        _controller.Move((move * moveSpeed + _velocity) * Time.deltaTime);
    }

    private void AssignMovementValues()
    {
        moveSpeed = playerValues.moveSpeed;
        turnSpeed = playerValues.turnSpeed;
        jumpHeight = playerValues.jumpHeight;
        gravityValue = playerValues.gravityValue;
        
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
        if (other.TryGetComponent(out LightSource _))
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

            Transform holdHere = null;
            if (_inPickUpRange.TryGetComponent(out LightSource lightSource))
            {
                lightProperties.ProjectionType lightType = lightSource.projectionType;
                if (lightType == lightProperties.ProjectionType.Lantern)
                {
                    holdHere = lanternHoldPosition;
                }
                else if (lightType == lightProperties.ProjectionType.Torch)
                {
                    holdHere = torcHoldPosition;
                }
            }
            PickUp(holdHere,_inPickUpRange.transform);
            _holdingLight = true;
            
        }
    }
    private void PickUp(Transform holdHere, Transform holdThis)
    {
        //holdThis.GetComponent<LightSource>().SetThisPlayer(transform);
		holdThis.GetComponent<Rigidbody>().isKinematic = true;
        holdThis.GetComponent<Rigidbody>().useGravity = false;
        holdThis.position = holdHere.position;
        holdThis.rotation = holdHere.rotation;
        holdThis.SetParent(holdHere);
        
        holdThis.GetComponent<SphereCollider>().enabled = false;
        holdThis.GetComponentInChildren<BoxCollider>().enabled = false;
        
        _lightSource = _inPickUpRange.GetComponent<LightSource>();
        if (_lightSource.lightOn)
        {
            faceMoveDirection = false;
        }
    }
    private void PutDown()
    {
        Transform heldLight = _lightSource.transform;
        //heldLight.GetComponent<LightSource>().ResetThisPlayer();
        heldLight.SetParent(null);
        heldLight.position = lanternHoldPosition.position;
        heldLight.rotation = lanternHoldPosition.rotation;
        heldLight.GetComponent<Rigidbody>().isKinematic = false;
        heldLight.GetComponent<SphereCollider>().enabled = true;
        heldLight.GetComponentInChildren<BoxCollider>().enabled = true;
        heldLight.GetComponent<Rigidbody>().useGravity = true;
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
        if (_lightSource != null)
        {
            _lightSource.SwitchOnOff();
            Strafe();
        }
    }

    private void ColourSwitch()
    { 
        if(_lightSource != null)  _lightSource.GreenBlueSwitch();
    }

}
