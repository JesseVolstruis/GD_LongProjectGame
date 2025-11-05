using System.Collections.Generic;
using UnityEngine;

public class ObstructionFadeTarget : MonoBehaviour
{
    private static readonly int AlphaScale = Shader.PropertyToID("_AlphaScale");
    private Renderer[] renderers;
    private readonly Dictionary<Camera, float> alphaPerCamera = new();

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }
    

    public float GetAlphaForCamera(Camera cam)
    {
        if (!alphaPerCamera.ContainsKey(cam))
            alphaPerCamera[cam] = 1f;
        return alphaPerCamera[cam];
    }

    public void SetAlphaForCamera(Camera cam, float alpha)
    {
        alphaPerCamera[cam] = alpha;
    }

    // Explicitly applies the correct alpha for a given camera
    public void ForceApplyForCamera(Camera cam)
    {
        float alpha = GetAlphaForCamera(cam);

        foreach (var rend in renderers)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat(AlphaScale, alpha);
            rend.SetPropertyBlock(mpb);
        }
    }
}