using UnityEngine;

public class blueChangeable : MonoBehaviour, IChangeable
{
    private Rigidbody _rigidbody;
    private FixedJoint _joint;
    private bool _isChanged;

    [Header("Grab Settings")]
    public float grabDelay = 0.05f;
    public float releaseDelay = 0.2f;
    private float _grabTimer = 0f;
    private float _releaseTimer = 0f;

    private Transform _player;
    private Rigidbody _grabRb;
    private bool _isTryingToGrab; // track if light is still blue

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Called by the light system when the object is illuminated
    public void Change(lightProperties.ColorOfLight colorOfLight, Transform player)
    {
        if (colorOfLight == lightProperties.ColorOfLight.BlueLight)
        {
            
            _isTryingToGrab = true;
            _player = player;
        }
        else
        {
            _isTryingToGrab = false;
        }
    }

    // Called by the light system when the object exits the light
    public void UnChange()
    {
        _isTryingToGrab = false;
    }

    void Update()
    {
        if (_isTryingToGrab)
        {
            // Try to grab if not already held
            if (!_isChanged)
            {
                _grabTimer += Time.deltaTime;
                if (_grabTimer >= grabDelay)
                {
                    Grab();
                }
            }

            _releaseTimer = 0f; // reset release timer if still under blue light
        }
        else
        {
            // Try to release if currently held
            if (_isChanged)
            {
                _releaseTimer += Time.deltaTime;
                if (_releaseTimer >= releaseDelay)
                {
                    Release();
                }
            }

            _grabTimer = 0f; // reset grab timer when not under blue light
        }
    }

    private void Grab()
    {
        if (_isChanged) return;

        var grabAnchor = _player.GetComponentInChildren<GrabAnchorFollower>();
        if (grabAnchor == null) return;

        _grabRb = grabAnchor.GetComponent<Rigidbody>();
        if (_grabRb == null) return;

        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = _grabRb;
        _joint.breakForce = Mathf.Infinity;
        _joint.breakTorque = Mathf.Infinity;

        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = false;

        _isChanged = true;
        _grabTimer = 0f;
    }

    private void Release()
    {
        if (!_isChanged) return;

        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }

        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;

        _isChanged = false;
        _releaseTimer = 0f;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground") && !_isChanged)
        {
            _rigidbody.isKinematic = true;
        }
    }
}
