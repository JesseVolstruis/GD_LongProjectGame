using UnityEngine;
using UnityEngine.Serialization;

public class basicLight : MonoBehaviour
{
    public float numberOfRays = 8f;
    
    private float _upwardTiltDegrees = 12f;
    private float _tilsInRads;
    private float _degreesBetweenRays;
    

    public Vector3 lightCentre;
    public float radius;
    
    public float xDistance;
    void Start()
    {
        _degreesBetweenRays = 360f / numberOfRays;;
        _tilsInRads = _upwardTiltDegrees * Mathf.Deg2Rad ;
    }
    void FixedUpdate()
    {
        Quaternion upTilt = Quaternion.AngleAxis(_upwardTiltDegrees, transform.TransformDirection(Vector3.left));
        Vector3 direction = upTilt * transform.forward;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10f))
        { 
            lightCentre =  hit.point;
            xDistance = Mathf.Abs(Vector3.Distance(transform.position, lightCentre));
            radius = GetRadius();
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow); 
        }
        else
        { 
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white); 
        }

        for (int i = 0; i < numberOfRays; i++)
        {
            Debug.DrawRay(transform.position, direction * 12, Color.red);
            Quaternion rotateValue = Quaternion.AngleAxis(_degreesBetweenRays, transform.TransformDirection(Vector3.forward));
            direction = rotateValue * direction;
        }
    }
    private float GetRadius()
    {
        return Mathf.Abs(Vector3.Distance(transform.position, lightCentre) * Mathf.Tan(_tilsInRads)) ;
    }
}


