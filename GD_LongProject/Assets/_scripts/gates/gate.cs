using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class gate : MonoBehaviour
{
    [SerializeField] private List<button> buttons;
    private bool _open;
    void Update()
    {
        _open = buttons.All(b => b.isPressed);

        if (_open)
        {
            Debug.Log("open");
        }
        else
        {
            //
        }
    }
}
