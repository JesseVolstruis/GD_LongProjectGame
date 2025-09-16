using System;
using UnityEngine;

public class lantern : MonoBehaviour
{
    public lightProperties lightProperties;
    private float _radiusOfLight;
    private float _intensityOfLight;
    public lightProperties.ColorOfLight colorOfLight = lightProperties.ColorOfLight.WhiteLight;
    private Light _light;
    
    void Start()
    {
        _light = gameObject.GetComponent<Light>();
        AssignLightProperties();
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * _radiusOfLight/2, Color.green);
    }
    void FixedUpdate()
    {
        LanternLook(transform.position, _radiusOfLight);
    }
    
    private void LanternLook(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, 0.7f*radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.TryGetComponent(out IChangable changeable))
            {
                changeable.Change(colorOfLight);
            }
        }
    }
    private void AssignLightProperties()
    {
        colorOfLight = lightProperties.currentColorOfLight;
        _radiusOfLight = lightProperties.radiusOfLight;
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
            default: 
                _light.color = Color.white;
                throw new ArgumentOutOfRangeException();
        }
    }
}
