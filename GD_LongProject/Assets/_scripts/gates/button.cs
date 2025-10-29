using System;
using UnityEngine;

public class button : MonoBehaviour
{
    public bool isPressed;
    public bool toolButton;
    private bool _countDown = false;
    
    private float _countDownTimer = 0;
    [SerializeField] private float countDownTime = 0.5f;
    

    private void Update()
    {
        if (!_countDown) return;
        
        _countDownTimer += Time.deltaTime;
        if (_countDownTimer >= countDownTime)
        {
            isPressed = true;
            _countDown = false;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (toolButton)
        {
            if (other.CompareTag("Tool"))
            {
                Pressed();
            }
        }
        else
        {
            if (other.CompareTag("Player") || other.CompareTag("Blue"))
            {
                Pressed();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (toolButton)
        {
            if (other.CompareTag("Tool"))
            {
                Released();
            }
        }
        else
        {
            if (other.CompareTag("Player") || other.CompareTag("Blue"))
            {
                Released();
            }
        }
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
