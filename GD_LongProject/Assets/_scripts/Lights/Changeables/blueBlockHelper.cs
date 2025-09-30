using System;
using UnityEngine;

public class blueBlockHelper : MonoBehaviour
{
    private Transform _blueBlock;
    private bool _doGroundCheck = false;

    void Start()
    {
        _blueBlock = transform.parent;
    }
    public void DoGroundCheck()
    {
        _doGroundCheck = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && _doGroundCheck)
        {
            _blueBlock.GetComponent<Rigidbody>().isKinematic = true;
            _doGroundCheck = false;
        }
    }
    
}
