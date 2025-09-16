using UnityEngine;
using UnityEngine.Serialization;

public class overlapChecker : MonoBehaviour
{
    public GameObject lightSourceA;
    public GameObject lightSourceB;
    
    public Vector3 _centreA;
    public Vector3 _centreB;
    
    public float _radiusA;
    public float _radiusB;
    
    public float _overlapDistance;

    public GameObject cube;
    public Material overlapping;
    public Material notOverlapping;
    
    public lightProperties.ColorOfLight lightColorA;
    public lightProperties.ColorOfLight lightColorB;
    
    public float distance;

    void Update()
    {
        _centreA = lightSourceA.GetComponent<torch>().lightCentre;
        _centreB = lightSourceB.GetComponent<torch>().lightCentre;
        _radiusA = lightSourceA.GetComponent<torch>().radius;
        _radiusB = lightSourceB.GetComponent<torch>().radius;
        lightColorA = lightSourceA.GetComponent<torch>().colorOfLight;
        lightColorB = lightSourceB.GetComponent<torch>().colorOfLight;
        _overlapDistance = _radiusA + _radiusB;
        distance = Mathf.Abs(Vector3.Distance(_centreA, _centreB));
        
        if(CheckOverlap())
        {
            Debug.Log(CheckColorCombos(lightColorA, lightColorB)); 
            cube.GetComponent<Renderer>().material = overlapping;
        }
        else
        {
            cube.GetComponent<Renderer>().material = notOverlapping;
        }
    }
    
    private bool CheckOverlap()
    {
        return Mathf.Abs(Vector3.Distance(_centreA, _centreB))  <= _overlapDistance;
    }

    private lightProperties.ColorOfLight CheckColorCombos(lightProperties.ColorOfLight colorA, lightProperties.ColorOfLight colorB)
    {
        //Yellow
        if ((colorA == lightProperties.ColorOfLight.RedLight || colorB == lightProperties.ColorOfLight.RedLight) 
            && (colorA == lightProperties.ColorOfLight.GreenLight || colorB == lightProperties.ColorOfLight.GreenLight))
        {
            return lightProperties.ColorOfLight.YellowLight;
        }
        //Cyan
        if ((colorA == lightProperties.ColorOfLight.GreenLight || colorB == lightProperties.ColorOfLight.GreenLight) 
            && (colorA == lightProperties.ColorOfLight.BlueLight || colorB == lightProperties.ColorOfLight.BlueLight))
        {
            return lightProperties.ColorOfLight.CyanLight;
        }
        //Magenta
        if ((colorA == lightProperties.ColorOfLight.RedLight || colorB == lightProperties.ColorOfLight.RedLight)
            && (colorA == lightProperties.ColorOfLight.BlueLight || colorB == lightProperties.ColorOfLight.BlueLight))
        {
            return lightProperties.ColorOfLight.MagentaLight;
        }
        
        return lightProperties.ColorOfLight.WhiteLight;
    } 
}
