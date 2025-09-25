using System;
using UnityEngine;

public class greenChangable : MonoBehaviour, IChangable
{
    public GameObject leaf;
    public void Change(lightProperties.ColorOfLight colorOfLight,  Transform none)
    {
        //Check for overlapping so mixing can be applied
        if (colorOfLight == lightProperties.ColorOfLight.GreenLight)
        {
            leaf.SetActive(true);
        }
    }
    public void UnChange()
    {
        leaf.SetActive(false);
    }
}
