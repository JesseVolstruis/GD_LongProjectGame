using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightSource : MonoBehaviour
{ 
    public float radiusOfLantern;
    public lightProperties lightProperties;
    public lightProperties.ColorOfLight colorOfLight = lightProperties.ColorOfLight.WhiteLight;
    public lightProperties.ProjectionType projectionType;
    private float _upwardTiltDegrees = 12f;

    private Light _light;
    private Transform _thisPlayer;

    public float numberOfRays = 8f;
    private float _tiltInRads;
    //private float _degreesBetweenRays;

    public GameObject lantern;
    public GameObject torch;

    // track changables
    private List<IChangable> _changeablesPrevious = new List<IChangable>(); 
    private List<IChangable> _changeablesCurrent = new List<IChangable>();  
    private List<IChangable> _changeablesExited = new List<IChangable>();   
    
    public IChangable CurrentChangable;
    private IChangable _previousChangable;
    
    [SerializeField] private bool lightOn;
    void Start()
    {
        _thisPlayer = transform.root;

        TorchSetUp();
        _light = GetComponent<Light>();
        AssignLightProperties();
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //LANTERN
        if (projectionType == lightProperties.ProjectionType.Lantern)
        {
            _changeablesPrevious = _changeablesCurrent;

            _changeablesCurrent = LanternLook(transform.position, radiusOfLantern);
            foreach (var changeable in _changeablesCurrent)
            {
                changeable.Change(colorOfLight,_thisPlayer);
            }
           
            _changeablesExited = _changeablesPrevious.Except(_changeablesCurrent).ToList();

            foreach (var changable in _changeablesExited)
            {
                changable.UnChange();
            }
        } 
        //TORCH
        else if (projectionType == lightProperties.ProjectionType.Torch)
        {
            _previousChangable = CurrentChangable;
            
            CurrentChangable = TorchLook();
            
            if (CurrentChangable != null)
            {
                CurrentChangable.Change(colorOfLight, _thisPlayer);
            }
            if (CurrentChangable == null && _previousChangable != null)
            {
                _previousChangable.UnChange();
            } 
        }
    }
    public Vector3 torchHitPoint; 
    public float radiusOfTorch; 
    public float xDistance;

    private float GetRadius()
    {
        return Mathf.Abs(Vector3.Distance(transform.position, torchHitPoint) * Mathf.Tan(_tiltInRads)) ; 
    }
    
    //LANTERN INTERACTION LOGIC
    private List<IChangable> LanternLook(Vector3 center, float lanternRadius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, 0.7f * lanternRadius);
        var changeables = new List<IChangable>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IChangable changeable) && lightOn)
            {
                changeables.Add(changeable);
            }
        }
        return changeables;
    }
    
    //TORCH INTERACTION LOGIC
    private IChangable TorchLook()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var centreHit)) 
        { 
            torchHitPoint = centreHit.point; 
            if (centreHit.collider.gameObject.TryGetComponent(out IChangable changable) && lightOn)
            {
                return changable;
            } 
            xDistance = Mathf.Abs(Vector3.Distance(transform.position, torchHitPoint)); 
            radiusOfTorch = GetRadius(); 
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * centreHit.distance, 
                Color.yellow); 
        }
        return null;
    }

    //Initial SetUp
    private void TorchSetUp()
    {
        //_degreesBetweenRays = 360f / numberOfRays;
        _tiltInRads = _upwardTiltDegrees * Mathf.Deg2Rad;
    }
    private void AssignLightProperties()
    {
        colorOfLight = lightProperties.currentColorOfLight;
        projectionType = lightProperties.currentProjectionType;

        switch (projectionType)
        {
            case lightProperties.ProjectionType.Lantern: MakeLantern(); break;
            case lightProperties.ProjectionType.Torch: MakeTorch(); break;
        }

        radiusOfLantern = lightProperties.rangeOfLight;
        _upwardTiltDegrees = lightProperties.spreadOfLight;
        _light.intensity = lightProperties.intensityOfLight;
        lightOn = lightProperties.lightOn;
        
        switch (colorOfLight)
        {
            case lightProperties.ColorOfLight.WhiteLight:   _light.color = Color.white;   break;
            case lightProperties.ColorOfLight.CyanLight:    _light.color = Color.cyan;    break;
            case lightProperties.ColorOfLight.YellowLight:  _light.color = Color.yellow;  break;
            case lightProperties.ColorOfLight.MagentaLight: _light.color = Color.magenta; break;
            case lightProperties.ColorOfLight.RedLight:     _light.color = Color.red;     break;
            case lightProperties.ColorOfLight.GreenLight:   _light.color = Color.green;   break;
            case lightProperties.ColorOfLight.BlueLight:    _light.color = Color.blue;    break;
        }
    }
    //ON/OFF Switch
    public void SwitchOnOff()
    {
        _light.enabled = !_light.enabled;
        lightOn = !lightOn;
    } 

    // Colour Changers
    public void MakeRed(Light l)     { colorOfLight = lightProperties.ColorOfLight.RedLight; l.color = Color.red; }
    public void MakeGreen(Light l)   { colorOfLight = lightProperties.ColorOfLight.GreenLight; l.color = Color.green; }
    public void MakeBlue(Light l)    { colorOfLight = lightProperties.ColorOfLight.BlueLight; l.color = Color.blue; }
    public void MakeCyan(Light l)    { colorOfLight = lightProperties.ColorOfLight.CyanLight; l.color = Color.cyan; }
    public void MakeYellow(Light l)  { colorOfLight = lightProperties.ColorOfLight.YellowLight; l.color = Color.yellow; }
    public void MakeMagenta(Light l) { colorOfLight = lightProperties.ColorOfLight.MagentaLight; l.color = Color.magenta; }
    
    //Tool Changers
    private void TorchProjectionProperties(Light l)
    {
        l.type = LightType.Spot;
        l.innerSpotAngle = 28;
        l.spotAngle = 30;
    }
    
    public void MakeTorch()
    {
        lantern.SetActive(false);
        torch.SetActive(true);
        projectionType = lightProperties.ProjectionType.Torch;
        TorchProjectionProperties(_light);
    }

    private void LanternProjectionProperties(Light l)
    {
        l.type = LightType.Point;
    }

    public void MakeLantern()
    {
        lantern.SetActive(true);
        torch.SetActive(false);
        projectionType = lightProperties.ProjectionType.Lantern;
        LanternProjectionProperties(_light);
    }
}
