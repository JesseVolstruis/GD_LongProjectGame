using UnityEngine;

public class LightSource : MonoBehaviour
{ 
    //LANTERN
    // public lightProperties lightProperties;
    public float _radiusOfLight;
    // private float _intensityOfLight;
    // public lightProperties.ColorOfLight colorOfLight = lightProperties.ColorOfLight.WhiteLight;
    // private Light _light;
    
    //TORCH
    public lightProperties lightProperties;
    public lightProperties.ColorOfLight colorOfLight = lightProperties.ColorOfLight.WhiteLight;
    public lightProperties.ProjectionType projectionType;
    private float _upwardTiltDegrees = 12f; // 12 seems to correspond with light size of 25
    private float _intensityOfLight;
    
    private Light _light;
    
    public float numberOfRays = 8f;
    private float _tiltInRads;
    private float _degreesBetweenRays;
     
    public Vector3 lightCentre;
    public float radius;
    public float xDistance;

    public GameObject lantern;
    public GameObject torch;
    
    private Transform _thisPlayer;
    void Start()
    {
        _thisPlayer = transform.root;
        
        TorchLogic();
        _light = gameObject.GetComponent<Light>();
        AssignLightProperties();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MakeLantern();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            MakeTorch();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MakeRed(_light);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MakeBlue(_light);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MakeGreen(_light);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MakeCyan(_light);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            MakeYellow(_light);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            MakeMagenta(_light);
        }
        
        
    }
    void FixedUpdate()
    {
        if (projectionType == lightProperties.ProjectionType.Lantern)
        {
            LanternLook(transform.position, _radiusOfLight);
        }
        else if (projectionType == lightProperties.ProjectionType.Torch)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var centreHit))
            {
                lightCentre = centreHit.point;
                if (centreHit.collider.gameObject.TryGetComponent(out IChangable changable))
                {
                    changable.Change(colorOfLight, _thisPlayer);
                }

                xDistance = Mathf.Abs(Vector3.Distance(transform.position, lightCentre));
                radius = GetRadius();
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * centreHit.distance,
                    Color.yellow);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white);
            }
        }
    }
    private void LanternLook(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, 0.7f*radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.TryGetComponent(out IChangable changeable))
            {
                changeable.Change(colorOfLight, _thisPlayer);
            }
        }
    }
    private void TorchLogic()
    {
        _degreesBetweenRays = 360f / numberOfRays;;
        _tiltInRads = _upwardTiltDegrees * Mathf.Deg2Rad;
    }
    private float GetRadius()
    {
        return Mathf.Abs(Vector3.Distance(transform.position, lightCentre) * Mathf.Tan(_tiltInRads)) ;
    }
    private void AssignLightProperties()
    {
        colorOfLight = lightProperties.currentColorOfLight;
        projectionType = lightProperties.currentProjectionType;
        
        switch (projectionType)
        {
            case lightProperties.ProjectionType.Lantern:
                MakeLantern();
                break;
            case lightProperties.ProjectionType.Torch:
                MakeTorch();
                break;
        }

        _radiusOfLight = lightProperties.radiusOfLight;
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
    public void MakeRed(Light redlight)
    {
        colorOfLight = lightProperties.ColorOfLight.RedLight;
        redlight.color = Color.red;
    }
    public void MakeGreen(Light greenlight)
    {
        colorOfLight = lightProperties.ColorOfLight.GreenLight;
        greenlight.color = Color.green;
    }
    public void MakeBlue(Light bluelight)
    {
        colorOfLight = lightProperties.ColorOfLight.BlueLight;
        bluelight.color = Color.blue;
    }
    public void MakeCyan(Light cyanlight)
    {
        colorOfLight = lightProperties.ColorOfLight.CyanLight;
        cyanlight.color = Color.cyan;
    }
    public void MakeYellow(Light yellowlight)
    {
        colorOfLight = lightProperties.ColorOfLight.YellowLight;
        yellowlight.color = Color.yellow;
    }
    public void MakeMagenta(Light magentalight)
    {
        colorOfLight = lightProperties.ColorOfLight.MagentaLight;
        magentalight.color = Color.magenta;
    }

    public void SwitchOnOff()
    {
        _light.enabled = !_light.enabled;
    }
    
    private void TorchProjectionProperties(Light torchlight)
    {
        torchlight.type = LightType.Spot;
        torchlight.innerSpotAngle = 28;
        torchlight.spotAngle = 30;
    }

    public void MakeTorch()
    {
        lantern.SetActive(false);
        torch.SetActive(true);
        projectionType = lightProperties.ProjectionType.Torch;
        TorchProjectionProperties(_light);
    }
    private void LanternProjectionProperties(Light lanternlight)
    {
        lanternlight.type = LightType.Point;
    }
    public void MakeLantern()
    {
        lantern.SetActive(true);
        torch.SetActive(false);
        projectionType = lightProperties.ProjectionType.Lantern;
        LanternProjectionProperties(_light);
    }
    

}
