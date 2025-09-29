using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class gate : MonoBehaviour
{
    [SerializeField] private List<button> buttons;
    
    private bool _open;
    private BoxCollider _gateCollider;
    private MeshRenderer _meshRenderer;
    void Start()
    {
        _gateCollider = GetComponent<BoxCollider>();
       _meshRenderer =  GetComponent<MeshRenderer>();
    }
    void Update()
    {
        _open = buttons.All(b => b.isPressed);

        _gateCollider.enabled = !_open;
        _meshRenderer.enabled = !_open;
    }
}
