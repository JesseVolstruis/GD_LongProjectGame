using System;
using UnityEngine;

public class greenChangable : MonoBehaviour, IChangable
{
    
    public void Change(lightProperties.ColorOfLight colorOfLight)
    {
        //Check for overlapping so mixing can be applied
        if (colorOfLight == lightProperties.ColorOfLight.GreenLight)
        {
            Debug.Log("Yippy");
        }
        else
        {
            return;
        }
    }
}
