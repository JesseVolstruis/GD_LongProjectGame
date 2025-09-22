using UnityEngine;

public class torch : MonoBehaviour
{
    public lightProperties lightProperties;
    private float _upwardTiltDegrees = 12f; // 12 seems to correspond with light size of 25
    private float _intensityOfLight;
    public lightProperties.ColorOfLight colorOfLight = lightProperties.ColorOfLight.WhiteLight;
    private Light _light;
    
    public float numberOfRays = 8f;
    private float _tiltInRads;
    private float _degreesBetweenRays;
     
    public Vector3 lightCentre;
    public float radius;
    public float xDistance;
    
    void Start()
    {
        _light = gameObject.GetComponent<Light>();
        _degreesBetweenRays = 360f / numberOfRays;;
        _tiltInRads = _upwardTiltDegrees * Mathf.Deg2Rad;
        AssignLightProperties();
    }
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var centreHit))
        { 
            lightCentre =  centreHit.point;
            if (centreHit.collider.gameObject.TryGetComponent(out IChangable changable))
            {
                changable.Change(colorOfLight);
            }
            xDistance = Mathf.Abs(Vector3.Distance(transform.position, lightCentre));
            radius = GetRadius();
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * centreHit.distance, Color.yellow); 
        }
        else
        { 
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white); 
        }
    }
    private float GetRadius()
    {
        return Mathf.Abs(Vector3.Distance(transform.position, lightCentre) * Mathf.Tan(_tiltInRads)) ;
    }
    private void AssignLightProperties()
    {
        colorOfLight = lightProperties.currentColorOfLight;
        _upwardTiltDegrees = lightProperties.spreadOfLight;
        _light.intensity = lightProperties.intensityOfLight;
        switch (colorOfLight)
        {
            case lightProperties.ColorOfLight.WhiteLight:
                _light.color = Color.white;
                break;
            case lightProperties.ColorOfLight.CyanLight:
                _light.color = Color.cyan;
                break;
            case lightProperties.ColorOfLight.YellowLight:
                _light.color = Color.yellow;
                break;
            case lightProperties.ColorOfLight.MagentaLight:
                _light.color = Color.magenta;
                break;
            case lightProperties.ColorOfLight.RedLight:
                _light.color = Color.red;
                break;
            case lightProperties.ColorOfLight.GreenLight:
                _light.color = Color.green;
                break;
            case lightProperties.ColorOfLight.BlueLight:
                _light.color = Color.blue;
                break;
        }
    }
    public void MakeRed()
    {
        colorOfLight = lightProperties.ColorOfLight.RedLight;
        _light.color = Color.red;
    }
    public void MakeGreen()
    {
        colorOfLight = lightProperties.ColorOfLight.GreenLight;
        _light.color = Color.green;
    }
    public void MakeBlue()
    {
        colorOfLight = lightProperties.ColorOfLight.BlueLight;
        _light.color = Color.blue;
    }
    public void MakeCyan()
    {
        colorOfLight = lightProperties.ColorOfLight.CyanLight;
        _light.color = Color.cyan;
    }
    public void MakeYellow()
    {
        colorOfLight = lightProperties.ColorOfLight.YellowLight;
        _light.color = Color.yellow;
    }
    public void MakeMagenta()
    {
        colorOfLight = lightProperties.ColorOfLight.MagentaLight;
        _light.color = Color.magenta;
    }
}


