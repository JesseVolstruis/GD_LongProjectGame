using System;
using UnityEngine;

public class ParentPlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Player") || other.CompareTag("Tool"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Tool"))
        {
            other.transform.SetParent(null);
        }
    }
}
