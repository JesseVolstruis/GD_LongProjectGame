using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class gate : MonoBehaviour
{
    [SerializeField] private bool constantPressure = false;
    [SerializeField] private List<button> buttons;
    
    private bool _open;
    private BoxCollider _gateCollider;

    /// <summary>
    /// TEMPORARY GOTTA ADD ANIMATIONS
    /// </summary>
    //[SerializeField] private MeshRenderer barsMeshRenderer;
    [SerializeField] private GameObject barsGameObject;
    void Start()
    {
        _gateCollider = GetComponent<BoxCollider>();
       //barsMeshRenderer =  GetComponent<MeshRenderer>();
       
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
        //barsMeshRenderer.enabled = !_open;
        barsGameObject.SetActive(!_open);
    }

    private void StayOpen()
    {
        if (buttons.All(b => b.isPressed))
        {
            _open = true;
        }

        if (!_open) return;
        _gateCollider.enabled = false;
        barsGameObject.SetActive(false);
    }
}
