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
    public enum ProjectionType
    {
        Torch,
        Lantern
    }
    
    public ProjectionType currentProjectionType;
    public ColorOfLight currentColorOfLight;
    public bool lightOn;
    public float spreadOfLight; // for the torch
    public float rangeOfLight; // for the lantern
    public float intensityOfLight; // for both

}
