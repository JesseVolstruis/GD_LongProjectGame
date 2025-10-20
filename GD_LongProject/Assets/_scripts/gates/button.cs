using System;
using UnityEngine;

public class button : MonoBehaviour
{
    public bool isPressed;
    private bool _countDown = false;
    
    private float _countDownTimer = 0;
    [SerializeField] private float countDownTime = 0.5f;
    private void Update()
    {
        if (!_countDown) return;
        
        _countDownTimer += Time.deltaTime;
        if (!(_countDownTimer >= countDownTime)) return;
        
        isPressed = true;
        _countDown = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Blue"))
        {
            Pressed();
        }
    }
    void OnTriggerExit(Collider other)
    {
        Released();
    }

    private void Pressed()
    {
        _countDownTimer = 0f;
        _countDown = true;
    }

    private void Released()
    {
        isPressed = false;
        _countDown = false;
        _countDownTimer = 0f;
    }
}
