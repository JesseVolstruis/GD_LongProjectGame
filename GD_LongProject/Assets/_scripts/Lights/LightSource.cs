using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class LightSource : MonoBehaviour
{ 
    [Header("Initial Value Data")]
    public lightProperties lightProperties;
    
    [Header("Universal Light Properties")]
    public lightProperties.ProjectionType projectionType;
    public lightProperties.ColorOfLight colorOfLight = lightProperties.ColorOfLight.WhiteLight;
    [SerializeField] private float intensityOfLight;
    [SerializeField] private bool lightOn;
    
    [Header("Lantern Specific Values")]
    public float radialRangeOfLantern;
    
    [Header("Torch Specific Values")]
    [SerializeField] private float spreadOfTorchLight = 12f; // 12f seems to correspond to 25
    [SerializeField] private float forwardRangeOfTorch = 12f;
    
    private Light _light;
    private Transform _thisPlayer;
    
    [Header("Models (ART TEAM)")]
    public GameObject lanternVisuals;
    public GameObject torchVisuals;

    // track changeable
    private List<IChangable> _changeablesPrevious = new List<IChangable>(); 
    private List<IChangable> _changeablesCurrent = new List<IChangable>();  
    private List<IChangable> _changeablesExited = new List<IChangable>();   
    
    public IChangable CurrentChangeable;
    private IChangable _previousChangeable;
    
    private void Awake()
    {
        _colourChangers = new List<Action<Light>>()
        {
            MakeRed,
            MakeGreen,
            MakeBlue,
            MakeCyan,
            MakeYellow,
            MakeMagenta
        };
    }
    void Start()
    {
        _thisPlayer = transform.root;
        
        _light = GetComponent<Light>();
        
        AssignLightProperties();
    }
    private void LateUpdate()
    {
        //LANTERN
        if (projectionType == lightProperties.ProjectionType.Lantern)
        {
            
            _changeablesPrevious = _changeablesCurrent;

            _changeablesCurrent = LanternLook(transform.position, radialRangeOfLantern);
            foreach (var changeable in _changeablesCurrent)
            {
                changeable.Change(colorOfLight,_thisPlayer);
            }
           
            _changeablesExited = _changeablesPrevious.Except(_changeablesCurrent).ToList();

            foreach (var changeable in _changeablesExited)
            {
                changeable.UnChange();
            }
            
            //HELPERS
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * radialRangeOfLantern, 
                Color.yellow); 
        } 
        //TORCH
        else if (projectionType == lightProperties.ProjectionType.Torch)
        {
            _previousChangeable = CurrentChangeable;
            
            CurrentChangeable = TorchLook();
            
            if (CurrentChangeable != null)
            {
                CurrentChangeable.Change(colorOfLight, _thisPlayer);
            }
            if (CurrentChangeable == null && _previousChangeable != null)
            {
                _previousChangeable.UnChange();
            } 
            //HELPERS
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * forwardRangeOfTorch, 
                Color.white);
            Quaternion upwardRotation = Quaternion.AngleAxis(-spreadOfTorchLight, transform.right);
            Quaternion downwardRotation = Quaternion.AngleAxis(spreadOfTorchLight, transform.right);
            Vector3 upDirection = upwardRotation * transform.forward;
            Vector3 downDirection = downwardRotation * transform.forward;
            Debug.DrawRay(transform.position, upDirection * forwardRangeOfTorch, Color.magenta);
            Debug.DrawRay(transform.position, downDirection * forwardRangeOfTorch, Color.magenta);
        }
    }
    [HideInInspector] public Vector3 torchHitPoint; 
    [HideInInspector] public float radiusOfTorch;
    private float GetRadius()
    {
        return Mathf.Abs(Vector3.Distance(transform.position, torchHitPoint) * Mathf.Tan(spreadOfTorchLight * Mathf.Deg2Rad)) ; 
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
            var abs = Mathf.Abs(Vector3.Distance(transform.position, torchHitPoint));
            if (abs <= forwardRangeOfTorch)
            {
                if (centreHit.collider.gameObject.TryGetComponent(out IChangable changeable) && lightOn)
                {
                    return changeable;
                }
            }
            radiusOfTorch = GetRadius(); 
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * centreHit.distance, 
                Color.red); 
        }
        return null;
    }
    
    //Initial SetUp
    private void AssignLightProperties()
    {
        colorOfLight = lightProperties.currentColorOfLight;
        projectionType = lightProperties.currentProjectionType;
        intensityOfLight =  lightProperties.intensityOfLight;
        radialRangeOfLantern = lightProperties.radialRangeOfLantern;
        spreadOfTorchLight = lightProperties.innerSpotAngle/2f;
        forwardRangeOfTorch = lightProperties.forwardRangeOfTorch;

        switch (projectionType)
        {
            case lightProperties.ProjectionType.Lantern: MakeLantern(); break;
            case lightProperties.ProjectionType.Torch: MakeTorch(); break;
        }
        
        lightOn = lightProperties.lightOn;

        _light.enabled = lightOn;
        
        switch (colorOfLight)
        {
            case lightProperties.ColorOfLight.WhiteLight:   _light.color = Color.white;   _colourIndex = 6; break;
            case lightProperties.ColorOfLight.CyanLight:    _light.color = Color.cyan;    _colourIndex = 3; break;
            case lightProperties.ColorOfLight.YellowLight:  _light.color = Color.yellow;  _colourIndex = 4; break;
            case lightProperties.ColorOfLight.MagentaLight: _light.color = Color.magenta; _colourIndex = 5; break;
            case lightProperties.ColorOfLight.RedLight:     _light.color = Color.red;     _colourIndex = 0; break;
            case lightProperties.ColorOfLight.GreenLight:   _light.color = Color.green;   _colourIndex = 1; break;
            case lightProperties.ColorOfLight.BlueLight:    _light.color = Color.blue;    _colourIndex = 2; break;
        }

    }
    //ON/OFF Switch
    public void SwitchOnOff()
    {
        _light.enabled = !_light.enabled;
        lightOn = !lightOn;
    } 

    // Colour Changers
    private void MakeRed(Light l)     { colorOfLight = lightProperties.ColorOfLight.RedLight; l.color = Color.red; }
    private void MakeGreen(Light l)   { colorOfLight = lightProperties.ColorOfLight.GreenLight; l.color = Color.green; }
    private void MakeBlue(Light l)    { colorOfLight = lightProperties.ColorOfLight.BlueLight; l.color = Color.blue; }
    private void MakeCyan(Light l)    { colorOfLight = lightProperties.ColorOfLight.CyanLight; l.color = Color.cyan; }
    private void MakeYellow(Light l)  { colorOfLight = lightProperties.ColorOfLight.YellowLight; l.color = Color.yellow; }
    private void MakeMagenta(Light l) { colorOfLight = lightProperties.ColorOfLight.MagentaLight; l.color = Color.magenta; }
    
    private int _colourIndex;

    private List<Action<Light>> _colourChangers = new List<Action<Light>>();
    public void ChangeColour(int howMany)
    {
        _colourIndex++;
        _colourChangers[_colourIndex](_light);
        
        if (_colourIndex == howMany)
        {
            _colourIndex = 0;
        }
    }
    //Tool Changers
    private void TorchProjectionProperties(Light l)
    {
        l.type = LightType.Spot;
        l.intensity = intensityOfLight;
        l.range = forwardRangeOfTorch;
        l.innerSpotAngle = 28;          //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        l.spotAngle = 30;
        
    }

    private void MakeTorch()
    {
        lanternVisuals.SetActive(false);
        torchVisuals.SetActive(true);
        projectionType = lightProperties.ProjectionType.Torch;
        TorchProjectionProperties(_light);
    }

    private void LanternProjectionProperties(Light l)
    {
        l.type = LightType.Point;
        l.intensity = intensityOfLight;
        l.range = radialRangeOfLantern;
    }

    private void MakeLantern()
    {
        lanternVisuals.SetActive(true);
        torchVisuals.SetActive(false);
        projectionType = lightProperties.ProjectionType.Lantern;
        LanternProjectionProperties(_light);
    }
}
