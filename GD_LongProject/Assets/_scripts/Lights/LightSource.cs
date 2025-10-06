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

    [Header("Lantern Specific Values")]
    public float radialRangeOfLantern;

    [Header("Torch Specific Values")]
    [SerializeField] private float spreadOfTorchLight = 12f; 
    [SerializeField] private float forwardRangeOfTorch = 12f;

    private Light _light;
    private Transform _thisLightSource;              // Who is holding this light?

    [Header("Models (ART TEAM)")]
    public GameObject lanternVisuals;
    public GameObject torchVisuals;

    // --- Changeable tracking ---
    private List<IChangeable> _changeablesPrevious = new List<IChangeable>(); 
    private List<IChangeable> _changeablesCurrent  = new List<IChangeable>();  
    private List<IChangeable> _changeablesExited   = new List<IChangeable>();   

    public IChangeable CurrentChangeable;
    private IChangeable _previousChangeable;

    private LayerMask _playerLayerMask;
    private int _ignoreRaycastLayerMask; 
    private int _mask;
    
    
    // --- Colour switching ---
    private int _colourIndex;
    private List<Action<Light>> _colourChangers = new List<Action<Light>>();

    [HideInInspector] public Vector3 torchHitPoint; 
    [HideInInspector] public float radiusOfTorch;

    private void Awake()
    {
        // Build the list of colour change methods
        _colourChangers = new List<Action<Light>>()
        {
            MakeRed, MakeGreen, MakeBlue, MakeCyan, MakeYellow, MakeMagenta
        };
    }

    private void Start()
    {
        _playerLayerMask = LayerMask.GetMask("Player");
        _ignoreRaycastLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        _mask = ~(_playerLayerMask | _ignoreRaycastLayerMask);
        _thisLightSource = transform.root;
        _light = GetComponent<Light>();
        AssignLightProperties();
    }

private void LateUpdate()
{
    switch (projectionType)
    {
        // --- Lantern Mode ---
        case lightProperties.ProjectionType.Lantern:
            _changeablesPrevious = _changeablesCurrent;
            _changeablesCurrent  = LanternLook(transform.position, radialRangeOfLantern);

            foreach (var changeable in _changeablesCurrent)
                changeable.Change(colorOfLight, _thisLightSource);

            _changeablesExited = _changeablesPrevious.Except(_changeablesCurrent).ToList();
            foreach (var changeable in _changeablesExited)
                changeable.UnChange();

            // Debug helper (Play mode only)
            Debug.DrawRay(transform.position, transform.forward * radialRangeOfLantern, Color.yellow);
            break;

        // --- Torch Mode ---
        case lightProperties.ProjectionType.Torch:
            _previousChangeable = CurrentChangeable;
            CurrentChangeable   = TorchLook();

            CurrentChangeable?.Change(colorOfLight, _thisLightSource);

            // If target changed or torch lost target, unchange old one
            if ((CurrentChangeable == null && _previousChangeable != null) || CurrentChangeable != _previousChangeable)
                _previousChangeable?.UnChange();

            // Debug helpers (Play mode only)
            var origin = transform.position;
            Debug.DrawRay(transform.position, transform.forward * forwardRangeOfTorch, Color.white);
            // Debug.DrawRay(new Vector3(origin.x + _sphereCastRadius,origin.y,origin.z) , transform.forward * forwardRangeOfTorch, Color.yellow);
            // Debug.DrawRay(new Vector3(origin.x - _sphereCastRadius,origin.y,origin.z) , transform.forward * forwardRangeOfTorch, Color.yellow);
            // Debug.DrawRay(new Vector3(origin.x,origin.y + _sphereCastRadius,origin.z) , transform.forward * forwardRangeOfTorch, Color.yellow);
            // Debug.DrawRay(new Vector3(origin.x,origin.y - _sphereCastRadius,origin.z) , transform.forward * forwardRangeOfTorch, Color.yellow);
            break;

        default:
            throw new ArgumentOutOfRangeException();
    }
}

    // --- Torch helpers ---
    private float GetRadius()
    {
        return Mathf.Abs(Vector3.Distance(transform.position, torchHitPoint) * Mathf.Tan(spreadOfTorchLight * Mathf.Deg2Rad));
    }

    private readonly float _sphereCastRadius = 0.3f; //Torch
    
    private IChangeable TorchLook()
    {
        if (Physics.SphereCast(transform.position, _sphereCastRadius, transform.forward, out var centreHit, forwardRangeOfTorch,
                _mask)) 
        { 
            torchHitPoint = centreHit.point; 
            float abs = Mathf.Abs(Vector3.Distance(transform.position, torchHitPoint));

            if (abs <= forwardRangeOfTorch)
            {
                if (centreHit.collider.gameObject.TryGetComponent(out IChangeable changeable) && lightOn)
                    return changeable;
            }

            radiusOfTorch = GetRadius(); 
            Debug.DrawRay(transform.position, transform.forward * centreHit.distance, Color.red); 
        }
        return null;
    }

    // --- Lantern helpers ---
    private List<IChangeable> LanternLook(Vector3 center, float lanternRadius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, 0.8f *lanternRadius); 
        var changeables = new List<IChangeable>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IChangeable changeable) && lightOn)
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
    }

    // --- On/Off ---
    public void SwitchOnOff()
    {
        _light.enabled = !_light.enabled;
        lightOn = !lightOn;
    } 

    // --- Colour control ---
    private void MakeRed(Light l)     { colorOfLight = lightProperties.ColorOfLight.RedLight;     l.color = Color.red; }
    private void MakeGreen(Light l)   { colorOfLight = lightProperties.ColorOfLight.GreenLight;   l.color = Color.green; }
    private void MakeBlue(Light l)    { colorOfLight = lightProperties.ColorOfLight.BlueLight;    l.color = Color.blue; }
    private void MakeCyan(Light l)    { colorOfLight = lightProperties.ColorOfLight.CyanLight;    l.color = Color.cyan; }
    private void MakeYellow(Light l)  { colorOfLight = lightProperties.ColorOfLight.YellowLight;  l.color = Color.yellow; }
    private void MakeMagenta(Light l) { colorOfLight = lightProperties.ColorOfLight.MagentaLight; l.color = Color.magenta; }

    public void ChangeColour(int howMany)
    {
        _colourIndex++;
        _colourChangers[_colourIndex](_light);

        if (_colourIndex == howMany)
            _colourIndex = 0;
    }

    public void GreenBlueSwitch()
    {
        // Reset objects when switching
        if (projectionType == lightProperties.ProjectionType.Lantern)
        {
            foreach (var changeable in _changeablesExited)   changeable.UnChange();
            foreach (var changeable in _changeablesCurrent)  changeable.UnChange();
            foreach (var changeable in _changeablesPrevious) changeable.UnChange();
        }
        else if (projectionType == lightProperties.ProjectionType.Torch)
        {
            _previousChangeable?.UnChange();
            CurrentChangeable?.UnChange();
        }

        // Swap between green and blue
        if (colorOfLight == lightProperties.ColorOfLight.BlueLight)
            MakeGreen(_light);
        else if (colorOfLight == lightProperties.ColorOfLight.GreenLight)
            MakeBlue(_light);
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
