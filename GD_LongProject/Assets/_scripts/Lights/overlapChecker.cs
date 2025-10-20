using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class overlapChecker : MonoBehaviour
{
    [Header("Lantern")]
    public GameObject lantern; //lantern
    [Header("Torch")]
    public GameObject torch;   //torch
    
    public lightProperties.ColorOfLight lanternColor;
    public lightProperties.ColorOfLight torchColor;
    public bool coloursAreDifferent;

    private List<IChangeable> _lanternCurrentChangeables = new List<IChangeable>();
    private List<IChangeable> _torchCurrentChangeables = new List<IChangeable>();
    private List<IChangeable> _overlappingChangeables = new List<IChangeable>();
    private List<IChangeable> _notOverlappingChangeables = new List<IChangeable>();

    private bool CheckColours(lightProperties.ColorOfLight colorLantern, lightProperties.ColorOfLight colorTorch)
    {
        return colorLantern != colorTorch;
    }

    void Update()
    {
        coloursAreDifferent = CheckColours(lanternColor, torchColor);

        if (!coloursAreDifferent) return;
        _overlappingChangeables = CheckOverlap(_lanternCurrentChangeables, _torchCurrentChangeables, _lanternCurrentChangeables);
        foreach (var changeable in _overlappingChangeables)
        {
            changeable.Change(lightProperties.ColorOfLight.CyanLight,null);
        }
    }

    private List<IChangeable> CheckOverlap(List<IChangeable> lanternsRange, List<IChangeable> torchRange, List<IChangeable> leftOvers)
    {
        leftOvers = lanternsRange.Except(torchRange).ToList();
        return lanternsRange.Intersect(torchRange).ToList();
    }
    
    // [Header("A- Lantern")]
    // public GameObject lightSourceA; //lantern
    // [Header("B- Torch")]
    // public GameObject lightSourceB; //torch
    
    // public Vector3 _centreA;
    // public Vector3 _centreB;
    //
    // public float _radiusA;
    // public float _radiusB;
    //
    // public float _overlapDistance;
    //
    // public lightProperties.ColorOfLight lightColorA;
    // public lightProperties.ColorOfLight lightColorB;
    //
    // public float distance;
    //
    // private IChangeable _currentChangeable;
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         Debug.Log(_overlapDistance);
    //         Debug.Log(distance);
    //     }
    //     _centreA = lightSourceA.transform.position;
    //     _centreB = lightSourceB.GetComponent<LightSource>().torchHitPoint;
    //     _radiusA = lightSourceA.GetComponent<LightSource>().radialRangeOfLantern;
    //     _radiusB = lightSourceB.GetComponent<LightSource>().radiusOfTorch;
    //     lightColorA = lightSourceA.GetComponent<LightSource>().colorOfLight;
    //     lightColorB = lightSourceB.GetComponent<LightSource>().colorOfLight;
    //     _overlapDistance = _radiusA + _radiusB;
    //     distance = Mathf.Abs(Vector3.Distance(_centreA, _centreB));
    //     
    //     if(IsOverlapping(_centreA,_centreB, _overlapDistance))
    //     {
    //         _currentChangeable = lightSourceB.GetComponent<LightSource>().CurrentChangeable;
    //         if (_currentChangeable != null)  _currentChangeable.Change(CheckColorCombos(lightColorA, lightColorB), null);
    //     }
    //    
    // }
    // private bool IsOverlapping(Vector3 positionA, Vector3 positionB, float maxDistance)
    // {
    //     return Mathf.Abs(Vector3.Distance(positionA, positionB))  <= maxDistance;
    // }
    //
    // //COLOR COMBOS
    // private lightProperties.ColorOfLight CheckColorCombos(lightProperties.ColorOfLight colorA, lightProperties.ColorOfLight colorB)
    // {
    //     //Yellow
    //     if ((colorA == lightProperties.ColorOfLight.RedLight || colorB == lightProperties.ColorOfLight.RedLight) 
    //         && (colorA == lightProperties.ColorOfLight.GreenLight || colorB == lightProperties.ColorOfLight.GreenLight))
    //     {
    //         return lightProperties.ColorOfLight.YellowLight;
    //     }
    //     //Cyan
    //     if ((colorA == lightProperties.ColorOfLight.GreenLight || colorB == lightProperties.ColorOfLight.GreenLight) 
    //         && (colorA == lightProperties.ColorOfLight.BlueLight || colorB == lightProperties.ColorOfLight.BlueLight))
    //     {
    //         return lightProperties.ColorOfLight.CyanLight;
    //     }
    //     //Magenta
    //     if ((colorA == lightProperties.ColorOfLight.RedLight || colorB == lightProperties.ColorOfLight.RedLight)
    //         && (colorA == lightProperties.ColorOfLight.BlueLight || colorB == lightProperties.ColorOfLight.BlueLight))
    //     {
    //         return lightProperties.ColorOfLight.MagentaLight;
    //     }
    //     
    //     return lightProperties.ColorOfLight.WhiteLight;
    // } 
}
