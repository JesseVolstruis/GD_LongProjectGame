using System;
using UnityEngine;

public class blueChangeable : MonoBehaviour, IChangable
{
    Rigidbody _rigidbody;

    private FixedJoint _joint;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void Change(lightProperties.ColorOfLight colorOfLight, Transform player)
    {
        if (colorOfLight == lightProperties.ColorOfLight.BlueLight)
        {
            _rigidbody.isKinematic = true;
            
            _joint = player.gameObject.AddComponent<FixedJoint>();
            _joint.connectedBody = _rigidbody;
        }
    }
    public void UnChange()
    {
        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.name == "Ground")
        {
            _rigidbody.isKinematic = true;
        }
    }
}
