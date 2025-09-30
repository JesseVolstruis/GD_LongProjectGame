using UnityEngine;

public class GrabAnchorFollower : MonoBehaviour
{
    public Transform followTarget;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        // Teleport the kinematic rigidbody to match the target
        _rb.MovePosition(followTarget.position);
        _rb.MoveRotation(followTarget.rotation);
    }
}

