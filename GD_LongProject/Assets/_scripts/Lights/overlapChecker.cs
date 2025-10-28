using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class overlapChecker : MonoBehaviour
{
    [Header("Light A")]
    public LightSource lightA; 
    [Header("Light B")]
    public LightSource lightB;  
    public lightProperties.ColorOfLight colorA;
    public lightProperties.ColorOfLight colorB;
    public bool coloursAreDifferent;

    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _inLightA;
    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _inLightB;
    
    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _overlapping;
    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _notOverlappingA;
    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _notOverlappingB;
    
    void Start()
    {
        colorA = lightA.colorOfLight;
        colorB = lightB.colorOfLight;

        _inLightA = lightA.OverlapData;
        _inLightB = lightB.OverlapData;
    }
    void Update()
    {
        var inLightA = lightA?.OverlapData;
        var inLightB = lightB?.OverlapData;
        if (inLightA == null || inLightB == null) return;

        bool coloursAreDifferent = lightA.colorOfLight != lightB.colorOfLight;

        // --- Find overlaps based on IChangeable identity ---
        _overlapping = inLightA
            .Where(a => inLightB.Any(b => b.changeable == a.changeable))
            .ToList();

        // Separate unique changeables for A and B
        _notOverlappingA = inLightA
            .Where(a => !_overlapping.Any(o => o.changeable == a.changeable))
            .ToList();

        _notOverlappingB = inLightB
            .Where(b => !_overlapping.Any(o => o.changeable == b.changeable))
            .ToList();

        // --- Apply changes ---
        if (coloursAreDifferent)
        {
            foreach (var entry in _overlapping)
                entry.changeable.Change(lightProperties.ColorOfLight.CyanLight, null);
            
            foreach (var entry in _notOverlappingA)
                entry.changeable.Change(entry.colour, entry.transform);
            foreach (var entry in _notOverlappingB)
                entry.changeable.Change(entry.colour, entry.transform);
        }
        else
        {
            foreach (var entry in _overlapping)
                entry.changeable.Change(entry.colour, entry.transform);
            foreach (var entry in _notOverlappingA)
                entry.changeable.Change(entry.colour, entry.transform);
            foreach (var entry in _notOverlappingB)
                entry.changeable.Change(entry.colour, entry.transform);
        }
    }


    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> 
        CheckOverlap(
            List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> lightARange,
            List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> lightBRange,
            out List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> leftOvers)
    {
        leftOvers = lightARange
            .Where(l => !lightBRange.Any(t => t.changeable == l.changeable))
            .ToList();

        var overlap = lightARange
            .Where(l => lightBRange.Any(t => t.changeable == l.changeable))
            .ToList();

        return overlap;
    }
    
    private bool CheckColours(lightProperties.ColorOfLight lightAColor, lightProperties.ColorOfLight lightBColor)
    {
        return lightAColor != lightBColor;
    }
}
