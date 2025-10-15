using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstructionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float fadeSpeed = 5f; // How quickly tiles fade in/out
    [SerializeField] private float minAlpha = 0f;  // Minimum alpha for faded tiles

    private Dictionary<Renderer, float> _fadingObjects = new Dictionary<Renderer, float>();

    void LateUpdate()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // SphereCastAll to detect all tiles in the way
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereRadius, direction, distance, obstructionMask);

        HashSet<Renderer> currentlyHit = new HashSet<Renderer>();
        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                currentlyHit.Add(rend);
                if (!_fadingObjects.ContainsKey(rend))
                {
                    _fadingObjects[rend] = rend.material.color.a; // Store current alpha
                }
            }
        }

        // Fade in/out logic
        List<Renderer> keys = new List<Renderer>(_fadingObjects.Keys);
        foreach (var rend in keys)
        {
            Color color = rend.material.color;

            if (currentlyHit.Contains(rend))
            {
                // Fade OUT
                color.a = Mathf.Lerp(color.a, minAlpha, Time.deltaTime * fadeSpeed);
            }
            else
            {
                // Fade IN
                color.a = Mathf.Lerp(color.a, 1f, Time.deltaTime * fadeSpeed);

                // Remove from dictionary once fully visible
                if (Mathf.Abs(color.a - 1f) < 0.01f)
                {
                    _fadingObjects.Remove(rend);
                }
            }

            rend.material.color = color;
        }
    }
}
