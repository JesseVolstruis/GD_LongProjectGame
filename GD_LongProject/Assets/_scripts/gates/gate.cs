using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class gate : MonoBehaviour
{
    private static readonly int Open = Animator.StringToHash("Open");
    [SerializeField] private bool constantPressure = false;
    [SerializeField] private List<button> buttons;
    
    private Animator _animator;
    
    private bool _open;
    private BoxCollider _gateCollider;
    
    [SerializeField] private GameObject barsGameObject;
    void Start()
    {
        _gateCollider = GetComponent<BoxCollider>();
        _animator = GetComponentInChildren<Animator>();
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
}
