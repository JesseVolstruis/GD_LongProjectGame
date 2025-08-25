using UnityEngine;
using UnityEngine.Serialization;

public class overlapChecker : MonoBehaviour
{
    public GameObject lightSourceA;
    public GameObject lightSourceB;
    
    public Vector3 _centreA;
    public Vector3 _centreB;
    
    public float _radiusA;
    public float _radiusB;
    
    public float _overlapDistance;

    public GameObject cube;
    public Material materialA;
    public Material materialB;
    
    public float distance;
    void Start()
    {

    }
    void Update()
    {
        _centreA = lightSourceA.GetComponent<basicLight>().lightCentre;
        _centreB = lightSourceB.GetComponent<basicLight>().lightCentre;
        _radiusA = lightSourceA.GetComponent<basicLight>().radius;
        _radiusB = lightSourceB.GetComponent<basicLight>().radius;
        _overlapDistance = _radiusA + _radiusB;
        distance = Mathf.Abs(Vector3.Distance(_centreA, _centreB));
        if(CheckOverlap())
        {
            cube.GetComponent<Renderer>().material = materialA;
        }
        else
        {
            cube.GetComponent<Renderer>().material = materialB;
        }
    }
    
    private bool CheckOverlap()
    {
        return Mathf.Abs(Vector3.Distance(_centreA, _centreB))  <= _overlapDistance;
    }
}
