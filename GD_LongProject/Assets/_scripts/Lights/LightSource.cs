using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightSource : MonoBehaviour
{ 
    public float radiusOfLight;
    public lightProperties lightProperties;
    public lightProperties.ColorOfLight colorOfLight = lightProperties.ColorOfLight.WhiteLight;
    public lightProperties.ProjectionType projectionType;
    private float _upwardTiltDegrees = 12f;

    private Light _light;
    private Transform _thisPlayer;

    public float numberOfRays = 8f;
    private float _tiltInRads;
    private float _degreesBetweenRays;

    public GameObject lantern;
    public GameObject torch;

    // track changables
    private List<IChangable> _changeablesPrevious = new List<IChangable>(); 
    private List<IChangable> _changeablesCurrent = new List<IChangable>();  
    private List<IChangable> _changeablesExited = new List<IChangable>();   
    
    private IChangable _currentChangable;
    private IChangable _previousChangable;
    
    [SerializeField] private bool _lightOn;
    void Start()
    {
        _thisPlayer = transform.root;

        TorchLogic();
        _light = GetComponent<Light>();
        AssignLightProperties();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) MakeLantern();
        if (Input.GetKeyDown(KeyCode.E)) MakeTorch();
        if (Input.GetKeyDown(KeyCode.Alpha1)) MakeRed(_light);
        if (Input.GetKeyDown(KeyCode.Alpha2)) MakeBlue(_light);
        if (Input.GetKeyDown(KeyCode.Alpha3)) MakeGreen(_light);
        if (Input.GetKeyDown(KeyCode.Alpha4)) MakeCyan(_light);
        if (Input.GetKeyDown(KeyCode.Alpha5)) MakeYellow(_light);
        if (Input.GetKeyDown(KeyCode.Alpha6)) MakeMagenta(_light);
    }

    private void LateUpdate()
    {
        if (projectionType == lightProperties.ProjectionType.Lantern)
        {
            // 1. Save current to previous
            _changeablesPrevious = _changeablesCurrent;

            // 2. Get a fresh list of changables this frame
            _changeablesCurrent = LanternLook(transform.position, radiusOfLight);
            foreach (var changeable in _changeablesCurrent)
            {
                changeable.Change(colorOfLight,_thisPlayer);
            }
            // 3. Find objects that were in previous but not in current
            _changeablesExited = _changeablesPrevious.Except(_changeablesCurrent).ToList();

            // 4. Call UnChange() on those
            foreach (var changable in _changeablesExited)
            {
                changable.UnChange();
            }
        } else if (projectionType == lightProperties.ProjectionType.Torch)
        {
            // 1. Save current to previous
            _previousChangable = _currentChangable;
            // 2. Get a fresh current
            _currentChangable = TorchLook();
            if (_currentChangable != null)
            {
                _currentChangable.Change(colorOfLight, _thisPlayer);
            }
            if (_currentChangable == null)
            {
                _previousChangable.UnChange();
            } 
            
        }
        
    }
    public Vector3 lightCentre; 
    public float radius; 
    public float xDistance;

    private float GetRadius()
    {
        return Mathf.Abs(Vector3.Distance(transform.position, lightCentre) * Mathf.Tan(_tiltInRads)) ; 
    }

    private IChangable TorchLook()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var centreHit)) 
        { 
            lightCentre = centreHit.point; 
            if (centreHit.collider.gameObject.TryGetComponent(out IChangable changable) && _lightOn)
            {
                return changable;
            } 
            xDistance = Mathf.Abs(Vector3.Distance(transform.position, lightCentre)); 
            radius = GetRadius(); 
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * centreHit.distance, 
                Color.yellow); 
        }
        return null;
    }
     // void FixedUpdate() 
     // { 
     //     if (projectionType == lightProperties.ProjectionType.Torch){
     //         if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var centreHit)) 
     //         { 
     //             lightCentre = centreHit.point; 
     //             if (centreHit.collider.gameObject.TryGetComponent(out IChangable changable)) 
     //             { 
     //                 changable.Change(colorOfLight, _thisPlayer); 
     //             } 
     //             xDistance = Mathf.Abs(Vector3.Distance(transform.position, lightCentre)); 
     //             radius = GetRadius(); 
     //             Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * centreHit.distance, 
     //                 Color.yellow); 
     //         }
     //         else 
     //         { 
     //             Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white); 
     //         } 
     //     } 
     // }

    //LANTERN INTERACTION LOGIC
    private List<IChangable> LanternLook(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, 0.7f * radius);
        var changeables = new List<IChangable>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IChangable changeable) && _lightOn)
            {
                changeables.Add(changeable);
            }
        }
        return changeables;
    }

    private void TorchLogic()
    {
        _degreesBetweenRays = 360f / numberOfRays;
        _tiltInRads = _upwardTiltDegrees * Mathf.Deg2Rad;
    }

    //Initial SetUp
    private void AssignLightProperties()
    {
        colorOfLight = lightProperties.currentColorOfLight;
        projectionType = lightProperties.currentProjectionType;

        switch (projectionType)
        {
            case lightProperties.ProjectionType.Lantern: MakeLantern(); break;
            case lightProperties.ProjectionType.Torch: MakeTorch(); break;
        }

        radiusOfLight = lightProperties.rangeOfLight;
        _upwardTiltDegrees = lightProperties.spreadOfLight;
        _light.intensity = lightProperties.intensityOfLight;
        _lightOn = lightProperties.lightOn;
        
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
    //ON OFF Switch
    public void SwitchOnOff()
    {
        _light.enabled = !_light.enabled;
        _lightOn = !_lightOn;
    } 

    // Colour Changers
    public void MakeRed(Light l)     { colorOfLight = lightProperties.ColorOfLight.RedLight; l.color = Color.red; }
    public void MakeGreen(Light l)   { colorOfLight = lightProperties.ColorOfLight.GreenLight; l.color = Color.green; }
    public void MakeBlue(Light l)    { colorOfLight = lightProperties.ColorOfLight.BlueLight; l.color = Color.blue; }
    public void MakeCyan(Light l)    { colorOfLight = lightProperties.ColorOfLight.CyanLight; l.color = Color.cyan; }
    public void MakeYellow(Light l)  { colorOfLight = lightProperties.ColorOfLight.YellowLight; l.color = Color.yellow; }
    public void MakeMagenta(Light l) { colorOfLight = lightProperties.ColorOfLight.MagentaLight; l.color = Color.magenta; }
    
    private void TorchProjectionProperties(Light l)
    {
        l.type = LightType.Spot;
        l.innerSpotAngle = 28;
        l.spotAngle = 30;
    }

    //Tool Changers
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
