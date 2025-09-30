using UnityEngine;

public class blueChangeable : MonoBehaviour, IChangable
{
    private Rigidbody _rigidbody;
    private FixedJoint _joint;
    private bool _isChanged;
    //private blueBlockHelper _blueBlockHelper;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //_blueBlockHelper = GetComponentInChildren<blueBlockHelper>();
    }

    public void Change(lightProperties.ColorOfLight colorOfLight, Transform player)
    {
        // Already attached? Don’t reattach
        if (_isChanged) return;

        if (colorOfLight == lightProperties.ColorOfLight.BlueLight)
        {
            _isChanged = true;
            Debug.Log("Blue light");
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = false;
            // Look for GrabAnchor child on the player
            var grabAnchor = player.GetComponentInChildren<GrabAnchorFollower>();
            if (grabAnchor == null)
            {
                Debug.LogWarning("GrabAnchor not found on player!");
                return;
            }

            var grabRb = grabAnchor.GetComponent<Rigidbody>();
            if (grabRb == null)
            {
                Debug.LogWarning("GrabAnchor has no Rigidbody!");
                return;
            }

            // Add a joint ON THIS block
            _joint = gameObject.AddComponent<FixedJoint>();
            _joint.connectedBody = grabRb;
            _joint.breakForce = Mathf.Infinity;
            _joint.breakTorque = Mathf.Infinity;

            
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
        //_blueBlockHelper.DoGroundCheck();
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground") && !_isChanged)
        {
            Debug.Log("groundHit");
            // Optional: freeze after touching ground again
            _rigidbody.isKinematic = true;
        }
    }
}