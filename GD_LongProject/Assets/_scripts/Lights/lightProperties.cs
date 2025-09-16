using UnityEngine;

[CreateAssetMenu(fileName = "lightProperties", menuName = "Scriptable Objects/lightProperties")]
public class lightProperties : ScriptableObject
{
    public enum ColorOfLight
    {
        RedLight,
        GreenLight,
        BlueLight,
        CyanLight,
        MagentaLight,
        YellowLight,
        WhiteLight,
    }
    
    public ColorOfLight currentColorOfLight;
    
    public float spreadOfLight; // for the torch
    public float radiusOfLight; // for the lantern
    public float intensityOfLight; // for both

}
