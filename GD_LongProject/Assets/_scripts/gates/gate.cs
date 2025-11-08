using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class gate : MonoBehaviour
{
    private static readonly int Open = Animator.StringToHash("Open");
    [SerializeField] private bool constantPressure = false;
    [SerializeField] private List<button> buttons;

    [SerializeField] private AudioClip openSound;
    private Animator _animator;

    private bool _open;
    private bool _previousOpen; // <— track previous state
    private BoxCollider _gateCollider;
    
    void Start()
    {
        _gateCollider = GetComponent<BoxCollider>();
        _animator = GetComponentInChildren<Animator>();
        _previousOpen = _open; // initialize
    }

    void Update()
    {
        if (constantPressure)
        {
            ConstantPressure();
        }
        else
        {
            StayOpen();
        }

        // ✅ detect change in open state
        if (_open != _previousOpen)
        {
            if (_open)
                PlaySound(); // only play when opening, not closing (optional)
            _previousOpen = _open;
        }
    }

    private void ConstantPressure()
    {
        _open = buttons.All(b => b.isPressed);
        _gateCollider.enabled = !_open;
        _animator.SetBool(Open, _open);
    }

    private void StayOpen()
    {
        if (buttons.All(b => b.isPressed))
        {
            _open = true;
        }

        if (!_open) return;

        _gateCollider.enabled = false;
        _animator.SetBool(Open, _open);
    }

    private void PlaySound()
    {
        if(SoundManager.Instance != null) SoundManager.Instance.PlaySoundFX(openSound, transform, 1f);
    }
}