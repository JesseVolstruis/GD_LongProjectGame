using System.Collections.Generic;
using UnityEngine;

public class CameraObstructionHandler : MonoBehaviour
{
    private static readonly int Color1 = Shader.PropertyToID("_BaseColor");

    [Header("References")]
    [SerializeField] private Transform player;
    
    [Header("Settings")]
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private float sphereRadius = 0.3f;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float minAlpha = 0f;

    private Dictionary<Renderer, (Material originalMat, Material tempMat)> _materials;
    private MaterialPropertyBlock _block;

    void Awake()
    {
        _materials = new Dictionary<Renderer, (Material, Material)>();
        _block = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (!player) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereRadius, direction, distance, obstructionMask);
        HashSet<Renderer> currentHits = new();

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (!rend) continue;

            currentHits.Add(rend);

            if (!_materials.ContainsKey(rend))
            {
                // Create temp material safely
                Material temp = new Material(rend.sharedMaterial);
                rend.material = temp;
                _materials[rend] = (rend.sharedMaterial, temp);
            }

            rend.GetPropertyBlock(_block);
            Color color = rend.material.GetColor(Color1);
            color.a = Mathf.Lerp(color.a, minAlpha, Time.deltaTime * fadeSpeed);
            _block.SetColor(Color1, color);
            rend.SetPropertyBlock(_block);
        }

        // Fade back and cleanup
        List<Renderer> toRemove = new();
        foreach (var kvp in _materials)
        {
            Renderer rend = kvp.Key;
            if (!rend) { toRemove.Add(rend); continue; }

            if (currentHits.Contains(rend)) continue;

            rend.GetPropertyBlock(_block);
            Color color = rend.material.GetColor(Color1);
            color.a = Mathf.Lerp(color.a, 1f, Time.deltaTime * fadeSpeed);
            _block.SetColor(Color1, color);
            rend.SetPropertyBlock(_block);

            if (color.a >= 0.99f)
            {
                Destroy(rend.material);
                rend.material = kvp.Value.originalMat;
                toRemove.Add(rend);
            }
        }

        foreach (var rend in toRemove)
            _materials.Remove(rend);
    }
}
