using System;
using UnityEngine;

public class button : MonoBehaviour
{
    public bool isPressed;
    public bool toolButton;
    private readonly Color _pressedColor = Color.grey;
    private Color _normalColor;
    [SerializeField] private GameObject changingColorObject;
    [SerializeField] private AudioClip buttonSound;
    private bool _countDown = false;
    
    private float _countDownTimer = 0;
    [SerializeField] private float countDownTime = 0.5f;

    void Start()
    {
       _normalColor =  changingColorObject.GetComponent<Renderer>().material.color;
    }
    private void Update()
    {
        if (_countDown)
        {
            _countDownTimer += Time.deltaTime;
            if (_countDownTimer >= countDownTime)
            {
                isPressed = true;
                _countDown = false;
            }
        }
        
        changingColorObject.GetComponent<Renderer>().material.color = isPressed ? _pressedColor : _normalColor;
        
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
        PlaySound();
        _countDownTimer = 0f;
        _countDown = true;
    }

    private void Released()
    {
        //PlaySound();
        isPressed = false;
        _countDown = false;
        _countDownTimer = 0f;
    }

    private void PlaySound()
    {
        if(SoundManager.Instance != null) SoundManager.Instance.PlaySoundFX(buttonSound, transform,1f);
    }
}
