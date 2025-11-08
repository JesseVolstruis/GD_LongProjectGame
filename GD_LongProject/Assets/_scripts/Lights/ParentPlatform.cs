using System;
using UnityEngine;

public class ParentPlatform : MonoBehaviour
{
    [SerializeField] private cyanChangeable cyanChangeable;
    
    private void OnTriggerEnter(Collider other)
    {
        if (cyanChangeable.rotating) return;
        if (other.CompareTag("Player") || other.CompareTag("Tool") || other.CompareTag("Blue"))
        {
            other.transform.SetParent(transform.parent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (cyanChangeable.rotating) return;
        if (other.CompareTag("Player") || other.CompareTag("Tool")  || other.CompareTag("Blue"))
        {
            other.transform.SetParent(null);
        }
    }
}
