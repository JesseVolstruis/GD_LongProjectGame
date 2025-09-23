using System;
using UnityEngine;

public class greenChangable : MonoBehaviour, IChangable
{
    public GameObject leaf;
    private bool _visible = false;

    private void Update()
    {
        if (_visible)
        {
            leaf.SetActive(true);
        }
        else
        {
            leaf.SetActive(false);
        }
    }

    public void Change(lightProperties.ColorOfLight colorOfLight,  Transform none)
    {
        //Check for overlapping so mixing can be applied
        if (colorOfLight == lightProperties.ColorOfLight.GreenLight)
        {
            _visible = true;
        }
        else
        {
            _visible = false;
        }
    }
    
    
    
}
