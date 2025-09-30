using System;
using UnityEngine;

public class resetPlane : MonoBehaviour
{
    [SerializeField] private sceneManager sceneManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sceneManager.Restart();
        }
    }
}
