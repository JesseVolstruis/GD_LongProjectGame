using System;
using UnityEngine;

public class ParentPlatform : MonoBehaviour
{
    [SerializeField] private cyanChangeable cyanChangeable;
    [SerializeField] private GameObject moveObject;
    
   
    
    private void OnTriggerEnter(Collider other)
    {
        if (cyanChangeable.rotating) return;
        if (other.CompareTag("Player") || other.CompareTag("Tool") || other.CompareTag("Blue"))
        {
            other.transform.SetParent(moveObject.transform, true);
            other.transform.position = moveObject.transform.position + (other.transform.position - moveObject.transform.position);
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
