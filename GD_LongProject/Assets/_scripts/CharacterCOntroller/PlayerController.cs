using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// This is basically bar for bar the documentation's implementation
    /// </summary>
    
    //Input
    public InputActionAsset  actionAsset;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    
    private Vector2 _moveInput;

    private CharacterController _controller;
    
    private Vector3 _velocity;
    
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityValue = -9.81f;
    private bool _grounded;
    void OnEnable()
    {
        actionAsset.Enable();
    }

    void OnDisable()
    {
        actionAsset.Disable();
    }

    void Start()
    {
        _controller =  gameObject.GetComponent<CharacterController>();
        _moveAction = actionAsset.FindAction("Move");
        _jumpAction = actionAsset.FindAction("Jump");
    }

    void Update()
    {
        //Ground Logic
        if (_grounded && _velocity.y < 0)
        {
            _velocity.y = 0f;
        }
        
        //Read Input
        _moveInput =  _moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);
        move = Vector3.ClampMagnitude(move, 1f);

        //Jump
        if (_jumpAction.triggered && _controller.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
        
        //Apply Gravity
        _velocity.y += gravityValue * Time.deltaTime;
        
        //Actually Move
        Vector3 finalMove = (move * playerSpeed) + (_velocity.y * Vector3.up);
        _controller.Move(finalMove * Time.deltaTime);
    }
}
