using System.Runtime.InteropServices;
using UnityEngine;

public interface IChangable
{
    void Change(lightProperties.ColorOfLight colorOfLight, Transform player);
}
