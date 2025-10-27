using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class LightSource : MonoBehaviour
{ 
    [Header("Initial Value Data")]
    public lightProperties lightProperties;     // ScriptableObject with initial settings
    
    [Header("Universal Light Properties")]
    public lightProperties.ProjectionType projectionType;
    public lightProperties.ColorOfLight colorOfLight = lightProperties.ColorOfLight.WhiteLight;
    [SerializeField] private float intensityOfLight;
    public bool lightOn;
    private GameObject _lightVisualization;
    private Material _visualizationMaterial;
    
    [Header("Lantern Specific Values")]
    public float radialRangeOfLantern;
    [SerializeField] private GameObject lanternVisualization;

    [Header("Torch Specific Values")]
    [SerializeField] private float spreadOfTorchLight = 12f; 
    [SerializeField] private float forwardRangeOfTorch = 12f;
    [SerializeField] private float horizontalRangeOfTorch = 0.3f;
    [SerializeField] private GameObject torchVisualization;

    private Light _light;
    private Transform _thisLightSource;              

    [Header("Models (ART TEAM)")]
    public GameObject lanternModel;
    public GameObject torchModel;
    

    // --- Changeable tracking ---
    private List<IChangeable> _changeablesPrevious = new List<IChangeable>();
    private List<IChangeable> _changeablesCurrent  = new List<IChangeable>();  
    private List<IChangeable> _changeablesExited   = new List<IChangeable>();   
     
    private LayerMask _playerLayerMask;
    private int _ignoreRaycastLayerMask; 
    private int _mask;
    
    
    // --- Colour switching ---
    private int _colourIndex;

    [HideInInspector] public Vector3 torchHitPoint; 
    [HideInInspector] public float radiusOfTorch;

    // --- For Overlap Checking ---
    public List<(IChangeable changeable, lightProperties.ColorOfLight, Transform transform)> OverlapData
        = new List<(IChangeable, lightProperties.ColorOfLight, Transform)>();
    private void Start()
    {
        _playerLayerMask = LayerMask.GetMask("Player");
        _ignoreRaycastLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        _mask = ~(_playerLayerMask | _ignoreRaycastLayerMask);
        _thisLightSource = transform.root;
        _light = GetComponent<Light>();
        //_torchVisualsCylinder = torchVisualization.transform.GetChild(0).gameObject;
        AssignLightProperties();
    }

private void LateUpdate()
{
    if (_changeablesCurrent != null)
    {
        _changeablesPrevious = _changeablesCurrent;
    }
    switch (projectionType)
    {
        // --- Lantern Mode ---
        case lightProperties.ProjectionType.Lantern:
            _changeablesCurrent  = LanternLook(transform.position, radialRangeOfLantern);
            break;
        // --- Torch Mode ---
        case lightProperties.ProjectionType.Torch:
            _changeablesCurrent   = TorchLook(transform.position, forwardRangeOfTorch);
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }

    if (_changeablesCurrent != null)
    {
        foreach (var changeable in _changeablesCurrent)
            changeable.Change(colorOfLight, _thisLightSource);
    }

    if (_changeablesCurrent != null) _changeablesExited = _changeablesPrevious.Except(_changeablesCurrent).ToList();
    foreach (var changeable in _changeablesExited)
        changeable.UnChange(false);
}

    // --- Torch helpers ---
    private List<IChangeable> TorchLook(Vector3 origin, float forwardRange)    
    {
        if(!lightOn) return null;
        var hitColliders =  Physics.SphereCastAll(origin, horizontalRangeOfTorch, transform.forward,
            forwardRange, _mask);
        var changeables = new List<IChangeable>();
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.collider.TryGetComponent(out IChangeable changeable))
                changeables.Add(changeable);
        }
        return changeables;
    }

    // --- Lantern helpers ---
    private List<IChangeable> LanternLook(Vector3 center, float lanternRadius)
    {
        if(!lightOn) return null;
        Collider[] hitColliders = Physics.OverlapSphere(center, 0.8f *lanternRadius); 
        var changeables = new List<IChangeable>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IChangeable changeable))
                changeables.Add(changeable);
        }
        return changeables;
    }

    // --- Initial setup ---
    private void AssignLightProperties()
    {
        // Copy values from ScriptableObject
        colorOfLight        = lightProperties.currentColorOfLight;
        projectionType      = lightProperties.currentProjectionType;
        intensityOfLight    = lightProperties.intensityOfLight;
        radialRangeOfLantern= lightProperties.radialRangeOfLantern;
        spreadOfTorchLight  = lightProperties.innerSpotAngle / 2f;
        forwardRangeOfTorch = lightProperties.forwardRangeOfTorch;
        horizontalRangeOfTorch = lightProperties.horizontalRangeOfTorch;

        // Mode-specific setup
        switch (projectionType)
        {
            case lightProperties.ProjectionType.Lantern: MakeLantern(); break;
            case lightProperties.ProjectionType.Torch:   MakeTorch();   break;
            default: throw new ArgumentOutOfRangeException();
        }

        // Set light enabled/disabled
        lightOn = lightProperties.lightOn;
        _light.enabled = lightOn;

        // Set initial colour
        switch (colorOfLight)
        {
            case lightProperties.ColorOfLight.WhiteLight:   _light.color = Color.white;   _colourIndex = 6; break;
            case lightProperties.ColorOfLight.CyanLight:    _light.color = Color.cyan;    _colourIndex = 3; break;
            case lightProperties.ColorOfLight.YellowLight:  _light.color = Color.yellow;  _colourIndex = 4; break;
            case lightProperties.ColorOfLight.MagentaLight: _light.color = Color.magenta; _colourIndex = 5; break;
            case lightProperties.ColorOfLight.RedLight:     _light.color = Color.red;     _colourIndex = 0; break;
            case lightProperties.ColorOfLight.GreenLight:   _light.color = Color.green;   _colourIndex = 1; break;
            case lightProperties.ColorOfLight.BlueLight:    _light.color = Color.blue;    _colourIndex = 2; break;
            default: throw new ArgumentOutOfRangeException();
        }

        _visualizationMaterial = _lightVisualization.GetComponentInChildren<Renderer>().material;
    }
    
    // --- On/Off ---
    public void SwitchOnOff()
    {
        ResetChangeables();
        _light.enabled = !_light.enabled;
        lightOn = !lightOn;
        _lightVisualization.SetActive(lightOn);
    } 
    
    public void GreenBlueSwitch()
    {
        // Reset objects when switching
        ResetChangeables();

        switch (colorOfLight)
        {
            case lightProperties.ColorOfLight.BlueLight:
                MakeGreen(_light, _visualizationMaterial);
                break;
            case lightProperties.ColorOfLight.GreenLight:
                MakeBlue(_light, _visualizationMaterial);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // --- Colour control ---
    private void MakeGreen(Light l, Material m)   
    { 
        colorOfLight = lightProperties.ColorOfLight.GreenLight;   
        l.color = Color.green; 
        m.color = Color.green;
    }
    private void MakeBlue(Light l, Material m)
    {
        colorOfLight = lightProperties.ColorOfLight.BlueLight;    
        l.color = Color.blue;
        m.color = Color.blue;
    }
    
    private void ResetChangeables()
    {
        if (_changeablesExited != null)
            foreach (var changeable in _changeablesExited)
                changeable.UnChange(true);
        if (_changeablesCurrent != null)
            foreach (var changeable in _changeablesCurrent)
                changeable.UnChange(true);
        if (_changeablesPrevious != null)
            foreach (var changeable in _changeablesPrevious)
                changeable.UnChange(true);
    }
    
    // --- Visual & light mode setup ---
    private void TorchProjectionProperties(Light l)
    {
        l.type = LightType.Spot;
        l.intensity = intensityOfLight;
        l.range = forwardRangeOfTorch;
        l.innerSpotAngle = spreadOfTorchLight * 2;          
        l.spotAngle = spreadOfTorchLight * 2 + 2;
    }

    private void MakeTorch()
    {
        lanternModel.SetActive(false);
        torchModel.SetActive(true);
        torchVisualization.transform.localScale = new Vector3(1f,1f,forwardRangeOfTorch-0.5f);
        projectionType = lightProperties.ProjectionType.Torch;
        TorchProjectionProperties(_light);
        _lightVisualization = torchVisualization;
    }

    private void LanternProjectionProperties(Light l)
    {
        l.type = LightType.Point;
        l.intensity = intensityOfLight;
        l.range = radialRangeOfLantern;
    }

    private void MakeLantern()
    {
        lanternModel.SetActive(true);
        torchModel.SetActive(false);
        lanternVisualization.transform.localScale = new Vector3(radialRangeOfLantern*4, radialRangeOfLantern*4, radialRangeOfLantern*4);
        projectionType = lightProperties.ProjectionType.Lantern;
        LanternProjectionProperties(_light);
        _lightVisualization = lanternVisualization;
    }
    
    //NOT CURRENTLY IN USE
    
    // private float GetRadius()
    // {
    //     return Mathf.Abs(Vector3.Distance(transform.position, torchHitPoint) * Mathf.Tan(spreadOfTorchLight * Mathf.Deg2Rad));
    // }
    
    // private void MakeRed(Light l)     { colorOfLight = lightProperties.ColorOfLight.RedLight;     l.color = Color.red; }
    // private void MakeCyan(Light l)    { colorOfLight = lightProperties.ColorOfLight.CyanLight;    l.color = Color.cyan; }
    // private void MakeYellow(Light l)  { colorOfLight = lightProperties.ColorOfLight.YellowLight;  l.color = Color.yellow; }
    // private void MakeMagenta(Light l) { colorOfLight = lightProperties.ColorOfLight.MagentaLight; l.color = Color.magenta; }
    //
    // public void ChangeColour(int howMany)
    // {
    //     _colourIndex++;
    //     _colourChangers[_colourIndex](_light);
    //
    //     if (_colourIndex == howMany)
    //         _colourIndex = 0;
    // }
}
