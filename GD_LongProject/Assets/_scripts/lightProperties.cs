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
    
    public float spreadOfLight;

}
