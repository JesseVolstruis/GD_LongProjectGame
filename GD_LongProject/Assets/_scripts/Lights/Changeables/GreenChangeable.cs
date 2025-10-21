using System;
using UnityEngine;

public class GreenChangeable : MonoBehaviour, IChangeable
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
    public void UnChange(bool immediately)
    {
        leaf.SetActive(false);
    }
}
