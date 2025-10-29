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
    //public bool coloursAreDifferent;

    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _inLightA;
    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _inLightB;
    
    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _overlapping;
    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _notOverlappingA;
    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> _notOverlappingB;
    
    private Dictionary<IChangeable, lightProperties.ColorOfLight> _previousFrameStates = new();

    private List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)>
        previousNotOverlapping = new();
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
        var overlapping = inLightA
            .Where(a => inLightB.Any(b => b.changeable == a.changeable))
            .ToList();

        // --- Find NON overlaps based on IChangeable identity ---
        var notOverlapping = inLightA
            .Where(a => !inLightB.Any(b => b.changeable == a.changeable))
            .Concat(
                inLightB.Where(b => !inLightA.Any(a => a.changeable == b.changeable))
            )
            .ToList();

        if (coloursAreDifferent)
        {
            foreach (var changeable in overlapping)
            {
                changeable.changeable.Change(lightProperties.ColorOfLight.CyanLight, null);
            }

            foreach (var changeable in notOverlapping)
            {
                changeable.changeable.Change(changeable.Item2,changeable.transform);
            }
        }
        else
        {
            foreach (var changeable in overlapping)
            {
                changeable.changeable.Change(changeable.Item2,changeable.transform);
            }
            
            foreach (var changeable in notOverlapping)
            {
                changeable.changeable.Change(changeable.Item2,changeable.transform);
            }
        }
        
        List<(IChangeable changeable, lightProperties.ColorOfLight colour, Transform transform)> leftNotOverlapping = new();

        leftNotOverlapping = previousNotOverlapping.Except(notOverlapping).ToList();

        foreach (var entry in leftNotOverlapping)
        {
            entry.changeable.UnChange(true);
        }
        
        // --- Store current as previous for next frame ---
        previousNotOverlapping = notOverlapping;
        
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


   // // Separate unique changeables for A and B
   //      var notOverlappingA = inLightA
   //          .Where(a => !overlapping.Any(o => o.changeable == a.changeable))
   //          .ToList();
   //
   //      var notOverlappingB = inLightB
   //          .Where(b => !overlapping.Any(o => o.changeable == b.changeable))
   //          .ToList();
   //     
   //      Dictionary<IChangeable, lightProperties.ColorOfLight> currentFrameStates = new();
   //
   //      if (coloursAreDifferent)
   //      {
   //          // Cyan overrides both if different colours
   //          foreach (var entry in overlapping)
   //              currentFrameStates[entry.changeable] = lightProperties.ColorOfLight.CyanLight;
   //
   //          foreach (var entry in notOverlappingA)
   //              currentFrameStates[entry.changeable] = entry.Item2;
   //
   //          foreach (var entry in notOverlappingB)
   //              currentFrameStates[entry.changeable] = entry.Item2;
   //      }
   //      else
   //      {
   //          // Same colour lights just reinforce
   //          foreach (var entry in overlapping)
   //              currentFrameStates[entry.changeable] = entry.Item2;
   //
   //          foreach (var entry in notOverlappingA)
   //              currentFrameStates[entry.changeable] = entry.Item2;
   //
   //          foreach (var entry in notOverlappingB)
   //              currentFrameStates[entry.changeable] = entry.Item2;
   //      }
   //
   //      // --- Compare to previous frame ---
   //      // 1. Handle new entries or colour changes
   //      foreach (var kvp in currentFrameStates)
   //      {
   //          var changeable = kvp.Key;
   //          var newColor = kvp.Value;
   //
   //          if (!_previousFrameStates.TryGetValue(changeable, out var oldColor) || oldColor != newColor)
   //          {
   //              changeable.Change(newColor, null);
   //          }
   //      }
   //
   //      // 2. Handle objects that left all lights
   //      foreach (var kvp in _previousFrameStates)
   //      {
   //          if (!currentFrameStates.ContainsKey(kvp.Key))
   //          {
   //              kvp.Key.UnChange(true);
   //          }
   //      }
