using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    // --- Input Actions ---
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _switchAction;
    private InputAction _interactAction;
    private InputAction _switchColourAction;

    // Cached callback delegates (avoid rubbish allocation every frame)
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

    // --- Animations ---
    private static readonly int IsWalkingAnimBool = Animator.StringToHash("IsWalking");
    private static readonly int JumpAnimTrigger = Animator.StringToHash("Jump");
    private static readonly int HasItemAnimBool = Animator.StringToHash("HasItem");
    
    [SerializeField] private Animator animator;
    
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            MakePlayerBlue();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            MakePlayerGreen();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            MakePlayerGray();
        }
        
        // --- Movement ---
        _moveInput = _moveAction.ReadValue<Vector2>();
        _grounded = _controller.isGrounded;

        // Camera-relative movement
        Vector3 forward = cameraTransform.forward; forward.y = 0; forward.Normalize();
        Vector3 right   = cameraTransform.right;   right.y = 0;   right.Normalize();
        Vector3 move    = forward * _moveInput.y + right * _moveInput.x;

        bool isMoving = move.sqrMagnitude > 0.01f;
        animator.SetBool(IsWalkingAnimBool, isMoving);
        
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
        coyoteTime   = playerValues.coyoteTime;
    }

    private void Jump()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySoundFX(jumpSound,transform,0f);
        }
        else
        {
            Debug.Log("No Sound Played for jump");
        }

        if (_coyoteTimeCounter > 0)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            animator.SetTrigger(JumpAnimTrigger);
        }
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
         animator.SetBool(HasItemAnimBool, true);
         var colorOfHeldLight = holdThis.GetComponent<LightSource>().colorOfLight;
         switch (colorOfHeldLight)
         {
             case lightProperties.ColorOfLight.BlueLight:
                 MakePlayerBlue();
                 break;
             case lightProperties.ColorOfLight.GreenLight:
                 MakePlayerGreen();
                 break;
         }
         
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

    public void PutDown()
    {
        if(!_holdingLight) return;
        if(!_grounded) return;
        
        Transform heldLight = _lightSource.transform;

        heldLight.SetParent(null);
        SceneManager.MoveGameObjectToScene(heldLight.gameObject, SceneManager.GetActiveScene());
        heldLight.position = lanternHoldPosition.position;  
        heldLight.rotation = lanternHoldPosition.rotation;

        var rb = heldLight.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        heldLight.GetComponent<SphereCollider>().enabled = true;
        heldLight.GetComponentInChildren<BoxCollider>().enabled = true;

        _holdingLight = false;
        MakePlayerGray();
        animator.SetBool(HasItemAnimBool, false);
        _lightSource = null;
        faceMoveDirection = true;
    }

    public void ResetPlayer()
    {
        if (_holdingLight)
        {
            Destroy(_lightSource.gameObject);
            _holdingLight = false;
            MakePlayerGray();
            animator.SetBool(HasItemAnimBool, false);
            _lightSource = null;
            faceMoveDirection = true;
             
        }
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
        var colorOfHeldLight = _lightSource.GetComponent<LightSource>().colorOfLight;
        SwitchPlayerColour(colorOfHeldLight);
    }
    

    [SerializeField] private Material greenFeetNeck;
    [SerializeField] private Material greenTorsoSleevesPants;
    [SerializeField] private Material greenSkin;
    [SerializeField] private Material greenBase;
    [SerializeField] private Material greenBangs;
    [SerializeField] private Material greenBuns;
    
    [SerializeField] private Material blueFeetNeck;
    [SerializeField] private Material blueTorsoSleevesPants;
    [SerializeField] private Material blueSkin;
    [SerializeField] private Material blueBase;
    [SerializeField] private Material blueBangs;
    [SerializeField] private Material blueBuns;
    
    [SerializeField] private Material grayFeetNeck;
    [SerializeField] private Material grayTorsoSleevesPants;
    [SerializeField] private Material graySkin;
    [SerializeField] private Material grayBase;
    [SerializeField] private Material grayBangs;
    [SerializeField] private Material grayBuns;
    
    [SerializeField] private GameObject feet;
    [SerializeField] private GameObject neck;
    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject sleeves;
    [SerializeField] private GameObject pants;
    [SerializeField] private GameObject skin;
    [SerializeField] private GameObject @base;
    [SerializeField] private GameObject bangs;

    [SerializeField] private GameObject pl1baseHair;
    [SerializeField] private GameObject pl1bangsHair;
        
    [SerializeField] private GameObject buns;

    private void MakePlayerBlue()
    {
        feet.GetComponent<SkinnedMeshRenderer>().material = blueFeetNeck;
        neck.GetComponent<SkinnedMeshRenderer>().material = blueFeetNeck;
        torso.GetComponent<SkinnedMeshRenderer>().material = blueTorsoSleevesPants;
        sleeves.GetComponent<SkinnedMeshRenderer>().material = blueTorsoSleevesPants;
        pants.GetComponent<SkinnedMeshRenderer>().material = blueTorsoSleevesPants;
        skin.GetComponent<SkinnedMeshRenderer>().material = blueSkin;
        
        if(pl1baseHair) pl1baseHair.GetComponent<SkinnedMeshRenderer>().material = blueBase;
        if(pl1bangsHair) pl1bangsHair.GetComponent<SkinnedMeshRenderer>().material = blueBangs;
        if(buns) buns.GetComponent<SkinnedMeshRenderer>().material = blueBuns;

        if (@base)
        {
            Material[] baseHair = @base.GetComponent<SkinnedMeshRenderer>().materials;
            baseHair[1] = blueBase;
            baseHair[0] = blueBangs;
            @base.GetComponent<SkinnedMeshRenderer>().materials = baseHair;
        }

        if (bangs)
        {
            Material[] bangHair = bangs.GetComponent<SkinnedMeshRenderer>().materials;
            bangHair[0] = blueBase;
            bangHair[1] = blueBangs;
            bangs.GetComponent<SkinnedMeshRenderer>().materials = bangHair;
        }
    }
    
    private void MakePlayerGreen()
    {
        feet.GetComponent<SkinnedMeshRenderer>().material = greenFeetNeck;
        neck.GetComponent<SkinnedMeshRenderer>().material = greenFeetNeck;
        torso.GetComponent<SkinnedMeshRenderer>().material = greenTorsoSleevesPants;
        sleeves.GetComponent<SkinnedMeshRenderer>().material = greenTorsoSleevesPants;
        pants.GetComponent<SkinnedMeshRenderer>().material = greenTorsoSleevesPants;
        skin.GetComponent<SkinnedMeshRenderer>().material = greenSkin;
        if(pl1baseHair) pl1baseHair.GetComponent<SkinnedMeshRenderer>().material = greenBase;
        if(pl1bangsHair) pl1bangsHair.GetComponent<SkinnedMeshRenderer>().material = greenBangs;
        if(buns) buns.GetComponent<SkinnedMeshRenderer>().material = greenBuns;
        
        Material[] baseHair = @base.GetComponent<SkinnedMeshRenderer>().materials;
        baseHair[0] = greenBase;
        baseHair[1] = greenBangs;
        @base.GetComponent<SkinnedMeshRenderer>().materials = baseHair;
        Material[] bangHair = bangs.GetComponent<SkinnedMeshRenderer>().materials;
        bangHair[1] = greenBase;
        bangHair[0] = greenBangs;
        bangs.GetComponent<SkinnedMeshRenderer>().materials = bangHair;
        
    }

    private void MakePlayerGray()
    {
        feet.GetComponent<SkinnedMeshRenderer>().material = grayFeetNeck;
        neck.GetComponent<SkinnedMeshRenderer>().material = grayFeetNeck;
        torso.GetComponent<SkinnedMeshRenderer>().material = grayTorsoSleevesPants;
        sleeves.GetComponent<SkinnedMeshRenderer>().material = grayTorsoSleevesPants;
        pants.GetComponent<SkinnedMeshRenderer>().material = grayTorsoSleevesPants; 
        skin.GetComponent<SkinnedMeshRenderer>().material = graySkin;
        
        if(pl1baseHair) pl1baseHair.GetComponent<SkinnedMeshRenderer>().material = grayBase;
        if(pl1bangsHair) pl1bangsHair.GetComponent<SkinnedMeshRenderer>().material = grayBangs;
        if(buns) buns.GetComponent<SkinnedMeshRenderer>().material = grayBuns;
        
        Material[] baseHair = @base.GetComponent<SkinnedMeshRenderer>().materials;
        baseHair[0] = grayBase;
        baseHair[1] = grayBangs;
        @base.GetComponent<SkinnedMeshRenderer>().materials = baseHair;
        Material[] bangHair = bangs.GetComponent<SkinnedMeshRenderer>().materials;
        bangHair[1] = grayBase;
        bangHair[0] = grayBangs;
        bangs.GetComponent<SkinnedMeshRenderer>().materials = bangHair;
       
        
    }

    private void SwitchPlayerColour(lightProperties.ColorOfLight colour)
    {
        switch (colour)
        {
            case lightProperties.ColorOfLight.BlueLight:
                MakePlayerBlue();
                break;
            case lightProperties.ColorOfLight.GreenLight:
                MakePlayerGreen();
                break;
        }
    }
    
}
