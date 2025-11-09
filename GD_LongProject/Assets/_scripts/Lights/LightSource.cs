using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class LightSource : MonoBehaviour
{
    private static readonly int FadeDistance = Shader.PropertyToID("_fadeDistance");
    private static readonly int Origin = Shader.PropertyToID("_origin");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

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
    //private int _colourIndex;

    [HideInInspector] public Vector3 torchHitPoint; 
    [HideInInspector] public float radiusOfTorch;

    // --- For Overlap Checking ---
    public List<(IChangeable changeable, lightProperties.ColorOfLight, Transform transform)> OverlapData = new();
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
        // Ensure OverlapData only contains this frame's hits
        OverlapData.Clear();

        // Get current hits depending on projection type
        List<IChangeable> current = projectionType switch
        {
            lightProperties.ProjectionType.Lantern => LanternLook(transform.position, radialRangeOfLantern),
            lightProperties.ProjectionType.Torch   => TorchLook(transform.position,  TorchActualDistance(forwardRangeOfTorch)), 
            _ => new List<IChangeable>()
        };

        TorchShaderLogic(_visualizationMaterial);
        // Normalize null -> empty list (so we can safely Except/Any)
        current = current ?? new List<IChangeable>();

        // Fill OverlapData (unique per IChangeable) — avoid duplicates
        var seen = new HashSet<IChangeable>();
        foreach (var changeable in current)
        {
            if (changeable == null) continue;
            if (seen.Add(changeable))
                OverlapData.Add((changeable, colorOfLight, _thisLightSource));
        }

        // Compute entered & exited using copies (no reference aliasing)
        var prev = _changeablesPrevious ?? new List<IChangeable>();
        var entered = current.Except(prev).ToList();
        var exited  = prev.Except(current).ToList();

        // Handle newly entered objects
       // foreach (var changeable in entered)
       // {
            // Do the change here if desired (original code had commented out Change)
       //     changeable.Change(colorOfLight, _thisLightSource);
       // }

        // Handle exited objects
        foreach (var changeable in exited)
        {
            changeable.UnChange(false);
        }

        // Finally set previous to a copy of current for next frame
        _changeablesPrevious = new List<IChangeable>(current);

        // Also keep _changeablesCurrent for other uses in your class if needed
        _changeablesCurrent = new List<IChangeable>(current);
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
    
    private void TorchShaderLogic(Material material)
    {
        float length = _lightVisualization.GetComponentInChildren<Renderer>().bounds.size.y;
        float fadeDistance = TorchActualDistance(forwardRangeOfTorch)/forwardRangeOfTorch;
        material.SetFloat(FadeDistance, fadeDistance);
        material.SetFloat(Origin, length);
    }

    private float TorchActualDistance(float forwardRange)
    {
        Debug.DrawRay(transform.position, transform.forward * forwardRange, Color.red);
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, horizontalRangeOfTorch, out hit, forwardRange ))
        {
            // if (hit.collider.CompareTag("Blue"))
            // {
            //     return hit.distance;
            // }
        }
        return forwardRange;
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
            case lightProperties.ColorOfLight.WhiteLight:   _light.color = Color.white;    break;
            case lightProperties.ColorOfLight.CyanLight:    _light.color = Color.cyan;    break;
            case lightProperties.ColorOfLight.YellowLight:  _light.color = Color.yellow;  break;
            case lightProperties.ColorOfLight.MagentaLight: _light.color = Color.magenta;  break;
            case lightProperties.ColorOfLight.RedLight:     _light.color = Color.red;     break;
            case lightProperties.ColorOfLight.GreenLight:   _light.color = Color.green;    break;
            case lightProperties.ColorOfLight.BlueLight:    _light.color = Color.blue;     break;
            default: throw new ArgumentOutOfRangeException();
        }

        _visualizationMaterial = _lightVisualization.GetComponentInChildren<Renderer>().material;
    }
    
    [SerializeField] private AudioClip switchSound;
    // --- On/Off ---
    public void SwitchOnOff()
    {
        if(SoundManager.Instance != null) SoundManager.Instance.PlaySoundFX(switchSound,transform,1f);
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
    
    Color _green = new Color(0f, 1f, 0f, 0f);
    Color _blue = new Color(0f, 0f, 1f, 0f);
    
    private void MakeGreen(Light l, Material m)   
    { 
        m.EnableKeyword("_EMISSION");
        colorOfLight = lightProperties.ColorOfLight.GreenLight;   
        l.color = Color.green; 
        m.color = _green;
        m.SetColor(EmissionColor, Color.green * 0.1f);
        
    }
    private void MakeBlue(Light l, Material m)
    {
        m.EnableKeyword("_EMISSION");
        colorOfLight = lightProperties.ColorOfLight.BlueLight;    
        l.color = Color.blue;
        m.color = _blue;
        m.SetColor(EmissionColor, Color.blue);
    }
    
    private void ResetChangeables()
    {
        // UnChange all items that are in any of the tracked lists
        var all = new HashSet<IChangeable>();

        if (_changeablesExited != null)
            foreach (var c in _changeablesExited) all.Add(c);

        if (_changeablesCurrent != null)
            foreach (var c in _changeablesCurrent) all.Add(c);

        if (_changeablesPrevious != null)
            foreach (var c in _changeablesPrevious) all.Add(c);

        // UnChange each (true indicates a forced reset, same as you used before)
        foreach (var changeable in all)
        {
            if (changeable != null)
                changeable.UnChange(true);
        }

        // Clear our internal trackers
        _changeablesExited?.Clear();
        _changeablesCurrent?.Clear();
        _changeablesPrevious?.Clear();
        OverlapData.Clear();
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

}
