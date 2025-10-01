using UnityEngine;

public class blueChangeable : MonoBehaviour, IChangeable
{
    private Rigidbody _rigidbody;
    private FixedJoint _joint;
    private bool _isChanged;

    [Header("Grab Settings")]
    public float grabDelay = 0.05f;    // time in seconds before grabbing
    public float releaseDelay = 0.2f; // time in seconds before letting go
    private float _grabTimer = 0f;
    private float _releaseTimer = 0f;

    private Transform _player;
    private Rigidbody _grabRb;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Change(lightProperties.ColorOfLight colorOfLight, Transform player)
    {
        if (colorOfLight != lightProperties.ColorOfLight.BlueLight)
        {
            // Not blue → start release timer
            if (_isChanged)
            {
                _releaseTimer += Time.deltaTime;
                if (_releaseTimer >= releaseDelay) UnChange();
            }
            return;
        }

        // Blue light → start grab timer if not already grabbed
        if (!_isChanged)
        {
            _player = player;
            var grabAnchor = player.GetComponentInChildren<GrabAnchorFollower>();
            if (grabAnchor == null) return;

            _grabRb = grabAnchor.GetComponent<Rigidbody>();
            if (_grabRb == null) return;

            _grabTimer += Time.deltaTime;
            if (_grabTimer >= grabDelay)
            {
                _isChanged = true;
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = false;

                _joint = gameObject.AddComponent<FixedJoint>();
                _joint.connectedBody = _grabRb;
                _joint.breakForce = Mathf.Infinity;
                _joint.breakTorque = Mathf.Infinity;

                _grabTimer = 0f;
                _releaseTimer = 0f;
            }
        }
        else
        {
            // reset release timer if already held
            _releaseTimer = 0f;
        }
    }

    public void UnChange()
    {
        if (!_isChanged) return;

        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }

        _isChanged = false;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _grabTimer = 0f;
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
